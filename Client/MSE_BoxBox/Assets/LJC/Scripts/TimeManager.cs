using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float startTime = 120f;

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
            OnTimeOver?.Invoke();
        }
    }

    public void StartTimer()
    {
        IsRunning = true;
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
}