using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,      // 게임 시작 전 (인트로/대기)
    Playing,  
    Clear,     
    Gameover    
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private GameState state = GameState.Ready;
    public int currentScore = 0;
    public int targetScore = 100;
    public float timeLimit = 120f; 

    [Header("UI References")]
    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject hudUI; // 현재 점수, 시간 

    private void Awake()
    {
        // 싱글톤 유지
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(GameState.Ready);
    }

    public void SetState(GameState newState)
    {
        if (state == newState) return;
        state = newState;

        switch (newState)
        {
            case GameState.Ready:
                currentScore = 0;
                HideAllUI();
                break;
            case GameState.Playing:
                if (hudUI) hudUI.SetActive(true);
                break;
            case GameState.Clear:
                ShowClearUI();
                break;
            case GameState.Gameover:
                ShowGameoverUI();
                break;
        }
    }
    
    public void AddScore(int amount)
    {
        if (state != GameState.Playing) return;

        currentScore += amount;
        Debug.Log($"Score: {currentScore} / {targetScore}");

        if (currentScore >= targetScore)
        {
            SetState(GameState.Clear);
        }
    }
    
    private void ShowClearUI()
    {
        if (clearUI) clearUI.SetActive(true);
        // 서버(Spring Boot)로 결과 전송 로직 실행 필요
    }

    private void ShowGameoverUI()
    {
        if (gameoverUI) gameoverUI.SetActive(true);
    }

    private void HideAllUI()
    {
        if (clearUI) clearUI.SetActive(false);
        if (gameoverUI) gameoverUI.SetActive(false);
    }
}