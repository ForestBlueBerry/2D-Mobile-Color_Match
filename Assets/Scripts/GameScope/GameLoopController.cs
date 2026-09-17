using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLoopController : IAsyncStartable, IDisposable
{
    private readonly ScoreModel _scoreModel;
    private readonly FishSpawner _fishSpawner;
    private readonly TimerView _timerView;
    private readonly ControlBucket _controlBucket;
    private readonly BucketCatcher _bucketCatcher;
    private readonly GameOverUI _gameOverUI;
    private readonly PauseController _pauseController;

    [Inject]
    public GameLoopController(
        ScoreModel scoreModel,
        FishSpawner fishSpawner,
        TimerView timerView,
        ControlBucket controlBucket,
        BucketCatcher bucketCatcher,
        GameOverUI gameOverUI,
        PauseController pauseController)
    {
        _scoreModel = scoreModel;
        _fishSpawner = fishSpawner;
        _timerView = timerView;
        _controlBucket = controlBucket;
        _bucketCatcher = bucketCatcher;
        _gameOverUI = gameOverUI;
        _pauseController = pauseController;
    }

    public async UniTask StartAsync(CancellationToken cancellationToken)
    {
        if (_pauseController != null)
        {
            _pauseController.OnPauseStateChanged += HandlePauseState;
        }

        InitializeRound();

        bool isCanceled = await WaitUntilGameOverAsync(cancellationToken);

        if (!isCanceled)
        {
            FinishRound();
        }
    }

    private void InitializeRound()
    {
        Time.timeScale = 1f;
        _scoreModel.Reset();
        _controlBucket.enabled = true;

        if (_bucketCatcher != null) _bucketCatcher.ResetCatcher();
        if (_gameOverUI != null) _gameOverUI.Hide();
    }

    private async UniTask<bool> WaitUntilGameOverAsync(CancellationToken ct)
    {
        var tcs = new UniTaskCompletionSource();

        Action handleTimeExpired = () =>
        {
            tcs.TrySetResult();
        };

        if (_timerView != null)
        {
            _timerView.OnTimeExpired += handleTimeExpired;
        }

        try
        {
            return await tcs.Task.AttachExternalCancellation(ct).SuppressCancellationThrow();
        }
        finally
        {
            if (_timerView != null)
            {
                _timerView.OnTimeExpired -= handleTimeExpired;
            }
        }
    }

    private void HandlePauseState(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            _controlBucket.enabled = false;
        }
        else
        {
            Time.timeScale = 1f;
            _controlBucket.enabled = true;
        }
    }

    private void FinishRound()
    {
        Time.timeScale = 1f;

        _scoreModel.IsLocked = true; 

        _fishSpawner.StopSpawning();
        _controlBucket.enabled = false;

        if (_bucketCatcher != null)
        {
            _bucketCatcher.StopCatching();
        }

        if (_gameOverUI != null)
        {
            _gameOverUI.Show(_scoreModel.CurrentScore, _scoreModel.HighScore);
        }
    }

    public void Dispose()
    {
        if (_pauseController != null)
        {
            _pauseController.OnPauseStateChanged -= HandlePauseState;
        }
        Time.timeScale = 1f;
    }
}