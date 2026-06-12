using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.UI;

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

    public int selectStage = 1;   // Selected stage number.

    [Header("Audio Settings")]
    public AudioMixer audioMixer;


    [Header("Game Settings")]
    [SerializeField] private GameState state = GameState.StartMenu;
    [SerializeField] private int targetScore = 30;
    
    public GameState State => state;
    
    // Holds stage-specific manager references.
    private StageConfig currentStageConfig;
    
    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private LJC.scripts.ResultManager resultManager;
    [SerializeField] private LeaderboardApiClient leaderboardApiClient;
    [SerializeField] private BoxSpawner boxSpawner;

    [Header("UI")]
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject pauseUI; // Pause menu UI.

    // Prevents the end-game flow from running more than once.
    private bool isGameEnded;
    // Tracks whether the target score was reached.
    private bool isCleared;
    
    private void Awake()
    {
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
        SetState(GameState.StartMenu, true);
    }

    private void Update()
    {
        if (state == GameState.Playing)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                PauseGame();
            }
        }
    }
    
    public void SelectStage(int stageNum)
    {
        selectStage = stageNum;
        Debug.Log($"Selected Stage: {selectStage}");
    } 
    
    // Connects stage managers and config values from the bootstrap.
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
        uiManager = bootstrap.UIManager;
        resultManager = bootstrap.ResultManager;
        leaderboardApiClient = bootstrap.LeaderboardApiClient;
        pauseUI = bootstrap.PauseUI;
        settingUI = bootstrap.SettingUI;

        boxSpawner = bootstrap.BoxSpawnManager;

        if (boxSpawner != null && currentStageConfig != null)
        {
            boxSpawner.SetSpawnInterval(currentStageConfig.BoxSpawnInterval);
        }

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
    
    // Registers stage events after loading a stage.
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

    // Unregisters events before reconnecting managers or destroying the singleton.
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
    
    // Central state transition handler.
    public void SetState(GameState newState, bool force = false)
    {
        if (!force && state == newState)
        {
            return;  // Ignore duplicate state changes.
        }

        GameState previousState = state;

        state = newState;

        HideAllUI();

        switch (newState)
        {
            case GameState.StartMenu:
                Time.timeScale = 1f;
                // ChangeManager handles the login screen panel.
                break;

            case GameState.StageSelect:
                Time.timeScale = 1f;
                // ChangeManager and LeaderScene handle stage and leaderboard panels.
                break;
            case GameState.Loading:
                Time.timeScale = 1f;

                if (BGM_Manager.instance != null)
                {
                    BGM_Manager.instance.PauseBGM();
                }

                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                
                if (BGM_Manager.instance != null)
                {
                    BGM_Manager.instance.PlayBGM();
                }

                if (previousState == GameState.Paused)
                {
                    if (settingUI != null)
                    {
                        if (pauseUI != null) pauseUI.SetActive(false);
                        settingUI.SetActive(false);
                    }

                    break;
                }

                isGameEnded = false;
                isCleared = false;

                if (boxSpawner != null)
                {
                    boxSpawner.ResetSpawner();
                    boxSpawner.StartSpawning();
                }
                
                // Reset score data for a new run.
                if (scoreManager != null)
                {
                    scoreManager.SetTargetScore(targetScore);
                    scoreManager.ResetScore();
                }

                // Reset delivery result counts.
                if (deliveryManager != null)
                {
                    deliveryManager.ResetDeliveryCounts();
                }

                // Reset and start the stage timer.
                if (timeManager != null)
                {
                    timeManager.ResetTimer();
                    timeManager.StartTimer();
                }

                // Hide old result UI before gameplay starts.
                if (uiManager != null)
                {
                    uiManager.HideResultPanel();
                }
                break;
            case GameState.Paused:
                Time.timeScale = 0f; 
                if (pauseUI) pauseUI.SetActive(true);
                break;
            case GameState.Clear:
                Time.timeScale = 1f;
                if (timeManager) timeManager.StopTimer();
                if (boxSpawner != null)
                {
                    boxSpawner.StopSpawning();
                }

                if (isCleared)
                {
                    
                }
                break;
            case GameState.Gameover:
                Time.timeScale = 1f;
                if (timeManager) timeManager.StopTimer();
                if (boxSpawner != null)
                {
                    boxSpawner.StopSpawning();
                }
                break;
        }
    }
    
    // Move to the stage select state.
    public void GoToStageSelect() => SetState(GameState.StageSelect);

    // Start the selected stage after the loading state.
    public void StartGame()
    {
        StartCoroutine(LoadStageAsync("Stage" + selectStage));
    }

    private IEnumerator LoadStageAsync(string sceneName)
    {
        SetState(GameState.Loading);
        yield return new WaitForSeconds(1.0f); // Minimum loading presentation time.

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    public void RestartStage()
    {
        StartGame(); // Restart the currently selected stage.
    }

    private void HideAllUI()
    {
        if (settingUI) settingUI.SetActive(false);
        if (pauseUI) pauseUI.SetActive(false);
    }
    
    private void HandleMaxScoreReached()
    {
        isCleared = true;
    }
    
    // Ends the game when the timer reaches zero.
    private void HandleTimeOver()
    {
        // Check the current score directly to decide clear or game over.
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
    
    // Runs shared end-game behavior.
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

    public void PauseGame()
    {
        SetState(GameState.Paused);
    }
    public void ResumeGame()
    {
        SetState(GameState.Playing);
    }
    public void OpenSetting()
    {
        if (settingUI) settingUI.SetActive(true);
    }
    public void CloseSetting()
    {
        if (settingUI) settingUI.SetActive(false);
    }
    public void BacktoStage()
    {
        Time.timeScale = 1f;
        SetState(GameState.StageSelect);
        ChangeManager.stageSelectMemo = true; 
        SceneManager.LoadScene("MainScene");
    }
}
