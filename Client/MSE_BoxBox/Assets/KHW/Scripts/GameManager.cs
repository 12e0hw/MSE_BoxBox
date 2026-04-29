using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState
{
    StartMenu,      
    StageSelect,   
    Loading,
    Playing,  
    Clear,     
    Gameover    
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int selectStage = 1;   // 선택 스테이지 번호


    [Header("Game Settings")]
    [SerializeField] private GameState state = GameState.StartMenu;
    public GameState State => state;
    public int targetScore = 100;

    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimeManager timeManager;
    // [SerializeField] private DeliveryManager deliveryManager;

    [Header("UI")]
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private GameObject stageSelectUI;
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private GameObject hudUI;
    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject gameoverUI;

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
        SetState(GameState.StartMenu);
    }

    public void SelectStage(int stageNum)
    {
        selectStage = stageNum;
        Debug.Log(stageNum);
    } 

    private void OnEnable()
    {
        if (scoreManager != null)
        {
           // scoreManager.On머시라 += () => SetState(GameState.Clear);
           // 점수 도달 시 호출 구현 필요
        }

        if (timeManager != null)
        {
            timeManager.OnTimeOver += () => SetState(GameState.Gameover);
        }
    }

    public void SetState(GameState newState)
    {
        if (state == newState) return;
        state = newState;

        HideAllUI();

        switch (newState)
        {
            case GameState.StartMenu:
                Time.timeScale = 1f;
                if (startMenuUI) startMenuUI.SetActive(true);
                break;
            case GameState.StageSelect:
                if (stageSelectUI) stageSelectUI.SetActive(true);
                break;
            case GameState.Loading:
                if (loadingUI) loadingUI.SetActive(true);
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                if(timeManager) timeManager.StartTimer();
                if (hudUI) hudUI.SetActive(true);
                break;
            case GameState.Clear:
                if(timeManager) timeManager.StopTimer();
                if (clearUI) clearUI.SetActive(true);
                break;
            case GameState.Gameover:
                Time.timeScale = 0f;
                if (gameoverUI) gameoverUI.SetActive(true);
                break;
        }
    }
    
    // 스테이지 선택 화면으로 가기
    public void GoToStageSelect() => SetState(GameState.StageSelect);

    // 실제 게임 시작 (로딩 거쳐서 이동)
    public void StartGame()
    {
        StartCoroutine(LoadStageAsync("Stage" + selectStage));
    }

    private IEnumerator LoadStageAsync(string sceneName)
    {
        SetState(GameState.Loading);
        yield return new WaitForSeconds(1.0f); // 최소 로딩 연출 시간

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        SetState(GameState.Playing);
    }

    public void RestartStage()
    {
        StartGame(); // 현재 선택된 스테이지 다시 시작
    }

   /* public void BackToMain()
    {
        
        SetState(GameState.StartMenu);
    }
    */


    private void HideAllUI()
    {
        if (startMenuUI) startMenuUI.SetActive(false);
        if (stageSelectUI) stageSelectUI.SetActive(false);
        if (loadingUI) loadingUI.SetActive(false);
        if (hudUI) hudUI.SetActive(false);
        if (clearUI) clearUI.SetActive(false);
        if (gameoverUI) gameoverUI.SetActive(false);
    }
}