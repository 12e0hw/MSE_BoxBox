using System.Collections;
using UnityEngine;

public class LeaderScene : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject leaderboard1Panel;
    [SerializeField] private GameObject leaderboard2Panel;

    [Header("Leaderboard Views")]
    [SerializeField] private LeaderboardView leaderboard1View;
    [SerializeField] private LeaderboardView leaderboard2View;
    
    [Header("API")]
    [SerializeField] private LeaderboardApiClient leaderboardApiClient;

    [Header("Stage IDs")]
    [SerializeField] private int stage1Id = 1;
    [SerializeField] private int stage2Id = 2;
    void Start()
    {
        if(leaderboard1Panel != null) leaderboard1Panel.SetActive(false);
        if(leaderboard2Panel != null) leaderboard2Panel.SetActive(false);

        if (leaderboard1Panel != null)
        {
            leaderboard1Panel.SetActive(false);
        }

        if (leaderboard2Panel != null)
        {
            leaderboard2Panel.SetActive(false);
        }
    }

    public void Leaderboard1Select()
    {
        if (leaderboard1Panel == null || leaderboard2Panel == null)
        {
            return;
        }

        bool nextActiveState = !leaderboard1Panel.activeSelf;

        leaderboard1Panel.SetActive(nextActiveState);
        leaderboard2Panel.SetActive(false);

        if (nextActiveState)
        {
            StartCoroutine(LoadLeaderboard(stage1Id, leaderboard1View));
        }
    }

    public void Leaderboard2Select()
    {
        if (leaderboard1Panel == null || leaderboard2Panel == null)
                {
                    return;
                }
        
                bool nextActiveState = !leaderboard2Panel.activeSelf;
        
                leaderboard2Panel.SetActive(nextActiveState);
                leaderboard1Panel.SetActive(false);
        
                if (nextActiveState)
                {
                    StartCoroutine(LoadLeaderboard(stage2Id, leaderboard2View));
                }
    }
    
    private IEnumerator LoadLeaderboard(int stageId, LeaderboardView targetView)
        {
            if (leaderboardApiClient == null)
            {
                Debug.LogError("[LeaderScene] LeaderboardApiClient가 연결되지 않았습니다.");
    
                if (targetView != null)
                {
                    targetView.ShowError("API client is not connected.");
                }
    
                yield break;
            }
    
            if (targetView == null)
            {
                Debug.LogError("[LeaderScene] LeaderboardView가 연결되지 않았습니다.");
                yield break;
            }
    
            targetView.ShowLoading();
    
            yield return StartCoroutine(
                leaderboardApiClient.LoadStageLeaderboard(
                    stageId,
                    items =>
                    {
                        targetView.ShowLeaderboard(items);
                    },
                    () =>
                    {
                        targetView.ShowError("Failed to load leaderboard.");
                    }
                )
            );
        }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game.");
    }

    
}
