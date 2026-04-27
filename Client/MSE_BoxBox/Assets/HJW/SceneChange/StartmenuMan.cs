using UnityEngine;
using UnityEngine.SceneManagement;

public class StartmenuMan : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void Login()
    {
        Debug.Log("login");
    }

    public void OpenSettings()
    {
        Debug.Log("setting");
    }
}
