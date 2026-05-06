using System.Collections;
using LJC.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LJC.scripts
{
    public class ResultManager : MonoBehaviour
    {
        [Header("Result Settings")] [SerializeField]
        private int userId = 1;

        [SerializeField] private int stageId = 1;

        private int targetScore;
        private ScoreManager scoreManager;
        private UIManager uiManager;
        private LeaderboardApiClient leaderboardApiClient;

        [Header("UI References")]
        public GameObject successPanel;
        public GameObject failPanel;
        public CanvasGroup dimCanvasGroup;
        public float delayTime = 3.0f;

        // GameManager로부터 필요한 매니저 연결
        public void Initialize(
            ScoreManager scoreManager,
            UIManager uiManager,
            LeaderboardApiClient leaderboardApiClient,
            int targetScore)
        {
            this.scoreManager = scoreManager;
            this.uiManager = uiManager;
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

            StartCoroutine(ResultSequence(isClear, finalScore));
        }

        private IEnumerator ResultSequence(bool isClear, int finalScore)
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

            // 결과 패널 활성화
            if (isClear) successPanel.SetActive(true);
            else failPanel.SetActive(true);

            // 게임 일시정지
            Time.timeScale = 0f;

            // 데이터 저장 및 리더보드 갱신
            yield return StartCoroutine(SaveScoreThenLoadLeaderboard(finalScore));
        }

        private IEnumerator SaveScoreThenLoadLeaderboard(int finalScore)
        {
            bool saveSuccess = false;

            yield return StartCoroutine(
                leaderboardApiClient.SaveScore(userId, stageId, finalScore, success => { saveSuccess = success; })
            );

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
            );
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
}