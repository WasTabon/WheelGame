using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using TMPro;

public class IAPManager : MonoBehaviour
{
    public string productId = "com.wheelgame.boosterpack";

    public GameObject loadingButton;

    [Header("Shop Panel")]
    public CanvasGroup panelGroup;
    public RectTransform panelRect;
    public CanvasGroup dimOverlay;

    [Header("UI")]
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI undoCountText;
    public TextMeshProUGUI slowmoCountText;
    public TextMeshProUGUI extraLifeCountText;
    public Button closeButton;

    private bool isOpen;

    private void Awake()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }
        if (dimOverlay != null)
        {
            dimOverlay.alpha = 0f;
            dimOverlay.blocksRaycasts = false;
            dimOverlay.interactable = false;
        }
    }

    private void OnEnable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Hide);
            closeButton.onClick.AddListener(Hide);
        }
        if (dimOverlay != null)
        {
            Button dimBtn = dimOverlay.GetComponent<Button>();
            if (dimBtn != null)
            {
                dimBtn.onClick.RemoveListener(Hide);
                dimBtn.onClick.AddListener(Hide);
            }
        }
    }

    public void Show()
    {
        if (isOpen) return;
        isOpen = true;

        if (statusText != null)
            statusText.text = "";

        if (loadingButton != null)
            loadingButton.SetActive(false);

        RefreshCounts();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelOpen();

        if (dimOverlay != null)
        {
            dimOverlay.alpha = 0f;
            dimOverlay.blocksRaycasts = true;
            dimOverlay.interactable = true;
            dimOverlay.DOFade(0.6f, 0.3f);
        }

        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
        panelRect.localScale = Vector3.one * 0.8f;
        panelGroup.DOFade(1f, 0.25f);
        panelRect.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        if (!isOpen) return;
        isOpen = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelClose();

        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;

        panelGroup.DOFade(0f, 0.2f);
        panelRect.DOScale(0.8f, 0.2f).SetEase(Ease.InBack);

        if (dimOverlay != null)
        {
            dimOverlay.DOFade(0f, 0.2f).OnComplete(() =>
            {
                dimOverlay.blocksRaycasts = false;
                dimOverlay.interactable = false;
            });
        }
    }

    public void OnBuyClicked()
    {
        if (loadingButton != null)
            loadingButton.SetActive(true);
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == productId)
        {
            Debug.Log("[IAP] Purchase complete: " + productId);

            GameManager.Instance.AddBoosterPack();

            if (loadingButton != null)
                loadingButton.SetActive(false);

            RefreshCounts();

            if (statusText != null)
            {
                statusText.color = new Color(0.25f, 0.82f, 0.50f, 1f);
                statusText.text = "+1 Undo  +1 Slowmo  +1 Life";
            }

            if (AudioManager.Instance != null && AudioManager.Instance.sfxButtonClick != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxButtonClick);

            if (panelRect != null)
                panelRect.DOPunchScale(Vector3.one * 0.1f, 0.3f, 5);
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        if (product.definition.id == productId)
        {
            Debug.Log("[IAP] Failed: " + description.message);

            if (loadingButton != null)
                loadingButton.SetActive(false);

            if (statusText != null)
            {
                statusText.color = new Color(0.90f, 0.22f, 0.35f, 1f);
                statusText.text = "Purchase failed";
            }
        }
    }

    public void OnProductFetched(Product product)
    {
        Debug.Log("[IAP] Fetched: " + product.metadata.localizedPriceString);
        if (priceText != null)
            priceText.text = product.metadata.localizedPriceString;
    }

    private void RefreshCounts()
    {
        if (undoCountText != null)
            undoCountText.text = GameManager.Instance.BoosterUndo.ToString();
        if (slowmoCountText != null)
            slowmoCountText.text = GameManager.Instance.BoosterSlowmo.ToString();
        if (extraLifeCountText != null)
            extraLifeCountText.text = GameManager.Instance.BoosterExtraLife.ToString();
    }

    private void OnDisable()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Hide);
        if (dimOverlay != null)
        {
            Button dimBtn = dimOverlay.GetComponent<Button>();
            if (dimBtn != null)
                dimBtn.onClick.RemoveListener(Hide);
        }
    }
}
