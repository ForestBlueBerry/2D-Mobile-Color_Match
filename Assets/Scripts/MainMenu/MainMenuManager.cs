using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject selectDifficults;
    [SerializeField] private GameObject optionsPanel;

    [Header("Audio Sliders")]
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;

    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindFirstObjectByType<AudioManager>();

        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (_musicSlider != null)
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
    }

    public void Play()
    {
        _audioManager?.PlayButtonClick();
        mainButtons.SetActive(false);
        selectDifficults.SetActive(true);
    }

    public void OpenOptions()
    {
        _audioManager?.PlayButtonClick();

        if (_audioManager != null)
        {
            if (_sfxSlider != null) _sfxSlider.value = _audioManager.SFXVolume;
            if (_musicSlider != null) _musicSlider.value = _audioManager.MusicVolume;
        }

        mainButtons.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        _audioManager?.PlayButtonClick();
        optionsPanel.SetActive(false);
        mainButtons.SetActive(true);
    }

    public void Back()
    {
        _audioManager?.PlayButtonClick();
        selectDifficults.SetActive(false);
        mainButtons.SetActive(true);
    }

    public void Quit()
    {
        _audioManager?.PlayButtonClick();
        Application.Quit();
    }

    private void OnSFXVolumeChanged(float value) => _audioManager?.SetSFXVolume(value);
    private void OnMusicVolumeChanged(float value) => _audioManager?.SetMusicVolume(value);
}