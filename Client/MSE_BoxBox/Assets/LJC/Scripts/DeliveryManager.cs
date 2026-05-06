using System;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event Action<BoxController, TruckController> OnDeliverySuccess;
    public event Action<BoxController, TruckController> OnDeliveryFail;

    public event Action<int, int, int> OnDeliveryCountChanged;
    // total, small, big

    private ScoreManager scoreManager;

    public int TotalDeliveredCount { get; private set; }
    public int SmallBoxDeliveredCount { get; private set; }
    public int BigBoxDeliveredCount { get; private set; }

    public void Initialize(ScoreManager scoreManager)
    {
        this.scoreManager = scoreManager;
    }

    public void ResetDeliveryCounts()
    {
        TotalDeliveredCount = 0;
        SmallBoxDeliveredCount = 0;
        BigBoxDeliveredCount = 0;

        OnDeliveryCountChanged?.Invoke(
            TotalDeliveredCount,
            SmallBoxDeliveredCount,
            BigBoxDeliveredCount
        );
    }

    public bool TryDeliver(BoxController box, TruckController truck)
    {
        if (box == null || truck == null)
        {
            return false;
        }

        if (box.IsDelivered)
        {
            return false;
        }

        bool isCorrectTruck = box.Color == truck.TruckColor;

        if (!isCorrectTruck)
        {
            Debug.Log("[DeliveryManager] Wrong truck");

            OnDeliveryFail?.Invoke(box, truck);
            return false;
        }

        box.MarkDelivered();

        AddDeliveryCount(box);
        AddScore(box);

        OnDeliverySuccess?.Invoke(box, truck);

        Debug.Log(
            $"[DeliveryManager] Delivery Success / Total: {TotalDeliveredCount}, Small: {SmallBoxDeliveredCount}, Big: {BigBoxDeliveredCount}"
        );

        return true;
    }

    private void AddDeliveryCount(BoxController box)
    {
        TotalDeliveredCount++;

        if (box.Size == BoxSize.Small)
        {
            SmallBoxDeliveredCount++;
        }
        else if (box.Size == BoxSize.Big)
        {
            BigBoxDeliveredCount++;
        }

        OnDeliveryCountChanged?.Invoke(
            TotalDeliveredCount,
            SmallBoxDeliveredCount,
            BigBoxDeliveredCount
        );
    }

    private void AddScore(BoxController box)
    {
        if (scoreManager == null)
        {
            Debug.LogError("[DeliveryManager] ScoreManager is not connected.");
            return;
        }

        scoreManager.AddScore(box.scoreValue);
    }
}