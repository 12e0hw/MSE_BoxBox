using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.InputSystem;

public class AuthManager : MonoBehaviour
{
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

    // 유니티에서 보낼 데이터 형식
    [System.Serializable]
    public class AuthRequest
    {
        public string username;
        public string password;
    }

    // 서버에서 받을 데이터 형식
    [System.Serializable]
    public class AuthResponse
    {
        public bool success;
        public string message;
    }

    void Update()
    {
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
                // 서버가 준 JSON을 파싱
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                
                if (response.success)
                {
                    Debug.Log("signup success");
                    signupSuccessPanel.SetActive(true);
                }
                else
                {
                    Debug.Log(response.message);
                    signupFailPanel.SetActive(true);

                }
            }
                if (request.downloadHandler != null)
                {
                    Debug.Log("서버 상세 응답: " + request.downloadHandler.text);
                }
        }
    }

    private IEnumerator SendLogin(string username, string pass)
    {
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
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                
                if (response.success)
                {
                    Debug.Log("login success");
                    loginPanel.SetActive(false);
                    checkPanel.SetActive(true);
                }
                else
                {
                    Debug.Log(response.message);
                    loginFailPanel.SetActive(true);
                }
            }
        }
    }

    private void ShowFailPanel(string message)
    {
        if (signupFailPanel != null) signupFailPanel.SetActive(true);
    }
}