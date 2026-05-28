using UnityEngine;

public class StageSceneBootstrap : MonoBehaviour
{
    [Header("Stage Config")]
    [SerializeField] private StageConfig stageConfig;

    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private BoxSpawner boxSpawnManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private LJC.scripts.ResultManager resultManager;
    [SerializeField] private LeaderboardApiClient leaderboardApiClient;

    [Header("Test")]
    [SerializeField] private CheatTester cheatTester;
    
    [Header("Stage UI")]
    [SerializeField] private GameObject hudUI;
    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject settingUI;

    public StageConfig StageConfig => stageConfig;

    public ScoreManager ScoreManager => scoreManager;
    public TimeManager TimeManager => timeManager;
    public DeliveryManager DeliveryManager => deliveryManager;
    public BoxSpawner BoxSpawnManager => boxSpawnManager;
    public UIManager UIManager => uiManager;
    public LJC.scripts.ResultManager ResultManager => resultManager;
    public LeaderboardApiClient LeaderboardApiClient => leaderboardApiClient;

    public GameObject HudUI => hudUI;
    public GameObject ClearUI => clearUI;
    public GameObject GameoverUI => gameoverUI;
    public GameObject PauseUI => pauseUI;
    public GameObject SettingUI => settingUI;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[StageSceneBootstrap] GameManager Instance가 없습니다.");
            return;
        }

        GameManager.Instance.InitializeStage(this);
    }
    
    private void Start()
    {
        InitializeStageReferences();
    }

    private void InitializeStageReferences()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[StageSceneBootstrap] GameManager Instance가 없어 Stage 초기화를 중단합니다.");
            return;
        }

        GameManager.Instance.InitializeStage(this);

        if (cheatTester != null)
        {
            cheatTester.Initialize(scoreManager, timeManager);
        }
    }

    public void OnResumeButtonClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.ResumeGame();
    }

    public void OnOpenSettingsButtonClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.OpenSetting();
    }

    public void OnCloseSettingsButtonClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.CloseSetting();
    }

    public void OnBackButtonClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.BacktoStage();
    }
}