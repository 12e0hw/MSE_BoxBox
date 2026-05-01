using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager1 : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider loadingSlider;
    public float loadingTime = 3.0f;
    private float currentLoadingTime = 0f;
    private bool loadingOver = false;
    private string newSceneName = "";

    void Start()
    {
        if(loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    void Update()
    {
        if(!loadingOver) return;
        currentLoadingTime += Time.deltaTime;
        loadingSlider.value = currentLoadingTime / loadingTime;

        if(currentLoadingTime >= loadingTime)
        {
            loadingOver = false;
            loadingSlider.value = 1f;
            SceneManager.LoadScene(newSceneName);
        }
    }

    private void Loading(string newScene)
    {
        newSceneName = newScene;
        currentLoadingTime = 0f;
        loadingSlider.value = 0f;
        loadingPanel.SetActive(true);
        loadingOver = true;
    }

    public void StartGame()
    {
        Loading("Stage1");
        Debug.Log("Stage1");
    }

    public void AnotherStage()
    {
        Loading("Stage2");
        Debug.Log("Stage2");
    }

    public void SelectLeaderboard()
    {
        SceneManager.LoadScene("LeaderboardMenu");
        Debug.Log("Leaderboard");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit game");
    }
}
