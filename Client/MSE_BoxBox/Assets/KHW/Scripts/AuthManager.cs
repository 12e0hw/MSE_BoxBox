using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.InputSystem;

public class AuthManager : MonoBehaviour
{
    public static int LoginUserId { get; private set; }

    [Header("UI References")]
    public TMP_InputField usernameInput; 
    public TMP_InputField passwordInput; 
    
    [Header("Panels")]
    public GameObject loginPanel;        
    public GameObject signupSuccessPanel;
    public GameObject signupFailPanel;  
    public GameObject checkPanel;        
    public GameObject loginFailPanel;

    private readonly string baseUrl = "http://localhost:8080/api/users"; 

    [System.Serializable]
    public class AuthRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class AuthResponse
    {
        public bool success;
        public string message;
        public UserData data;
    }

    [System.Serializable]
    public class UserData
    {
        public int userId;
        public string username;
        public string password;
    }

    [System.Serializable]
    public class SignupResponse
    {
        public bool success;
        public string message;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public UserData data;
        public string message;
        public bool success;
    }

    void Update()
    {
        // Switch focus between login fields with Tab.
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (usernameInput.isFocused) passwordInput.ActivateInputField();
            else if (passwordInput.isFocused) usernameInput.ActivateInputField();
        }
    }

    public void OnClickSignUp()
    {
        StartCoroutine(SendSignUp(usernameInput.text, passwordInput.text));
    }

    public void OnClickLogin()
    {
        StartCoroutine(SendLogin(usernameInput.text, passwordInput.text));
    }

    private IEnumerator SendSignUp(string username, string pass)
    {
        // Send signup data to the backend server.
        AuthRequest req = new AuthRequest { username = username, password = pass };
        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/signup", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SignupResponse response = JsonUtility.FromJson<SignupResponse>(request.downloadHandler.text);
                
                if (response.success)
                {
                    Debug.Log("[AuthManager] Signup succeeded.");
                    signupSuccessPanel.SetActive(true);
                }
                else
                {
                    Debug.Log("[AuthManager] Signup failed: " + response.message);
                    signupFailPanel.SetActive(true);

                }
            }
                if (request.downloadHandler != null)
                {
                    Debug.Log("[AuthManager] Server detail response: " + request.downloadHandler.text);
                }
        }
    }

    private IEnumerator SendLogin(string username, string pass)
    {
        // Send login data and store the returned user id.
        AuthRequest req = new AuthRequest { username = username, password = pass };
        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                
                if (response.success && response.data != null)
                {
                    LoginUserId = response.data.userId;
                    Debug.Log($"[AuthManager] Parsed user ID: {LoginUserId}");
                    Debug.Log("[AuthManager] Login succeeded.");
                    loginPanel.SetActive(false);
                    checkPanel.SetActive(true);
                }
                else
                {
                    Debug.Log("[AuthManager] Login failed: " + response.message);
                    loginFailPanel.SetActive(true);
                }
            }
        }
    }
    
}
