using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class Iteration6_LevelSelectSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 6 - Setup Level Select (MainMenu scene)")]
    public static void ShowWindow()
    {
        GetWindow<Iteration6_LevelSelectSetup>("Iteration 6 - Level Select");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 6 - Level Select Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Run on MainMenu scene.\n" +
                         "Will delete old LevelSelectPanel if exists.\n" +
                         "Creates LevelButton prefab + ScrollView with 30 levels.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Level Select (Iteration 6)", GUILayout.Height(40)))
        {
            Setup();
        }
    }

    private const int TOTAL_LEVELS = 30;
    private const float BUTTON_HEIGHT = 115f;
    private const float BUTTON_SPACING = 10f;
    private static string prefabPath = "Assets/WheelGame/Prefabs";
    private static string prefabFile = "Assets/WheelGame/Prefabs/LevelButton.prefab";

    private static void Setup()
    {
        GameObject canvasObj = GameObject.Find("MainMenuCanvas");
        Debug.Assert(canvasObj != null, "[Iteration 6] MainMenuCanvas not found!");

        CleanupOldPanel(canvasObj.transform);
        CreatePrefab();
        CreateLevelSelectPanel(canvasObj.transform);
        WireMainMenuUI(canvasObj);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 6] Level Select setup complete!");
    }

    private static void CleanupOldPanel(Transform canvas)
    {
        Transform old = canvas.Find("LevelSelectPanel");
        if (old != null)
        {
            Object.DestroyImmediate(old.gameObject);
            Debug.Log("[Iteration 6] Removed old LevelSelectPanel");
        }
    }

    private static void CreatePrefab()
    {
        if (!Directory.Exists(prefabPath))
            Directory.CreateDirectory(prefabPath);

        GameObject template = new GameObject("LevelButton");

        RectTransform rt = template.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, BUTTON_HEIGHT);

        LayoutElement le = template.AddComponent<LayoutElement>();
        le.minHeight = BUTTON_HEIGHT;
        le.preferredHeight = BUTTON_HEIGHT;
        le.flexibleWidth = 1f;

        Image bgImg = template.AddComponent<Image>();
        bgImg.color = new Color(0.25f, 0.22f, 0.45f);

        Button btn = template.AddComponent<Button>();
        btn.targetGraphic = bgImg;
        btn.transition = Selectable.Transition.None;

        template.AddComponent<ButtonFeedback>();

        GameObject numObj = CreateChild(template.transform, "LevelNumber");
        RectTransform numRT = numObj.GetComponent<RectTransform>();
        numRT.anchorMin = new Vector2(0, 0);
        numRT.anchorMax = new Vector2(0, 1);
        numRT.pivot = new Vector2(0, 0.5f);
        numRT.anchoredPosition = new Vector2(25, 0);
        numRT.sizeDelta = new Vector2(80, 0);
        TextMeshProUGUI numTMP = numObj.AddComponent<TextMeshProUGUI>();
        numTMP.text = "1";
        numTMP.fontSize = 46;
        numTMP.fontStyle = FontStyles.Bold;
        numTMP.alignment = TextAlignmentOptions.Center;
        numTMP.color = Color.white;

        GameObject lockObj = CreateChild(template.transform, "LockIcon");
        RectTransform lockRT = lockObj.GetComponent<RectTransform>();
        lockRT.anchorMin = new Vector2(0.5f, 0.5f);
        lockRT.anchorMax = new Vector2(0.5f, 0.5f);
        lockRT.sizeDelta = new Vector2(45, 45);
        Image lockImg = lockObj.AddComponent<Image>();
        lockImg.color = new Color(0.5f, 0.45f, 0.65f, 0.7f);

        GameObject lockText = CreateChild(lockObj.transform, "LockText");
        RectTransform lockTextRT = lockText.GetComponent<RectTransform>();
        lockTextRT.anchorMin = Vector2.zero;
        lockTextRT.anchorMax = Vector2.one;
        lockTextRT.sizeDelta = Vector2.zero;
        TextMeshProUGUI lockTMP = lockText.AddComponent<TextMeshProUGUI>();
        lockTMP.text = "🔒";
        lockTMP.fontSize = 24;
        lockTMP.alignment = TextAlignmentOptions.Center;
        lockTMP.color = Color.white;

        Image[] starImgs = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject starObj = CreateChild(template.transform, "Star_" + i);
            RectTransform starRT = starObj.GetComponent<RectTransform>();
            starRT.anchorMin = new Vector2(1, 0.5f);
            starRT.anchorMax = new Vector2(1, 0.5f);
            starRT.pivot = new Vector2(1, 0.5f);
            starRT.anchoredPosition = new Vector2(-20 - (2 - i) * 48, 0);
            starRT.sizeDelta = new Vector2(38, 38);
            Image starImg = starObj.AddComponent<Image>();
            starImg.color = new Color(0.3f, 0.28f, 0.4f, 0.5f);
            starImgs[i] = starImg;
        }

        LevelButton lb = template.AddComponent<LevelButton>();
        lb.levelNumberText = numTMP;
        lb.backgroundImage = bgImg;
        lb.lockIcon = lockImg;
        lb.starImages = starImgs;
        lb.button = btn;

        PrefabUtility.SaveAsPrefabAssetAndConnect(template, prefabFile, InteractionMode.AutomatedAction);
        Object.DestroyImmediate(template);

        Debug.Log("[Iteration 6] Created LevelButton prefab at " + prefabFile);
    }

    private static void CreateLevelSelectPanel(Transform canvasTransform)
    {
        GameObject root = new GameObject("LevelSelectPanel");
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
        panelRT.sizeDelta = new Vector2(920, 1400);
        Image panelImg = panelBg.AddComponent<Image>();
        panelImg.color = new Color(0.12f, 0.1f, 0.22f, 0.97f);
        panelBg.AddComponent<CanvasGroup>();

        GameObject titleObj = CreateChild(panelBg.transform, "Title");
        RectTransform titleRT = titleObj.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0, 1);
        titleRT.anchorMax = new Vector2(1, 1);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0, -25);
        titleRT.sizeDelta = new Vector2(0, 60);
        TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "SELECT LEVEL";
        titleTMP.fontSize = 46;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = Color.white;

        GameObject starsObj = CreateChild(panelBg.transform, "TotalStars");
        RectTransform starsRT = starsObj.GetComponent<RectTransform>();
        starsRT.anchorMin = new Vector2(0, 1);
        starsRT.anchorMax = new Vector2(1, 1);
        starsRT.pivot = new Vector2(0.5f, 1f);
        starsRT.anchoredPosition = new Vector2(0, -85);
        starsRT.sizeDelta = new Vector2(0, 35);
        TextMeshProUGUI starsTMP = starsObj.AddComponent<TextMeshProUGUI>();
        starsTMP.text = "0 / " + (TOTAL_LEVELS * 3);
        starsTMP.fontSize = 26;
        starsTMP.alignment = TextAlignmentOptions.Center;
        starsTMP.color = new Color(1f, 0.85f, 0.1f, 0.8f);

        GameObject scrollObj = CreateChild(panelBg.transform, "ScrollView");
        RectTransform scrollRT = scrollObj.GetComponent<RectTransform>();
        scrollRT.anchorMin = Vector2.zero;
        scrollRT.anchorMax = Vector2.one;
        scrollRT.offsetMin = new Vector2(15, 110);
        scrollRT.offsetMax = new Vector2(-15, -130);
        Image scrollBgImg = scrollObj.AddComponent<Image>();
        scrollBgImg.color = new Color(0, 0, 0, 0.01f);
        scrollObj.AddComponent<Mask>().showMaskGraphic = false;

        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.1f;
        scrollRect.decelerationRate = 0.135f;
        scrollRect.scrollSensitivity = 40f;

        GameObject contentObj = CreateChild(scrollObj.transform, "Content");
        RectTransform contentRT = contentObj.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1f);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0, 0);

        VerticalLayoutGroup vlg = contentObj.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(20, 20, 10, 10);
        vlg.spacing = BUTTON_SPACING;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = contentObj.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = contentRT;
        scrollRect.viewport = scrollRT;

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFile);
        Debug.Assert(prefab != null, "[Iteration 6] LevelButton prefab not found at " + prefabFile);

        LevelSelectPanel lsp = root.AddComponent<LevelSelectPanel>();

        for (int i = 0; i < TOTAL_LEVELS; i++)
        {
            int lvl = i + 1;
            GameObject btnInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, contentObj.transform);
            btnInstance.name = "LevelButton_" + lvl.ToString("00");

            LevelButton lb = btnInstance.GetComponent<LevelButton>();
            lb.levelNumber = lvl;
            lb.Setup(lvl, lvl == 1, 0);

            lsp.levelButtons.Add(lb);
            EditorUtility.SetDirty(lb);
        }

        GameObject backObj = CreateChild(panelBg.transform, "BackButton");
        RectTransform backRT = backObj.GetComponent<RectTransform>();
        backRT.anchorMin = new Vector2(0, 0);
        backRT.anchorMax = new Vector2(1, 0);
        backRT.pivot = new Vector2(0.5f, 0f);
        backRT.anchoredPosition = new Vector2(0, 15);
        backRT.sizeDelta = new Vector2(-200, 80);
        Image backImg = backObj.AddComponent<Image>();
        backImg.color = new Color(0.35f, 0.3f, 0.55f);
        Button backBtn = backObj.AddComponent<Button>();
        backBtn.targetGraphic = backImg;
        backBtn.transition = Selectable.Transition.None;
        backObj.AddComponent<ButtonFeedback>();

        GameObject backLabel = CreateChild(backObj.transform, "Label");
        RectTransform backLabelRT = backLabel.GetComponent<RectTransform>();
        backLabelRT.anchorMin = Vector2.zero;
        backLabelRT.anchorMax = Vector2.one;
        backLabelRT.sizeDelta = Vector2.zero;
        TextMeshProUGUI backTMP = backLabel.AddComponent<TextMeshProUGUI>();
        backTMP.text = "BACK";
        backTMP.fontSize = 34;
        backTMP.fontStyle = FontStyles.Bold;
        backTMP.alignment = TextAlignmentOptions.Center;
        backTMP.color = Color.white;

        lsp.panelRect = panelBg.GetComponent<RectTransform>();
        lsp.panelCanvasGroup = panelBg.GetComponent<CanvasGroup>();
        lsp.dimOverlay = dimCG;
        lsp.titleText = titleTMP;
        lsp.totalStarsText = starsTMP;
        lsp.scrollRect = scrollRect;
        lsp.backButton = backBtn;
        lsp.totalLevels = TOTAL_LEVELS;
        lsp.uniqueLevels = 5;

        EditorUtility.SetDirty(lsp);

        root.SetActive(false);
        Debug.Log("[Iteration 6] Created ScrollView with " + TOTAL_LEVELS + " level buttons from prefab");
    }

    private static void WireMainMenuUI(GameObject canvasObj)
    {
        MainMenuUI menuUI = canvasObj.GetComponent<MainMenuUI>();
        Debug.Assert(menuUI != null, "[Iteration 6] MainMenuUI not found!");

        Transform lsp = canvasObj.transform.Find("LevelSelectPanel");
        if (lsp != null)
            menuUI.levelSelectPanel = lsp.GetComponent<LevelSelectPanel>();

        EditorUtility.SetDirty(menuUI);
    }

    private static GameObject CreateChild(Transform parent, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }
}
