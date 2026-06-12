using System.Collections;
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
    public GameObject BacktoLoginPanel;
    public GameObject TryAgainPanel;
    public GameObject CheckPanel;
    public GameObject LoginFailPanel;
    public GameObject CantStartGamePanel;

    [Header("Memo")]
    // Static variables used to remember the UI state across scene transitions  
    public static bool stageSelectMemo = false;
    public static bool leaderboardMemo = false;

    [Header("Loading Info")]
    public Slider loadingSlider;
    public float loadingTime = 3.0f;
    private float currentLoadingTime = 0f;
    private bool loadingOver = false;
    private string newSceneName = "";
    
    [Header("Leaderboard")]
    [SerializeField] private LeaderScene leaderScene;
    
    void Start()
    {
        HideAllLeaderboardPanels();

        // Ensure the GameManager recognizes the player is in the Start Menu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameState.StartMenu, true);
        }
        
        // Initialize all main panels to a hidden state to prevent overlapping UI
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (BacktoLoginPanel != null) BacktoLoginPanel.SetActive(false);
        if (TryAgainPanel!= null) TryAgainPanel.SetActive(false);
        if (LoginFailPanel != null) LoginFailPanel.SetActive(false);
        // Check static memos to restore a specific UI state if returning from another scene
        if (stageSelectMemo)
        {
            if (CheckPanel != null) CheckPanel.SetActive(true); 
            StageSelect();
            stageSelectMemo = false;
        }
        else if (leaderboardMemo)
        {
            if (CheckPanel != null) CheckPanel.SetActive(true); 
            SelectLeaderboard(0);
            leaderboardMemo = false;
        }
        else
        {
            if (CheckPanel != null) CheckPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Only process loading logic if a scene transition has been triggered
        if (!loadingOver) return;

        currentLoadingTime += Time.deltaTime;
        
        if (loadingSlider != null)
        {
            loadingSlider.value = currentLoadingTime / loadingTime;
        }

        // Once the artificial loading time finishes, load the actual scene
        if (currentLoadingTime >= loadingTime)
        {
            loadingOver = false;
            if (loadingSlider != null) loadingSlider.value = 1f;
            SceneManager.LoadScene(newSceneName);
        }
    }

    public void StageSelect() 
    {
        // CheckPanel active state when the user has successfully logged in
        if (CheckPanel != null && CheckPanel.activeSelf)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToStageSelect();
            }
            
            if (StageSelectPanel != null) StageSelectPanel.SetActive(true);
        
            if (LoginPanel != null) LoginPanel.SetActive(false);
            if (SettingPanel != null) SettingPanel.SetActive(false);
            if (SignupPanel != null) SignupPanel.SetActive(false);
            HideAllLeaderboardPanels();
        }
        else
        {
            // Show error message if the user tries to play without logging in
            if (CantStartGamePanel != null)
            {
                CantStartGamePanel.SetActive(true);
            }
        }
    }

    public void Login()
    {
        if (LoginPanel != null) LoginPanel.SetActive(true);
        
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (TryAgainPanel != null) TryAgainPanel.SetActive(false);
        if (CantStartGamePanel != null) CantStartGamePanel.SetActive(false);
        HideAllLeaderboardPanels();
    }

    public void Signup()
    {
        if (SignupPanel != null) SignupPanel.SetActive(true);
        
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        HideAllLeaderboardPanels();
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
        HideAllLeaderboardPanels();
    }

    public void BacktoStage()
    {
        if (StageSelectPanel != null) StageSelectPanel.SetActive(true);
        HideAllLeaderboardPanels();
    }

    public void Back()
    {
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        HideAllLeaderboardPanels();
    }

    public void BacktoMain()
    {
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
    }
    public void BacktoLogin()
    {
        if (BacktoLoginPanel != null) BacktoLoginPanel.SetActive(false);
        if (LoginFailPanel != null) LoginFailPanel.SetActive(false);
        if (LoginPanel != null) LoginPanel.SetActive(true);
    }
    
    public void SuccessLogin()
    {
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (BacktoLoginPanel != null) BacktoLoginPanel.SetActive(false);
        if (CheckPanel!=null) CheckPanel.SetActive(true);
        HideAllLeaderboardPanels();
    }

    //Prepares the loading screen and starts the artificial loading timer.
    private void Loading(string newScene)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameState.Loading);
        }
        
        newSceneName = newScene;
        currentLoadingTime = 0f;
        if (loadingSlider != null) loadingSlider.value = 0f;
        
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        
        if (loadingPanel != null) loadingPanel.SetActive(true);
        loadingOver = true;
    }

    public void Stage1() 
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectStage(1);
        }
        
        Loading("Stage1");
        Debug.Log("Stage 1 Loading...");
    }

    public void Stage2() 
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectStage(2);
        }
        
        Loading("Stage2");
        Debug.Log("Stage 2 Loading...");
    }

    //Safely exits the game, handling both the Unity Editor and the compiled build.
    public void ExitGame()
    {
        Debug.Log("Exit game");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    public void SelectLeaderboard(int leaderboardIndex)
    {
        if (CheckPanel != null && !CheckPanel.activeSelf)
        {
            if (CantStartGamePanel != null)
            {
                CantStartGamePanel.SetActive(true);
            }

            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToStageSelect();
        }
        
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);

        if (leaderScene != null)
        {
            leaderScene.OpenLeaderboard(leaderboardIndex);
        }
    }
    
    private void HideAllLeaderboardPanels()
    {
        if (leaderScene != null)
        {
            leaderScene.CloseLeaderboard();
        }
    }
}