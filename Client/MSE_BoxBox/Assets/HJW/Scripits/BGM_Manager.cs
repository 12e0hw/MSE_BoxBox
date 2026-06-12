using UnityEngine;
using System.Collections;

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
    
    private Coroutine warningSoundCoroutine;

    public float defaultVolume = 1.0f;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
        AudioListener.volume = defaultVolume;
    }
    
    public void PlayBGM()
    {
        if (bgmSource == null)
        {
            return;
        }

        // Checks if the BGM is already playing to prevent it from restarting
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void PauseBGM()
    {
        if (bgmSource == null)
        {
            return;
        }

        bgmSource.Pause();
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
        // Checks the clip to prevent accidentally stopping loopSfxSource when another sound is currently using it
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
        // Prevents a bug where the coroutine keeps running and triggers
        if (warningSound && loopSfxSource)
        {
            loopSfxSource.clip = warningSound;
            loopSfxSource.loop = true;
            loopSfxSource.Play();
        }
    }

    public void StopWarningSound()
    {
        if (warningSoundCoroutine != null)
        {
            StopCoroutine(warningSoundCoroutine);
            warningSoundCoroutine = null;
        }

        if (loopSfxSource && loopSfxSource.clip == warningSound)
        {
            loopSfxSource.Stop();
            loopSfxSource.loop = false;
        }
    }
    
    // 
    public void StartWarningSoundForSeconds(float duration)
    {
        // Cancels the existing coroutine to prevent duplicate timers if this function is called multiple times consecutively
        if (warningSoundCoroutine != null)
        {
            StopCoroutine(warningSoundCoroutine);
            warningSoundCoroutine = null;
        }

        StartWarningSound();

        warningSoundCoroutine = StartCoroutine(StopWarningSoundAfterSeconds(duration));
    }

    // Internal coroutine that waits for the specified duration before stopping the warning sound
    private IEnumerator StopWarningSoundAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);

        StopWarningSound();

        warningSoundCoroutine = null;
    }
}