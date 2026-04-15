using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI score;
    private int currentScore = 0;
    private int maxScore = 99999;
    private bool maxClear = false;
    void Start()
    {
        UpdateScore();
    }

    public void AddSmallBoxScore()
    {
        currentScore += 2;
        UpdateScore();
    }
    public void AddBigBoxScore()
    {
        currentScore += 5;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if(score != null)
        {
            score.text = currentScore.ToString("D5");
        }
    }



    void Update()
    {
        if(maxClear == false && maxScore <= currentScore)
        {
            maxClear = true;
            Debug.Log("Game Clear");
        }
    }
}
