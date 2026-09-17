using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Source Sound")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _waterfallsound;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip _catchSuccessClip;
    [SerializeField] private AudioClip _catchFailClip;
    [SerializeField] private AudioClip _buttonClickClip;
    [SerializeField] private AudioClip _gameOverClip;

    private const string SFXVolumeKey = "SFXVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string WaterfallVolumeKey = "WaterfallVolume";

    public float SFXVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float WaterfallVolume { get; private set; }

    private float _maxSFXVolume = 1f;
    private float _maxMusicVolume = 1f;
    private float _maxWaterfallVolume = 0.5f;

    private void Awake()
    {
        if (_sfxSource != null) _maxSFXVolume = _sfxSource.volume;
        if (_musicSource != null) _maxMusicVolume = _musicSource.volume;
        if (_waterfallsound != null) _maxWaterfallVolume = _waterfallsound.volume;

        SFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        WaterfallVolume = PlayerPrefs.GetFloat(WaterfallVolumeKey, 1f);

        ApplyVolume();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
        if (_sfxSource != null) _sfxSource.volume = SFXVolume * _maxSFXVolume;

        PlayerPrefs.SetFloat(SFXVolumeKey, SFXVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        if (_musicSource != null) _musicSource.volume = MusicVolume * _maxMusicVolume;

        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        PlayerPrefs.Save();
    }

    public void SetWaterfallVolume(float volume)
    {
        WaterfallVolume = Mathf.Clamp01(volume);
        if (_waterfallsound != null) _waterfallsound.volume = WaterfallVolume * _maxWaterfallVolume;

        PlayerPrefs.SetFloat(WaterfallVolumeKey, WaterfallVolume);
        PlayerPrefs.Save();
    }

    private void ApplyVolume()
    {
        if (_sfxSource != null) _sfxSource.volume = SFXVolume * _maxSFXVolume;
        if (_musicSource != null) _musicSource.volume = MusicVolume * _maxMusicVolume;
        if (_waterfallsound != null) _waterfallsound.volume = WaterfallVolume * _maxWaterfallVolume;
    }

    public void PlayCatchSuccess() => PlaySFX(_catchSuccessClip);
    public void PlayCatchFail() => PlaySFX(_catchFailClip);
    public void PlayButtonClick() => PlaySFX(_buttonClickClip);
    public void PlayGameOver() => PlaySFX(_gameOverClip);

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip, SFXVolume * _maxSFXVolume);
        }
    }
}