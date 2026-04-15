using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager2 : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Ingame");
    }

    public void AnotherStage()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
