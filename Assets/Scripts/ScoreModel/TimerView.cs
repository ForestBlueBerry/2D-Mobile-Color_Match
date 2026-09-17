using System;
using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;

    private float _timeRemaining;
    private bool _isTimerRunning;

    public event Action OnTimeExpired;

    private void Start()
    {
        float initialTime = GameSettings.SelectedDifficulty != null
            ? GameSettings.SelectedDifficulty.timeLimit
            : 60f;

        _timeRemaining = initialTime;
        _isTimerRunning = true;

        UpdateTimerText(_timeRemaining);
    }

    private void Update()
    {
        if (!_isTimerRunning) return;

        if (_timeRemaining > 0)
        {
            _timeRemaining -= Time.deltaTime;
            UpdateTimerText(_timeRemaining);
        }
        else
        {
            _timeRemaining = 0;
            _isTimerRunning = false;
            UpdateTimerText(0);

            OnTimeExpired?.Invoke();
        }
    }

    private void UpdateTimerText(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer() => _isTimerRunning = false;
}