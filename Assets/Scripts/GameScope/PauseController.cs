using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class PauseController : MonoBehaviour
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _menuButton;

    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private Button _optionsBackButton;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _waterfallsoundSlider;
    [SerializeField] private string _menuSceneName = "MainMenu";

    private AudioManager _audioManager;

    public event Action<bool> OnPauseStateChanged;

    public bool IsPaused { get; private set; }

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Awake()
    {
        if (_pauseButton != null)
            _pauseButton.onClick.AddListener(PauseGame);

        if (_resumeButton != null)
            _resumeButton.onClick.AddListener(ResumeGame);

        if (_optionsButton != null)
            _optionsButton.onClick.AddListener(OpenOptions);

        if (_optionsBackButton != null)
            _optionsBackButton.onClick.AddListener(CloseOptions);

        if (_menuButton != null)
            _menuButton.onClick.AddListener(ToMainMenu);

        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (_musicSlider != null)
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (_waterfallsoundSlider != null)
            _waterfallsoundSlider.onValueChanged.AddListener(OnWaterfallVolumeChanged);

        SetButtonsState(isPaused: false);
    }

    private void PauseGame()
    {
        _audioManager?.PlayButtonClick();
        IsPaused = true;
        OnPauseStateChanged?.Invoke(IsPaused);
        SetButtonsState(isPaused: true);
    }

    private void ResumeGame()
    {
        _audioManager?.PlayButtonClick();
        IsPaused = false;
        OnPauseStateChanged?.Invoke(IsPaused);
        SetButtonsState(isPaused: false);
    }

    private void OpenOptions()
    {
        _audioManager?.PlayButtonClick();

        if (_audioManager != null)
        {
            if (_sfxSlider != null) _sfxSlider.value = _audioManager.SFXVolume;
            if (_musicSlider != null) _musicSlider.value = _audioManager.MusicVolume;
            if (_waterfallsoundSlider != null) _waterfallsoundSlider.value = _audioManager.WaterfallVolume;
        }

        SetPauseButtonsActive(false);
        if (_optionsPanel != null) _optionsPanel.SetActive(true);
    }

    private void CloseOptions()
    {
        _audioManager?.PlayButtonClick();

        if (_optionsPanel != null) _optionsPanel.SetActive(false);
        SetPauseButtonsActive(true);
    }

    private void ToMainMenu()
    {
        _audioManager?.PlayButtonClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(_menuSceneName);
    }

    private void SetButtonsState(bool isPaused)
    {
        if (_pauseButton != null) _pauseButton.gameObject.SetActive(!isPaused);

        SetPauseButtonsActive(isPaused);

        if (!isPaused && _optionsPanel != null)
        {
            _optionsPanel.SetActive(false);
        }
    }

    private void SetPauseButtonsActive(bool active)
    {
        if (_resumeButton != null) _resumeButton.gameObject.SetActive(active);
        if (_optionsButton != null) _optionsButton.gameObject.SetActive(active);
        if (_menuButton != null) _menuButton.gameObject.SetActive(active);
    }

    private void OnSFXVolumeChanged(float value) => _audioManager?.SetSFXVolume(value);
    private void OnMusicVolumeChanged(float value) => _audioManager?.SetMusicVolume(value);
    private void OnWaterfallVolumeChanged(float value) => _audioManager?.SetWaterfallVolume(value);
}