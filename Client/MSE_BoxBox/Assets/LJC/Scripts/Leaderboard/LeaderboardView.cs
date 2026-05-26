using TMPro;
using UnityEngine;

public class LeaderboardView : MonoBehaviour
{
    [Header("Leaderboard Slots")]
    [SerializeField] private TMP_Text[] leaderboardSlots;

    public void ShowLoading()
    {
        ClearSlots();

        if (leaderboardSlots != null && leaderboardSlots.Length > 0 && leaderboardSlots[0] != null)
        {
            leaderboardSlots[0].text = "Loading leaderboard...";
        }
    }

    public void ShowLeaderboard(LeaderboardItem[] items)
    {
        ClearSlots();

        if (leaderboardSlots == null || leaderboardSlots.Length == 0)
        {
            Debug.LogError("[LeaderboardView] Leaderboard slots are not connected.");
            return;
        }

        if (items == null || items.Length == 0)
        {
            leaderboardSlots[0].text = "No leaderboard data.";
            return;
        }

        int count = Mathf.Min(10, leaderboardSlots.Length, items.Length);

        for (int i = 0; i < count; i++)
        {
            LeaderboardItem item = items[i];
            leaderboardSlots[i].text = $"{item.rank}. {item.username} - {item.score}";
        }
    }

    public void ShowError(string message)
    {
        ClearSlots();

        if (leaderboardSlots != null && leaderboardSlots.Length > 0 && leaderboardSlots[0] != null)
        {
            leaderboardSlots[0].text = message;
        }
    }

    public void ClearSlots()
    {
        if (leaderboardSlots == null)
        {
            return;
        }

        for (int i = 0; i < leaderboardSlots.Length; i++)
        {
            if (leaderboardSlots[i] != null)
            {
                leaderboardSlots[i].text = "";
            }
        }
    }
}