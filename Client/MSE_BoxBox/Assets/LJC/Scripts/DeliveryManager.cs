using System;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event Action<BoxController, TruckController> OnDeliverySuccess;
    public event Action<BoxController, TruckController> OnDeliveryFail;

    // Notify listeners when total, small, and big delivery counts change.
    public event Action<int, int, int> OnDeliveryCountChanged;

    private ScoreManager scoreManager;

    public int TotalDeliveredCount { get; private set; }
    public int SmallBoxDeliveredCount { get; private set; }
    public int BigBoxDeliveredCount { get; private set; }

    // Set the score manager and reset delivery counts.
    public void Initialize(ScoreManager scoreManager)
    {
        this.scoreManager = scoreManager;
        ResetDeliveryCounts();
    }

    // Reset all delivery count values.
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

    // Try to deliver a box to the selected truck.
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
        PlayTruckInputSound();

        OnDeliverySuccess?.Invoke(box, truck);
        
        Debug.Log(
            $"[DeliveryManager] Delivery Success / Total: {TotalDeliveredCount}, Small: {SmallBoxDeliveredCount}, Big: {BigBoxDeliveredCount}"
        );

        return true;
    }

    // Increase delivery counts based on the delivered box size.
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

    // Add score for the delivered box.
    private void AddScore(BoxController box)
    {
        if (scoreManager == null)
        {
            Debug.LogError("[DeliveryManager] ScoreManager is not connected.");
            return;
        }

        scoreManager.AddScore(box.scoreValue);
    }

    // Play the truck input sound effect.
    private void PlayTruckInputSound()
    { 
        if (BGM_Manager.instance != null)
        {
            BGM_Manager.instance.PlayTruckInSound();
        }
    }
}