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

    // Set stage managers used by the cheat tester.
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

    // Check whether cheat keys can be used in the current game state.
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

    // Add test score with the F3 key.
    private void AddCheatScore()
    {
        if (scoreManager == null)
        {
            Debug.LogError("[CheatTester] ScoreManager is not assigned.");
            return;
        }

        scoreManager.AddScore(addScoreAmount);

        Debug.Log($"[CheatTester] F3 Add Score: +{addScoreAmount}, Current Score: {scoreManager.CurrentScore}");
    }

    // Add test time with the F4 key.
    private void AddCheatTime()
    {
        if (timeManager == null)
        {
            Debug.LogError("[CheatTester] TimeManager is not assigned.");
            return;
        }

        timeManager.AddTime(addTimeAmount);

        Debug.Log($"[CheatTester] F4 Add Time: +{addTimeAmount}, Remaining Time: {timeManager.RemainingTime:F1}");
    }

    // Subtract test time with the F5 key.
    private void SubtractCheatTime()
    {
        if (timeManager == null)
        {
            Debug.LogError("[CheatTester] TimeManager is not assigned.");
            return;
        }

        timeManager.SubtractTime(subtractTimeAmount);

        Debug.Log($"[CheatTester] F5 Subtract Time: -{subtractTimeAmount}, Remaining Time: {timeManager.RemainingTime:F1}");
    }

    // Reset test time with the F6 key.
    private void ResetCheatTime()
    {
        if (timeManager == null)
        {
            Debug.LogError("[CheatTester] TimeManager is not assigned.");
            return;
        }

        timeManager.ResetTimer();

        Debug.Log($"[CheatTester] F6 Reset Time: {timeManager.RemainingTime:F1}");
    }
}