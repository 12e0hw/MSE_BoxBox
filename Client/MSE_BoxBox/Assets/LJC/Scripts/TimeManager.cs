using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float startTime = 10f;
    [SerializeField] private float warningStartTime = 10f;

    public float StartTime => startTime;
    public float RemainingTime { get; private set; }
    public bool IsRunning { get; private set; }

    // Notify listeners when the remaining time changes.
    public event Action<float> OnTimeChanged;
    // Notify listeners when the timer reaches zero.
    public event Action OnTimeOver;
    
    private bool hasTimeOverTriggered;
    private bool hasWarningTriggered;
    
    // Notify listeners when the warning state starts.
    public event Action OnWarningStarted;
    // Notify listeners when the warning state stops.
    public event Action OnWarningStopped;

    private void Awake()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        RemainingTime -= Time.deltaTime;

        if (RemainingTime < 0f)
        {
            RemainingTime = 0f;
        }

        OnTimeChanged?.Invoke(RemainingTime);
        
        if (RemainingTime > 0f && RemainingTime <= warningStartTime)
        {
            StartWarning();
        }

        if (RemainingTime <= 0f)
        {
            TriggerTimeOver();
        }
    }
    
    // Start the warning state and warning sound.
    private void StartWarning()
    {
        if (hasWarningTriggered)
        {
            return;
        }

        hasWarningTriggered = true;

        OnWarningStarted?.Invoke();

        if (BGM_Manager.instance != null)
        {
            BGM_Manager.instance.StartWarningSound();
        }
    }

    // Start counting down the timer.
    public void StartTimer()
    {
        IsRunning = true;
        hasTimeOverTriggered = false;
        CheckWarningReset();
        OnTimeChanged?.Invoke(RemainingTime);
    }

    // Stop the timer and warning sound.
    public void StopTimer()
    {
        IsRunning = false;
        if (BGM_Manager.instance != null)
        {
            BGM_Manager.instance.StopWarningSound();
        }
    }

    // Reset the timer to the start time.
    public void ResetTimer()
    {
        RemainingTime = startTime;
        hasTimeOverTriggered = false;
        
        StopWarning();

        OnTimeChanged?.Invoke(RemainingTime);
    }
    
    // Set the timer start value.
    public void SetStartTime(float timeLimit)
    {
        startTime = Mathf.Max(0f, timeLimit);
    }
    
    // Add time to the remaining time.
    public void AddTime(float seconds)
    {
        if (seconds <= 0f)
        {
            return;
        }

        RemainingTime += seconds;
        hasTimeOverTriggered = false;
        CheckWarningReset();
        OnTimeChanged?.Invoke(RemainingTime);
    }

    // Subtract time from the remaining time.
    public void SubtractTime(float seconds)
    {
        if (seconds <= 0f)
        {
            return;
        }

        RemainingTime -= seconds;

        if (RemainingTime < 0f)
        {
            RemainingTime = 0f;
        }

        OnTimeChanged?.Invoke(RemainingTime);

        if (RemainingTime <= 0f)
        {
            TriggerTimeOver();
        }
    }

    // Stop the warning state and warning sound.
    private void StopWarning()
    {
        if (!hasWarningTriggered)
        {
            return;
        }

        hasWarningTriggered = false;

        OnWarningStopped?.Invoke();

        if (BGM_Manager.instance != null)
        {
            BGM_Manager.instance.StopWarningSound();
        }
    }

    // Trigger the time-over event once.
    private void TriggerTimeOver()
    {
        if (hasTimeOverTriggered)
        {
            return;
        }

        hasTimeOverTriggered = true;
        IsRunning = false;

        StopWarning();

        OnTimeOver?.Invoke();
    }

    // Stop the warning state if enough time remains.
    private void CheckWarningReset()
    {
        if (RemainingTime > warningStartTime)
        {
            StopWarning();
        }
    }
}