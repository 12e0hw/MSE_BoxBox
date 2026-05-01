using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    public void Stage()
    {
        SceneManager.LoadScene("StageSelect"); 
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene("LeaderboardMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
