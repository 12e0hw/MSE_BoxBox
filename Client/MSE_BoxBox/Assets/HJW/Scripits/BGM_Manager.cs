using UnityEngine;
using UnityEngine.UI;

public class BGM_Manager : MonoBehaviour
{
    public static BGM_Manager instance;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource loopSfxSource;

    [Header("SFX")]
    public AudioClip buttonClickSound;
    public AudioClip extinguisherSound;
    public AudioClip truckInSound;
    public AudioClip warningSound;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlayButtonSound()
    {
        if (buttonClickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
        }
    }
    public void PlayTruckInSound()
    {
        if (truckInSound) sfxSource.PlayOneShot(truckInSound);
    }

    public void StartExtinguisherSound()
    {
        if (extinguisherSound && loopSfxSource)
        {
            loopSfxSource.clip = extinguisherSound;
            loopSfxSource.loop = true;
            loopSfxSource.Play();
        }
    }

    public void StopExtinguisherSound()
    {
        if (loopSfxSource && loopSfxSource.clip == extinguisherSound)
        {
            loopSfxSource.Stop();
        }
    }

    public void StartWarningSound()
    {
        if (warningSound && loopSfxSource)
        {
            loopSfxSource.clip = warningSound;
            loopSfxSource.loop = true;
            loopSfxSource.Play();
        }
    }

    public void StopWarningSound()
    {
        if (loopSfxSource && loopSfxSource.clip == warningSound)
        {
            loopSfxSource.Stop();
        }
    }
}