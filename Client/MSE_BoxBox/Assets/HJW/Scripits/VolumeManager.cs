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

    void Start()
    {
        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        bgmSlider.value = bgmVol;
        sfxSlider.value = sfxVol;

        SetBGMVolume(bgmVol);
        SetSFXVolume(sfxVol);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}