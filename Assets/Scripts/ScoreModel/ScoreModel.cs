using System;
using UnityEngine;

public class ScoreModel
{
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }
    public bool IsLocked { get; set; } 

    private readonly Difficulty _currentDifficulty;

    public ScoreModel()
    {
        _currentDifficulty = GameSettings.SelectedDifficulty != null
            ? GameSettings.SelectedDifficulty.difficulty
            : Difficulty.Easy;

        LoadHighScore();
    }

    public void AddScore(int amount)
    {
        if (IsLocked) return; 

        CurrentScore = Mathf.Max(0, CurrentScore + amount);
        OnScoreChanged?.Invoke(CurrentScore);

        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            SaveHighScore();
            OnHighScoreChanged?.Invoke(HighScore);
        }
    }

    public void Reset()
    {
        IsLocked = false; 
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt($"HighScore_{_currentDifficulty}", 0);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt($"HighScore_{_currentDifficulty}", HighScore);
        PlayerPrefs.Save();
    }

    public static int GetHighScoreForDifficulty(Difficulty difficulty)
    {
        return PlayerPrefs.GetInt($"HighScore_{difficulty}", 0);
    }
}