using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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
    
    // Stage마다 Manager 연결을 담당
    private StageConfig currentStageConfig;
    
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
    // 게임 성공/실패 판단
    private bool isCleared;
    
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

        // Manager 연결은 StageSceneBootstrap에서 연결 담당
    
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
        //RegisterStageEvents가 이벤트 연결을 담당
    }

    private void OnDisable()
    {
        //UnregisterStageEvents가 이벤트 해제를 담당
    }
    
    // Manager 연결 함수
    public void InitializeStage(StageSceneBootstrap bootstrap)
    {
        UnregisterStageEvents();

        currentStageConfig = bootstrap.StageConfig;

        if (currentStageConfig != null)
        {
            selectStage = currentStageConfig.StageId;
            targetScore = currentStageConfig.TargetScore;
        }

        scoreManager = bootstrap.ScoreManager;
        timeManager = bootstrap.TimeManager;
        deliveryManager = bootstrap.DeliveryManager;
        boxSpawnManager = bootstrap.BoxSpawnManager;
        uiManager = bootstrap.UIManager;
        resultManager = bootstrap.ResultManager;
        leaderboardApiClient = bootstrap.LeaderboardApiClient;

        // hudUI = bootstrap.HudUI;
        // clearUI = bootstrap.ClearUI;
        // gameoverUI = bootstrap.GameoverUI;

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
            resultManager.Initialize(
                scoreManager,
                uiManager,
                deliveryManager,
                leaderboardApiClient,
                targetScore
            );
        }

        if (timeManager != null && currentStageConfig != null)
        {
            timeManager.SetStartTime(currentStageConfig.TimeLimit);
        }

        if (uiManager != null && timeManager != null)
        {
            uiManager.InitializeTimer(timeManager.StartTime);
        }

        RegisterStageEvents();

        SetState(GameState.Playing, true);
    }
    
    // 스테이지마다 이벤트 연결을 담당
    private void RegisterStageEvents()
    {
        if (scoreManager != null && uiManager != null)
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

    // 스테이지마다 이벤트 해체를 담당
    private void UnregisterStageEvents()
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
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            UnregisterStageEvents();
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

                if (previousState == GameState.Paused)
                {
                    if (settingUI != null)
                    {
                        settingUI.SetActive(false);
                    }

                    break;
                }

                isGameEnded = false;
                isCleared = false;

                // 스코어 초기화
                if (scoreManager != null)
                {
                    scoreManager.SetTargetScore(targetScore);
                    scoreManager.ResetScore();
                }

                // 박스 결과 초기화
                if (deliveryManager != null)
                {
                    deliveryManager.ResetDeliveryCounts();
                }

                //타이머 초기화
                if (timeManager != null)
                {
                    timeManager.ResetTimer();
                    timeManager.StartTimer();
                }

                // UI 점수 초기화
                if (uiManager != null)
                {
                    uiManager.HideResultPanel();
                }
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

        // StageSceneBootstrap이 InitializeStage()를 호출함
        //SetState(GameState.Playing);
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
        isCleared = true;
    }
    
    // 게임 종료 이벤트용 함수
    private void HandleTimeOver()
    {
        // 점수를 직접 확인하여 게임 종료 여부를 확인하는 방식으로 변환
        bool reachedTargetScore = scoreManager != null && scoreManager.CurrentScore >= targetScore;

        if (reachedTargetScore)
        {
            EndGame(GameState.Clear);
        }
        else
        {
            EndGame(GameState.Gameover);
        }
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