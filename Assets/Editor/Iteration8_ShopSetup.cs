using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class Iteration8_ShopSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 8 - Setup Shop (MainMenu scene)")]
    public static void ShowWindow()
    {
        GetWindow<Iteration8_ShopSetup>("Iteration 8 - Shop Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 8 - Shop + IAP Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Run on MainMenu scene.\n" +
                         "Creates Shop Panel with:\n" +
                         "- Booster pack display (current counts)\n" +
                         "- Buy button area (you add IAP Button component)\n" +
                         "- Loading overlay button\n" +
                         "- Close button + dim overlay\n\n" +
                         "After running this:\n" +
                         "1. Add IAP Button component to BuyButton\n" +
                         "2. Set Product ID: com.wheelgame.boosterpack\n" +
                         "3. Wire OnPurchaseComplete, OnPurchaseFailed,\n" +
                         "   OnProductFetched to IAPManager\n" +
                         "4. Wire BuyButton.OnClick → IAPManager.OnBuyClicked", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Shop (Iteration 8)", GUILayout.Height(40)))
        {
            Setup();
        }
    }

    private static void Setup()
    {
        GameObject canvasObj = GameObject.Find("MainMenuCanvas");
        Debug.Assert(canvasObj != null, "[Iteration 8] MainMenuCanvas not found!");

        CleanupOld(canvasObj.transform);
        CreateShopPanel(canvasObj.transform);
        WireMainMenuUI(canvasObj);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 8] Shop setup complete!");
    }

    private static void CleanupOld(Transform canvas)
    {
        Transform old = canvas.Find("ShopPanel");
        if (old != null)
        {
            Object.DestroyImmediate(old.gameObject);
            Debug.Log("[Iteration 8] Removed old ShopPanel");
        }
    }

    private static void CreateShopPanel(Transform canvasTransform)
    {
        GameObject root = new GameObject("ShopPanel");
        root.transform.SetParent(canvasTransform, false);
        RectTransform rootRT = root.AddComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.sizeDelta = Vector2.zero;

        GameObject dimObj = CreateChild(root.transform, "Dim");
        RectTransform dimRT = dimObj.GetComponent<RectTransform>();
        dimRT.anchorMin = Vector2.zero;
        dimRT.anchorMax = Vector2.one;
        dimRT.sizeDelta = Vector2.zero;
        Image dimImg = dimObj.AddComponent<Image>();
        dimImg.color = new Color(0, 0, 0, 1f);
        CanvasGroup dimCG = dimObj.AddComponent<CanvasGroup>();
        dimCG.alpha = 0f;
        dimCG.blocksRaycasts = false;
        dimObj.AddComponent<Button>();

        GameObject panelBg = CreateChild(root.transform, "PanelBg");
        RectTransform panelRT = panelBg.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(850, 850);
        Image panelImg = panelBg.AddComponent<Image>();
        panelImg.color = new Color(0.12f, 0.1f, 0.22f, 0.97f);
        CanvasGroup panelCG = panelBg.AddComponent<CanvasGroup>();

        CreateTMP(panelBg.transform, "Title", "BOOSTER SHOP",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            new Vector2(0, -30), new Vector2(0, 60),
            46, FontStyles.Bold, Color.white);

        CreateTMP(panelBg.transform, "Subtitle", "Get +1 of each booster!",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            new Vector2(0, -95), new Vector2(0, 35),
            26, FontStyles.Normal, new Color(0.7f, 0.65f, 0.9f, 0.8f));

        float boosterY = -160f;
        float boosterSpacing = 70f;

        CreateBoosterRow(panelBg.transform, "UndoRow", "Undo", "↩",
            new Color(0.3f, 0.5f, 0.8f), boosterY);
        CreateBoosterRow(panelBg.transform, "SlowmoRow", "Slowmo", "⏳",
            new Color(0.6f, 0.35f, 0.75f), boosterY - boosterSpacing);
        CreateBoosterRow(panelBg.transform, "ExtraLifeRow", "Extra Life", "♥",
            new Color(0.8f, 0.3f, 0.35f), boosterY - boosterSpacing * 2);

        CreateTMP(panelBg.transform, "PackLabel", "Booster Pack",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            new Vector2(0, -410), new Vector2(0, 40),
            32, FontStyles.Bold, new Color(1f, 0.85f, 0.1f, 1f));

        CreateTMP(panelBg.transform, "PackDesc", "+1 Undo  •  +1 Slowmo  •  +1 Life",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            new Vector2(0, -450), new Vector2(0, 30),
            22, FontStyles.Normal, new Color(0.8f, 0.75f, 0.95f, 0.7f));

        GameObject buyBtnObj = CreateChild(panelBg.transform, "BuyButton");
        RectTransform buyRT = buyBtnObj.GetComponent<RectTransform>();
        buyRT.anchorMin = new Vector2(0.5f, 0.5f);
        buyRT.anchorMax = new Vector2(0.5f, 0.5f);
        buyRT.anchoredPosition = new Vector2(0, -100);
        buyRT.sizeDelta = new Vector2(450, 100);
        Image buyImg = buyBtnObj.AddComponent<Image>();
        buyImg.color = new Color(0.2f, 0.7f, 0.3f);
        Button buyBtn = buyBtnObj.AddComponent<Button>();
        buyBtn.targetGraphic = buyImg;
        buyBtn.transition = Selectable.Transition.None;
        buyBtnObj.AddComponent<ButtonFeedback>();

        TextMeshProUGUI priceTMP = CreateTMP(buyBtnObj.transform, "PriceText", "$0.99",
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero,
            36, FontStyles.Bold, Color.white);

        GameObject loadingObj = CreateChild(buyBtnObj.transform, "LoadingOverlay");
        RectTransform loadRT = loadingObj.GetComponent<RectTransform>();
        loadRT.anchorMin = Vector2.zero;
        loadRT.anchorMax = Vector2.one;
        loadRT.sizeDelta = Vector2.zero;
        Image loadImg = loadingObj.AddComponent<Image>();
        loadImg.color = new Color(0.1f, 0.1f, 0.15f, 0.85f);
        loadImg.raycastTarget = true;

        CreateTMP(loadingObj.transform, "LoadingText", "Loading...",
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero,
            28, FontStyles.Normal, new Color(0.8f, 0.8f, 0.9f, 0.8f));

        loadingObj.SetActive(false);

        TextMeshProUGUI statusTMP = CreateTMP(panelBg.transform, "StatusText", "",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            new Vector2(0, -590), new Vector2(0, 35),
            24, FontStyles.Normal, Color.white);

        GameObject closeBtnObj = CreateChild(panelBg.transform, "CloseButton");
        RectTransform closeRT = closeBtnObj.GetComponent<RectTransform>();
        closeRT.anchorMin = new Vector2(0.5f, 0f);
        closeRT.anchorMax = new Vector2(0.5f, 0f);
        closeRT.pivot = new Vector2(0.5f, 0f);
        closeRT.anchoredPosition = new Vector2(0, 25);
        closeRT.sizeDelta = new Vector2(300, 80);
        Image closeImg = closeBtnObj.AddComponent<Image>();
        closeImg.color = new Color(0.35f, 0.3f, 0.55f);
        Button closeBtn = closeBtnObj.AddComponent<Button>();
        closeBtn.targetGraphic = closeImg;
        closeBtn.transition = Selectable.Transition.None;
        closeBtnObj.AddComponent<ButtonFeedback>();

        CreateTMP(closeBtnObj.transform, "Label", "CLOSE",
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero,
            32, FontStyles.Bold, Color.white);

        IAPManager iap = root.AddComponent<IAPManager>();
        iap.productId = "com.wheelgame.boosterpack";
        iap.loadingButton = loadingObj;
        iap.panelGroup = panelCG;
        iap.panelRect = panelRT;
        iap.dimOverlay = dimCG;
        iap.priceText = priceTMP;
        iap.statusText = statusTMP;
        iap.closeButton = closeBtn;

        iap.undoCountText = panelBg.transform.Find("UndoRow")?.Find("Count")?.GetComponent<TextMeshProUGUI>();
        iap.slowmoCountText = panelBg.transform.Find("SlowmoRow")?.Find("Count")?.GetComponent<TextMeshProUGUI>();
        iap.extraLifeCountText = panelBg.transform.Find("ExtraLifeRow")?.Find("Count")?.GetComponent<TextMeshProUGUI>();

        EditorUtility.SetDirty(iap);

        Debug.Log("[Iteration 8] Created ShopPanel. TODO: Add IAP Button component to BuyButton and wire callbacks.");
    }

    private static void CreateBoosterRow(Transform parent, string name, string label, string icon, Color color, float yPos)
    {
        GameObject row = CreateChild(parent, name);
        RectTransform rowRT = row.GetComponent<RectTransform>();
        rowRT.anchorMin = new Vector2(0, 1);
        rowRT.anchorMax = new Vector2(1, 1);
        rowRT.pivot = new Vector2(0.5f, 1f);
        rowRT.anchoredPosition = new Vector2(0, yPos);
        rowRT.sizeDelta = new Vector2(-80, 55);

        Image rowBg = row.AddComponent<Image>();
        rowBg.color = new Color(color.r, color.g, color.b, 0.15f);

        GameObject iconObj = CreateChild(row.transform, "Icon");
        RectTransform iconRT = iconObj.GetComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0, 0);
        iconRT.anchorMax = new Vector2(0, 1);
        iconRT.pivot = new Vector2(0, 0.5f);
        iconRT.anchoredPosition = new Vector2(15, 0);
        iconRT.sizeDelta = new Vector2(45, 0);
        TextMeshProUGUI iconTMP = iconObj.AddComponent<TextMeshProUGUI>();
        iconTMP.text = icon;
        iconTMP.fontSize = 28;
        iconTMP.alignment = TextAlignmentOptions.Center;
        iconTMP.color = color;

        CreateTMP(row.transform, "Label", label,
            new Vector2(0, 0), new Vector2(0.6f, 1), new Vector2(0, 0.5f),
            new Vector2(70, 0), Vector2.zero,
            28, FontStyles.Normal, new Color(0.85f, 0.82f, 0.95f));

        TextMeshProUGUI countTMP = CreateTMP(row.transform, "Count", "5",
            new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 0.5f),
            new Vector2(-15, 0), new Vector2(60, 0),
            30, FontStyles.Bold, Color.white);
    }

    private static void WireMainMenuUI(GameObject canvasObj)
    {
        MainMenuUI menuUI = canvasObj.GetComponent<MainMenuUI>();
        Debug.Assert(menuUI != null, "[Iteration 8] MainMenuUI not found!");

        Transform shopT = canvasObj.transform.Find("ShopPanel");
        if (shopT != null)
            menuUI.shopPanel = shopT.GetComponent<IAPManager>();

        EditorUtility.SetDirty(menuUI);
    }

    private static TextMeshProUGUI CreateTMP(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 size,
        float fontSize, FontStyles style, Color color)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            TextMeshProUGUI t = existing.GetComponent<TextMeshProUGUI>();
            if (t != null) { t.text = text; return t; }
        }

        GameObject obj = CreateChild(parent, name);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        return tmp;
    }

    private static GameObject CreateChild(Transform parent, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }
}
