using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace HJW.scripts
{

    // DTO used to format data into JSON when sending a save request to the server
    [System.Serializable]
    public class NameSaveData 
    { 
        public int userId; 
        public int index; 
        public string characterName; 
    }

    // DTO used to parse the JSON response received from the server when loading a name
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

        private void Start()
        {
            // Restrict input to alphanumeric characters only to prevent special character bugs or injection
            if (p1InputField != null)
            {
                p1InputField.contentType = TMP_InputField.ContentType.Alphanumeric;
            }
            if (p2InputField != null)
            {
                p2InputField.contentType = TMP_InputField.ContentType.Alphanumeric;
            }
        }

        private void OnEnable()
        {
            // If the user is playing as a guest (not logged in), default the names and skip the API call
            if (AuthManager.LoginUserId == 0) 
            {
                p1InputField.text = "Player1";
                p2InputField.text = "Player2";
                return;
            }

            // Set temporary defaults while fetching actual data from the server
            p1InputField.text = "Player1";
            p2InputField.text = "Player2";

            // Fetch saved names from the database
            StartCoroutine(LoadName(1, p1InputField));
            StartCoroutine(LoadName(2, p2InputField));
        }

        public void SaveNames()
        {
            // Do not attempt to save to the database if the user is a guest
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

        // Saves the name locally, updates active player objects in the scene, and pushes to the server.
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

        // Coroutine to handle the POST request for saving a name
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

        // Coroutine to handle the GET request for retrieving a name
        private IEnumerator LoadName(int index, TMP_InputField inputField)
        {
            string url = $"{loadApiUrl}?userId={AuthManager.LoginUserId}&index={index}";

            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    NameLoadResponse res = JsonUtility.FromJson<NameLoadResponse>(req.downloadHandler.text);
                    
                    // If valid data is returned, update the UI and cache it locally
                    if (res != null && res.data != null && !string.IsNullOrEmpty(res.data.characterName))
                    {
                        inputField.text = res.data.characterName;
                        PlayerPrefs.SetString("Player" + index + "_Name", res.data.characterName);
                    }
                    else
                    {
                        // Fallback to default name if the player hasn't saved a custom name yet
                        string defaultName = "Player" + index;
                        inputField.text = defaultName;
                        PlayerPrefs.SetString("Player" + index + "_Name", defaultName);
                    }
                }
            }
        }
        private void OnApplicationQuit()
        {
            // Clear local cached names so the next session starts fresh
            PlayerPrefs.DeleteKey("Player1_Name");
            PlayerPrefs.DeleteKey("Player2_Name");
        }
    }
}