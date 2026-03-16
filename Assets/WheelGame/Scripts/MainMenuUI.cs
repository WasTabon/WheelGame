using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Title")]
    public RectTransform titleTransform;
    public CanvasGroup titleCanvasGroup;

    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button shopButton;

    [Header("Panels")]
    public SettingsPanel settingsPanel;
    public LevelSelectPanel levelSelectPanel;
    public IAPManager shopPanel;

    [Header("Button Containers")]
    public RectTransform playButtonRect;
    public RectTransform settingsButtonRect;
    public RectTransform shopButtonRect;

    private bool isTransitioning;

    private void OnEnable()
    {
        playButton.onClick.RemoveListener(OnPlayClicked);
        playButton.onClick.AddListener(OnPlayClicked);

        settingsButton.onClick.RemoveListener(OnSettingsClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);

        shopButton.onClick.RemoveListener(OnShopClicked);
        shopButton.onClick.AddListener(OnShopClicked);
    }

    private void Start()
    {
        AnimateEntrance();
    }

    private void AnimateEntrance()
    {
        if (titleCanvasGroup != null)
        {
            titleCanvasGroup.alpha = 0f;
            titleTransform.anchoredPosition += Vector2.up * 60f;
            DOTween.Sequence()
                .Append(titleCanvasGroup.DOFade(1f, 0.6f).SetEase(Ease.OutQuad))
                .Join(titleTransform.DOAnchorPosY(titleTransform.anchoredPosition.y - 60f, 0.7f).SetEase(Ease.OutCubic));
        }

        Transform[] buttons = { playButtonRect, settingsButtonRect, shopButtonRect };
        UIAnimations.StaggeredAppear(buttons, 0.12f, 0.4f).SetDelay(0.3f);
    }

    private void OnPlayClicked()
    {
        if (isTransitioning) return;
        levelSelectPanel.Open();
    }

    private void OnSettingsClicked()
    {
        if (isTransitioning) return;
        settingsPanel.Open();
    }

    private void OnShopClicked()
    {
        if (isTransitioning) return;
        if (shopPanel != null)
            shopPanel.Show();
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlayClicked);
        settingsButton.onClick.RemoveListener(OnSettingsClicked);
        shopButton.onClick.RemoveListener(OnShopClicked);
    }
}
