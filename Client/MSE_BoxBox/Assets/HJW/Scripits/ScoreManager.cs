using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int maxScore = 99999;
    
    public int CurrentScore { get; private set; }
    public bool MaxClear { get; private set; }
    
    public event Action<int> OnScoreChanged; //Events that other scripts
    public event Action OnMaxScoreReached; // Broadcasts exactly once when the player reaches the target score. 
    
    void Start()
    {
        // the score is completely reset when the scene loads or game starts
        ResetScore();
    }
    
    // Dynamically sets the target score required to win/clear the stage
    public void SetTargetScore(int targetScore)
    {
        maxScore = targetScore;
    }

    // Resets the score and win state
    public void ResetScore()
    {
        CurrentScore = 0;
        MaxClear = false;
        
        OnScoreChanged?.Invoke(CurrentScore);
    }
    
    // Core function to increase the score
    public void AddScore(int points)
    {
        if (points <= 0)
        {
            return;
        }

        CurrentScore += points;
        
        OnScoreChanged?.Invoke(CurrentScore);
        CheckMaxScore();
    }
    
    public void AddSmallBoxScore()
    {
        AddScore(2);
    }
    public void AddBigBoxScore()
    {
        AddScore(5);
    }

    // Evaluates if the current score meets or exceeds the target
    private void CheckMaxScore()
    {
        if (MaxClear)
        {
            return;
        }

        if (CurrentScore >= maxScore)
        {
            MaxClear = true;

            OnMaxScoreReached?.Invoke();
        }
    }
}
