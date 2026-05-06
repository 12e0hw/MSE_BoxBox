using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int maxScore = 99999;
    
    public int CurrentScore { get; private set; }
    public bool MaxClear { get; private set; }
    
    public event Action<int> OnScoreChanged; // 점수가 바뀔 때마다 알려주는 역할
    public event Action OnMaxScoreReached;
    
    void Start()
    {
        ResetScore();
    }
    
    public void SetTargetScore(int targetScore)
    {
        maxScore = targetScore;
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        MaxClear = false;
        
        OnScoreChanged?.Invoke(CurrentScore);
    }
    
    //
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

    //이전 Update() 함수에서 이름을 직관적으로 수정하고 클리어 점수 도달 이벤트 알림
    private void CheckMaxScore()
    {
        if (MaxClear)
        {
            return;
        }

        if (CurrentScore >= maxScore)
        {
            MaxClear = true;
            Debug.Log("Game Clear");

            OnMaxScoreReached?.Invoke();
        }
    }

    /*
    void Update()
    {
        if(maxClear == false && maxScore <= currentScore)
        {
            maxClear = true;
            Debug.Log("Game Clear");
        }
    }
    */
}
