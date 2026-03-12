using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private float fadeDuration = 0.4f;
    private Tween currentFadeTween;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }
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

    public void FadeOut(Action onComplete = null)
    {
        currentFadeTween?.Kill();
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;
        currentFadeTween = fadeCanvasGroup.DOFade(1f, fadeDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void FadeIn(Action onComplete = null)
    {
        currentFadeTween?.Kill();
        currentFadeTween = fadeCanvasGroup.DOFade(0f, fadeDuration)
            .SetEase(Ease.OutQuad)
            .SetDelay(0.15f)
            .OnComplete(() =>
            {
                fadeCanvasGroup.blocksRaycasts = false;
                fadeCanvasGroup.interactable = false;
                onComplete?.Invoke();
            });
    }

    public void SetFadeDuration(float duration)
    {
        fadeDuration = duration;
    }
}
