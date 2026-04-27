using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager1 : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Ingame");
    }

    public void AnotherStage()
    {
        SceneManager.LoadScene("Stage2");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
