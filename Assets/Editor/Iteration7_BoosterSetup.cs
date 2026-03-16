using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class Iteration7_BoosterSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 7 - Setup Boosters (GameScene)")]
    public static void ShowWindow()
    {
        GetWindow<Iteration7_BoosterSetup>("Iteration 7 - Boosters");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 7 - Booster Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Run on GameScene.\n" +
                         "Adds BoosterManager + BoosterUI panel with 3 buttons:\n" +
                         "- Undo (revert wrong click + restore life)\n" +
                         "- Slowmo (slow rotation + audio filter)\n" +
                         "- +1 Life", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Boosters (Iteration 7)", GUILayout.Height(40)))
        {
            Setup();
        }
    }

    private static void Setup()
    {
        SetupBoosterManager();
        SetupBoosterUI();
        WireGameSceneUI();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 7] Booster setup complete!");
    }

    private static void SetupBoosterManager()
    {
        if (Object.FindObjectOfType<BoosterManager>() != null) return;

        GameObject obj = new GameObject("BoosterManager");
        obj.AddComponent<BoosterManager>();
        Debug.Log("[Iteration 7] Created BoosterManager");
    }

    private static void SetupBoosterUI()
    {
        GameObject canvasObj = GameObject.Find("GameCanvas");
        Debug.Assert(canvasObj != null, "[Iteration 7] GameCanvas not found!");

        Transform existing = canvasObj.transform.Find("BoosterPanel");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
            Debug.Log("[Iteration 7] Removed old BoosterPanel");
        }

        GameObject panel = new GameObject("BoosterPanel");
        panel.transform.SetParent(canvasObj.transform, false);

        RectTransform panelRT = panel.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0, 0);
        panelRT.anchorMax = new Vector2(1, 0);
        panelRT.pivot = new Vector2(0.5f, 0f);
        panelRT.anchoredPosition = new Vector2(0, 30);
        panelRT.sizeDelta = new Vector2(0, 130);

        HorizontalLayoutGroup hlg = panel.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 25f;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.padding = new RectOffset(40, 40, 0, 0);

        Button undoBtn = CreateBoosterButton(panel.transform, "UndoButton", "UNDO",
            new Color(0.3f, 0.5f, 0.8f), "↩");
        Button slowmoBtn = CreateBoosterButton(panel.transform, "SlowmoButton", "SLOW",
            new Color(0.6f, 0.35f, 0.75f), "⏳");
        Button lifeBtn = CreateBoosterButton(panel.transform, "ExtraLifeButton", "+1 HP",
            new Color(0.8f, 0.3f, 0.35f), "♥");

        GameObject slowmoTimerObj = CreateChild(slowmoBtn.transform, "TimerFill");
        RectTransform timerRT = slowmoTimerObj.GetComponent<RectTransform>();
        timerRT.anchorMin = Vector2.zero;
        timerRT.anchorMax = Vector2.one;
        timerRT.sizeDelta = Vector2.zero;
        Image timerImg = slowmoTimerObj.AddComponent<Image>();
        timerImg.color = new Color(0.8f, 0.5f, 1f, 0.3f);
        timerImg.type = Image.Type.Filled;
        timerImg.fillMethod = Image.FillMethod.Vertical;
        timerImg.fillOrigin = 0;
        timerImg.fillAmount = 0f;
        timerImg.raycastTarget = false;
        slowmoTimerObj.transform.SetAsFirstSibling();

        BoosterUI bui = panel.AddComponent<BoosterUI>();

        bui.undoButton = undoBtn;
        bui.undoCountText = undoBtn.transform.Find("Count").GetComponent<TextMeshProUGUI>();
        bui.undoIcon = undoBtn.transform.Find("Icon").GetComponent<Image>();
        bui.undoCanvasGroup = undoBtn.GetComponent<CanvasGroup>();

        bui.slowmoButton = slowmoBtn;
        bui.slowmoCountText = slowmoBtn.transform.Find("Count").GetComponent<TextMeshProUGUI>();
        bui.slowmoIcon = slowmoBtn.transform.Find("Icon").GetComponent<Image>();
        bui.slowmoCanvasGroup = slowmoBtn.GetComponent<CanvasGroup>();
        bui.slowmoTimerFill = timerImg;

        bui.extraLifeButton = lifeBtn;
        bui.extraLifeCountText = lifeBtn.transform.Find("Count").GetComponent<TextMeshProUGUI>();
        bui.extraLifeIcon = lifeBtn.transform.Find("Icon").GetComponent<Image>();
        bui.extraLifeCanvasGroup = lifeBtn.GetComponent<CanvasGroup>();

        bui.panelRect = panelRT;

        EditorUtility.SetDirty(bui);
        Debug.Log("[Iteration 7] Created BoosterUI panel with 3 buttons");
    }

    private static Button CreateBoosterButton(Transform parent, string name, string label, Color bgColor, string iconText)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 110);

        Image bgImg = btnObj.AddComponent<Image>();
        bgImg.color = bgColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = bgImg;
        btn.transition = Selectable.Transition.None;

        btnObj.AddComponent<ButtonFeedback>();
        btnObj.AddComponent<CanvasGroup>();

        GameObject iconObj = CreateChild(btnObj.transform, "Icon");
        RectTransform iconRT = iconObj.GetComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0.5f, 1f);
        iconRT.anchorMax = new Vector2(0.5f, 1f);
        iconRT.pivot = new Vector2(0.5f, 1f);
        iconRT.anchoredPosition = new Vector2(0, -8);
        iconRT.sizeDelta = new Vector2(45, 45);
        Image iconImg = iconObj.AddComponent<Image>();
        iconImg.color = Color.white;
        iconImg.raycastTarget = false;

        GameObject iconLabel = CreateChild(iconObj.transform, "IconText");
        RectTransform ilRT = iconLabel.GetComponent<RectTransform>();
        ilRT.anchorMin = Vector2.zero;
        ilRT.anchorMax = Vector2.one;
        ilRT.sizeDelta = Vector2.zero;
        TextMeshProUGUI ilTMP = iconLabel.AddComponent<TextMeshProUGUI>();
        ilTMP.text = iconText;
        ilTMP.fontSize = 28;
        ilTMP.alignment = TextAlignmentOptions.Center;
        ilTMP.color = bgColor;
        ilTMP.raycastTarget = false;

        GameObject labelObj = CreateChild(btnObj.transform, "Label");
        RectTransform labelRT = labelObj.GetComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0, 0);
        labelRT.anchorMax = new Vector2(1, 0);
        labelRT.pivot = new Vector2(0.5f, 0f);
        labelRT.anchoredPosition = new Vector2(0, 25);
        labelRT.sizeDelta = new Vector2(0, 25);
        TextMeshProUGUI labelTMP = labelObj.AddComponent<TextMeshProUGUI>();
        labelTMP.text = label;
        labelTMP.fontSize = 20;
        labelTMP.fontStyle = FontStyles.Bold;
        labelTMP.alignment = TextAlignmentOptions.Center;
        labelTMP.color = Color.white;
        labelTMP.raycastTarget = false;

        GameObject countObj = CreateChild(btnObj.transform, "Count");
        RectTransform countRT = countObj.GetComponent<RectTransform>();
        countRT.anchorMin = new Vector2(1, 1);
        countRT.anchorMax = new Vector2(1, 1);
        countRT.pivot = new Vector2(1, 1);
        countRT.anchoredPosition = new Vector2(-8, -5);
        countRT.sizeDelta = new Vector2(50, 30);
        TextMeshProUGUI countTMP = countObj.AddComponent<TextMeshProUGUI>();
        countTMP.text = "5";
        countTMP.fontSize = 22;
        countTMP.fontStyle = FontStyles.Bold;
        countTMP.alignment = TextAlignmentOptions.Right;
        countTMP.color = new Color(1f, 1f, 1f, 0.8f);
        countTMP.raycastTarget = false;

        GameObject countBg = CreateChild(btnObj.transform, "CountBg");
        countBg.transform.SetAsFirstSibling();
        RectTransform cbRT = countBg.GetComponent<RectTransform>();
        cbRT.anchorMin = new Vector2(1, 1);
        cbRT.anchorMax = new Vector2(1, 1);
        cbRT.pivot = new Vector2(1, 1);
        cbRT.anchoredPosition = new Vector2(-3, -2);
        cbRT.sizeDelta = new Vector2(42, 28);
        Image cbImg = countBg.AddComponent<Image>();
        cbImg.color = new Color(0, 0, 0, 0.3f);
        cbImg.raycastTarget = false;

        return btn;
    }

    private static void WireGameSceneUI()
    {
        GameSceneUI gsUI = Object.FindObjectOfType<GameSceneUI>();
        if (gsUI == null) return;

        BoosterUI bui = Object.FindObjectOfType<BoosterUI>();
        if (bui != null)
            gsUI.boosterUI = bui;

        EditorUtility.SetDirty(gsUI);
    }

    private static GameObject CreateChild(Transform parent, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }
}
