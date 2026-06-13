using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("In-Game UI")] [SerializeField]
    private TMP_Text timerText;

    [SerializeField] private Slider timeGauge;
    [SerializeField] private TMP_Text scoreText;

    [Header("Result UI")] [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultTitleText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text smallBoxCountText;
    [SerializeField] private TMP_Text bigBoxCountText;
    
    [Header("Warning UI")]
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private CanvasGroup warningCanvas;
    [SerializeField] private float warningMinAlpha = 0.08f;
    [SerializeField] private float warningMaxAlpha = 0.32f;
    [SerializeField] private float warningBlinkSpeed = 1.5f;

    private bool isWarningActive;
    
    [Header("Leaderboard UI")] [SerializeField]
    private TMP_Text[] leaderboardSlots;

    private void Awake()
    {
        HideWarningPanel();
    }

    private void OnEnable()
    {
        if (timeManager == null)
        {
            return;
        }

        timeManager.OnWarningStarted += ShowWarningPanel;
        timeManager.OnWarningStopped += HideWarningPanel;
    }

    private void OnDisable()
    {
        if (timeManager == null)
        {
            return;
        }

        timeManager.OnWarningStarted -= ShowWarningPanel;
        timeManager.OnWarningStopped -= HideWarningPanel;
    }
    
    private void Update()
    {
        UpdateWarningPanelAlpha();
    }
    
    // Initialize the timer text and gauge.
    public void InitializeTimer(float maxTime)
    {
        if (timeGauge != null)
        {
            timeGauge.minValue = 0f;
            timeGauge.maxValue = maxTime;
            timeGauge.value = maxTime;
        }

        UpdateTimer(maxTime);
    }

    // Update the timer text and gauge.
    public void UpdateTimer(float remainingTime)
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = $"{seconds}";
        }

        if (timeGauge != null)
        {
            timeGauge.value = remainingTime;
        }
    }

    // Update the score text.
    public void UpdateScore(int score)
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = $"{score}";
    }

    // Hide the result panel and result texts.
    public void HideResultPanel()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        
        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(false);
        }

        if (smallBoxCountText != null)
        {
            smallBoxCountText.gameObject.SetActive(false);
        }

        if (bigBoxCountText != null)
        {
            bigBoxCountText.gameObject.SetActive(false);
        }
    }

    // Show the result panel with final score and box counts.
    public void ShowResultPanel(
        bool isClear,
        int finalScore,
        int smallBoxCount,
        int bigBoxCount)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultTitleText != null)
        {
            resultTitleText.text = isClear ? "Stage Clear" : "Stage Failed";
        }

        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(true);
            finalScoreText.text = $"Final Score: {finalScore}";
        }

        if (smallBoxCountText != null)
        {
            smallBoxCountText.gameObject.SetActive(true);
            smallBoxCountText.text = $"{smallBoxCount}";
        }

        if (bigBoxCountText != null)
        {
            bigBoxCountText.gameObject.SetActive(true);
            bigBoxCountText.text = $"{bigBoxCount}";
        }

        ShowLeaderboardLoading();
    }

    // Show a loading message while leaderboard data is loading.
    public void ShowLeaderboardLoading()
    {
        ClearLeaderboardSlots();

        if (leaderboardSlots != null && leaderboardSlots.Length > 0 && leaderboardSlots[0] != null)
        {
            leaderboardSlots[0].text = "Loading leaderboard...";
        }
    }

    // Display leaderboard data in the leaderboard slots.
    public void ShowLeaderboard(LeaderboardItem[] items)
    {
        ClearLeaderboardSlots();

        if (leaderboardSlots == null || leaderboardSlots.Length == 0)
        {
            return;
        }

        if (items == null || items.Length == 0)
        {
            leaderboardSlots[0].text = "No leaderboard data.";
            return;
        }

        int count = Mathf.Min(10, leaderboardSlots.Length, items.Length);

        for (int i = 0; i < count; i++)
        {
            LeaderboardItem item = items[i];
            leaderboardSlots[i].text = $"{item.rank}. {item.username} - {item.score}";
        }
    }

    // Show a leaderboard loading error message.
    public void ShowLeaderboardError()
    {
        ClearLeaderboardSlots();

        if (leaderboardSlots != null && leaderboardSlots.Length > 0 && leaderboardSlots[0] != null)
        {
            leaderboardSlots[0].text = "Failed to load leaderboard.";
        }
    }

    // Show a score save error message.
    public void ShowScoreSaveError()
    {
        ClearLeaderboardSlots();

        if (leaderboardSlots != null && leaderboardSlots.Length > 0 && leaderboardSlots[0] != null)
        {
            leaderboardSlots[0].text = "Failed to save score.";
        }
    }

    // Clear all leaderboard text slots.
    private void ClearLeaderboardSlots()
    {
        if (leaderboardSlots == null)
        {
            return;
        }

        for (int i = 0; i < leaderboardSlots.Length; i++)
        {
            if (leaderboardSlots[i] != null)
            {
                leaderboardSlots[i].text = "";
            }
        }
    }
    
    // Show the warning panel.
    public void ShowWarningPanel()
    {
        isWarningActive = true;

        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
        }

        if (warningCanvas != null)
        {
            warningCanvas.alpha = warningMaxAlpha;
        }
    }

    // Hide the warning panel.
    public void HideWarningPanel()
    {
        isWarningActive = false;

        if (warningCanvas != null)
        {
            warningCanvas.alpha = 0f;
        }

        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    // Update the warning panel blink alpha.
    private void UpdateWarningPanelAlpha()
    {
        if (!isWarningActive)
        {
            return;
        }

        if (warningCanvas == null)
        {
            return;
        }

        float wave = (Mathf.Sin(Time.time * warningBlinkSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
        float smoothWave = Mathf.SmoothStep(0f, 1f, wave);

        warningCanvas.alpha = Mathf.Lerp(
            warningMinAlpha,
            warningMaxAlpha,
            smoothWave
        );
    }
}
