using UnityEngine;
using UnityEngine.SceneManagement;

public class StartmenuMan : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void Login()
    {
        Debug.Log("login");
    }

    public void OpenSettings()
    {
        Debug.Log("setting");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit game");
    }
}
