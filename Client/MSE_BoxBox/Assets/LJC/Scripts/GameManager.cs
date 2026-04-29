using UnityEngine;

namespace LJC
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private DeliveryManager deliveryManager;

        [Header("UI")]
        [SerializeField] private TempHUDUI tempHUDUI;

        private void OnEnable()
        {
            if (deliveryManager != null)
            {
                deliveryManager.OnDeliverySuccess += HandleDeliverySuccess;
                deliveryManager.OnDeliveryFail += HandleDeliveryFail;
            }

            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged += HandleScoreChanged;
            }

            if (timeManager != null)
            {
                timeManager.OnTimeChanged += HandleTimeChanged;
                timeManager.OnTimeOver += HandleTimeOver;
            }
        }

        private void OnDisable()
        {
            if (deliveryManager != null)
            {
                deliveryManager.OnDeliverySuccess -= HandleDeliverySuccess;
                deliveryManager.OnDeliveryFail -= HandleDeliveryFail;
            }

            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged -= HandleScoreChanged;
            }

            if (timeManager != null)
            {
                timeManager.OnTimeChanged -= HandleTimeChanged;
                timeManager.OnTimeOver -= HandleTimeOver;
            }
        }

        private void Start()
        {
            scoreManager.ResetScore();
            timeManager.ResetTimer();
            timeManager.StartTimer();
        }

        private void HandleDeliverySuccess(BoxController box, TruckController truck)
        {
            scoreManager.AddScore(box.scoreValue);
            Debug.Log($"적재 성공 / +{box.scoreValue}점 / 현재 점수: {scoreManager.CurrentScore}");
        }

        private void HandleDeliveryFail(BoxController box, TruckController truck)
        {
            Debug.Log($"적재 실패 / Box: {box.Color} / Truck: {truck.TruckColor}");
        }

        private void HandleScoreChanged(int score)
        {
            if (tempHUDUI != null)
            {
                tempHUDUI.UpdateScore(score);
            }
        }

        private void HandleTimeChanged(float time)
        {
            if (tempHUDUI != null)
            {
                tempHUDUI.UpdateTime(time);
            }
        }

        private void HandleTimeOver()
        {
            Debug.Log("시간 종료");
            timeManager.StopTimer();
        }
    }
}