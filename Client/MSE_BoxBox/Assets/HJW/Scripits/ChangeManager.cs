using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LeaderboardSettings
{
    public GameObject panel;
    public LeaderboardApiClient apiClient;
    public LeaderboardView view;
    public Image rotatingArrow;
    public bool useStageLeaderboard = true;
    public int stageId = 1;
}


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
    
    void Start()
    {
        InitializeLeaderboardCoroutines();
        // 리더보드 패널 숨기는 함수
        HideAllLeaderboardPanels();
        
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
            SelectLeaderboard();
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
        if (CheckPanel.activeSelf)
        {
             if (StageSelectPanel != null) StageSelectPanel.SetActive(true);
        
            if (LoginPanel != null) LoginPanel.SetActive(false);
            if (SettingPanel != null) SettingPanel.SetActive(false);
            if (SignupPanel != null) SignupPanel.SetActive(false);
            HideAllLeaderboardPanels();
        }
        else
        {
            CantStartGamePanel.SetActive(true);
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
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    // 리더보드 관련 함수 따라 모아 놓았음.
    #region Leaderboard
    [Header("Leaderboard")]
    [SerializeField] private LeaderboardApiClient leaderboardApiClient;
    [SerializeField] private LeaderboardSettings[] leaderboardSettings;

    [SerializeField] private float arrowRotateDuration = 0.5f;

    private Coroutine[] leaderboardRefreshCoroutines;
    private Coroutine[] arrowRotateCoroutines;
    private Quaternion[] arrowInitialRotations;
    
    private int currentLeaderboardIndex = -1;
    
    private void InitializeLeaderboardCoroutines()
    {
        int count = leaderboardSettings == null ? 0 : leaderboardSettings.Length;

        leaderboardRefreshCoroutines = new Coroutine[count];
        arrowRotateCoroutines = new Coroutine[count];
        arrowInitialRotations = new Quaternion[count];

        for (int i = 0; i < count; i++)
        {
            if (leaderboardSettings[i].rotatingArrow != null)
            {
                arrowInitialRotations[i] = leaderboardSettings[i].rotatingArrow.rectTransform.localRotation;
            }
            else
            {
                arrowInitialRotations[i] = Quaternion.identity;
            }
        }
    }

    private bool IsValidLeaderboardIndex(int leaderboardIndex)
    {
        if (leaderboardSettings == null || leaderboardSettings.Length == 0)
        {
            Debug.LogError("[ChangeManager] LeaderboardSettings가 비어 있습니다.");
            return false;
        }

        if (leaderboardIndex < 0 || leaderboardIndex >= leaderboardSettings.Length)
        {
            Debug.LogError($"[ChangeManager] 잘못된 Leaderboard Index입니다: {leaderboardIndex}");
            return false;
        }

        return true;
    }

    private void HideAllLeaderboardPanels()
    {
        ResetAllRotatingArrows();

        if (leaderboardSettings != null)
        {
            for (int i = 0; i < leaderboardSettings.Length; i++)
            {
                if (leaderboardSettings[i].panel != null)
                {
                    leaderboardSettings[i].panel.SetActive(false);
                }
            }
        }

        if (LeaderboardPanel != null)
        {
            LeaderboardPanel.SetActive(false);
        }
    }
    
    private void ResetRotatingArrow(int leaderboardIndex)
    {
        if (!IsValidLeaderboardIndex(leaderboardIndex))
        {
            return;
        }

        if (leaderboardSettings[leaderboardIndex].rotatingArrow == null)
        {
            return;
        }

        if (arrowRotateCoroutines[leaderboardIndex] != null)
        {
            StopCoroutine(arrowRotateCoroutines[leaderboardIndex]);
            arrowRotateCoroutines[leaderboardIndex] = null;
        }

        leaderboardSettings[leaderboardIndex].rotatingArrow.rectTransform.localRotation = arrowInitialRotations[leaderboardIndex];
    }
    
    private void ResetAllRotatingArrows()
    {
        if (leaderboardSettings == null)
        {
            return;
        }

        for (int i = 0; i < leaderboardSettings.Length; i++)
        {
            ResetRotatingArrow(i);
        }
    }

    public void SelectLeaderboard()
    {
        SelectLeaderboard(0);
    }
    
    public void SelectLeaderboard(int leaderboardIndex)
    {
        if (!IsValidLeaderboardIndex(leaderboardIndex))
        {
            return;
        }

        ResetRotatingArrow(leaderboardIndex);

        if (LeaderboardPanel != null)
        {
            LeaderboardPanel.SetActive(true);
        }

        if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
        if (LoginPanel != null) LoginPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
        if (SignupPanel != null) SignupPanel.SetActive(false);
        if (SettingPanel!=null) SettingPanel.SetActive(false);

        for (int i = 0; i < leaderboardSettings.Length; i++)
        {
            if (leaderboardSettings[i].panel != null)
            {
                leaderboardSettings[i].panel.SetActive(i == leaderboardIndex);
            }
        }

        RefreshLeaderboard(leaderboardIndex);
    }

    public void RefreshLeaderboard(int leaderboardIndex)
    {
        if (!IsValidLeaderboardIndex(leaderboardIndex))
        {
            return;
        }

        LeaderboardSettings selectedSettings = leaderboardSettings[leaderboardIndex];

        if (leaderboardApiClient == null)
        {
            Debug.LogError("[ChangeManager] LeaderboardApiClient가 연결되지 않았습니다.");
            return;
        }

        if (selectedSettings.view == null)
        {
            Debug.LogError("[ChangeManager] LeaderboardView가 연결되지 않았습니다.");
            return;
        }

        RotateArrowOnce(leaderboardIndex);

        if (leaderboardRefreshCoroutines[leaderboardIndex] != null)
        {
            StopCoroutine(leaderboardRefreshCoroutines[leaderboardIndex]);
        }

        leaderboardRefreshCoroutines[leaderboardIndex] = StartCoroutine(RefreshLeaderboardRoutine(leaderboardIndex));
    }

    private IEnumerator RefreshLeaderboardRoutine(int leaderboardIndex)
    {
        LeaderboardSettings selectedSettings = leaderboardSettings[leaderboardIndex];

        selectedSettings.view.ShowLoading();

        LeaderboardItem[] loadedItems = null;
        bool isSuccess = false;

        if (selectedSettings.useStageLeaderboard)
        {
            yield return StartCoroutine(leaderboardApiClient.LoadStageLeaderboard(
                selectedSettings.stageId,
                items =>
                {
                    loadedItems = items;
                    isSuccess = true;
                },
                () =>
                {
                    isSuccess = false;
                }
            ));
        }
        else
        {
            yield return StartCoroutine(leaderboardApiClient.LoadAllLeaderboard(
                items =>
                {
                    loadedItems = items;
                    isSuccess = true;
                },
                () =>
                {
                    isSuccess = false;
                }
            ));
        }

        if (!isSuccess)
        {
            selectedSettings.view.ShowError("Failed to load leaderboard.");
            leaderboardRefreshCoroutines[leaderboardIndex] = null;
            yield break;
        }

        selectedSettings.view.ShowLeaderboard(loadedItems);
        leaderboardRefreshCoroutines[leaderboardIndex] = null;
    }

    private void RotateArrowOnce(int leaderboardIndex)
    {
        LeaderboardSettings selectedSettings = leaderboardSettings[leaderboardIndex];

        if (selectedSettings.rotatingArrow == null)
        {
            return;
        }

        if (arrowRotateCoroutines[leaderboardIndex] != null)
        {
            StopCoroutine(arrowRotateCoroutines[leaderboardIndex]);
        }

        arrowRotateCoroutines[leaderboardIndex] = StartCoroutine(
            RotateArrowRoutine(leaderboardIndex, selectedSettings.rotatingArrow.rectTransform)
        );
    }

    private IEnumerator RotateArrowRoutine(int leaderboardIndex, RectTransform arrowTransform)
    {
        Quaternion startRotation = arrowInitialRotations[leaderboardIndex];
        float elapsedTime = 0f;

        while (elapsedTime < arrowRotateDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / arrowRotateDuration);
            float rotateZ = Mathf.Lerp(0f, -360f, progress);

            arrowTransform.localRotation = startRotation * Quaternion.Euler(0f, 0f, rotateZ);

            yield return null;
        }

        arrowTransform.localRotation = arrowInitialRotations[leaderboardIndex];
        arrowRotateCoroutines[leaderboardIndex] = null;
    }

#endregion
}