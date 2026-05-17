using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using LJC.Scripts;
using UnityEngine.InputSystem;

public enum GameState
{
    StartMenu,      
    StageSelect,   
    Loading,
    Playing, 
    Paused, 
    Clear,     
    Gameover    
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int selectStage = 1;   // 선택 스테이지 번호


    [Header("Game Settings")]
    [SerializeField] private GameState state = GameState.StartMenu;
    [SerializeField] private int targetScore = 100;
    
    public GameState State => state;
    public int TargetScore => targetScore;
    
    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private BoxSpawner boxSpawnManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private LJC.scripts.ResultManager resultManager;
    [SerializeField] private LeaderboardApiClient leaderboardApiClient;

    [Header("UI")]
    // [SerializeField] private GameObject startMenuUI;
    // [SerializeField] private GameObject stageSelectUI;
    // [SerializeField] private GameObject loadingUI;
    // [SerializeField] private GameObject hudUI;
    // [SerializeField] private GameObject clearUI;
    // [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject settingUI;

    // 게임 끝났을 때 Ture로 변경
    private bool isGameEnded;
    
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
        
        // 클리어 점수 설정
        if (scoreManager != null)
        {
            scoreManager.SetTargetScore(targetScore);
        }
        
        if (deliveryManager != null)
        {
            deliveryManager.Initialize(scoreManager);
        }
        
        if (resultManager != null)
        {
            resultManager.Initialize(scoreManager, uiManager, leaderboardApiClient, targetScore);
        }

        if (uiManager != null && timeManager != null)
        {
            uiManager.InitializeTimer(timeManager.StartTime);
        }
    
    }

    
    private void Start()
    {
        //SetState(GameState.StartMenu);
        
        // play화면에서 실행
        SetState(GameState.Playing, true);
    }

    private void Update()
    {
        if (state == GameState.Playing || state == GameState.Paused)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
        }
    }
    
    public void SelectStage(int stageNum)
    {
        selectStage = stageNum;
        Debug.Log($"Selected Stage: {selectStage}");
    } 

    private void OnEnable()
    {
        if (scoreManager != null  && uiManager != null)
        {
            scoreManager.OnScoreChanged += uiManager.UpdateScore;
            scoreManager.OnMaxScoreReached += HandleMaxScoreReached;
        }

        if (timeManager != null)
        {
            if (uiManager != null)
            {
                timeManager.OnTimeChanged += uiManager.UpdateTimer;
            }

            timeManager.OnTimeOver += HandleTimeOver;
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null && uiManager != null)
        {
            scoreManager.OnScoreChanged -= uiManager.UpdateScore;
            scoreManager.OnMaxScoreReached -= HandleMaxScoreReached;
        }

        if (timeManager != null)
        {
            if (uiManager != null)
            {
                timeManager.OnTimeChanged -= uiManager.UpdateTimer;
            }

            timeManager.OnTimeOver -= HandleTimeOver;
        }
    }
    //테스트용으로 playing 화면에서 실행하게 바꿈
    public void SetState(GameState newState, bool force = false)
    {
        if (!force && state == newState)
        {
            return;  // 같은 State라서 return
        }

        GameState previousState = state;

        //if (state == newState) return;
        state = newState;

        HideAllUI();

        switch (newState)
        {
            // case GameState.StartMenu:
            //     Time.timeScale = 1f;
            //     if (startMenuUI) startMenuUI.SetActive(true);
            //     break;
            // case GameState.StageSelect:
            //     if (stageSelectUI) stageSelectUI.SetActive(true);
            //     break;
            // case GameState.Loading:
            //     if (loadingUI) loadingUI.SetActive(true);
            //     break;
            case GameState.Playing:
                Time.timeScale = 1f;
                if (previousState != GameState.Paused)
                {
                    isGameEnded = false;
                    if (scoreManager != null) scoreManager.ResetScore();
                    // 배송 상자 개수 초기화
                    if (deliveryManager != null) deliveryManager.ResetDeliveryCounts();
                    // 타이머 초기화
                    if (timeManager != null)
                    {
                        timeManager.ResetTimer();
                        timeManager.StartTimer();
                    }
                    if (uiManager != null) uiManager.HideResultPanel();
                }
                else
                {
                    settingUI.SetActive(false);
                }
                if (timeManager != null) timeManager.StartTimer();
                // if (hudUI) hudUI.SetActive(true);
                break;
            case GameState.Paused:
                Time.timeScale = 0f; 
                if (settingUI) settingUI.SetActive(true); 
                break;
            case GameState.Clear:
                Time.timeScale = 1f;
                if (timeManager) timeManager.StopTimer();
                // if (clearUI) clearUI.SetActive(true);
                break;
            case GameState.Gameover:
                Time.timeScale = 1f;
                if (timeManager) timeManager.StopTimer();
                // if (gameoverUI) gameoverUI.SetActive(true);
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
        // if (startMenuUI) startMenuUI.SetActive(false);
        // if (stageSelectUI) stageSelectUI.SetActive(false);
        // if (loadingUI) loadingUI.SetActive(false);
        // if (hudUI) hudUI.SetActive(false);
        // if (clearUI) clearUI.SetActive(false);
        // if (gameoverUI) gameoverUI.SetActive(false);
        if (settingUI) settingUI.SetActive(false);
    }
    
    private void HandleMaxScoreReached()
    {
        EndGame(GameState.Clear);
    }
    
    // 게임 종료 이벤트용 함수
    private void HandleTimeOver()
    {
        EndGame(GameState.Gameover);
    }
    
    // 게임 종료 시 관련 기능
    private void EndGame(GameState endState)
    {
        if (isGameEnded)
        {
            return;
        }

        isGameEnded = true;

        if (timeManager != null)
        {
            timeManager.StopTimer();
        }

        SetState(endState);

        if (resultManager != null)
        {
            resultManager.SetStageId(selectStage);
            resultManager.ProcessGameResult();
        }
    }

    public void TogglePause()
    {
        if (state == GameState.Playing)
        {
            SetState(GameState.Paused);
        }
        else if (state == GameState.Paused)
        {
            SetState(GameState.Playing);
        }
    }
}