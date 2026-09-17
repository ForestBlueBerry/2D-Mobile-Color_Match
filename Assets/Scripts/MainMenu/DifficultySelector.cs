using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    [Serializable]
    public struct DifficultyUI
    {
        public DifficultyConfig config;
        public TMP_Text recordText;
    }

    [Header("UI Records on top the button")]
    [SerializeField] private DifficultyUI[] _difficultyUIList;

    [Header("Scene Game")]
    [SerializeField] private string gameSceneName = "SampleScene";

    private void OnEnable()
    {
        DisplayHighScores();
    }

    public void DisplayHighScores()
    {
        if (_difficultyUIList == null) return;

        foreach (var item in _difficultyUIList)
        {
            if (item.config != null && item.recordText != null)
            {
                int record = ScoreModel.GetHighScoreForDifficulty(item.config.difficulty);
                item.recordText.text = $"RECORD: {record}";
            }
        }
    }

    public void SelectDifficultyAndPlay(DifficultyConfig config)
    {
        GameSettings.SelectedDifficulty = config;
        SceneManager.LoadScene(gameSceneName);
    }
}