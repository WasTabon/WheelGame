using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Iteration45_BootstrapSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 4.5 - Setup Bootstrap Scene")]
    public static void ShowWindow()
    {
        GetWindow<Iteration45_BootstrapSetup>("Iteration 4.5 - Bootstrap Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 4.5 - Bootstrap Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("This will create/update the Bootstrap scene with:\n" +
                         "- AddressableLoader (DontDestroyOnLoad)\n" +
                         "- MusicService (DontDestroyOnLoad)\n" +
                         "- Loading UI (progress bar, status, retry button)\n" +
                         "- GameManager + AudioManager + SceneTransition\n" +
                         "  (if not already present from Iteration 1)", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Bootstrap Scene (Iteration 4.5)", GUILayout.Height(40)))
        {
            SetupBootstrapScene();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Bootstrap Scene File", GUILayout.Height(30)))
        {
            CreateBootstrapScene();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Update Build Settings (Bootstrap first)", GUILayout.Height(30)))
        {
            UpdateBuildSettings();
        }
    }

    private static void CreateBootstrapScene()
    {
        string scenePath = "Assets/Scenes/Bootstrap.unity";
        string dir = System.IO.Path.GetDirectoryName(scenePath);
        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        if (System.IO.File.Exists(scenePath))
        {
            Debug.Log("[Iteration 4.5] Bootstrap scene already exists at " + scenePath);
            return;
        }

        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
        EditorSceneManager.SaveScene(newScene, scenePath);
        EditorSceneManager.CloseScene(newScene, true);
        Debug.Log("[Iteration 4.5] Created Bootstrap scene at " + scenePath);
    }

    private static void SetupBootstrapScene()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.backgroundColor = new Color(0.06f, 0.04f, 0.12f);
        }

        SetupManagers();
        SetupEventSystem();
        SetupLoadingUI();
        SetupTransitionCanvas();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 4.5] Bootstrap scene setup complete!");
    }

    private static void SetupManagers()
    {
        if (Object.FindObjectOfType<GameManager>() == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
            Debug.Log("[Iteration 4.5] Created GameManager");
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
            Debug.Log("[Iteration 4.5] Created AudioManager");
        }

        if (Object.FindObjectOfType<AddressableLoader>() == null)
        {
            GameObject alObj = new GameObject("AddressableLoader");
            alObj.AddComponent<AddressableLoader>();
            Debug.Log("[Iteration 4.5] Created AddressableLoader");
        }

        if (Object.FindObjectOfType<MusicService>() == null)
        {
            GameObject msObj = new GameObject("MusicService");
            msObj.AddComponent<MusicService>();
            Debug.Log("[Iteration 4.5] Created MusicService");
        }
    }

    private static void SetupEventSystem()
    {
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private static void SetupLoadingUI()
    {
        Canvas existingCanvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
        {
            if (c.gameObject.name == "BootstrapCanvas")
            {
                existingCanvas = c;
                break;
            }
        }

        GameObject canvasObj;
        if (existingCanvas != null)
        {
            canvasObj = existingCanvas.gameObject;
        }
        else
        {
            canvasObj = new GameObject("BootstrapCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        SetupBackground(canvasObj.transform);

        CreateTMP(canvasObj.transform, "TitleText", "WHEEL GAME",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -400), new Vector2(800, 100),
            64, FontStyles.Bold, Color.white);

        CreateTMP(canvasObj.transform, "SubtitleText", "Loading...",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -500), new Vector2(600, 50),
            28, FontStyles.Normal, new Color(0.7f, 0.65f, 0.9f, 0.7f));

        GameObject barBg = FindOrCreateUIChild(canvasObj.transform, "ProgressBarBg");
        RectTransform barBgRT = barBg.GetComponent<RectTransform>();
        barBgRT.anchorMin = new Vector2(0.5f, 0.5f);
        barBgRT.anchorMax = new Vector2(0.5f, 0.5f);
        barBgRT.pivot = new Vector2(0.5f, 0.5f);
        barBgRT.anchoredPosition = new Vector2(0, -100);
        barBgRT.sizeDelta = new Vector2(700, 30);

        Image barBgImg = GetOrAdd<Image>(barBg);
        barBgImg.color = new Color(0.12f, 0.1f, 0.2f, 1f);

        GameObject barFill = FindOrCreateUIChild(barBg.transform, "Fill");
        RectTransform fillRT = barFill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero;
        fillRT.anchoredPosition = Vector2.zero;

        Image fillImg = GetOrAdd<Image>(barFill);
        fillImg.color = new Color(0.45f, 0.35f, 0.85f, 1f);
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 0f;

        TextMeshProUGUI statusText = CreateTMP(canvasObj.transform, "StatusText", "Initializing...",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -55), new Vector2(700, 40),
            24, FontStyles.Normal, new Color(0.8f, 0.75f, 1f, 0.8f));

        TextMeshProUGUI percentText = CreateTMP(canvasObj.transform, "PercentText", "0%",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -145), new Vector2(200, 35),
            22, FontStyles.Normal, new Color(0.7f, 0.65f, 0.9f, 0.6f));

        GameObject retryContainer = FindOrCreateUIChild(canvasObj.transform, "RetryContainer");
        RectTransform retryCRT = retryContainer.GetComponent<RectTransform>();
        retryCRT.anchorMin = new Vector2(0.5f, 0.5f);
        retryCRT.anchorMax = new Vector2(0.5f, 0.5f);
        retryCRT.pivot = new Vector2(0.5f, 0.5f);
        retryCRT.anchoredPosition = new Vector2(0, -250);
        retryCRT.sizeDelta = new Vector2(350, 90);

        CanvasGroup retryCG = GetOrAdd<CanvasGroup>(retryContainer);
        retryCG.alpha = 0f;
        retryCG.interactable = false;
        retryCG.blocksRaycasts = false;

        GameObject retryBtnObj = FindOrCreateUIChild(retryContainer.transform, "RetryButton");
        RectTransform retryBtnRT = retryBtnObj.GetComponent<RectTransform>();
        retryBtnRT.anchorMin = Vector2.zero;
        retryBtnRT.anchorMax = Vector2.one;
        retryBtnRT.sizeDelta = Vector2.zero;
        retryBtnRT.anchoredPosition = Vector2.zero;

        Image retryImg = GetOrAdd<Image>(retryBtnObj);
        retryImg.color = new Color(0.7f, 0.3f, 0.25f);

        Button retryBtn = GetOrAdd<Button>(retryBtnObj);
        retryBtn.targetGraphic = retryImg;
        retryBtn.transition = Selectable.Transition.None;

        if (retryBtnObj.GetComponent<ButtonFeedback>() == null)
            retryBtnObj.AddComponent<ButtonFeedback>();

        CreateTMP(retryBtnObj.transform, "Label", "TAP TO RETRY",
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero,
            32, FontStyles.Bold, Color.white);

        RectTransform retryLabelRT = retryBtnObj.transform.Find("Label").GetComponent<RectTransform>();
        retryLabelRT.anchorMin = Vector2.zero;
        retryLabelRT.anchorMax = Vector2.one;
        retryLabelRT.sizeDelta = Vector2.zero;
        retryLabelRT.anchoredPosition = Vector2.zero;

        BootstrapUI bootstrapUI = GetOrAdd<BootstrapUI>(canvasObj);
        bootstrapUI.progressBarFill = fillImg;
        bootstrapUI.statusText = statusText;
        bootstrapUI.progressPercentText = percentText;
        bootstrapUI.titleText = canvasObj.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        bootstrapUI.retryButton = retryBtn;
        bootstrapUI.retryCanvasGroup = retryCG;
        bootstrapUI.loader = Object.FindObjectOfType<AddressableLoader>();
        bootstrapUI.musicService = Object.FindObjectOfType<MusicService>();

        EditorUtility.SetDirty(bootstrapUI);
    }

    private static void SetupBackground(Transform parent)
    {
        Transform existing = parent.Find("Background");
        if (existing != null) return;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(parent, false);

        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.06f, 0.04f, 0.12f, 1f);

        RectTransform rt = bg.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        bg.transform.SetAsFirstSibling();
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
    }

    private static void UpdateBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

        string bootstrapPath = "Assets/Scenes/Bootstrap.unity";
        string mainMenuPath = null;
        string gameScenePath = "Assets/Scenes/GameScene.unity";

        string[] allScenes = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in allScenes)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("MainMenu"))
                mainMenuPath = path;
        }

        if (System.IO.File.Exists(bootstrapPath))
            scenes.Add(new EditorBuildSettingsScene(bootstrapPath, true));

        if (mainMenuPath != null)
            scenes.Add(new EditorBuildSettingsScene(mainMenuPath, true));

        if (System.IO.File.Exists(gameScenePath))
            scenes.Add(new EditorBuildSettingsScene(gameScenePath, true));

        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("[Iteration 4.5] Build Settings updated: Bootstrap (0), MainMenu (1), GameScene (2)");
    }

    private static TextMeshProUGUI CreateTMP(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 size, float fontSize, FontStyles style, Color color)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            TextMeshProUGUI existTmp = existing.GetComponent<TextMeshProUGUI>();
            if (existTmp != null) { existTmp.text = text; return existTmp; }
        }

        GameObject obj = FindOrCreateUIChild(parent, name);
        TextMeshProUGUI tmp = GetOrAdd<TextMeshProUGUI>(obj);
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

    private static GameObject FindOrCreateUIChild(Transform parent, string name)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject child = new GameObject(name);
        child.transform.SetParent(parent, false);
        child.AddComponent<RectTransform>();
        return child;
    }

    private static T GetOrAdd<T>(GameObject obj) where T : Component
    {
        T comp = obj.GetComponent<T>();
        if (comp == null) comp = obj.AddComponent<T>();
        return comp;
    }
}
