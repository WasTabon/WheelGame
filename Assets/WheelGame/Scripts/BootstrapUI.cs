using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BootstrapUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image progressBarFill;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI progressPercentText;
    public TextMeshProUGUI titleText;
    public Button retryButton;
    public CanvasGroup retryCanvasGroup;

    [Header("References")]
    public AddressableLoader loader;
    public MusicService musicService;

    private float displayProgress;
    private float targetProgress;
    private bool isComplete;

    private void OnEnable()
    {
        retryButton.onClick.RemoveListener(OnRetryClicked);
        retryButton.onClick.AddListener(OnRetryClicked);
    }

    private void Start()
    {
        retryCanvasGroup.alpha = 0f;
        retryCanvasGroup.interactable = false;
        retryCanvasGroup.blocksRaycasts = false;

        progressBarFill.fillAmount = 0f;
        displayProgress = 0f;
        targetProgress = 0f;

        AnimateEntrance();

        loader.OnDownloadProgress -= OnProgress;
        loader.OnDownloadProgress += OnProgress;

        loader.OnStatusChanged -= OnStatus;
        loader.OnStatusChanged += OnStatus;

        loader.OnDownloadComplete -= OnComplete;
        loader.OnDownloadComplete += OnComplete;

        loader.OnDownloadFailed -= OnFailed;
        loader.OnDownloadFailed += OnFailed;

    #if UNITY_ANDROID
            if (statusText != null)
                statusText.text = "Loading...";
            targetProgress = 1f;
            DOVirtual.DelayedCall(0.8f, TransitionToMainMenu);
    #else
            DOVirtual.DelayedCall(0.5f, () => loader.StartDownload());
    #endif
    }

    private void Update()
    {
        if (Mathf.Abs(displayProgress - targetProgress) > 0.001f)
        {
            displayProgress = Mathf.Lerp(displayProgress, targetProgress, Time.deltaTime * 8f);
            progressBarFill.fillAmount = displayProgress;

            if (progressPercentText != null)
                progressPercentText.text = Mathf.RoundToInt(displayProgress * 100f) + "%";
        }
    }

    private void AnimateEntrance()
    {
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f);
        }

        if (statusText != null)
        {
            statusText.alpha = 0f;
            statusText.DOFade(1f, 0.4f).SetDelay(0.4f);
        }
    }

    private void OnProgress(float progress)
    {
        targetProgress = progress;
    }

    private void OnStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = status;
            statusText.transform.DOKill(true);
            statusText.transform.DOPunchScale(Vector3.one * 0.05f, 0.2f, 6, 0.5f);
        }
    }

    private void OnComplete()
    {
        targetProgress = 1f;
        isComplete = true;

        if (statusText != null)
            statusText.text = "Loading music...";

        musicService.LoadMusic(
            onComplete: () =>
            {
                if (statusText != null)
                    statusText.text = "Ready!";
                DOVirtual.DelayedCall(0.5f, TransitionToMainMenu);
            },
            onFailed: (error) =>
            {
                ShowRetry("Failed to load music");
            }
        );
    }

    private void OnFailed(string error)
    {
        ShowRetry(error);
    }

    private void ShowRetry(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = new Color(1f, 0.4f, 0.35f);
        }

        retryCanvasGroup.DOFade(1f, 0.35f).SetEase(Ease.OutQuad);
        retryCanvasGroup.interactable = true;
        retryCanvasGroup.blocksRaycasts = true;

        retryButton.transform.localScale = Vector3.zero;
        retryButton.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }

    private void HideRetry()
    {
        retryCanvasGroup.DOFade(0f, 0.2f);
        retryCanvasGroup.interactable = false;
        retryCanvasGroup.blocksRaycasts = false;

        if (statusText != null)
            statusText.color = Color.white;
    }

    private void OnRetryClicked()
    {
        HideRetry();
        targetProgress = 0f;
        displayProgress = 0f;
        progressBarFill.fillAmount = 0f;
        loader.StartDownload();
    }

    private void TransitionToMainMenu()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }

    private void OnDisable()
    {
        retryButton.onClick.RemoveListener(OnRetryClicked);

        if (loader != null)
        {
            loader.OnDownloadProgress -= OnProgress;
            loader.OnStatusChanged -= OnStatus;
            loader.OnDownloadComplete -= OnComplete;
            loader.OnDownloadFailed -= OnFailed;
        }
    }
}
