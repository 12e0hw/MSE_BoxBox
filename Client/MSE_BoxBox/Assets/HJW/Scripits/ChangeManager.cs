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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameState.StartMenu, true);
        }
        
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (BacktoLoginPanel != null) BacktoLoginPanel.SetActive(false);
        if (TryAgainPanel!= null) TryAgainPanel.SetActive(false);
        if (LoginFailPanel != null) LoginFailPanel.SetActive(false);
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
        /*
        else if (leaderboardMemo)
        {
            SelectLeaderboard();
            leaderboardMemo = false;
        }
        */
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