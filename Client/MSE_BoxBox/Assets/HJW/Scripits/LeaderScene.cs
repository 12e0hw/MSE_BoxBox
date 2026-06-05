using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LeaderSceneSettings
{
    public GameObject panel;
    public LeaderboardView view;
    public Image rotatingArrow;
    public bool useStageLeaderboard = true;
    public int stageId = 1;
}

public class LeaderScene : MonoBehaviour
{
    [Header("Root Panel")]
    [SerializeField] private GameObject leaderboardRootPanel;
    
    [Header("Leaderboard")]
    [SerializeField] private LeaderboardApiClient leaderboardApiClient;
    [SerializeField] private LeaderSceneSettings[] leaderboardSettings;
    
    [Header("Arrow")]
    [SerializeField] private float arrowRotateDuration = 0.5f;

    private Coroutine[] leaderboardRefreshCoroutines;
    private Coroutine[] arrowRotateCoroutines;
    private Quaternion[] arrowInitialRotations;

    private int currentLeaderboardIndex = -1;
    private bool isInitialized = false;
    
    private void Awake()
    {
        InitializeLeaderboard();
        CloseLeaderboard();
    }

    private void OnDisable()
    {
        StopAllLeaderboardCoroutines();
    }

    private void InitializeLeaderboard()
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

        isInitialized = true;
    }
    
    private void EnsureInitialized()
    {
        if (!isInitialized)
        {
            InitializeLeaderboard();
        }
    }

    private bool IsValidLeaderboardIndex(int leaderboardIndex)
    {
        if (leaderboardSettings == null || leaderboardSettings.Length == 0)
        {
            Debug.LogError("[LeaderScene] LeaderboardSettings가 비어 있습니다.");
            return false;
        }

        if (leaderboardIndex < 0 || leaderboardIndex >= leaderboardSettings.Length)
        {
            Debug.LogError($"[LeaderScene] 잘못된 Leaderboard Index입니다: {leaderboardIndex}");
            return false;
        }

        return true;
    }
    
    public void OpenLeaderboard(int leaderboardIndex)
    {
        EnsureInitialized();

        if (!IsValidLeaderboardIndex(leaderboardIndex))
        {
            return;
        }

        currentLeaderboardIndex = leaderboardIndex;

        if (leaderboardRootPanel != null)
        {
            leaderboardRootPanel.SetActive(true);
        }

        for (int i = 0; i < leaderboardSettings.Length; i++)
        {
            if (leaderboardSettings[i].panel != null)
            {
                leaderboardSettings[i].panel.SetActive(i == leaderboardIndex);
            }
        }

        ResetRotatingArrow(leaderboardIndex);
        RefreshLeaderboard(leaderboardIndex);
    }

    public void CloseLeaderboard()
    {
        EnsureInitialized();

        currentLeaderboardIndex = -1;

        StopAllLeaderboardCoroutines();
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

        if (leaderboardRootPanel != null)
        {
            leaderboardRootPanel.SetActive(false);
        }
    }

    public void RefreshCurrentLeaderboard()
    {
        EnsureInitialized();

        if (!IsValidLeaderboardIndex(currentLeaderboardIndex))
        {
            return;
        }

        RefreshLeaderboard(currentLeaderboardIndex);
    }
    
    private void RefreshLeaderboard(int leaderboardIndex)
    {
        EnsureInitialized();

        if (!IsValidLeaderboardIndex(leaderboardIndex))
        {
            return;
        }

        LeaderSceneSettings selectedSettings = leaderboardSettings[leaderboardIndex];

        if (leaderboardApiClient == null)
        {
            Debug.LogError("[LeaderScene] LeaderboardApiClient가 연결되지 않았습니다.");

            if (selectedSettings.view != null)
            {
                selectedSettings.view.ShowError("API client is not connected.");
            }

            return;
        }

        if (selectedSettings.view == null)
        {
            Debug.LogError("[LeaderScene] LeaderboardView가 연결되지 않았습니다.");
            return;
        }

        RotateArrowOnce(leaderboardIndex);

        if (leaderboardRefreshCoroutines[leaderboardIndex] != null)
        {
            StopCoroutine(leaderboardRefreshCoroutines[leaderboardIndex]);
        }

        leaderboardRefreshCoroutines[leaderboardIndex] = StartCoroutine(
            RefreshLeaderboardRoutine(leaderboardIndex)
        );
    }
    
    private IEnumerator RefreshLeaderboardRoutine(int leaderboardIndex)
    {
        LeaderSceneSettings selectedSettings = leaderboardSettings[leaderboardIndex];

        selectedSettings.view.ShowLoading();

        LeaderboardItem[] loadedItems = null;
        bool isSuccess = false;

        if (selectedSettings.useStageLeaderboard)
        {
            yield return StartCoroutine(
                leaderboardApiClient.LoadStageLeaderboard(
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
                )
            );
        }
        else
        {
            yield return StartCoroutine(
                leaderboardApiClient.LoadAllLeaderboard(
                    items =>
                    {
                        loadedItems = items;
                        isSuccess = true;
                    },
                    () =>
                    {
                        isSuccess = false;
                    }
                )
            );
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
        LeaderSceneSettings selectedSettings = leaderboardSettings[leaderboardIndex];

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

        leaderboardSettings[leaderboardIndex].rotatingArrow.rectTransform.localRotation =
            arrowInitialRotations[leaderboardIndex];
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

    private void StopAllLeaderboardCoroutines()
    {
        if (leaderboardRefreshCoroutines != null)
        {
            for (int i = 0; i < leaderboardRefreshCoroutines.Length; i++)
            {
                if (leaderboardRefreshCoroutines[i] != null)
                {
                    StopCoroutine(leaderboardRefreshCoroutines[i]);
                    leaderboardRefreshCoroutines[i] = null;
                }
            }
        }

        if (arrowRotateCoroutines != null)
        {
            for (int i = 0; i < arrowRotateCoroutines.Length; i++)
            {
                if (arrowRotateCoroutines[i] != null)
                {
                    StopCoroutine(arrowRotateCoroutines[i]);
                    arrowRotateCoroutines[i] = null;
                }
            }
        }
    }
    
    public void Leaderboard1Select()
    {
        OpenLeaderboard(0);
    }

    public void Leaderboard2Select()
    {
        OpenLeaderboard(1);
    }
    
    public void SelectLeaderboard(int leaderboardIndex)
    {
        OpenLeaderboard(leaderboardIndex);
    }

    public void RefreshStage1Leaderboard()
    {
        currentLeaderboardIndex = 0;
        RefreshLeaderboard(0);
    }

    public void RefreshStage2Leaderboard()
    {
        currentLeaderboardIndex = 1;
        RefreshLeaderboard(1);
    }
    
    public void OpenLeaderboardPanel()
    {
        OpenLeaderboard(0);
    }

    public void CloseLeaderboardPanel()
    {
        CloseLeaderboard();
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game.");
    }

    
}
