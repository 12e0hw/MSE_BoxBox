using UnityEngine;
using UnityEngine.SceneManagement;

public class StartmenuMan : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject SettingPanel;

    void Start()
    {
        if(LoginPanel != null) LoginPanel.SetActive(false);
        if(SettingPanel != null) SettingPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void Login()
    {
        LoginPanel.SetActive(true);
        if(SettingPanel != null) SettingPanel.SetActive(false);
    }

    public void Back()
    {
        LoginPanel.SetActive(false);
        if(SettingPanel !=null) SettingPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if(SettingPanel != null) SettingPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit game");
    }
}
