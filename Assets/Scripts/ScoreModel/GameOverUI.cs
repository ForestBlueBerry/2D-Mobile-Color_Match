using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _highScoreText;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;

    [SerializeField] private string _menuSceneName = "MainMenu";

    [SerializeField] private float _scoreCountDuration = 0.8f;

    private AudioManager _audioManager;
    private Tween _scoreTween;

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Awake()
    {
        if (_restartButton != null)
            _restartButton.onClick.AddListener(RestartGame);

        if (_menuButton != null)
            _menuButton.onClick.AddListener(ToMainMenu);
    }

    public void Show(int currentScore, int highScore)
    {
        if (_panel != null) _panel.SetActive(true);

        _audioManager?.PlayGameOver();

        if (_highScoreText != null)
            _highScoreText.text = $"BEST: {highScore}";

        if (_scoreText != null)
        {
            _scoreTween?.Kill();
            _scoreText.text = "SCORE: 0";
            _scoreText.transform.localScale = Vector3.one;

            _scoreTween = DOVirtual.Float(0, currentScore, _scoreCountDuration, value =>
            {
                _scoreText.text = $"SCORE: {Mathf.RoundToInt(value)}";
            }).SetEase(Ease.OutCubic);

            _scoreText.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0f), 0.35f, 6, 0.5f);
        }
    }

    public void Hide()
    {
        _scoreTween?.Kill();
        if (_panel != null) _panel.SetActive(false);
    }

    private void RestartGame()
    {
        _audioManager?.PlayButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ToMainMenu()
    {
        _audioManager?.PlayButtonClick();
        SceneManager.LoadScene(_menuSceneName);
    }

    private void OnDestroy()
    {
        _scoreTween?.Kill();
    }
}