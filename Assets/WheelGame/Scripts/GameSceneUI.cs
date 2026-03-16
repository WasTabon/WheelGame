using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameSceneUI : MonoBehaviour
{
    public static GameSceneUI Instance { get; private set; }

    [Header("Top Panel")]
    public RectTransform topPanel;
    public Image zodiacIcon;
    public TextMeshProUGUI zodiacNameText;
    public TextMeshProUGUI levelText;

    [Header("Lives")]
    public RectTransform livesContainer;
    public Image[] lifeIcons;
    public TextMeshProUGUI livesText;

    [Header("Buttons")]
    public Button backButton;
    public Button pauseButton;

    [Header("Progress")]
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI timerText;

    [Header("Panels")]
    public WinPanel winPanel;
    public LosePanel losePanel;
    public BoosterUI boosterUI;

    private int currentLives = 5;
    private int maxLives = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackClicked);
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void Start()
    {
        AnimateEntrance();
    }

    private void AnimateEntrance()
    {
        if (topPanel != null)
        {
            UIAnimations.SlideInFromTop(topPanel, 200f, 0.5f, 0.1f);
        }
    }

    public void SetZodiacInfo(string name, Sprite icon)
    {
        if (zodiacNameText != null)
            zodiacNameText.text = name;

        if (zodiacIcon != null && icon != null)
            zodiacIcon.sprite = icon;
    }

    public void SetLevel(int level)
    {
        if (levelText != null)
            levelText.text = "Level " + level;
    }

    public void SetProgress(int current, int total)
    {
        if (progressText != null)
            progressText.text = current + " / " + total;
    }

    public void SetLives(int lives)
    {
        currentLives = lives;
        if (livesText != null)
        {
            livesText.text = lives.ToString();
            livesText.transform.DOKill(true);
            livesText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 0.5f);
        }
    }

    public void AnimateLoseLife()
    {
        SetLives(currentLives - 1);

        if (livesContainer != null)
        {
            livesContainer.DOKill(true);
            livesContainer.DOShakePosition(0.4f, 8f, 20, 90, false, true);
        }

        if (livesText != null)
        {
            livesText.DOKill();
            Color origColor = livesText.color;
            livesText.color = new Color(1f, 0.3f, 0.3f);
            livesText.DOColor(origColor, 0.5f);
        }
    }

    public void AnimateGainLife()
    {
        SetLives(currentLives + 1);

        if (livesText != null)
        {
            livesText.DOKill();
            Color origColor = livesText.color;
            livesText.color = new Color(0.3f, 1f, 0.4f);
            livesText.DOColor(origColor, 0.5f);
        }
    }

    public void SetTimer(float time)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void ShowWinPanel(int stars, float time, int levelNumber)
    {
        if (winPanel != null)
            winPanel.Show(stars, time, levelNumber);
    }

    public void ShowLosePanel()
    {
        if (losePanel != null)
            losePanel.Show();
    }

    private void OnBackClicked()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }

    private void OnDisable()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }
}
