using UnityEngine;

public class LeaderScene : MonoBehaviour
{
    public GameObject Leaderboard1Panel;
    public GameObject Leaderboard2Panel;

    void Start()
    {
        if(Leaderboard1Panel != null) Leaderboard1Panel.SetActive(false);
        if(Leaderboard2Panel != null) Leaderboard2Panel.SetActive(false);

    }

    public void Leaderboard1Select()
    {
        bool isLeaderboard1PanelActive = Leaderboard1Panel.activeSelf;
        Leaderboard1Panel.SetActive(!isLeaderboard1PanelActive);
        Leaderboard2Panel.SetActive(false);
    }

    public void Leaderboard2Select()
    {
        bool isLeaderboard2PanelActive = Leaderboard2Panel.activeSelf;
        Leaderboard2Panel.SetActive(!isLeaderboard2PanelActive);
        Leaderboard1Panel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game.");
    }

    
}
