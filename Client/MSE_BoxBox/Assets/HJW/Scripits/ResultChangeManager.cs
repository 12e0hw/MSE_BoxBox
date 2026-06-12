using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HJW.scripts
{
    public class ResultChangeManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject successPanel; 
        public GameObject failPanel;
        // Used to create a screen-darkening
        public CanvasGroup dimCanvasGroup;

        public float delayTime = 3.0f;

        // Panel Reset
        void Start()
        {
            successPanel.SetActive(false);
            failPanel.SetActive(false);
            dimCanvasGroup.gameObject.SetActive(false);
            dimCanvasGroup.alpha = 0f;
        }

        // Triggers the end-game result sequence.
        public void StartResultSequence(bool isClear)
        {
            StartCoroutine(ShowResultCoroutine(isClear));
        }

        // Coroutine that handles the smooth background fade-in before showing the result panel
        private IEnumerator ShowResultCoroutine(bool isClear)
        {
            dimCanvasGroup.gameObject.SetActive(true);
            float timer = 0f;

            while (timer < delayTime)
            {
                timer += Time.deltaTime;
                dimCanvasGroup.alpha = timer / delayTime; 
                yield return null;
            }
            
            if(isClear) successPanel.SetActive(true);
            else failPanel.SetActive(true);

            Time.timeScale = 0f; 
        }

        public void Stage()
        {
            
            Time.timeScale = 1f;
            ChangeManager.stageSelectMemo = true;
            SceneManager.LoadScene("MainScene");
        }

        public void LeaderBoard()
        {
            ChangeManager.leaderboardMemo = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainScene"); 
        }
    }
}

