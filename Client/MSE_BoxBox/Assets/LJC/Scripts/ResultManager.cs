using System.Collections;
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
        private DeliveryManager deliveryManager;
        private UIManager uiManager;
        private LeaderboardApiClient leaderboardApiClient;

        [Header("UI References")]
        public GameObject successPanel;
        public GameObject failPanel;
        public CanvasGroup dimCanvasGroup;
        public float delayTime = 3.0f;

        // Connect required managers from the GameManager.
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

        // Set the current stage ID.
        public void SetStageId(int stageId)
        {
            this.stageId = stageId;
        }

        // Set the fallback user ID.
        public void SetUserId(int userId)
        {
            this.userId = userId;
        }

        // Calculate the final game result and start the result sequence.
        public void ProcessGameResult()
        {
            if (scoreManager == null || uiManager == null || leaderboardApiClient == null)
            {
                Debug.LogError("[ResultManager] Required managers are not assigned.");
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
                Debug.LogWarning("[ResultManager] DeliveryManager is not assigned.");
            }
            
            StartCoroutine(ResultSequence(
                isClear,
                finalScore,
                totalBoxCount,
                smallBoxCount,
                bigBoxCount
            ));
        }

        // Show the result UI, pause the game, and update leaderboard data.
        private IEnumerator ResultSequence(
            bool isClear,
            int finalScore,
            int totalBoxCount,
            int smallBoxCount,
            int bigBoxCount)
        {
            // Fade in the dim background.
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
            
            // Show score and box counts on the result UI.
            if (uiManager != null)
            {
                uiManager.ShowResultPanel(
                    isClear,
                    finalScore,
                    smallBoxCount,
                    bigBoxCount
                );
            }

            // Activate the correct result panel.
            if (successPanel != null)
            {
                successPanel.SetActive(isClear);
            }

            if (failPanel != null)
            {
                failPanel.SetActive(!isClear);
            }

            // Pause the game.
            Time.timeScale = 0f;

            // Save score and refresh the leaderboard.
            yield return StartCoroutine(SaveScoreThenLoadLeaderboard(finalScore));
        }

        // Save the final score and load the stage leaderboard.
        private IEnumerator SaveScoreThenLoadLeaderboard(int finalScore)
        {
            bool saveSuccess = false;

            int currentUserId = AuthManager.LoginUserId;

            if (currentUserId == 0)
            {
                Debug.LogWarning("[ResultManager] Login is required.");
                currentUserId = this.userId;
            }

            yield return StartCoroutine(
                leaderboardApiClient.SaveScore(currentUserId, stageId, finalScore, success => { saveSuccess = success; })
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
        
        // Return to the stage select screen.
        public void GoToStageSelect()
        {
            ResumeTime();
            ChangeManager.stageSelectMemo = true;
            SceneManager.LoadScene("MainScene");
        }

        // Return to the leaderboard screen.
        public void GoToLeaderboard()
        {
            ResumeTime();
            ChangeManager.leaderboardMemo = true;
            SceneManager.LoadScene("MainScene");
        }

        // Resume game time before changing scenes.
        private void ResumeTime()
        {
            Time.timeScale = 1f;
        }
    }
}