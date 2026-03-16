using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIJuice : MonoBehaviour
{
    public static UIJuice Instance { get; private set; }

    [Header("References")]
    public TextMeshProUGUI livesText;
    public RectTransform livesContainer;
    public TextMeshProUGUI timerText;

    [Header("Low Life Warning")]
    public int lowLifeThreshold = 2;
    public Color lowLifeColor = new Color(1f, 0.3f, 0.3f);
    public float pulseSpeed = 2f;

    [Header("Timer Colors")]
    public Color timerNormalColor = new Color(0.7f, 0.65f, 0.9f, 0.6f);
    public Color timerWarningColor = new Color(1f, 0.7f, 0.2f, 0.8f);
    public Color timerDangerColor = new Color(1f, 0.3f, 0.3f, 0.9f);

    private bool isLowLifePulsing;
    private Tween lowLifeTween;
    private Color livesOriginalColor;
    private float threeStarTime;
    private float twoStarTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (livesText != null)
            livesOriginalColor = livesText.color;
    }

    public void SetStarThresholds(float threeStar, float twoStar)
    {
        threeStarTime = threeStar;
        twoStarTime = twoStar;
    }

    public void UpdateLivesWarning(int lives)
    {
        if (lives <= lowLifeThreshold && !isLowLifePulsing)
        {
            StartLowLifePulse();
        }
        else if (lives > lowLifeThreshold && isLowLifePulsing)
        {
            StopLowLifePulse();
        }
    }

    public void UpdateTimerColor(float currentTime)
    {
        if (timerText == null) return;

        if (currentTime > twoStarTime)
        {
            timerText.color = timerDangerColor;
        }
        else if (currentTime > threeStarTime)
        {
            timerText.color = timerWarningColor;
        }
        else
        {
            timerText.color = timerNormalColor;
        }
    }

    private void StartLowLifePulse()
    {
        isLowLifePulsing = true;
        lowLifeTween?.Kill();

        if (livesText != null)
        {
            lowLifeTween = livesText.DOColor(lowLifeColor, 0.4f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        if (livesContainer != null)
        {
            livesContainer.DOKill();
            livesContainer.DOScale(Vector3.one * 1.05f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void StopLowLifePulse()
    {
        isLowLifePulsing = false;
        lowLifeTween?.Kill();

        if (livesText != null)
            livesText.color = livesOriginalColor;

        if (livesContainer != null)
        {
            livesContainer.DOKill();
            livesContainer.localScale = Vector3.one;
        }
    }

    public void AnimateScoreNumber(TextMeshProUGUI text, int from, int to, float duration = 0.5f)
    {
        DOTween.To(() => from, x =>
        {
            text.text = x.ToString();
        }, to, duration).SetEase(Ease.OutCubic);
    }

    private void OnDestroy()
    {
        lowLifeTween?.Kill();
    }
}
