using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("Audio mixer")]
    public AudioMixer audioMixer;

    [Header("UI slider")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    private const float MinVolume = 0.0001f;
    void Start()
    {
        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(bgmVol);
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(sfxVol);
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        ApplyBGMVolume(bgmVol);
        ApplySFXVolume(sfxVol);
    }

    public void SetBGMVolume(float volume)
    {
        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();

        ApplyBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();

        ApplySFXVolume(volume);
    }
    
    private void ApplyBGMVolume(float volume)
    {
        if (audioMixer == null)
        {
            return;
        }

        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Max(MinVolume, volume)) * 20f);
    }

    private void ApplySFXVolume(float volume)
    {
        if (audioMixer == null)
        {
            return;
        }

        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(MinVolume, volume)) * 20f);
    }
}