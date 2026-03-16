using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TutorialPanel : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform panelRect;
    public CanvasGroup panelCanvasGroup;
    public CanvasGroup dimOverlay;

    [Header("Content")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI stepIndicator;
    public Button nextButton;
    public TextMeshProUGUI nextButtonText;

    public event System.Action OnTutorialComplete;

    private int currentStep;
    private bool isAnimating;
    private Sequence currentSequence;

    private static readonly string[] titles = {
        "Welcome!",
        "The Wheel",
        "Collect Fragments",
        "Watch Out!",
        "Boosters",
        "Ready?"
    };

    private static readonly string[] bodies = {
        "Match zodiac fragments to complete\neach sign. Let's learn how!",
        "The wheel spins to the music.\nTap on sections to collect fragments.",
        "Find the correct fragments that\nmatch the zodiac sign in the center.\nThey'll fly in and turn green!",
        "Tap a wrong fragment and you\nlose a life. You have 5 lives.\nRun out and it's game over!",
        "Use boosters to help you:\n• Undo — revert a wrong tap\n• Slow — slow down the wheel\n• +1 HP — gain an extra life",
        "Complete all signs to win.\nThe faster you finish,\nthe more stars you earn!"
    };

    private void OnEnable()
    {
        nextButton.onClick.RemoveListener(OnNextClicked);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    public bool ShouldShow()
    {
        return PlayerPrefs.GetInt("TutorialShown", 0) == 0;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        currentStep = 0;

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        panelRect.localScale = Vector3.one * 0.8f;

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        if (dimOverlay != null)
        {
            dimOverlay.alpha = 0f;
            dimOverlay.blocksRaycasts = true;
            dimOverlay.interactable = true;
            currentSequence.Join(dimOverlay.DOFade(0.7f, 0.3f));
        }

        currentSequence.Join(panelCanvasGroup.DOFade(1f, 0.3f));
        currentSequence.Join(panelRect.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
        currentSequence.OnComplete(() =>
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
        });

        UpdateContent();
    }

    private void UpdateContent()
    {
        titleText.text = titles[currentStep];

        bodyText.DOKill();
        bodyText.alpha = 0f;
        bodyText.text = bodies[currentStep];
        bodyText.DOFade(1f, 0.3f);

        stepIndicator.text = (currentStep + 1) + " / " + titles.Length;

        bool isLast = currentStep >= titles.Length - 1;
        nextButtonText.text = isLast ? "GOT IT!" : "NEXT";

        titleText.transform.DOKill(true);
        titleText.transform.localScale = Vector3.one * 0.8f;
        titleText.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    private void OnNextClicked()
    {
        if (isAnimating) return;

        currentStep++;

        if (currentStep >= titles.Length)
        {
            PlayerPrefs.SetInt("TutorialShown", 1);
            PlayerPrefs.Save();
            Hide();
            return;
        }

        isAnimating = true;
        panelRect.DOPunchScale(Vector3.one * 0.03f, 0.2f, 6, 0.5f)
            .OnComplete(() => isAnimating = false);

        UpdateContent();
    }

    private void Hide()
    {
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence.Append(panelCanvasGroup.DOFade(0f, 0.25f));
        currentSequence.Join(panelRect.DOScale(Vector3.one * 0.8f, 0.25f).SetEase(Ease.InBack));

        if (dimOverlay != null)
            currentSequence.Join(dimOverlay.DOFade(0f, 0.25f));

        currentSequence.OnComplete(() =>
        {
            if (dimOverlay != null)
            {
                dimOverlay.blocksRaycasts = false;
                dimOverlay.interactable = false;
            }
            gameObject.SetActive(false);
            OnTutorialComplete?.Invoke();
        });
    }

    public static void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialShown", 0);
        PlayerPrefs.Save();
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(OnNextClicked);
    }
}
