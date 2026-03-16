using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class LevelButton : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI levelNumberText;
    public Image backgroundImage;
    public Image lockIcon;
    public Image[] starImages;
    public Button button;

    [Header("State")]
    public int levelNumber;
    public bool isUnlocked;
    public int earnedStars;

    public event Action<int> OnLevelSelected;

    private Color unlockedColor = new Color(0.25f, 0.22f, 0.45f);
    private Color lockedColor = new Color(0.15f, 0.13f, 0.25f);
    private Color starEarnedColor = new Color(1f, 0.85f, 0.1f, 1f);
    private Color starEmptyColor = new Color(0.3f, 0.28f, 0.4f, 0.5f);

    private void OnEnable()
    {
        button.onClick.RemoveListener(OnClicked);
        button.onClick.AddListener(OnClicked);
    }

    public void Setup(int level, bool unlocked, int stars)
    {
        levelNumber = level;
        isUnlocked = unlocked;
        earnedStars = stars;

        levelNumberText.text = level.ToString();
        backgroundImage.color = unlocked ? unlockedColor : lockedColor;
        button.interactable = unlocked;

        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(!unlocked);
        }

        if (unlocked)
        {
            levelNumberText.color = Color.white;
        }
        else
        {
            levelNumberText.color = new Color(0.4f, 0.38f, 0.55f, 0.5f);
        }

        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < stars)
                starImages[i].color = starEarnedColor;
            else
                starImages[i].color = starEmptyColor;
        }
    }

    public void AnimateAppear(float delay)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.35f)
            .SetEase(Ease.OutBack)
            .SetDelay(delay);
    }

    private void OnClicked()
    {
        if (!isUnlocked) return;
        OnLevelSelected?.Invoke(levelNumber);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClicked);
    }
}
