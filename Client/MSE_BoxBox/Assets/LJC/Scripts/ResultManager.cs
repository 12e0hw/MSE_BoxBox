using System.Collections;
using LJC.Scripts;
using UnityEngine;

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

            uiManager.ShowResultPanel(isClear, finalScore);

            StartCoroutine(SaveScoreThenLoadLeaderboard(finalScore));
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
    }
}