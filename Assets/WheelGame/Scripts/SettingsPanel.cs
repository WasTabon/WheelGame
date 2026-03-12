using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform panelRect;
    public CanvasGroup panelCanvasGroup;
    public CanvasGroup dimOverlay;

    [Header("Controls")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button closeButton;

    [Header("Labels")]
    public TextMeshProUGUI titleLabel;
    public TextMeshProUGUI musicLabel;
    public TextMeshProUGUI sfxLabel;

    private bool isOpen;
    private bool isAnimating;
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private Sequence currentSequence;

    private void OnEnable()
    {
        closeButton.onClick.RemoveListener(Close);
        closeButton.onClick.AddListener(Close);

        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);

        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        if (dimOverlay != null)
        {
            Button dimButton = dimOverlay.GetComponent<Button>();
            if (dimButton != null)
            {
                dimButton.onClick.RemoveListener(Close);
                dimButton.onClick.AddListener(Close);
            }
        }
    }

    private void Start()
    {
        shownPosition = Vector2.zero;
        hiddenPosition = new Vector2(0f, -1200f);

        panelRect.anchoredPosition = hiddenPosition;
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        if (dimOverlay != null)
        {
            dimOverlay.alpha = 0f;
            dimOverlay.blocksRaycasts = false;
            dimOverlay.interactable = false;
        }

        if (AudioManager.Instance != null)
        {
            musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SfxVolume);
        }
    }

    public void Open()
    {
        if (isOpen || isAnimating) return;
        isAnimating = true;
        isOpen = true;

        gameObject.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelOpen();

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        if (dimOverlay != null)
        {
            dimOverlay.blocksRaycasts = true;
            dimOverlay.interactable = true;
            currentSequence.Join(dimOverlay.DOFade(0.6f, 0.3f).SetEase(Ease.OutQuad));
        }

        currentSequence.Join(panelRect.DOAnchorPos(shownPosition, 0.45f).SetEase(Ease.OutBack, 1.1f));
        currentSequence.Join(panelCanvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad));
        currentSequence.OnComplete(() =>
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
            isAnimating = false;
        });
    }

    public void Close()
    {
        if (!isOpen || isAnimating) return;
        isAnimating = true;
        isOpen = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelClose();

        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        if (dimOverlay != null)
        {
            currentSequence.Join(dimOverlay.DOFade(0f, 0.25f).SetEase(Ease.InQuad));
        }

        currentSequence.Join(panelRect.DOAnchorPos(hiddenPosition, 0.35f).SetEase(Ease.InBack, 1.1f));
        currentSequence.Join(panelCanvasGroup.DOFade(0f, 0.25f).SetEase(Ease.InQuad));
        currentSequence.OnComplete(() =>
        {
            if (dimOverlay != null)
            {
                dimOverlay.blocksRaycasts = false;
                dimOverlay.interactable = false;
            }
            isAnimating = false;
        });
    }

    private void OnMusicChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSfxVolume(value);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(Close);
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
    }
}
