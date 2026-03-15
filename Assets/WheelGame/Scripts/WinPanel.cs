using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WinPanel : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform panelRect;
    public CanvasGroup panelCanvasGroup;
    public CanvasGroup dimOverlay;

    [Header("Content")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI timeText;
    public Image[] starImages;
    public TextMeshProUGUI levelCompleteText;

    [Header("Buttons")]
    public Button nextButton;
    public Button menuButton;

    private bool isAnimating;
    private Sequence currentSequence;

    private void OnEnable()
    {
        nextButton.onClick.RemoveListener(OnNextClicked);
        nextButton.onClick.AddListener(OnNextClicked);

        menuButton.onClick.RemoveListener(OnMenuClicked);
        menuButton.onClick.AddListener(OnMenuClicked);
    }

    public void Show(int stars, float time, int levelNumber)
    {
        gameObject.SetActive(true);
        isAnimating = true;

        if (levelCompleteText != null)
            levelCompleteText.text = "Level " + levelNumber + " Complete!";

        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        foreach (Image star in starImages)
        {
            star.color = new Color(0.3f, 0.3f, 0.35f, 0.5f);
            star.transform.localScale = Vector3.zero;
        }

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

        for (int i = 0; i < starImages.Length; i++)
        {
            int idx = i;
            bool earned = idx < stars;
            float delay = 0.5f + idx * 0.25f;

            currentSequence.InsertCallback(delay, () =>
            {
                if (earned)
                    starImages[idx].color = new Color(1f, 0.85f, 0.1f, 1f);
                else
                    starImages[idx].color = new Color(0.3f, 0.3f, 0.35f, 0.5f);

                starImages[idx].transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
            });
        }

        float totalDelay = 0.5f + starImages.Length * 0.25f + 0.2f;
        currentSequence.InsertCallback(totalDelay, () =>
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

    private void OnNextClicked()
    {
        if (isAnimating) return;
        GameplayManager.Instance.NextLevel();
    }

    private void OnMenuClicked()
    {
        if (isAnimating) return;
        GameplayManager.Instance.GoToMenu();
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(OnNextClicked);
        menuButton.onClick.RemoveListener(OnMenuClicked);
    }
}
