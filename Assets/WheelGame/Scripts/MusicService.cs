using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections;

public class MusicService : MonoBehaviour
{
    public static MusicService Instance { get; private set; }

    [Header("Addressable Keys")]
    public string gameMusicKey = "GameMusic";

    private AudioClip gameMusicClip;
    private AsyncOperationHandle<AudioClip> musicHandle;
    private bool isLoaded;

    public bool IsLoaded => isLoaded;
    public AudioClip GameMusicClip => gameMusicClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMusic(Action onComplete = null, Action<string> onFailed = null)
    {
        if (isLoaded)
        {
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(LoadMusicRoutine(onComplete, onFailed));
    }

    private IEnumerator LoadMusicRoutine(Action onComplete, Action<string> onFailed)
    {
        musicHandle = Addressables.LoadAssetAsync<AudioClip>(gameMusicKey);
        yield return musicHandle;

        if (musicHandle.Status == AsyncOperationStatus.Succeeded)
        {
            gameMusicClip = musicHandle.Result;
            isLoaded = true;
            Debug.Log("MusicService: Loaded GameMusic clip, length: " + gameMusicClip.length + "s");
            onComplete?.Invoke();
        }
        else
        {
            Debug.LogWarning("MusicService: Failed to load GameMusic");
            onFailed?.Invoke("Failed to load music asset");
        }
    }

    public void PlayGameMusic()
    {
        if (!isLoaded || gameMusicClip == null)
        {
            Debug.LogWarning("MusicService: Music not loaded yet!");
            return;
        }

        Debug.Assert(AudioManager.Instance != null, "MusicService: AudioManager not found!");

        if (AudioManager.Instance.musicSource.clip == gameMusicClip && AudioManager.Instance.musicSource.isPlaying)
            return;

        AudioManager.Instance.musicSource.clip = gameMusicClip;
        AudioManager.Instance.musicSource.loop = true;
        AudioManager.Instance.musicSource.Play();
        AudioManager.Instance.FadeMusicIn(0.5f);
    }

    public void StopGameMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeMusicOut(0.5f, () =>
            {
                AudioManager.Instance.musicSource.Stop();
            });
        }
    }

    private void OnDestroy()
    {
        if (musicHandle.IsValid())
            Addressables.Release(musicHandle);
    }
}
