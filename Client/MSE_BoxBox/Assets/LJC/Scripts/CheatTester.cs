using UnityEngine;
using UnityEngine.InputSystem;

public class CheatTester : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimeManager timeManager;

    [Header("Score Cheat")]
    [SerializeField] private int addScoreAmount = 20;

    [Header("Time Cheat")]
    [SerializeField] private float addTimeAmount = 10f;
    [SerializeField] private float subtractTimeAmount = 10f;

    [Header("Options")]
    [SerializeField] private bool onlyInPlayingState = true;

    public void Initialize(ScoreManager stageScoreManager, TimeManager stageTimeManager)
    {
        scoreManager = stageScoreManager;
        timeManager = stageTimeManager;
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (!CanUseCheat())
        {
            return;
        }

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            AddCheatScore();
        }

        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            AddCheatTime();
        }

        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            SubtractCheatTime();
        }

        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            ResetCheatTime();
        }
    }

    private bool CanUseCheat()
    {
        if (!onlyInPlayingState)
        {
            return true;
        }

        if (GameManager.Instance == null)
        {
            return false;
        }

        return GameManager.Instance.State == GameState.Playing;
    }

    private void AddCheatScore()
    {
        if (scoreManager == null)
        {
            Debug.LogError("[CheatTester] ScoreManager가 연결되지 않았습니다.");
            return;
        }

        scoreManager.AddScore(addScoreAmount);

        Debug.Log($"[CheatTester] F3 점수 추가: +{addScoreAmount}, 현재 점수: {scoreManager.CurrentScore}");
    }

    private void AddCheatTime()
    {
        if (timeManager == null)
        {
            Debug.LogError("[CheatTester] TimeManager가 연결되지 않았습니다.");
            return;
        }

        timeManager.AddTime(addTimeAmount);

        Debug.Log($"[CheatTester] F4 시간 추가: +{addTimeAmount}, 남은 시간: {timeManager.RemainingTime:F1}");
    }

    private void SubtractCheatTime()
    {
        if (timeManager == null)
        {
            Debug.LogError("[CheatTester] TimeManager가 연결되지 않았습니다.");
            return;
        }

        timeManager.SubtractTime(subtractTimeAmount);

        Debug.Log($"[CheatTester] F5 시간 감소: -{subtractTimeAmount}, 남은 시간: {timeManager.RemainingTime:F1}");
    }

    private void ResetCheatTime()
    {
        if (timeManager == null)
        {
            Debug.LogError("[CheatTester] TimeManager가 연결되지 않았습니다.");
            return;
        }

        timeManager.ResetTimer();

        Debug.Log($"[CheatTester] F6 시간 초기화: {timeManager.RemainingTime:F1}");
    }
}