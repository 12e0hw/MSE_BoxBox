using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreCheatTester : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;

    [Header("Cheat Settings")]
    [SerializeField] private int addScoreAmount = 20;
    [SerializeField] private bool onlyInPlayingState = true;

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (onlyInPlayingState && GameManager.Instance != null)
        {
            if (GameManager.Instance.State != GameState.Playing)
            {
                return;
            }
        }

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            AddCheatScore();
        }
    }

    private void AddCheatScore()
    {
        if (scoreManager == null)
        {
            Debug.LogError("[ScoreCheatTester] ScoreManager가 연결되지 않았습니다.");
            return;
        }

        scoreManager.AddScore(addScoreAmount);

        Debug.Log($"[ScoreCheatTester] F3 치트 점수 추가: +{addScoreAmount}, 현재 점수: {scoreManager.CurrentScore}");
    }
}