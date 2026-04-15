namespace LJC
{
    using System;
    using UnityEngine;
    
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private int currentScore = 0;
    
        public int CurrentScore => currentScore;
    
        public event Action<int> OnScoreChanged;
    
        public void ResetScore()
        {
            currentScore = 0;
            OnScoreChanged?.Invoke(currentScore);
        }
    
        public void AddScore(int amount)
        {
            currentScore += amount;
            OnScoreChanged?.Invoke(currentScore);
        }
    }
}