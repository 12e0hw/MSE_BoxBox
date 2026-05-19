using LJC.scripts;
using System.Collections;
using LJC.Scripts;
using UnityEngine;

public class ApiTest : MonoBehaviour
{
    [SerializeField] private LeaderboardApiClient leaderboardApiClient;

    [Header("Test Data")]
    [SerializeField] private int userId = 1;
    [SerializeField] private int stageId = 1;
    [SerializeField] private int testScore = 123;

    public void OnClickSaveScore()
    {
        StartCoroutine(TestSaveScore());
    }

    public void OnClickLoadLeaderboard()
    {
        StartCoroutine(TestLoadLeaderboard());
    }

    public void OnClickSaveAndLoad()
    {
        StartCoroutine(TestSaveScoreAndLoadLeaderboard());
    }

    private IEnumerator TestSaveScore()
    {
        if (leaderboardApiClient == null)
        {
            Debug.LogError("[ApiTest] LeaderboardApiClient가 연결되지 않았습니다.");
            yield break;
        }


        Debug.Log("[ApiTest] 점수 저장 테스트 시작");

        bool saveSuccess = false;

        yield return StartCoroutine(
            leaderboardApiClient.SaveScore(userId, stageId, testScore, success =>
            {
                saveSuccess = success;
            })
        );

        if (saveSuccess)
        {
            Debug.Log("[ApiTest] 점수 저장 성공");
        }
        else
        {
            Debug.LogError("[ApiTest] 점수 저장 실패");
        }
    }

    private IEnumerator TestLoadLeaderboard()
    {
        if (leaderboardApiClient == null)
        {
            Debug.LogError("[ApiTest] LeaderboardApiClient가 연결되지 않았습니다.");
            yield break;
        }

        Debug.Log("[ApiTest] 리더보드 조회 테스트 시작");

        yield return StartCoroutine(
            leaderboardApiClient.LoadStageLeaderboard(
                stageId,
                items =>
                {
                    Debug.Log($"[ApiTest] 리더보드 조회 성공 / 개수: {items.Length}");

                    foreach (LeaderboardItem item in items)
                    {
                        Debug.Log($"{item.rank}. {item.username} - {item.score}");
                    }
                },
                () =>
                {
                    Debug.LogError("[ApiTest] 리더보드 조회 실패");
                }
            )
        );
    }

    private IEnumerator TestSaveScoreAndLoadLeaderboard()
    {
        if (leaderboardApiClient == null)
        {
            Debug.LogError("[ApiTest] LeaderboardApiClient가 연결되지 않았습니다.");
            yield break;
        }

        Debug.Log("[ApiTest] 점수 저장 후 리더보드 조회 테스트 시작");

        bool saveSuccess = false;

        yield return StartCoroutine(
            leaderboardApiClient.SaveScore(userId, stageId, testScore, success =>
            {
                saveSuccess = success;
            })
        );

        if (!saveSuccess)
        {
            Debug.LogError("[ApiTest] 점수 저장 실패");
            yield break;
        }

        Debug.Log("[ApiTest] 점수 저장 성공");
        Debug.Log("[ApiTest] 리더보드 조회 시작");

        yield return StartCoroutine(
            leaderboardApiClient.LoadStageLeaderboard(
                stageId,
                items =>
                {
                    Debug.Log($"[ApiTest] 리더보드 조회 성공 / 개수: {items.Length}");

                    foreach (LeaderboardItem item in items)
                    {
                        Debug.Log($"{item.rank}. {item.username} - {item.score}");
                    }
                },
                () =>
                {
                    Debug.LogError("[ApiTest] 리더보드 조회 실패");
                }
            )
        );
    }
}