using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class Iteration2_GameSceneSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 2 - Setup Game Scene")]
    public static void ShowWindow()
    {
        GetWindow<Iteration2_GameSceneSetup>("Iteration 2 - Game Scene Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 2 - Game Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("This will:\n" +
                         "- Generate wheel sprites (pie slices 512px, center, glow, ring)\n" +
                         "- Create wheel object with 12 sections on the scene\n" +
                         "- Create center circle\n" +
                         "- Create game UI (top panel with lives, zodiac icon, back button)\n" +
                         "- Add adaptive scaling for different screen sizes\n" +
                         "- Wire all references", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("1. Generate Sprites (Iteration 2)", GUILayout.Height(35)))
        {
            GenerateAllSprites();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("2. Setup Game Scene (Iteration 2)", GUILayout.Height(40)))
        {
            SetupGameScene();
        }
    }

    private static string spritePath = "Assets/WheelGame/GeneratedSprites";

    private const int SECTION_SIZE = 512;
    private const float PPU = 100f;
    private const float OUTER_RADIUS_PX = 250f;
    private const float INNER_RADIUS_PX = 112f;
    private const float OUTER_RADIUS_UNITS = OUTER_RADIUS_PX / PPU;
    private const float INNER_RADIUS_UNITS = INNER_RADIUS_PX / PPU;
    private const int NUM_SECTIONS = 12;
    private const float SLICE_ANGLE = 360f / NUM_SECTIONS;

    private static void GenerateAllSprites()
    {
        if (!Directory.Exists(spritePath))
            Directory.CreateDirectory(spritePath);

        GeneratePieSliceSprite(SECTION_SIZE, NUM_SECTIONS, "WheelSection", new Color(0.25f, 0.22f, 0.4f));
        GeneratePieSliceSprite(SECTION_SIZE, NUM_SECTIONS, "WheelSectionHighlight", new Color(1f, 1f, 1f, 0.3f));
        GenerateCircleSprite(256, "WheelCenter", new Color(0.15f, 0.12f, 0.25f));
        GenerateCircleSprite(300, "WheelCenterGlow", new Color(0.5f, 0.4f, 0.9f, 0.3f), true);
        GenerateRingSprite(SECTION_SIZE, "WheelRing", new Color(0.4f, 0.35f, 0.7f), OUTER_RADIUS_PX, 10);
        GenerateCircleSprite(64, "FragmentPlaceholder", new Color(0.6f, 0.5f, 0.9f, 0.8f));
        GenerateCircleSprite(128, "ContourPlaceholder", new Color(0.8f, 0.7f, 1f, 0.5f));

        AssetDatabase.Refresh();
        SetSpriteImportSettings();
        Debug.Log("[Iteration 2] All sprites generated at " + spritePath);
    }

    private static void GeneratePieSliceSprite(int size, int numSlices, string name, Color color)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;

        float cx = size / 2f;
        float cy = size / 2f;
        float outerR = OUTER_RADIUS_PX;
        float innerR = INNER_RADIUS_PX;
        float sliceAngle = 360f / numSlices;
        float gapAngle = 1.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist > outerR || dist < innerR) continue;

                float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;

                if (angle > gapAngle && angle < sliceAngle - gapAngle)
                {
                    float edgeFade = 1f;
                    float outerEdge = outerR - dist;
                    float innerEdge = dist - innerR;
                    if (outerEdge < 4f) edgeFade = outerEdge / 4f;
                    if (innerEdge < 4f) edgeFade = Mathf.Min(edgeFade, innerEdge / 4f);

                    float angleFade = 1f;
                    float angleFromEdge = Mathf.Min(angle - gapAngle, sliceAngle - gapAngle - angle);
                    if (angleFromEdge < 2.5f) angleFade = angleFromEdge / 2.5f;

                    float alpha = color.a * edgeFade * angleFade;
                    pixels[y * size + x] = new Color(color.r, color.g, color.b, alpha);
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        SaveTexture(tex, name);
        Object.DestroyImmediate(tex);
    }

    private static void GenerateCircleSprite(int size, string name, Color color, bool softEdge = false)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        float cx = size / 2f;
        float cy = size / 2f;
        float radius = size / 2f - 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= radius)
                {
                    float alpha = color.a;
                    float edge = radius - dist;
                    if (edge < 3f) alpha *= edge / 3f;
                    if (softEdge)
                    {
                        float t = dist / radius;
                        alpha *= 1f - (t * t);
                    }
                    pixels[y * size + x] = new Color(color.r, color.g, color.b, alpha);
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        SaveTexture(tex, name);
        Object.DestroyImmediate(tex);
    }

    private static void GenerateRingSprite(int size, string name, Color color, float ringRadius, int thickness)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        float cx = size / 2f;
        float cy = size / 2f;
        float outerR = ringRadius + thickness / 2f;
        float innerR = ringRadius - thickness / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= outerR && dist >= innerR)
                {
                    float alpha = color.a;
                    float outerEdge = outerR - dist;
                    float innerEdge = dist - innerR;
                    if (outerEdge < 2.5f) alpha *= outerEdge / 2.5f;
                    if (innerEdge < 2.5f) alpha *= innerEdge / 2.5f;
                    pixels[y * size + x] = new Color(color.r, color.g, color.b, alpha);
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        SaveTexture(tex, name);
        Object.DestroyImmediate(tex);
    }

    private static void SaveTexture(Texture2D tex, string name)
    {
        byte[] bytes = tex.EncodeToPNG();
        string path = spritePath + "/" + name + ".png";
        File.WriteAllBytes(path, bytes);
    }

    private static void SetSpriteImportSettings()
    {
        string[] files = Directory.GetFiles(spritePath, "*.png");
        foreach (string file in files)
        {
            string assetPath = file.Replace("\\", "/");
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = PPU;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
        }
    }

    private static void SetupGameScene()
    {
        SetupCamera();
        SetupWheel();
        SetupGameUI();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 2] Game Scene setup complete!");
    }

    private static void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 6.5f;
            cam.backgroundColor = new Color(0.08f, 0.06f, 0.16f);
            cam.transform.position = new Vector3(0, 0, -10);
        }
    }

    private static void SetupWheel()
    {
        Sprite sectionSprite = LoadSprite("WheelSection");
        Sprite highlightSprite = LoadSprite("WheelSectionHighlight");
        Sprite centerSprite = LoadSprite("WheelCenter");
        Sprite glowSprite = LoadSprite("WheelCenterGlow");
        Sprite ringSprite = LoadSprite("WheelRing");
        Sprite contourPlaceholder = LoadSprite("ContourPlaceholder");

        GameObject wheelRoot = GameObject.Find("WheelRoot");
        if (wheelRoot == null)
        {
            wheelRoot = new GameObject("WheelRoot");
            wheelRoot.transform.position = new Vector3(0, -0.8f, 0);
        }

        WheelAdaptive adaptive = GetOrAddComponent<WheelAdaptive>(wheelRoot);
        adaptive.screenWidthPercent = 0.88f;
        adaptive.wheelWorldRadius = OUTER_RADIUS_UNITS;

        GameObject wheelPivot = FindOrCreateChild(wheelRoot, "WheelPivot");
        wheelPivot.transform.localPosition = Vector3.zero;
        wheelPivot.transform.localScale = Vector3.one;

        WheelSection[] sections = new WheelSection[NUM_SECTIONS];

        for (int i = 0; i < NUM_SECTIONS; i++)
        {
            string sectionName = "Section_" + i.ToString("00");
            GameObject sectionObj = FindOrCreateChild(wheelPivot, sectionName);

            float angle = i * SLICE_ANGLE;
            sectionObj.transform.localRotation = Quaternion.Euler(0, 0, angle);
            sectionObj.transform.localPosition = Vector3.zero;
            sectionObj.transform.localScale = Vector3.one;

            SpriteRenderer bgRenderer = GetOrAddComponent<SpriteRenderer>(sectionObj);
            bgRenderer.sprite = sectionSprite;
            bgRenderer.sortingOrder = 1;
            bgRenderer.color = GetSectionColor(i);

            GameObject highlightObj = FindOrCreateChild(sectionObj, "Highlight");
            highlightObj.transform.localPosition = Vector3.zero;
            highlightObj.transform.localRotation = Quaternion.identity;
            highlightObj.transform.localScale = Vector3.one;
            SpriteRenderer hlRenderer = GetOrAddComponent<SpriteRenderer>(highlightObj);
            hlRenderer.sprite = highlightSprite;
            hlRenderer.sortingOrder = 2;
            hlRenderer.color = new Color(1, 1, 1, 0);

            float midRadius = (INNER_RADIUS_UNITS + OUTER_RADIUS_UNITS) / 2f;
            float midAngleDeg = SLICE_ANGLE / 2f;
            float midAngleRad = midAngleDeg * Mathf.Deg2Rad;
            Vector3 fragLocalPos = new Vector3(
                Mathf.Cos(midAngleRad) * midRadius,
                Mathf.Sin(midAngleRad) * midRadius,
                0f
            );

            GameObject fragObj = FindOrCreateChild(sectionObj, "Fragment");
            fragObj.transform.localPosition = fragLocalPos;
            fragObj.transform.localRotation = Quaternion.Euler(0, 0, -angle);
            fragObj.transform.localScale = Vector3.one * 0.6f;

            SpriteRenderer fragRenderer = GetOrAddComponent<SpriteRenderer>(fragObj);
            fragRenderer.sortingOrder = 3;
            fragRenderer.sprite = null;

            PolygonCollider2D collider = GetOrAddComponent<PolygonCollider2D>(sectionObj);
            SetPieCollider(collider, OUTER_RADIUS_UNITS, INNER_RADIUS_UNITS, 0f, SLICE_ANGLE);

            WheelSection ws = GetOrAddComponent<WheelSection>(sectionObj);
            ws.sectionIndex = i;
            ws.backgroundRenderer = bgRenderer;
            ws.highlightRenderer = hlRenderer;
            ws.fragmentRenderer = fragRenderer;
            ws.isEmpty = true;

            sections[i] = ws;
        }

        GameObject centerObj = FindOrCreateChild(wheelPivot, "WheelCenter");
        centerObj.transform.localPosition = Vector3.zero;

        float centerTargetRadius = INNER_RADIUS_UNITS * 1.05f;
        float centerSpriteRadius = 256f / 2f / PPU;
        float centerScale = centerTargetRadius / centerSpriteRadius;

        centerObj.transform.localScale = Vector3.one * centerScale;

        SpriteRenderer centerBgRenderer = GetOrAddComponent<SpriteRenderer>(centerObj);
        centerBgRenderer.sprite = centerSprite;
        centerBgRenderer.sortingOrder = 5;

        GameObject centerGlow = FindOrCreateChild(centerObj, "Glow");
        centerGlow.transform.localPosition = Vector3.zero;
        centerGlow.transform.localScale = Vector3.one * 1.3f;
        SpriteRenderer glowRenderer = GetOrAddComponent<SpriteRenderer>(centerGlow);
        glowRenderer.sprite = glowSprite;
        glowRenderer.sortingOrder = 4;

        GameObject contourObj = FindOrCreateChild(centerObj, "Contour");
        contourObj.transform.localPosition = Vector3.zero;
        contourObj.transform.localScale = Vector3.one * 0.65f;
        SpriteRenderer contourRenderer = GetOrAddComponent<SpriteRenderer>(contourObj);
        contourRenderer.sprite = contourPlaceholder;
        contourRenderer.sortingOrder = 6;
        contourRenderer.color = new Color(1, 1, 1, 0.5f);

        GameObject ringObj = FindOrCreateChild(wheelPivot, "Ring");
        ringObj.transform.localPosition = Vector3.zero;
        ringObj.transform.localScale = Vector3.one;
        SpriteRenderer ringRenderer = GetOrAddComponent<SpriteRenderer>(ringObj);
        ringRenderer.sprite = ringSprite;
        ringRenderer.sortingOrder = 7;

        WheelCenter wc = GetOrAddComponent<WheelCenter>(centerObj);
        wc.backgroundRenderer = centerBgRenderer;
        wc.contourRenderer = contourRenderer;
        wc.glowRenderer = glowRenderer;

        WheelController controller = GetOrAddComponent<WheelController>(wheelRoot);
        controller.wheelTransform = wheelPivot.transform;
        controller.sections = sections;
        controller.wheelCenter = wc;
        controller.baseRotationSpeed = 25f;

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(adaptive);
    }

    private static void SetPieCollider(PolygonCollider2D collider, float outerRadius, float innerRadius, float startAngle, float sliceAngle)
    {
        int segments = 16;
        Vector2[] points = new Vector2[segments * 2 + 2];

        float gapDeg = 1.5f;
        float startRad = (startAngle + gapDeg) * Mathf.Deg2Rad;
        float endRad = (startAngle + sliceAngle - gapDeg) * Mathf.Deg2Rad;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float a = Mathf.Lerp(startRad, endRad, t);
            points[i] = new Vector2(Mathf.Cos(a) * outerRadius, Mathf.Sin(a) * outerRadius);
        }

        for (int i = segments; i >= 0; i--)
        {
            float t = (float)i / segments;
            float a = Mathf.Lerp(startRad, endRad, t);
            points[segments + 1 + (segments - i)] = new Vector2(Mathf.Cos(a) * innerRadius, Mathf.Sin(a) * innerRadius);
        }

        collider.SetPath(0, points);
    }

    private static Color GetSectionColor(int index)
    {
        float hue = (index * 30f) / 360f;
        return Color.HSVToRGB(hue, 0.35f, 0.3f);
    }

    private static void SetupGameUI()
    {
        Canvas existingCanvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
        {
            if (c.gameObject.name == "GameCanvas")
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
            canvasObj = new GameObject("GameCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        GameObject topPanel = FindOrCreateUIChild(canvasObj.transform, "TopPanel");
        RectTransform topRT = topPanel.GetComponent<RectTransform>();
        topRT.anchorMin = new Vector2(0, 1);
        topRT.anchorMax = new Vector2(1, 1);
        topRT.pivot = new Vector2(0.5f, 1);
        topRT.anchoredPosition = Vector2.zero;
        topRT.sizeDelta = new Vector2(0, 200);

        Image topBg = GetOrAddComponent<Image>(topPanel);
        topBg.color = new Color(0.1f, 0.08f, 0.2f, 0.9f);

        GameObject backBtnObj = FindOrCreateUIChild(topPanel.transform, "BackButton");
        RectTransform backRT = backBtnObj.GetComponent<RectTransform>();
        backRT.anchorMin = new Vector2(0, 0.5f);
        backRT.anchorMax = new Vector2(0, 0.5f);
        backRT.pivot = new Vector2(0, 0.5f);
        backRT.anchoredPosition = new Vector2(30, 0);
        backRT.sizeDelta = new Vector2(100, 100);

        Image backImg = GetOrAddComponent<Image>(backBtnObj);
        backImg.color = new Color(0.3f, 0.25f, 0.5f);

        Button backBtn = GetOrAddComponent<Button>(backBtnObj);
        backBtn.transition = Selectable.Transition.None;
        if (backBtnObj.GetComponent<ButtonFeedback>() == null)
            backBtnObj.AddComponent<ButtonFeedback>();

        TextMeshProUGUI backLabel = CreateOrGetTMP(backBtnObj.transform, "Label", "<", 48, FontStyles.Bold);
        RectTransform backLabelRT = backLabel.GetComponent<RectTransform>();
        backLabelRT.anchorMin = Vector2.zero;
        backLabelRT.anchorMax = Vector2.one;
        backLabelRT.sizeDelta = Vector2.zero;
        backLabelRT.anchoredPosition = Vector2.zero;

        GameObject zodiacContainer = FindOrCreateUIChild(topPanel.transform, "ZodiacContainer");
        RectTransform zodiacRT = zodiacContainer.GetComponent<RectTransform>();
        zodiacRT.anchorMin = new Vector2(0.5f, 0.5f);
        zodiacRT.anchorMax = new Vector2(0.5f, 0.5f);
        zodiacRT.pivot = new Vector2(0.5f, 0.5f);
        zodiacRT.anchoredPosition = Vector2.zero;
        zodiacRT.sizeDelta = new Vector2(400, 180);

        GameObject iconObj = FindOrCreateUIChild(zodiacContainer.transform, "ZodiacIcon");
        RectTransform iconRT = iconObj.GetComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0.5f, 1f);
        iconRT.anchorMax = new Vector2(0.5f, 1f);
        iconRT.pivot = new Vector2(0.5f, 1f);
        iconRT.anchoredPosition = new Vector2(0, -10);
        iconRT.sizeDelta = new Vector2(90, 90);

        Image zodiacImg = GetOrAddComponent<Image>(iconObj);
        zodiacImg.color = new Color(0.6f, 0.5f, 0.9f, 0.7f);

        TextMeshProUGUI zodiacName = CreateOrGetTMP(zodiacContainer.transform, "ZodiacName", "Aries", 32, FontStyles.Bold);
        RectTransform nameRT = zodiacName.GetComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0.5f, 0f);
        nameRT.anchorMax = new Vector2(0.5f, 0f);
        nameRT.pivot = new Vector2(0.5f, 0f);
        nameRT.anchoredPosition = new Vector2(0, 10);
        nameRT.sizeDelta = new Vector2(300, 45);

        TextMeshProUGUI levelLabel = CreateOrGetTMP(zodiacContainer.transform, "LevelText", "Level 1", 24, FontStyles.Normal);
        levelLabel.color = new Color(0.7f, 0.65f, 0.9f, 0.8f);
        RectTransform levelRT = levelLabel.GetComponent<RectTransform>();
        levelRT.anchorMin = new Vector2(0.5f, 0f);
        levelRT.anchorMax = new Vector2(0.5f, 0f);
        levelRT.pivot = new Vector2(0.5f, 1f);
        levelRT.anchoredPosition = new Vector2(0, 10);
        levelRT.sizeDelta = new Vector2(200, 30);

        GameObject livesObj = FindOrCreateUIChild(topPanel.transform, "LivesContainer");
        RectTransform livesRT = livesObj.GetComponent<RectTransform>();
        livesRT.anchorMin = new Vector2(1, 0.5f);
        livesRT.anchorMax = new Vector2(1, 0.5f);
        livesRT.pivot = new Vector2(1, 0.5f);
        livesRT.anchoredPosition = new Vector2(-30, 0);
        livesRT.sizeDelta = new Vector2(150, 80);

        GameObject heartIcon = FindOrCreateUIChild(livesObj.transform, "HeartIcon");
        RectTransform heartRT = heartIcon.GetComponent<RectTransform>();
        heartRT.anchorMin = new Vector2(0, 0.5f);
        heartRT.anchorMax = new Vector2(0, 0.5f);
        heartRT.pivot = new Vector2(0, 0.5f);
        heartRT.anchoredPosition = new Vector2(10, 0);
        heartRT.sizeDelta = new Vector2(50, 50);

        Image heartImg = GetOrAddComponent<Image>(heartIcon);
        heartImg.color = new Color(0.9f, 0.25f, 0.3f);

        TextMeshProUGUI livesText = CreateOrGetTMP(livesObj.transform, "LivesText", "5", 42, FontStyles.Bold);
        RectTransform livesTextRT = livesText.GetComponent<RectTransform>();
        livesTextRT.anchorMin = new Vector2(0, 0.5f);
        livesTextRT.anchorMax = new Vector2(1, 0.5f);
        livesTextRT.pivot = new Vector2(0.5f, 0.5f);
        livesTextRT.anchoredPosition = new Vector2(20, 0);
        livesTextRT.sizeDelta = new Vector2(0, 60);

        TextMeshProUGUI progressText = CreateOrGetTMP(topPanel.transform, "ProgressText", "0 / 4", 28, FontStyles.Normal);
        progressText.color = new Color(0.8f, 0.75f, 1f, 0.7f);
        RectTransform progRT = progressText.GetComponent<RectTransform>();
        progRT.anchorMin = new Vector2(0.5f, 0);
        progRT.anchorMax = new Vector2(0.5f, 0);
        progRT.pivot = new Vector2(0.5f, 0);
        progRT.anchoredPosition = new Vector2(0, 5);
        progRT.sizeDelta = new Vector2(200, 35);

        GameSceneUI gsUI = GetOrAddComponent<GameSceneUI>(canvasObj);
        gsUI.topPanel = topRT;
        gsUI.zodiacIcon = zodiacImg;
        gsUI.zodiacNameText = zodiacName;
        gsUI.levelText = levelLabel;
        gsUI.livesContainer = livesRT;
        gsUI.livesText = livesText;
        gsUI.backButton = backBtn;
        gsUI.progressText = progressText;

        EditorUtility.SetDirty(gsUI);
    }

    private static Sprite LoadSprite(string name)
    {
        string path = spritePath + "/" + name + ".png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
            Debug.LogWarning("[Iteration 2] Sprite not found: " + path + " — run 'Generate Sprites' first!");
        return sprite;
    }

    private static GameObject FindOrCreateChild(GameObject parent, string name)
    {
        Transform existing = parent.transform.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject child = new GameObject(name);
        child.transform.SetParent(parent.transform, false);
        return child;
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

    private static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T comp = obj.GetComponent<T>();
        if (comp == null) comp = obj.AddComponent<T>();
        return comp;
    }

    private static TextMeshProUGUI CreateOrGetTMP(Transform parent, string name, string text, float size, FontStyles style)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            TextMeshProUGUI existTMP = existing.GetComponent<TextMeshProUGUI>();
            if (existTMP != null)
            {
                existTMP.text = text;
                return existTMP;
            }
        }

        GameObject obj = FindOrCreateUIChild(parent, name);
        TextMeshProUGUI tmp = GetOrAddComponent<TextMeshProUGUI>(obj);
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return tmp;
    }
}
