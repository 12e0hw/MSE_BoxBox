using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject LoginPanel;
    public GameObject SettingPanel;
    public GameObject SignupPanel;
    public GameObject StageSelectPanel;
    public GameObject LeaderboardPanel;
    public GameObject loadingPanel;

    [Header("Memo")]
    public static bool stageSelectMemo = false;
    public static bool leaderboardMemo = false;

    [Header("Loading Info")]
    public Slider loadingSlider;
    public float loadingTime = 3.0f;
    private float currentLoadingTime = 0f;
    private bool loadingOver = false;
    private string newSceneName = "";

    void Start()
    {
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (LeaderboardPanel != null) LeaderboardPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);

        if (stageSelectMemo)
        {
            StageSelect();
            stageSelectMemo = false;
        }
        else if (leaderboardMemo)
        {
            SelectLeaderboard();
            leaderboardMemo = false;
        }
    }

    void Update()
    {
        if (!loadingOver) return;

        currentLoadingTime += Time.deltaTime;
        
        if (loadingSlider != null)
        {
            loadingSlider.value = currentLoadingTime / loadingTime;
        }

        if (currentLoadingTime >= loadingTime)
        {
            loadingOver = false;
            if (loadingSlider != null) loadingSlider.value = 1f;
            SceneManager.LoadScene(newSceneName);
        }
    }

    public void StageSelect() 
    {
        if (StageSelectPanel != null) StageSelectPanel.SetActive(true);
        
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (LeaderboardPanel != null) LeaderboardPanel.SetActive(false);
    }

    public void Login()
    {
        if (LoginPanel != null) LoginPanel.SetActive(true);
        
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (LeaderboardPanel != null) LeaderboardPanel.SetActive(false);
    }

    public void Signup()
    {
        if (SignupPanel != null) SignupPanel.SetActive(true);
        
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (LeaderboardPanel != null) LeaderboardPanel.SetActive(false);
    }

    public void Register()
    {
        Debug.Log("Register confirm");
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (LoginPanel != null) LoginPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        if (SettingPanel != null) SettingPanel.SetActive(true);
        
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (LeaderboardPanel != null) LeaderboardPanel.SetActive(false);
    }

    public void SelectLeaderboard()
    {
        if (LeaderboardPanel != null) LeaderboardPanel.SetActive(true);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
    }

    public void BacktoStage()
    {
        if (StageSelectPanel != null) StageSelectPanel.SetActive(true);
        if (LeaderboardPanel != null) LeaderboardPanel.SetActive(false);
    }

    public void Back()
    {
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (LeaderboardPanel != null) LeaderboardPanel.SetActive(false);
    }

    private void Loading(string newScene)
    {
        newSceneName = newScene;
        currentLoadingTime = 0f;
        if (loadingSlider != null) loadingSlider.value = 0f;
        
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        
        if (loadingPanel != null) loadingPanel.SetActive(true);
        loadingOver = true;
    }

    public void Stage1() 
    {
        Loading("Stage1");
        Debug.Log("Stage 1 Loading...");
    }

    public void Stage2() 
    {
        Loading("Stage2");
        Debug.Log("Stage 2 Loading...");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit game");
    }
}