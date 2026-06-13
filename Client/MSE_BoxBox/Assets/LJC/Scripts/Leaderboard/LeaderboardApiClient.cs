using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardApiClient : MonoBehaviour
{
    [Header("Server Settings")] 
    [SerializeField] private string serverBaseUrl = "http://localhost:8080";

    // Save the player's score to the server.
    public IEnumerator SaveScore(int userId, int stageId, int points, Action<bool> onFinished)
    {
        string url = $"{serverBaseUrl}/api/game/score";

        ScoreSaveRequest requestData = new ScoreSaveRequest
        {
            userId = userId,
            stageId = stageId,
            points = points
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[LeaderboardApiClient] Score save failed: {request.error}");
                Debug.LogError($"[LeaderboardApiClient] Response: {request.downloadHandler.text}");

                onFinished?.Invoke(false);
                yield break;
            }

            ScoreSaveResponse response = JsonUtility.FromJson<ScoreSaveResponse>(request.downloadHandler.text);

            if (response == null || !response.success)
            {
                Debug.LogError("[LeaderboardApiClient] Score save response failed.");
                Debug.LogError($"[LeaderboardApiClient] Response: {request.downloadHandler.text}");

                onFinished?.Invoke(false);
                yield break;
            }

            Debug.Log("[LeaderboardApiClient] Score save success.");
            onFinished?.Invoke(true);
        }
    }

    // Load the overall leaderboard.
    public IEnumerator LoadAllLeaderboard(Action<LeaderboardItem[]> onSuccess, Action onFailed)
    {
        string url = $"{serverBaseUrl}/api/game/rank";
        yield return StartCoroutine(GetLeaderboard(url, onSuccess, onFailed));
    }

    // Load the leaderboard for a specific stage.
    public IEnumerator LoadStageLeaderboard(int stageId, Action<LeaderboardItem[]> onSuccess, Action onFailed)
    {
        string url = $"{serverBaseUrl}/api/game/rank/{stageId}";
        yield return StartCoroutine(GetLeaderboard(url, onSuccess, onFailed));
    }

    // Load the best score for a specific user.
    public IEnumerator LoadUserBestScore(int userId, Action<UserBestScoreData> onSuccess, Action onFailed)
    {
        string url = $"{serverBaseUrl}/api/game/rank/user/{userId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[LeaderboardApiClient] User best score load failed: {request.error}");
                Debug.LogError($"[LeaderboardApiClient] Response: {request.downloadHandler.text}");

                onFailed?.Invoke();
                yield break;
            }

            UserBestScoreResponse response =
                JsonUtility.FromJson<UserBestScoreResponse>(request.downloadHandler.text);

            if (response == null || !response.success || response.data == null)
            {
                Debug.LogError("[LeaderboardApiClient] User best score response failed.");
                Debug.LogError($"[LeaderboardApiClient] Response: {request.downloadHandler.text}");

                onFailed?.Invoke();
                yield break;
            }

            onSuccess?.Invoke(response.data);
        }
    }

    // Request leaderboard data from the given URL.
    private IEnumerator GetLeaderboard(string url, Action<LeaderboardItem[]> onSuccess, Action onFailed)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[LeaderboardApiClient] Leaderboard load failed: {request.error}");
                Debug.LogError($"[LeaderboardApiClient] Response: {request.downloadHandler.text}");

                onFailed?.Invoke();
                yield break;
            }

            LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(request.downloadHandler.text);

            if (response == null || !response.success || response.data == null)
            {
                Debug.LogError("[LeaderboardApiClient] Leaderboard response failed.");
                Debug.LogError($"[LeaderboardApiClient] Response: {request.downloadHandler.text}");

                onFailed?.Invoke();
                yield break;
            }

            onSuccess?.Invoke(response.data);
        }
    }
}

    [Serializable]
    public class ScoreSaveRequest
    {
        public int userId;
        public int stageId;
        public int points;
    }

    [Serializable]
    public class ScoreSaveResponse
    {
        public bool success;
        public string message;
        public ScoreSaveData data;
    }

    [Serializable]
    public class ScoreSaveData
    {
        public int recordId;
        public int userId;
        public int stageId;
        public int points;
    }

    [Serializable]
    public class LeaderboardResponse
    {
        public bool success;
        public string message;
        public LeaderboardItem[] data;
    }

    [Serializable]
    public class LeaderboardItem
    {
        public int rank;
        public string username;
        public int score;
    }

    [Serializable]
    public class UserBestScoreResponse
    {
        public bool success;
        public string message;
        public UserBestScoreData data;
    }

    [Serializable]
    public class UserBestScoreData
    {
        public int userId;
        public string username;
        public int bestScore;
    }
