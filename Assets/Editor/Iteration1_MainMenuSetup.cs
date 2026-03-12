using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration1_MainMenuSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 1 - Setup Main Menu")]
    public static void ShowWindow()
    {
        GetWindow<Iteration1_MainMenuSetup>("Iteration 1 - Main Menu Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 1 - Main Menu Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("This will create/update the MainMenu scene with:\n" +
                         "- GameManager (DontDestroyOnLoad)\n" +
                         "- AudioManager (DontDestroyOnLoad)\n" +
                         "- SceneTransition (DontDestroyOnLoad)\n" +
                         "- Main Menu UI (Title, Play, Settings, Shop)\n" +
                         "- Settings Panel with sliders\n" +
                         "- EventSystem", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Main Menu Scene (Iteration 1)", GUILayout.Height(40)))
        {
            SetupMainMenu();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create GameScene (empty placeholder)", GUILayout.Height(30)))
        {
            CreateGameScene();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add Scenes to Build Settings", GUILayout.Height(30)))
        {
            AddScenesToBuildSettings();
        }
    }

    private static void SetupMainMenu()
    {
        SetupManagers();
        SetupEventSystem();
        SetupMainCanvas();
        SetupTransitionCanvas();

        EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 1] Main Menu setup complete!");
    }

    private static void SetupManagers()
    {
        if (Object.FindObjectOfType<GameManager>() == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
            Debug.Log("[Iteration 1] Created GameManager");
        }

        AudioManager audioManager = Object.FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            GameObject amObj = new GameObject("AudioManager");
            audioManager = amObj.AddComponent<AudioManager>();

            AudioSource musicSource = amObj.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.volume = 1f;

            AudioSource sfxSource = amObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;

            audioManager.musicSource = musicSource;
            audioManager.sfxSource = sfxSource;

            Debug.Log("[Iteration 1] Created AudioManager with AudioSources");
        }
    }

    private static void SetupEventSystem()
    {
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("[Iteration 1] Created EventSystem");
        }
    }

    private static void SetupMainCanvas()
    {
        Canvas existingCanvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
        {
            if (c.gameObject.name == "MainMenuCanvas")
            {
                existingCanvas = c;
                break;
            }
        }

        GameObject canvasObj;
        if (existingCanvas != null)
        {
            canvasObj = existingCanvas.gameObject;
            Debug.Log("[Iteration 1] Found existing MainMenuCanvas, updating...");
        }
        else
        {
            canvasObj = new GameObject("MainMenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("[Iteration 1] Created MainMenuCanvas");
        }

        SetupBackground(canvasObj.transform);
        RectTransform titleRect = SetupTitle(canvasObj.transform);
        CanvasGroup titleCG = titleRect.GetComponent<CanvasGroup>();

        Button playBtn = SetupMenuButton(canvasObj.transform, "PlayButton", "PLAY", new Vector2(0, 50), new Color(0.2f, 0.7f, 0.3f));
        Button settingsBtn = SetupMenuButton(canvasObj.transform, "SettingsButton", "SETTINGS", new Vector2(0, -80), new Color(0.3f, 0.5f, 0.8f));
        Button shopBtn = SetupMenuButton(canvasObj.transform, "ShopButton", "SHOP", new Vector2(0, -210), new Color(0.8f, 0.5f, 0.2f));

        SettingsPanel settingsPanel = SetupSettingsPanel(canvasObj.transform);

        MainMenuUI menuUI = canvasObj.GetComponent<MainMenuUI>();
        if (menuUI == null)
            menuUI = canvasObj.AddComponent<MainMenuUI>();

        menuUI.titleTransform = titleRect;
        menuUI.titleCanvasGroup = titleCG;
        menuUI.playButton = playBtn;
        menuUI.settingsButton = settingsBtn;
        menuUI.shopButton = shopBtn;
        menuUI.playButtonRect = playBtn.GetComponent<RectTransform>();
        menuUI.settingsButtonRect = settingsBtn.GetComponent<RectTransform>();
        menuUI.shopButtonRect = shopBtn.GetComponent<RectTransform>();
        menuUI.settingsPanel = settingsPanel;
    }

    private static void SetupBackground(Transform parent)
    {
        Transform existing = parent.Find("Background");
        if (existing != null) return;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(parent, false);

        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.12f, 0.1f, 0.22f, 1f);

        RectTransform rt = bg.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        bg.transform.SetAsFirstSibling();
    }

    private static RectTransform SetupTitle(Transform parent)
    {
        Transform existing = parent.Find("TitleContainer");
        GameObject titleContainer;

        if (existing != null)
        {
            titleContainer = existing.gameObject;
        }
        else
        {
            titleContainer = new GameObject("TitleContainer");
            titleContainer.transform.SetParent(parent, false);

            RectTransform containerRT = titleContainer.AddComponent<RectTransform>();
            containerRT.anchorMin = new Vector2(0.5f, 1f);
            containerRT.anchorMax = new Vector2(0.5f, 1f);
            containerRT.pivot = new Vector2(0.5f, 1f);
            containerRT.anchoredPosition = new Vector2(0, -180);
            containerRT.sizeDelta = new Vector2(800, 200);

            titleContainer.AddComponent<CanvasGroup>();
        }

        Transform titleTextTransform = titleContainer.transform.Find("TitleText");
        if (titleTextTransform == null)
        {
            GameObject titleText = new GameObject("TitleText");
            titleText.transform.SetParent(titleContainer.transform, false);

            TextMeshProUGUI tmp = titleText.AddComponent<TextMeshProUGUI>();
            tmp.text = "WHEEL GAME";
            tmp.fontSize = 72;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            RectTransform textRT = titleText.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;
        }

        Transform subtitleTransform = titleContainer.transform.Find("SubtitleText");
        if (subtitleTransform == null)
        {
            GameObject subtitle = new GameObject("SubtitleText");
            subtitle.transform.SetParent(titleContainer.transform, false);

            TextMeshProUGUI subTmp = subtitle.AddComponent<TextMeshProUGUI>();
            subTmp.text = "Zodiac Puzzle";
            subTmp.fontSize = 32;
            subTmp.alignment = TextAlignmentOptions.Center;
            subTmp.color = new Color(0.7f, 0.65f, 0.9f, 0.8f);

            RectTransform subRT = subtitle.GetComponent<RectTransform>();
            subRT.anchorMin = new Vector2(0, 0);
            subRT.anchorMax = new Vector2(1, 0);
            subRT.pivot = new Vector2(0.5f, 1f);
            subRT.anchoredPosition = new Vector2(0, -10);
            subRT.sizeDelta = new Vector2(0, 50);
        }

        return titleContainer.GetComponent<RectTransform>();
    }

    private static Button SetupMenuButton(Transform parent, string name, string label, Vector2 position, Color bgColor)
    {
        Transform existing = parent.Find(name);
        GameObject btnObj;

        if (existing != null)
        {
            btnObj = existing.gameObject;
            TextMeshProUGUI existingTmp = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (existingTmp != null) existingTmp.text = label;
        }
        else
        {
            btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = bgColor;

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = new Vector2(500, 100);

            GameObject textObj = new GameObject("Label");
            textObj.transform.SetParent(btnObj.transform, false);

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 42;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;
        }

        Button btn = btnObj.GetComponent<Button>();
        if (btn == null)
            btn = btnObj.AddComponent<Button>();

        btn.transition = Selectable.Transition.None;

        if (btnObj.GetComponent<ButtonFeedback>() == null)
            btnObj.AddComponent<ButtonFeedback>();

        return btn;
    }

    private static SettingsPanel SetupSettingsPanel(Transform parent)
    {
        Transform existing = parent.Find("SettingsPanel");
        GameObject panelRoot;

        if (existing != null)
        {
            panelRoot = existing.gameObject;
            SettingsPanel existingSP = panelRoot.GetComponent<SettingsPanel>();
            if (existingSP != null) return existingSP;
        }
        else
        {
            panelRoot = new GameObject("SettingsPanel");
            panelRoot.transform.SetParent(parent, false);

            RectTransform rootRT = panelRoot.AddComponent<RectTransform>();
            rootRT.anchorMin = Vector2.zero;
            rootRT.anchorMax = Vector2.one;
            rootRT.sizeDelta = Vector2.zero;
            rootRT.anchoredPosition = Vector2.zero;
        }

        CanvasGroup dimCG = null;
        Transform dimExisting = panelRoot.transform.Find("DimOverlay");
        if (dimExisting == null)
        {
            GameObject dimObj = new GameObject("DimOverlay");
            dimObj.transform.SetParent(panelRoot.transform, false);

            Image dimImage = dimObj.AddComponent<Image>();
            dimImage.color = new Color(0, 0, 0, 1f);

            dimCG = dimObj.AddComponent<CanvasGroup>();
            dimCG.alpha = 0f;
            dimCG.blocksRaycasts = false;

            dimObj.AddComponent<Button>();

            RectTransform dimRT = dimObj.GetComponent<RectTransform>();
            dimRT.anchorMin = Vector2.zero;
            dimRT.anchorMax = Vector2.one;
            dimRT.sizeDelta = Vector2.zero;
        }
        else
        {
            dimCG = dimExisting.GetComponent<CanvasGroup>();
        }

        Transform panelBgExisting = panelRoot.transform.Find("PanelBg");
        GameObject panelBg;
        if (panelBgExisting != null)
        {
            panelBg = panelBgExisting.gameObject;
        }
        else
        {
            panelBg = new GameObject("PanelBg");
            panelBg.transform.SetParent(panelRoot.transform, false);

            Image panelImage = panelBg.AddComponent<Image>();
            panelImage.color = new Color(0.18f, 0.16f, 0.3f, 1f);

            RectTransform panelRT = panelBg.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.pivot = new Vector2(0.5f, 0.5f);
            panelRT.anchoredPosition = Vector2.zero;
            panelRT.sizeDelta = new Vector2(900, 700);

            panelBg.AddComponent<CanvasGroup>();
        }

        TextMeshProUGUI settingsTitle = CreateOrUpdateTMPChild(panelBg.transform, "SettingsTitle", "SETTINGS",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -40), new Vector2(400, 70), 48, FontStyles.Bold, Color.white);

        CreateOrUpdateTMPChild(panelBg.transform, "MusicLabel", "Music",
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(60, -150), new Vector2(200, 50), 34, FontStyles.Normal, new Color(0.8f, 0.8f, 0.9f));

        Slider musicSlider = CreateOrUpdateSlider(panelBg.transform, "MusicSlider",
            new Vector2(0.5f, 1f), new Vector2(0, -220), new Vector2(700, 50));

        CreateOrUpdateTMPChild(panelBg.transform, "SfxLabel", "Sound Effects",
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(60, -310), new Vector2(300, 50), 34, FontStyles.Normal, new Color(0.8f, 0.8f, 0.9f));

        Slider sfxSlider = CreateOrUpdateSlider(panelBg.transform, "SfxSlider",
            new Vector2(0.5f, 1f), new Vector2(0, -380), new Vector2(700, 50));

        Button closeBtn = CreateOrUpdateButton(panelBg.transform, "CloseButton", "CLOSE",
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0, 80), new Vector2(300, 80),
            new Color(0.7f, 0.25f, 0.25f), 36);

        SettingsPanel sp = panelRoot.GetComponent<SettingsPanel>();
        if (sp == null)
            sp = panelRoot.AddComponent<SettingsPanel>();

        sp.panelRect = panelBg.GetComponent<RectTransform>();
        sp.panelCanvasGroup = panelBg.GetComponent<CanvasGroup>();
        sp.dimOverlay = dimCG;
        sp.musicSlider = musicSlider;
        sp.sfxSlider = sfxSlider;
        sp.closeButton = closeBtn;
        sp.titleLabel = settingsTitle;
        sp.musicLabel = panelBg.transform.Find("MusicLabel")?.GetComponent<TextMeshProUGUI>();
        sp.sfxLabel = panelBg.transform.Find("SfxLabel")?.GetComponent<TextMeshProUGUI>();

        return sp;
    }

    private static TextMeshProUGUI CreateOrUpdateTMPChild(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 size, float fontSize, FontStyles style, Color color)
    {
        Transform existing = parent.Find(name);
        GameObject obj;
        TextMeshProUGUI tmp;

        if (existing != null)
        {
            obj = existing.gameObject;
            tmp = obj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
        }
        else
        {
            obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            tmp = obj.AddComponent<TextMeshProUGUI>();
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

        return tmp;
    }

    private static Slider CreateOrUpdateSlider(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
            return existing.GetComponent<Slider>();

        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);

        RectTransform sliderRT = sliderObj.AddComponent<RectTransform>();
        sliderRT.anchorMin = anchor;
        sliderRT.anchorMax = anchor;
        sliderRT.pivot = new Vector2(0.5f, 0.5f);
        sliderRT.anchoredPosition = position;
        sliderRT.sizeDelta = size;

        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f);
        RectTransform bgRT = bgObj.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = new Vector2(0, 0.25f);
        fillAreaRT.anchorMax = new Vector2(1, 0.75f);
        fillAreaRT.offsetMin = new Vector2(10, 0);
        fillAreaRT.offsetMax = new Vector2(-10, 0);

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillArea.transform, false);
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = new Color(0.4f, 0.35f, 0.75f, 1f);
        RectTransform fillRT = fillObj.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero;

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRT = handleArea.AddComponent<RectTransform>();
        handleAreaRT.anchorMin = Vector2.zero;
        handleAreaRT.anchorMax = Vector2.one;
        handleAreaRT.offsetMin = new Vector2(15, 0);
        handleAreaRT.offsetMax = new Vector2(-15, 0);

        GameObject handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(handleArea.transform, false);
        Image handleImage = handleObj.AddComponent<Image>();
        handleImage.color = new Color(0.6f, 0.55f, 0.95f, 1f);
        RectTransform handleRT = handleObj.GetComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(40, 40);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRT;
        slider.handleRect = handleRT;
        slider.targetGraphic = handleImage;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.direction = Slider.Direction.LeftToRight;

        return slider;
    }

    private static Button CreateOrUpdateButton(Transform parent, string name, string label,
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

    private static void SetupTransitionCanvas()
    {
        if (Object.FindObjectOfType<SceneTransition>() != null) return;

        GameObject transObj = new GameObject("SceneTransition");

        Canvas canvas = transObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = transObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        transObj.AddComponent<GraphicRaycaster>();

        GameObject fadeObj = new GameObject("FadeOverlay");
        fadeObj.transform.SetParent(transObj.transform, false);

        Image fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = Color.black;

        RectTransform fadeRT = fadeObj.GetComponent<RectTransform>();
        fadeRT.anchorMin = Vector2.zero;
        fadeRT.anchorMax = Vector2.one;
        fadeRT.sizeDelta = Vector2.zero;

        CanvasGroup fadeCG = fadeObj.AddComponent<CanvasGroup>();
        fadeCG.alpha = 0f;
        fadeCG.blocksRaycasts = false;
        fadeCG.interactable = false;

        SceneTransition st = transObj.AddComponent<SceneTransition>();
        SerializedObject so = new SerializedObject(st);
        so.FindProperty("fadeCanvasGroup").objectReferenceValue = fadeCG;
        so.ApplyModifiedProperties();

        Debug.Log("[Iteration 1] Created SceneTransition overlay");
    }

    private static void CreateGameScene()
    {
        string scenePath = "Assets/Scenes/GameScene.unity";
        string dir = System.IO.Path.GetDirectoryName(scenePath);
        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        if (System.IO.File.Exists(scenePath))
        {
            Debug.Log("[Iteration 1] GameScene already exists at " + scenePath);
            return;
        }

        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
        EditorSceneManager.SaveScene(newScene, scenePath);
        EditorSceneManager.CloseScene(newScene, true);
        Debug.Log("[Iteration 1] Created empty GameScene at " + scenePath);
    }

    private static void AddScenesToBuildSettings()
    {
        string mainMenuPath = null;
        string gameScenePath = "Assets/Scenes/GameScene.unity";

        string[] allScenes = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in allScenes)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("MainMenu"))
            {
                mainMenuPath = path;
                break;
            }
        }

        if (mainMenuPath == null)
        {
            string currentPath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
            if (!string.IsNullOrEmpty(currentPath))
                mainMenuPath = currentPath;
        }

        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

        if (mainMenuPath != null)
            scenes.Add(new EditorBuildSettingsScene(mainMenuPath, true));

        if (System.IO.File.Exists(gameScenePath))
            scenes.Add(new EditorBuildSettingsScene(gameScenePath, true));

        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("[Iteration 1] Updated Build Settings with " + scenes.Count + " scenes");
    }
}
