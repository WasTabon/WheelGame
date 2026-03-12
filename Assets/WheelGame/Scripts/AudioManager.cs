using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip sfxButtonClick;
    public AudioClip sfxButtonHover;
    public AudioClip sfxPanelOpen;
    public AudioClip sfxPanelClose;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private Tween musicFadeTween;

    public float MusicVolume => musicVolume;
    public float SfxVolume => sfxVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);

        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayButtonClick()
    {
        PlaySFX(sfxButtonClick);
    }

    public void PlayPanelOpen()
    {
        PlaySFX(sfxPanelOpen);
    }

    public void PlayPanelClose()
    {
        PlaySFX(sfxPanelClose);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void FadeMusicOut(float duration = 0.5f, Action onComplete = null)
    {
        musicFadeTween?.Kill();
        musicFadeTween = musicSource.DOFade(0f, duration).OnComplete(() => onComplete?.Invoke());
    }

    public void FadeMusicIn(float duration = 0.5f, Action onComplete = null)
    {
        musicFadeTween?.Kill();
        musicSource.volume = 0f;
        musicFadeTween = musicSource.DOFade(musicVolume, duration).OnComplete(() => onComplete?.Invoke());
    }

    public void CrossfadeToClip(AudioClip newClip, float duration = 1f)
    {
        if (newClip == null) return;
        FadeMusicOut(duration * 0.5f, () =>
        {
            musicSource.clip = newClip;
            musicSource.Play();
            FadeMusicIn(duration * 0.5f);
        });
    }

    public void SetMusicPitch(float pitch)
    {
        if (musicSource != null)
            musicSource.pitch = pitch;
    }
}
