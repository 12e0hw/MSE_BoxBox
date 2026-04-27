using TMPro;
using UnityEngine;

public class TempHUDUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text scoreText;

    public void UpdateTime(float time)
    {
        int minute = Mathf.FloorToInt(time / 60f);
        int second = Mathf.FloorToInt(time % 60f);

        timeText.text = $"Time : {minute:00}:{second:00}";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score : {score}";
    }
}