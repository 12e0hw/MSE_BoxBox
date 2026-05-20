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

    [Header("Stage UI")]
    [SerializeField] private GameObject hudUI;
    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject gameoverUI;

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

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[StageSceneBootstrap] GameManager Instance가 없습니다.");
            return;
        }

        GameManager.Instance.InitializeStage(this);
    }
}