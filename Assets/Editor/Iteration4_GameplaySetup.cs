using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class Iteration4_GameplaySetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 4 - Setup Core Gameplay")]
    public static void ShowWindow()
    {
        GetWindow<Iteration4_GameplaySetup>("Iteration 4 - Core Gameplay");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 4 - Core Gameplay Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("This will add to the GameScene:\n" +
                         "- GameplayManager with LevelData/ZodiacData references\n" +
                         "- Timer text in top panel\n" +
                         "- Win Panel (stars, time, next/menu buttons)\n" +
                         "- Lose Panel (retry/menu buttons)\n" +
                         "- Wire everything together", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Core Gameplay (Iteration 4)", GUILayout.Height(40)))
        {
            SetupGameplay();
        }
    }

    private static string levelsPath = "Assets/WheelGame/Data/Levels";
    private static string zodiacsPath = "Assets/WheelGame/Data/Zodiacs";

    private static void SetupGameplay()
    {
        SetupGameplayManager();
        SetupTimerText();
        SetupWinPanel();
        SetupLosePanel();
        WireGameSceneUI();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 4] Core gameplay setup complete!");
    }

    private static void SetupGameplayManager()
    {
        GameplayManager gm = Object.FindObjectOfType<GameplayManager>();
        if (gm == null)
        {
            GameObject gmObj = new GameObject("GameplayManager");
            gm = gmObj.AddComponent<GameplayManager>();
            Debug.Log("[Iteration 4] Created GameplayManager");
        }

        WheelController wc = Object.FindObjectOfType<WheelController>();
        Debug.Assert(wc != null, "[Iteration 4] WheelController not found! Run Iteration 2 setup first.");
        gm.wheelController = wc;

        List<LevelData> levels = new List<LevelData>();
        string[] levelGuids = AssetDatabase.FindAssets("t:LevelData", new[] { levelsPath });
        foreach (string guid in levelGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelData ld = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (ld != null) levels.Add(ld);
        }
        levels.Sort((a, b) => a.levelNumber.CompareTo(b.levelNumber));
        gm.allLevels = levels.ToArray();

        List<ZodiacData> zodiacs = new List<ZodiacData>();
        string[] zodiacGuids = AssetDatabase.FindAssets("t:ZodiacData", new[] { zodiacsPath });
        foreach (string guid in zodiacGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ZodiacData zd = AssetDatabase.LoadAssetAtPath<ZodiacData>(path);
            if (zd != null) zodiacs.Add(zd);
        }
        gm.allZodiacs = zodiacs.ToArray();

        EditorUtility.SetDirty(gm);
        Debug.Log("[Iteration 4] GameplayManager wired with " + levels.Count + " levels and " + zodiacs.Count + " zodiacs");
    }

    private static void SetupTimerText()
    {
        GameObject canvasObj = GameObject.Find("GameCanvas");
        Debug.Assert(canvasObj != null, "[Iteration 4] GameCanvas not found! Run Iteration 2 setup first.");

        Transform topPanel = canvasObj.transform.Find("TopPanel");
        Debug.Assert(topPanel != null, "[Iteration 4] TopPanel not found!");

        Transform existingTimer = topPanel.Find("TimerText");
        if (existingTimer != null) return;

        GameObject timerObj = new GameObject("TimerText");
        timerObj.transform.SetParent(topPanel, false);

        RectTransform rt = timerObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -5);
        rt.sizeDelta = new Vector2(200, 40);

        TextMeshProUGUI tmp = timerObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "00:00";
        tmp.fontSize = 26;
        tmp.fontStyle = FontStyles.Normal;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.7f, 0.65f, 0.9f, 0.6f);

        Debug.Log("[Iteration 4] Created TimerText");
    }

    private static void SetupWinPanel()
    {
        GameObject canvasObj = GameObject.Find("GameCanvas");

        Transform existing = canvasObj.transform.Find("WinPanel");
        if (existing != null)
        {
            Debug.Log("[Iteration 4] WinPanel already exists, updating references...");
            WireWinPanel(existing.gameObject);
            return;
        }

        GameObject winRoot = new GameObject("WinPanel");
        winRoot.transform.SetParent(canvasObj.transform, false);

        RectTransform rootRT = winRoot.AddComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.sizeDelta = Vector2.zero;
        rootRT.anchoredPosition = Vector2.zero;

        GameObject dimObj = CreateDimOverlay(winRoot.transform, "WinDim");

        GameObject panelBg = CreatePanelBackground(winRoot.transform, "WinPanelBg",
            new Vector2(850, 750), new Color(0.12f, 0.1f, 0.25f, 0.97f));

        CreateTMPChild(panelBg.transform, "LevelCompleteText", "Level Complete!",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -40), new Vector2(700, 60), 44, FontStyles.Bold, Color.white);

        CreateTMPChild(panelBg.transform, "TitleText", "VICTORY",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -100), new Vector2(400, 55), 36, FontStyles.Bold,
            new Color(0.4f, 1f, 0.5f, 1f));

        Image[] stars = new Image[3];
        float starStartX = -110f;
        for (int i = 0; i < 3; i++)
        {
            GameObject starObj = new GameObject("Star_" + i);
            starObj.transform.SetParent(panelBg.transform, false);

            RectTransform starRT = starObj.AddComponent<RectTransform>();
            starRT.anchorMin = new Vector2(0.5f, 0.5f);
            starRT.anchorMax = new Vector2(0.5f, 0.5f);
            starRT.pivot = new Vector2(0.5f, 0.5f);
            starRT.anchoredPosition = new Vector2(starStartX + i * 110f, 60f);
            starRT.sizeDelta = new Vector2(90, 90);

            Image starImg = starObj.AddComponent<Image>();
            starImg.color = new Color(0.3f, 0.3f, 0.35f, 0.5f);
            stars[i] = starImg;
        }

        CreateTMPChild(panelBg.transform, "TimeText", "00:00",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -30), new Vector2(300, 50), 38, FontStyles.Normal,
            new Color(0.8f, 0.75f, 1f, 0.9f));

        CreateButton(panelBg.transform, "NextButton", "NEXT LEVEL",
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0, 140), new Vector2(400, 90),
            new Color(0.2f, 0.7f, 0.3f), 38);

        CreateButton(panelBg.transform, "MenuButton", "MENU",
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0, 40), new Vector2(400, 80),
            new Color(0.4f, 0.35f, 0.6f), 34);

        WireWinPanel(winRoot);
        winRoot.SetActive(false);

        Debug.Log("[Iteration 4] Created WinPanel");
    }

    private static void WireWinPanel(GameObject winRoot)
    {
        WinPanel wp = winRoot.GetComponent<WinPanel>();
        if (wp == null) wp = winRoot.AddComponent<WinPanel>();

        Transform panelBg = winRoot.transform.Find("WinPanelBg");
        Debug.Assert(panelBg != null, "WinPanelBg not found");

        wp.panelRect = panelBg.GetComponent<RectTransform>();
        wp.panelCanvasGroup = GetOrAddComponent<CanvasGroup>(panelBg.gameObject);

        Transform dim = winRoot.transform.Find("WinDim");
        wp.dimOverlay = dim != null ? dim.GetComponent<CanvasGroup>() : null;

        wp.titleText = panelBg.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        wp.timeText = panelBg.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        wp.levelCompleteText = panelBg.Find("LevelCompleteText")?.GetComponent<TextMeshProUGUI>();

        List<Image> starsList = new List<Image>();
        for (int i = 0; i < 3; i++)
        {
            Transform starT = panelBg.Find("Star_" + i);
            if (starT != null) starsList.Add(starT.GetComponent<Image>());
        }
        wp.starImages = starsList.ToArray();

        Transform nextBtn = panelBg.Find("NextButton");
        wp.nextButton = nextBtn != null ? nextBtn.GetComponent<Button>() : null;

        Transform menuBtn = panelBg.Find("MenuButton");
        wp.menuButton = menuBtn != null ? menuBtn.GetComponent<Button>() : null;

        EditorUtility.SetDirty(wp);
    }

    private static void SetupLosePanel()
    {
        GameObject canvasObj = GameObject.Find("GameCanvas");

        Transform existing = canvasObj.transform.Find("LosePanel");
        if (existing != null)
        {
            Debug.Log("[Iteration 4] LosePanel already exists, updating references...");
            WireLosePanel(existing.gameObject);
            return;
        }

        GameObject loseRoot = new GameObject("LosePanel");
        loseRoot.transform.SetParent(canvasObj.transform, false);

        RectTransform rootRT = loseRoot.AddComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.sizeDelta = Vector2.zero;
        rootRT.anchoredPosition = Vector2.zero;

        CreateDimOverlay(loseRoot.transform, "LoseDim");

        GameObject panelBg = CreatePanelBackground(loseRoot.transform, "LosePanelBg",
            new Vector2(850, 550), new Color(0.2f, 0.08f, 0.1f, 0.97f));

        CreateTMPChild(panelBg.transform, "TitleText", "GAME OVER",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -50), new Vector2(600, 65), 52, FontStyles.Bold,
            new Color(1f, 0.35f, 0.3f, 1f));

        CreateTMPChild(panelBg.transform, "SubtitleText", "You ran out of lives!",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -130), new Vector2(600, 45), 30, FontStyles.Normal,
            new Color(0.8f, 0.7f, 0.75f, 0.8f));

        CreateButton(panelBg.transform, "RetryButton", "RETRY",
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0, 160), new Vector2(400, 90),
            new Color(0.8f, 0.35f, 0.25f), 38);

        CreateButton(panelBg.transform, "MenuButton", "MENU",
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0, 55), new Vector2(400, 80),
            new Color(0.4f, 0.35f, 0.6f), 34);

        WireLosePanel(loseRoot);
        loseRoot.SetActive(false);

        Debug.Log("[Iteration 4] Created LosePanel");
    }

    private static void WireLosePanel(GameObject loseRoot)
    {
        LosePanel lp = loseRoot.GetComponent<LosePanel>();
        if (lp == null) lp = loseRoot.AddComponent<LosePanel>();

        Transform panelBg = loseRoot.transform.Find("LosePanelBg");
        Debug.Assert(panelBg != null, "LosePanelBg not found");

        lp.panelRect = panelBg.GetComponent<RectTransform>();
        lp.panelCanvasGroup = GetOrAddComponent<CanvasGroup>(panelBg.gameObject);

        Transform dim = loseRoot.transform.Find("LoseDim");
        lp.dimOverlay = dim != null ? dim.GetComponent<CanvasGroup>() : null;

        lp.titleText = panelBg.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        lp.subtitleText = panelBg.Find("SubtitleText")?.GetComponent<TextMeshProUGUI>();

        Transform retryBtn = panelBg.Find("RetryButton");
        lp.retryButton = retryBtn != null ? retryBtn.GetComponent<Button>() : null;

        Transform menuBtn = panelBg.Find("MenuButton");
        lp.menuButton = menuBtn != null ? menuBtn.GetComponent<Button>() : null;

        EditorUtility.SetDirty(lp);
    }

    private static void WireGameSceneUI()
    {
        GameSceneUI gsUI = Object.FindObjectOfType<GameSceneUI>();
        Debug.Assert(gsUI != null, "[Iteration 4] GameSceneUI not found!");

        GameObject canvasObj = gsUI.gameObject;

        Transform topPanel = canvasObj.transform.Find("TopPanel");
        Transform timerT = topPanel.Find("TimerText");
        if (timerT != null)
            gsUI.timerText = timerT.GetComponent<TextMeshProUGUI>();

        Transform winT = canvasObj.transform.Find("WinPanel");
        if (winT != null)
            gsUI.winPanel = winT.GetComponent<WinPanel>();

        Transform loseT = canvasObj.transform.Find("LosePanel");
        if (loseT != null)
            gsUI.losePanel = loseT.GetComponent<LosePanel>();

        EditorUtility.SetDirty(gsUI);
    }

    private static GameObject CreateDimOverlay(Transform parent, string name)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject dimObj = new GameObject(name);
        dimObj.transform.SetParent(parent, false);

        Image dimImage = dimObj.AddComponent<Image>();
        dimImage.color = new Color(0, 0, 0, 1f);

        CanvasGroup cg = dimObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;

        RectTransform rt = dimObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        return dimObj;
    }

    private static GameObject CreatePanelBackground(Transform parent, string name, Vector2 size, Color bgColor)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject panelBg = new GameObject(name);
        panelBg.transform.SetParent(parent, false);

        Image panelImage = panelBg.AddComponent<Image>();
        panelImage.color = bgColor;

        panelBg.AddComponent<CanvasGroup>();

        RectTransform rt = panelBg.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;

        return panelBg;
    }

    private static void CreateTMPChild(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 size, float fontSize, FontStyles style, Color color)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            TextMeshProUGUI existTmp = existing.GetComponent<TextMeshProUGUI>();
            if (existTmp != null) existTmp.text = text;
            return;
        }

        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

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
    }

    private static Button CreateButton(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 size, Color bgColor, float fontSize)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            TextMeshProUGUI tmp = existing.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = label;
            return existing.GetComponent<Button>();
        }

        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = bgColor;

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(btnObj.transform, false);

        TextMeshProUGUI tmp2 = textObj.AddComponent<TextMeshProUGUI>();
        tmp2.text = label;
        tmp2.fontSize = fontSize;
        tmp2.fontStyle = FontStyles.Bold;
        tmp2.alignment = TextAlignmentOptions.Center;
        tmp2.color = Color.white;

        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        btn.transition = Selectable.Transition.None;

        btnObj.AddComponent<ButtonFeedback>();

        return btn;
    }

    private static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T comp = obj.GetComponent<T>();
        if (comp == null) comp = obj.AddComponent<T>();
        return comp;
    }
}
