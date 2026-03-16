using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BoosterUI : MonoBehaviour
{
    [Header("Undo")]
    public Button undoButton;
    public TextMeshProUGUI undoCountText;
    public Image undoIcon;
    public CanvasGroup undoCanvasGroup;

    [Header("Slowmo")]
    public Button slowmoButton;
    public TextMeshProUGUI slowmoCountText;
    public Image slowmoIcon;
    public CanvasGroup slowmoCanvasGroup;
    public Image slowmoTimerFill;

    [Header("Extra Life")]
    public Button extraLifeButton;
    public TextMeshProUGUI extraLifeCountText;
    public Image extraLifeIcon;
    public CanvasGroup extraLifeCanvasGroup;

    [Header("Panel")]
    public RectTransform panelRect;

    private Color activeColor = new Color(1f, 1f, 1f, 1f);
    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

    private void OnEnable()
    {
        undoButton.onClick.RemoveListener(OnUndoClicked);
        undoButton.onClick.AddListener(OnUndoClicked);

        slowmoButton.onClick.RemoveListener(OnSlowmoClicked);
        slowmoButton.onClick.AddListener(OnSlowmoClicked);

        extraLifeButton.onClick.RemoveListener(OnExtraLifeClicked);
        extraLifeButton.onClick.AddListener(OnExtraLifeClicked);

        if (BoosterManager.Instance != null)
        {
            BoosterManager.Instance.OnBoostersChanged -= RefreshUI;
            BoosterManager.Instance.OnBoostersChanged += RefreshUI;
        }
    }

    private void Start()
    {
        AnimateEntrance();
        RefreshUI();
    }

    private void Update()
    {
        if (BoosterManager.Instance != null && BoosterManager.Instance.IsSlowmoActive())
        {
            if (slowmoTimerFill != null)
            {
                float remaining = BoosterManager.Instance.GetSlowmoTimeRemaining();
                slowmoTimerFill.fillAmount = remaining / 5f;
            }
        }
        else
        {
            if (slowmoTimerFill != null)
                slowmoTimerFill.fillAmount = 0f;
        }
    }

    private void AnimateEntrance()
    {
        if (panelRect != null)
        {
            UIAnimations.SlideInFromBottom(panelRect, 150f, 0.5f, 0.3f);
        }
    }

    public void RefreshUI()
    {
        int undoCount = GameManager.Instance.BoosterUndo;
        int slowmoCount = GameManager.Instance.BoosterSlowmo;
        int lifeCount = GameManager.Instance.BoosterExtraLife;

        undoCountText.text = undoCount.ToString();
        slowmoCountText.text = slowmoCount.ToString();
        extraLifeCountText.text = lifeCount.ToString();

        bool canUndo = BoosterManager.Instance != null && BoosterManager.Instance.CanUseUndo();
        SetButtonState(undoButton, undoCanvasGroup, undoIcon, canUndo);

        bool canSlowmo = BoosterManager.Instance != null && BoosterManager.Instance.CanUseSlowmo();
        SetButtonState(slowmoButton, slowmoCanvasGroup, slowmoIcon, canSlowmo);

        bool canLife = BoosterManager.Instance != null && BoosterManager.Instance.CanUseExtraLife();
        SetButtonState(extraLifeButton, extraLifeCanvasGroup, extraLifeIcon, canLife);
    }

    private void SetButtonState(Button btn, CanvasGroup cg, Image icon, bool active)
    {
        btn.interactable = active;
        if (cg != null)
            cg.alpha = active ? 1f : 0.4f;
        if (icon != null)
            icon.color = active ? activeColor : inactiveColor;
    }

    private void OnUndoClicked()
    {
        if (BoosterManager.Instance == null) return;
        BoosterManager.Instance.UseUndo();
        AnimateButtonUse(undoButton.transform);
        RefreshUI();
    }

    private void OnSlowmoClicked()
    {
        if (BoosterManager.Instance == null) return;
        BoosterManager.Instance.UseSlowmo();
        AnimateButtonUse(slowmoButton.transform);
        RefreshUI();
    }

    private void OnExtraLifeClicked()
    {
        if (BoosterManager.Instance == null) return;
        BoosterManager.Instance.UseExtraLife();
        AnimateButtonUse(extraLifeButton.transform);
        RefreshUI();
    }

    private void AnimateButtonUse(Transform btn)
    {
        btn.DOKill(true);
        btn.DOPunchScale(Vector3.one * 0.15f, 0.35f, 8, 0.5f);
    }

    private void OnDisable()
    {
        undoButton.onClick.RemoveListener(OnUndoClicked);
        slowmoButton.onClick.RemoveListener(OnSlowmoClicked);
        extraLifeButton.onClick.RemoveListener(OnExtraLifeClicked);

        if (BoosterManager.Instance != null)
            BoosterManager.Instance.OnBoostersChanged -= RefreshUI;
    }
}
