using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class LevelSelectPanel : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform panelRect;
    public CanvasGroup panelCanvasGroup;
    public CanvasGroup dimOverlay;

    [Header("Content")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI totalStarsText;
    public ScrollRect scrollRect;
    public Button backButton;

    [Header("Level Config")]
    public int totalLevels = 30;
    public int uniqueLevels = 5;

    [HideInInspector]
    public List<LevelButton> levelButtons = new List<LevelButton>();

    private bool isOpen;
    private bool isAnimating;
    private Sequence currentSequence;

    private static int[] levelConfigMap;

    private void OnEnable()
    {
        backButton.onClick.RemoveListener(Close);
        backButton.onClick.AddListener(Close);

        if (dimOverlay != null)
        {
            Button dimBtn = dimOverlay.GetComponent<Button>();
            if (dimBtn != null)
            {
                dimBtn.onClick.RemoveListener(Close);
                dimBtn.onClick.AddListener(Close);
            }
        }

        foreach (LevelButton lb in levelButtons)
        {
            lb.OnLevelSelected -= OnLevelSelected;
            lb.OnLevelSelected += OnLevelSelected;
        }
    }

    private void Start()
    {
        GenerateConfigMap();
    }

    private void GenerateConfigMap()
    {
        if (levelConfigMap != null) return;

        levelConfigMap = new int[totalLevels];
        for (int i = 0; i < totalLevels; i++)
        {
            if (i < uniqueLevels)
            {
                levelConfigMap[i] = i + 1;
            }
            else
            {
                string key = "LevelConfigMap_" + (i + 1);
                int saved = PlayerPrefs.GetInt(key, 0);
                if (saved > 0)
                {
                    levelConfigMap[i] = saved;
                }
                else
                {
                    levelConfigMap[i] = Random.Range(1, uniqueLevels + 1);
                    PlayerPrefs.SetInt(key, levelConfigMap[i]);
                }
            }
        }
        PlayerPrefs.Save();
    }

    public static int GetConfigForLevel(int levelNumber)
    {
        if (levelConfigMap == null || levelNumber < 1 || levelNumber > levelConfigMap.Length)
            return Mathf.Clamp(((levelNumber - 1) % 5) + 1, 1, 5);

        return levelConfigMap[levelNumber - 1];
    }

    public void Open()
    {
        if (isOpen || isAnimating) return;
        isAnimating = true;
        isOpen = true;
        gameObject.SetActive(true);

        RefreshButtons();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelOpen();

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        panelRect.localScale = Vector3.one * 0.8f;

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        if (dimOverlay != null)
        {
            dimOverlay.alpha = 0f;
            dimOverlay.blocksRaycasts = true;
            dimOverlay.interactable = true;
            currentSequence.Join(dimOverlay.DOFade(0.6f, 0.3f));
        }

        currentSequence.Join(panelCanvasGroup.DOFade(1f, 0.3f));
        currentSequence.Join(panelRect.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

        int visibleCount = Mathf.Min(levelButtons.Count, 6);
        for (int i = 0; i < visibleCount; i++)
        {
            levelButtons[i].AnimateAppear(0.25f + i * 0.04f);
        }

        currentSequence.InsertCallback(0.6f, () =>
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
            isAnimating = false;
        });
    }

    public void Close()
    {
        if (!isOpen || isAnimating) return;
        isAnimating = true;
        isOpen = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPanelClose();

        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence.Append(panelCanvasGroup.DOFade(0f, 0.25f));
        currentSequence.Join(panelRect.DOScale(Vector3.one * 0.8f, 0.25f).SetEase(Ease.InBack));

        if (dimOverlay != null)
            currentSequence.Join(dimOverlay.DOFade(0f, 0.25f));

        currentSequence.OnComplete(() =>
        {
            if (dimOverlay != null)
            {
                dimOverlay.blocksRaycasts = false;
                dimOverlay.interactable = false;
            }
            isAnimating = false;
            gameObject.SetActive(false);
        });
    }

    private void RefreshButtons()
    {
        int maxLevel = ProgressManager.GetMaxUnlockedLevel();

        foreach (LevelButton lb in levelButtons)
        {
            bool unlocked = lb.levelNumber <= maxLevel;
            int stars = ProgressManager.GetStars(lb.levelNumber);
            lb.Setup(lb.levelNumber, unlocked, stars);
        }

        if (totalStarsText != null)
            totalStarsText.text = ProgressManager.GetTotalStars() + " / " + (totalLevels * 3);

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    private void OnLevelSelected(int level)
    {
        if (isAnimating) return;

        GameManager.Instance.currentLevel = level;
        GameManager.Instance.LoadScene("GameScene");
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveListener(Close);

        if (dimOverlay != null)
        {
            Button dimBtn = dimOverlay.GetComponent<Button>();
            if (dimBtn != null)
                dimBtn.onClick.RemoveListener(Close);
        }

        foreach (LevelButton lb in levelButtons)
            lb.OnLevelSelected -= OnLevelSelected;
    }
}
