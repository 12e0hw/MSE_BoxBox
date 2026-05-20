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
            IsRunning = false;
            Debug.Log("[TimeManager] Time Over 이벤트 발생");
            OnTimeOver?.Invoke();
        }
    }

    public void StartTimer()
    {
        IsRunning = true;
        OnTimeChanged?.Invoke(RemainingTime);
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void ResetTimer()
    {
        RemainingTime = startTime;
        OnTimeChanged?.Invoke(RemainingTime);
    }
    
    public void SetStartTime(float timeLimit)
    {
        startTime = timeLimit;
    }
}