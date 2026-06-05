using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("Result Settings")] [SerializeField]
    private int userId = 1;

    [SerializeField] private int stageId = 1;

    private int targetScore;
    private ScoreManager scoreManager;
    private DeliveryManager deliveryManager;
    private UIManager uiManager;
    private LeaderboardApiClient leaderboardApiClient;

    [Header("UI References")] public GameObject successPanel;
    public GameObject failPanel;
    public CanvasGroup dimCanvasGroup;
    public float delayTime = 3.0f;

    // GameManager로부터 필요한 매니저 연결
    public void Initialize(
        ScoreManager scoreManager,
        UIManager uiManager,
        DeliveryManager deliveryManager,
        LeaderboardApiClient leaderboardApiClient,
        int targetScore)
    {
        this.scoreManager = scoreManager;
        this.uiManager = uiManager;
        this.deliveryManager = deliveryManager;
        this.leaderboardApiClient = leaderboardApiClient;
        this.targetScore = targetScore;

        if (successPanel) successPanel.SetActive(false);
        if (failPanel) failPanel.SetActive(false);

        if (dimCanvasGroup)
        {
            dimCanvasGroup.gameObject.SetActive(false);
            dimCanvasGroup.alpha = 0f;
        }
    }

    public void SetStageId(int stageId)
    {
        this.stageId = stageId;
    }

    public void SetUserId(int userId)
    {
        this.userId = userId;
    }

    public void ProcessGameResult()
    {
        if (scoreManager == null || uiManager == null || leaderboardApiClient == null)
        {
            Debug.LogError("[ResultManager] 필요한 Manager가 연결되지 않았습니다.");
            return;
        }

        int finalScore = scoreManager.CurrentScore;
        bool isClear = finalScore >= targetScore;

        int totalBoxCount = 0;
        int smallBoxCount = 0;
        int bigBoxCount = 0;

        if (deliveryManager != null)
        {
            totalBoxCount = deliveryManager.TotalDeliveredCount;
            smallBoxCount = deliveryManager.SmallBoxDeliveredCount;
            bigBoxCount = deliveryManager.BigBoxDeliveredCount;
        }
        else
        {
            Debug.LogWarning("[ResultManager] DeliveryManager가 연결되지 않았습니다.");
        }

        StartCoroutine(ResultSequence(
            isClear,
            finalScore,
            totalBoxCount,
            smallBoxCount,
            bigBoxCount
        ));
    }

    private IEnumerator ResultSequence(
        bool isClear,
        int finalScore,
        int totalBoxCount,
        int smallBoxCount,
        int bigBoxCount)
    {
        // 화면 암전 연출
        if (dimCanvasGroup != null)
        {
            dimCanvasGroup.gameObject.SetActive(true);
            float timer = 0f;
            while (timer < delayTime)
            {
                timer += Time.deltaTime;
                dimCanvasGroup.alpha = timer / delayTime;
                yield return null;
            }
        }

        // 결과창에 점수 띄우기
        if (uiManager != null)
        {
            uiManager.ShowResultPanel(
                isClear,
                finalScore,
                smallBoxCount,
                bigBoxCount
            );
        }

        // 결과 패널 활성화 + 결과창 연결여부 확인
        if (successPanel != null)
        {
            successPanel.SetActive(isClear);
        }

        if (failPanel != null)
        {
            failPanel.SetActive(!isClear);
        }

        // 게임 일시정지
        Time.timeScale = 0f;

        // 데이터 저장 및 리더보드 갱신
        yield return StartCoroutine(SaveScoreThenLoadLeaderboard(finalScore));
    }

    private IEnumerator SaveScoreThenLoadLeaderboard(int finalScore)
    {
        bool saveSuccess = false;

        int currentUserId = AuthManager.LoginUserId;

        if (currentUserId == 0)
        {
            Debug.LogWarning("[ResultManager] 로그인 필요");
            currentUserId = this.userId;
            // yield break; 이용해서 로그인 안되면 실행 불가하게
        }

        yield return StartCoroutine(
            leaderboardApiClient.SaveScore(currentUserId, stageId, finalScore, success => { saveSuccess = success; })
        );
/*
            if (!saveSuccess)
            {
                uiManager.ShowLeaderboardError();
                yield break;
            }

            yield return StartCoroutine(
                leaderboardApiClient.LoadStageLeaderboard(
                    stageId,
                    items => { uiManager.ShowLeaderboard(items); },
                    () => { uiManager.ShowLeaderboardError(); }
                )
            );*/
    }

    public void GoToStageSelect()
    {
        ResumeTime();
        ChangeManager.stageSelectMemo = true;
        SceneManager.LoadScene("MainScene");
    }

    public void GoToLeaderboard()
    {
        ResumeTime();
        ChangeManager.leaderboardMemo = true;
        SceneManager.LoadScene("MainScene");
    }

    private void ResumeTime()
    {
        Time.timeScale = 1f;
    }
}