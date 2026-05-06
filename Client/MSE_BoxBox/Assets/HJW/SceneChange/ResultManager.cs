using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject successPanel;     
    public GameObject failPanel;        
    public CanvasGroup dimCanvasGroup;

    private bool isResultShowing = false;
    
    public float delayTime = 3.0f;
    public int targetScore = 100;
    private int score = 200; // 체크용


    void Start()
    {
        successPanel.SetActive(false);
        failPanel.SetActive(false);
        dimCanvasGroup.gameObject.SetActive(false);
        dimCanvasGroup.alpha = 0f;
        isResultShowing = false;
        
        Time.timeScale = 1f; 
    }

    void Update()
    {
        if(!isResultShowing && TimeGauage.timeOver){
            isResultShowing = true;
            bool isClear = (score >= targetScore);
            StartCoroutine(ShowResultCoroutine(isClear));
        }
    }

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
        
        if(isClear == true)
        {
            successPanel.SetActive(true);
        }
        else
        {
            failPanel.SetActive(true);
        }

        Time.timeScale = 0f; 
    }


    public void Stage()
    {
        Time.timeScale = 1f;
        ChangeManager.stageSelectMemo = true;
        SceneManager.LoadScene("ChangeScene");
    }

    public void LeaderBoard()
    {
        ChangeManager.leaderboardMemo = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("ChangeScene"); 
    }
}

