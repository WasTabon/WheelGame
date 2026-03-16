using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class Iteration10_TutorialSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 10 - Setup Tutorial (GameScene)")]
    public static void ShowWindow()
    {
        GetWindow<Iteration10_TutorialSetup>("Iteration 10 - Tutorial");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 10 - Tutorial Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Run on GameScene.\n" +
                         "Creates TutorialPanel with step-by-step text.\n" +
                         "Shows on first play, saved in PlayerPrefs.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Tutorial (Iteration 10)", GUILayout.Height(40)))
        {
            Setup();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reset Tutorial (show again next play)", GUILayout.Height(30)))
        {
            TutorialPanel.ResetTutorial();
            Debug.Log("[Iteration 10] Tutorial reset — will show on next play");
        }
    }

    private static void Setup()
    {
        GameObject canvasObj = GameObject.Find("GameCanvas");
        Debug.Assert(canvasObj != null, "[Iteration 10] GameCanvas not found!");

        Transform existing = canvasObj.transform.Find("TutorialPanel");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
            Debug.Log("[Iteration 10] Removed old TutorialPanel");
        }

        CreateTutorialPanel(canvasObj.transform);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 10] Tutorial setup complete!");
    }

    private static void CreateTutorialPanel(Transform canvasTransform)
    {
        GameObject root = new GameObject("TutorialPanel");
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

        GameObject panelBg = CreateChild(root.transform, "PanelBg");
        RectTransform panelRT = panelBg.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(820, 600);
        Image panelImg = panelBg.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.08f, 0.2f, 0.97f);
        CanvasGroup panelCG = panelBg.AddComponent<CanvasGroup>();

        TextMeshProUGUI titleTMP = CreateTMP(panelBg.transform, "Title", "Welcome!",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            new Vector2(0, -35), new Vector2(0, 55),
            42, FontStyles.Bold, new Color(0.6f, 0.5f, 1f));

        TextMeshProUGUI bodyTMP = CreateTMP(panelBg.transform, "Body", "",
            new Vector2(0, 0.3f), new Vector2(1, 0.85f), new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero,
            28, FontStyles.Normal, new Color(0.85f, 0.82f, 0.95f, 0.9f));
        bodyTMP.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI stepTMP = CreateTMP(panelBg.transform, "StepIndicator", "1 / 6",
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0f),
            new Vector2(0, 110), new Vector2(0, 30),
            22, FontStyles.Normal, new Color(0.6f, 0.55f, 0.8f, 0.6f));

        GameObject nextBtnObj = CreateChild(panelBg.transform, "NextButton");
        RectTransform nextRT = nextBtnObj.GetComponent<RectTransform>();
        nextRT.anchorMin = new Vector2(0.5f, 0f);
        nextRT.anchorMax = new Vector2(0.5f, 0f);
        nextRT.pivot = new Vector2(0.5f, 0f);
        nextRT.anchoredPosition = new Vector2(0, 25);
        nextRT.sizeDelta = new Vector2(350, 80);
        Image nextImg = nextBtnObj.AddComponent<Image>();
        nextImg.color = new Color(0.35f, 0.3f, 0.7f);
        Button nextBtn = nextBtnObj.AddComponent<Button>();
        nextBtn.targetGraphic = nextImg;
        nextBtn.transition = Selectable.Transition.None;
        nextBtnObj.AddComponent<ButtonFeedback>();

        TextMeshProUGUI nextTextTMP = CreateTMP(nextBtnObj.transform, "Label", "NEXT",
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero,
            34, FontStyles.Bold, Color.white);

        TutorialPanel tp = root.AddComponent<TutorialPanel>();
        tp.panelRect = panelRT;
        tp.panelCanvasGroup = panelCG;
        tp.dimOverlay = dimCG;
        tp.titleText = titleTMP;
        tp.bodyText = bodyTMP;
        tp.stepIndicator = stepTMP;
        tp.nextButton = nextBtn;
        tp.nextButtonText = nextTextTMP;

        EditorUtility.SetDirty(tp);

        root.SetActive(false);
    }

    private static TextMeshProUGUI CreateTMP(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 size,
        float fontSize, FontStyles style, Color color)
    {
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
