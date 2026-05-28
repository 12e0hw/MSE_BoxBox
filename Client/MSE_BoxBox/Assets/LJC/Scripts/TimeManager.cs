using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float startTime = 10f;

    public float StartTime => startTime;
    public float RemainingTime { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<float> OnTimeChanged;
    public event Action OnTimeOver;
    
    private bool hasTimeOverTriggered;

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

        if (RemainingTime <= 0f)
        {
            TriggerTimeOver();
        }
    }

    public void StartTimer()
    {
        IsRunning = true;
        hasTimeOverTriggered = false;
        OnTimeChanged?.Invoke(RemainingTime);
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void ResetTimer()
    {
        RemainingTime = startTime;
        hasTimeOverTriggered = false;
        OnTimeChanged?.Invoke(RemainingTime);
    }
    
    public void SetStartTime(float timeLimit)
    {
        startTime = Mathf.Max(0f, timeLimit);
    }
    
    public void AddTime(float seconds)
    {
        if (seconds <= 0f)
        {
            return;
        }

        RemainingTime += seconds;
        hasTimeOverTriggered = false;
        OnTimeChanged?.Invoke(RemainingTime);
    }

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

    public void SetRemainingTime(float remainingTime)
    {
        RemainingTime = Mathf.Max(0f, remainingTime);

        OnTimeChanged?.Invoke(RemainingTime);

        if (RemainingTime <= 0f)
        {
            TriggerTimeOver();
        }
        else
        {
            hasTimeOverTriggered = false;
        }
    }

    private void TriggerTimeOver()
    {
        if (hasTimeOverTriggered)
        {
            return;
        }

        hasTimeOverTriggered = true;
        IsRunning = false;

        Debug.Log("[TimeManager] Time Over 이벤트 발생");
        OnTimeOver?.Invoke();
    }
}