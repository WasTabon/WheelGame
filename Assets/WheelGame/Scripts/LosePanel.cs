using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LosePanel : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform panelRect;
    public CanvasGroup panelCanvasGroup;
    public CanvasGroup dimOverlay;

    [Header("Content")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;

    [Header("Buttons")]
    public Button retryButton;
    public Button menuButton;

    private bool isAnimating;
    private Sequence currentSequence;

    private void OnEnable()
    {
        retryButton.onClick.RemoveListener(OnRetryClicked);
        retryButton.onClick.AddListener(OnRetryClicked);

        menuButton.onClick.RemoveListener(OnMenuClicked);
        menuButton.onClick.AddListener(OnMenuClicked);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        isAnimating = true;

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        panelRect.localScale = Vector3.one * 0.7f;

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        if (dimOverlay != null)
        {
            dimOverlay.alpha = 0f;
            dimOverlay.blocksRaycasts = true;
            dimOverlay.interactable = true;
            currentSequence.Append(dimOverlay.DOFade(0.7f, 0.3f));
        }

        currentSequence.Join(panelCanvasGroup.DOFade(1f, 0.35f));
        currentSequence.Join(panelRect.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

        if (panelRect != null)
        {
            currentSequence.Append(panelRect.DOShakeRotation(0.5f, new Vector3(0, 0, 5f), 10, 90).SetDelay(0.1f));
        }

        currentSequence.AppendCallback(() =>
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
            isAnimating = false;
        });
    }

    public void Hide()
    {
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        currentSequence.Append(panelCanvasGroup.DOFade(0f, 0.25f));
        currentSequence.Join(panelRect.DOScale(Vector3.one * 0.7f, 0.25f).SetEase(Ease.InBack));

        if (dimOverlay != null)
        {
            currentSequence.Join(dimOverlay.DOFade(0f, 0.25f));
        }

        currentSequence.OnComplete(() =>
        {
            if (dimOverlay != null)
            {
                dimOverlay.blocksRaycasts = false;
                dimOverlay.interactable = false;
            }
            gameObject.SetActive(false);
        });
    }

    private void OnRetryClicked()
    {
        if (isAnimating) return;
        Hide();
        DOVirtual.DelayedCall(0.35f, () => GameplayManager.Instance.RetryLevel());
    }

    private void OnMenuClicked()
    {
        if (isAnimating) return;
        GameplayManager.Instance.GoToMenu();
    }

    private void OnDisable()
    {
        retryButton.onClick.RemoveListener(OnRetryClicked);
        menuButton.onClick.RemoveListener(OnMenuClicked);
    }
}
