using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace HJW.scripts
{
    [System.Serializable]
    public class NameSaveData 
    { 
        public int userId; 
        public int index; 
        public string characterName; 
    }

    [System.Serializable]
    public class NameLoadResponse 
    { 
        public bool success; 
        public string message; 
        public NameSaveData data; 
    }

    public class NameManager : MonoBehaviour
    {
        [Header("PLAYER1 UI")]
        public TMP_InputField p1InputField; 

        [Header("PLAYER2 UI")]
        public TMP_InputField p2InputField; 

        private string saveApiUrl = "http://localhost:8080/api/users/savename";
        private string loadApiUrl = "http://localhost:8080/api/users/loadname";

        private void OnEnable()
        {
            if (AuthManager.LoginUserId == 0) 
            {
                p1InputField.text = "Player1";
                p2InputField.text = "Player2";
                return;
            }

            p1InputField.text = PlayerPrefs.GetString("Player1_Name", "Player1");
            p2InputField.text = PlayerPrefs.GetString("Player2_Name", "Player2");

            StartCoroutine(LoadName(1, p1InputField));
            StartCoroutine(LoadName(2, p2InputField));
        }

        public void SaveNames()
        {
            if (AuthManager.LoginUserId == 0) return;
            if (!string.IsNullOrEmpty(p1InputField.text))
            {
                SaveSinglePlayer(1, p1InputField.text);
            }
            if (!string.IsNullOrEmpty(p2InputField.text))
            {
                SaveSinglePlayer(2, p2InputField.text);
            }
        }

        private void SaveSinglePlayer(int index, string newName)
        {
            PlayerPrefs.SetString("Player" + index + "_Name", newName);
            Player[] playersInScene = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (Player player in playersInScene)
            {
                if (player.myPlayerIndex == index) player.RefreshName();
            }

            StartCoroutine(SaveNameToServer(index, newName));
        }

        private IEnumerator SaveNameToServer(int index, string newName)
        {
            NameSaveData saveData = new NameSaveData { userId = AuthManager.LoginUserId, index = index, characterName = newName };
            string json = JsonUtility.ToJson(saveData);

            using (UnityWebRequest req = new UnityWebRequest(saveApiUrl, "POST"))
            {
                req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                yield return req.SendWebRequest();
            }
        }

        private IEnumerator LoadName(int index, TMP_InputField inputField)
        {
            string url = $"{loadApiUrl}?userId={AuthManager.LoginUserId}&index={index}";

            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    NameLoadResponse res = JsonUtility.FromJson<NameLoadResponse>(req.downloadHandler.text);
                    if (res != null && res.data != null && !string.IsNullOrEmpty(res.data.characterName))
                    {
                        inputField.text = res.data.characterName;
                        PlayerPrefs.SetString("Player" + index + "_Name", res.data.characterName);
                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.DeleteKey("Player1_Name");
            PlayerPrefs.DeleteKey("Player2_Name");
            
            Debug.Log("Game end.");
        }
    }
}