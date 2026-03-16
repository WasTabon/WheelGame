using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("References")]
    public WheelController wheelController;
    public LevelData[] allLevels;

    [Header("Decoy Source")]
    public ZodiacData[] allZodiacs;

    private LevelData currentLevelData;
    private int currentZodiacIndex;
    private ZodiacData currentZodiac;
    private int fragmentsRemaining;
    private int lives;
    private int maxLives = 5;
    private float levelTimer;
    private bool isPlaying;
    private bool isProcessingClick;
    private int totalZodiacsInLevel;
    private int zodiacsCompleted;

    private bool hasUndoData;
    private WheelSection lastWrongSection;
    private Sprite lastWrongSprite;
    private int lastWrongFragId;
    private bool lastWrongIsCorrect;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        int levelNum = GameManager.Instance.currentLevel;
        int configNum = LevelSelectPanel.GetConfigForLevel(levelNum);
        int configIndex = configNum - 1;

        if (configIndex >= 0 && configIndex < allLevels.Length)
        {
            currentLevelData = allLevels[configIndex];
        }
        else
        {
            currentLevelData = allLevels[Random.Range(0, allLevels.Length)];
        }

        if (MusicService.Instance != null && MusicService.Instance.IsLoaded)
        {
            MusicService.Instance.PlayGameMusic();
        }

        StartLevel();
    }

    private void Update()
    {
        if (isPlaying)
        {
            levelTimer += Time.deltaTime;
            GameSceneUI.Instance.SetTimer(levelTimer);
        }
    }

    private void StartLevel()
    {
        lives = maxLives;
        levelTimer = 0f;
        currentZodiacIndex = 0;
        zodiacsCompleted = 0;
        totalZodiacsInLevel = currentLevelData.zodiacSequence.Length;

        GameSceneUI.Instance.SetLevel(GameManager.Instance.currentLevel);
        GameSceneUI.Instance.SetLives(lives);
        GameSceneUI.Instance.SetProgress(0, totalZodiacsInLevel);

        SubscribeToSections();
        StartNextZodiac();
    }

    private void SubscribeToSections()
    {
        for (int i = 0; i < wheelController.sections.Length; i++)
        {
            WheelSection section = wheelController.sections[i];
            section.OnSectionClicked -= OnSectionClicked;
            section.OnSectionClicked += OnSectionClicked;
        }
    }

    private void UnsubscribeFromSections()
    {
        for (int i = 0; i < wheelController.sections.Length; i++)
        {
            WheelSection section = wheelController.sections[i];
            section.OnSectionClicked -= OnSectionClicked;
        }
    }

    private void StartNextZodiac()
    {
        Debug.Assert(currentZodiacIndex < currentLevelData.zodiacSequence.Length,
            "Zodiac index out of range: " + currentZodiacIndex);

        currentZodiac = currentLevelData.zodiacSequence[currentZodiacIndex];
        fragmentsRemaining = currentZodiac.fragmentCount;

        GameSceneUI.Instance.SetZodiacInfo(currentZodiac.zodiacName, currentZodiac.iconSprite);

        wheelController.wheelCenter.ResetCenter();
        wheelController.wheelCenter.SetContour(currentZodiac.contourSprite);

        DistributeFragments();

        isPlaying = true;
        isProcessingClick = false;
    }

    private void DistributeFragments()
    {
        int sectionCount = wheelController.GetSectionCount();

        for (int i = 0; i < sectionCount; i++)
        {
            wheelController.sections[i].ClearFragment();
        }

        List<FragmentInfo> fragments = new List<FragmentInfo>();

        for (int i = 0; i < currentZodiac.fragmentCount; i++)
        {
            fragments.Add(new FragmentInfo
            {
                sprite = currentZodiac.fragmentSprites[i],
                fragmentId = i,
                isCorrect = true
            });
        }

        int decoyCount = sectionCount - currentZodiac.fragmentCount;
        List<Sprite> decoyPool = BuildDecoyPool();

        for (int i = 0; i < decoyCount; i++)
        {
            Sprite decoySprite = decoyPool[Random.Range(0, decoyPool.Count)];
            fragments.Add(new FragmentInfo
            {
                sprite = decoySprite,
                fragmentId = -1,
                isCorrect = false
            });
        }

        ShuffleList(fragments);

        for (int i = 0; i < sectionCount; i++)
        {
            WheelSection section = wheelController.sections[i];
            FragmentInfo frag = fragments[i];

            DOVirtual.DelayedCall(i * 0.08f, () =>
            {
                section.SetFragment(frag.sprite, frag.fragmentId, frag.isCorrect);
            });
        }
    }

    private List<Sprite> BuildDecoyPool()
    {
        List<Sprite> pool = new List<Sprite>();

        foreach (ZodiacData zd in allZodiacs)
        {
            if (zd == currentZodiac) continue;
            if (zd.fragmentSprites == null) continue;

            foreach (Sprite s in zd.fragmentSprites)
            {
                if (s != null)
                    pool.Add(s);
            }
        }

        if (pool.Count == 0)
        {
            Debug.LogWarning("GameplayManager: No decoy sprites available, using current zodiac fragments as decoys");
            foreach (Sprite s in currentZodiac.fragmentSprites)
            {
                if (s != null) pool.Add(s);
            }
        }

        return pool;
    }

    private void OnSectionClicked(WheelSection section)
    {
        if (!isPlaying || isProcessingClick) return;

        if (section.isCorrectFragment)
        {
            HandleCorrectClick(section);
        }
        else
        {
            HandleWrongClick(section);
        }
    }

    private void HandleCorrectClick(WheelSection section)
    {
        isProcessingClick = true;
        ClearUndoData();

        section.AnimateCorrect();

        Sprite fragSprite = section.fragmentRenderer.sprite;
        Vector3 worldPos = section.fragmentRenderer.transform.position;

        section.fragmentRenderer.sprite = null;
        section.fragmentRenderer.color = Color.clear;
        section.isEmpty = true;
        section.isCorrectFragment = false;
        section.fragmentId = -1;

        GameObject flyObj = new GameObject("FlyingFragment");
        SpriteRenderer flySR = flyObj.AddComponent<SpriteRenderer>();
        flySR.sprite = fragSprite;
        flySR.sortingOrder = 20;
        flyObj.transform.position = worldPos;
        flyObj.transform.localScale = section.fragmentRenderer.transform.lossyScale;

        Vector3 centerPos = wheelController.wheelCenter.transform.position;

        DOTween.Sequence()
            .Append(flyObj.transform.DOMove(centerPos, 0.45f).SetEase(Ease.InOutCubic))
            .Join(flyObj.transform.DOScale(flyObj.transform.localScale * 0.6f, 0.45f).SetEase(Ease.InQuad))
            .Join(flySR.DOFade(0.5f, 0.35f).SetDelay(0.1f))
            .OnComplete(() =>
            {
                Destroy(flyObj);
                wheelController.wheelCenter.OnFragmentCollected(fragSprite);
                fragmentsRemaining--;

                if (fragmentsRemaining <= 0)
                {
                    OnZodiacComplete();
                }
                else
                {
                    isProcessingClick = false;
                }
            });

        if (AudioManager.Instance != null && AudioManager.Instance.sfxButtonClick != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxButtonClick);
    }

    private void HandleWrongClick(WheelSection section)
    {
        isProcessingClick = true;

        hasUndoData = true;
        lastWrongSection = section;

        section.AnimateWrong();
        lives--;
        GameSceneUI.Instance.AnimateLoseLife();
        GameSceneUI.Instance.SetLives(lives);

        wheelController.PunchScale(0.04f, 0.3f);

        if (lives <= 0)
        {
            DOVirtual.DelayedCall(0.6f, OnLevelFailed);
        }
        else
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                isProcessingClick = false;
                if (BoosterUI.FindObjectOfType<BoosterUI>() != null)
                    FindObjectOfType<BoosterUI>().RefreshUI();
            });
        }
    }

    private void OnZodiacComplete()
    {
        isPlaying = false;
        zodiacsCompleted++;

        wheelController.wheelCenter.AnimateComplete();

        GameSceneUI.Instance.SetProgress(zodiacsCompleted, totalZodiacsInLevel);

        if (currentZodiacIndex + 1 < currentLevelData.zodiacSequence.Length)
        {
            DOVirtual.DelayedCall(1.2f, () =>
            {
                ClearAllSections();
                currentZodiacIndex++;
                StartNextZodiac();
            });
        }
        else
        {
            DOVirtual.DelayedCall(1.0f, OnLevelComplete);
        }
    }

    private void ClearAllSections()
    {
        for (int i = 0; i < wheelController.GetSectionCount(); i++)
        {
            wheelController.sections[i].ClearFragment();
        }
    }

    private void OnLevelComplete()
    {
        isPlaying = false;
        UnsubscribeFromSections();

        int stars = CalculateStars();
        int actualLevel = GameManager.Instance.currentLevel;

        GameManager.Instance.UnlockLevel(actualLevel + 1);
        GameManager.Instance.SaveLevelStars(actualLevel, stars);
        GameManager.Instance.SaveProgress();

        GameSceneUI.Instance.ShowWinPanel(stars, levelTimer, actualLevel);
    }

    private void OnLevelFailed()
    {
        isPlaying = false;
        UnsubscribeFromSections();

        GameSceneUI.Instance.ShowLosePanel();
    }

    private int CalculateStars()
    {
        if (levelTimer <= currentLevelData.threeStarTime) return 3;
        if (levelTimer <= currentLevelData.twoStarTime) return 2;
        return 1;
    }

    public void RetryLevel()
    {
        ClearAllSections();
        ClearUndoData();
        if (BoosterManager.Instance != null)
            BoosterManager.Instance.ResetSlowmo();
        wheelController.wheelCenter.ResetCenter();
        StartLevel();
    }

    public void NextLevel()
    {
        if (BoosterManager.Instance != null)
            BoosterManager.Instance.ResetSlowmo();
        GameManager.Instance.currentLevel++;
        GameManager.Instance.LoadScene("GameScene");
    }

    public void GoToMenu()
    {
        if (BoosterManager.Instance != null)
            BoosterManager.Instance.ResetSlowmo();
        GameManager.Instance.LoadScene("MainMenu");
    }

    public bool HasUndoData()
    {
        return hasUndoData && lastWrongSection != null && isPlaying;
    }

    public void ExecuteUndo()
    {
        if (!HasUndoData()) return;

        lives++;
        GameSceneUI.Instance.SetLives(lives);
        GameSceneUI.Instance.AnimateGainLife();

        hasUndoData = false;
        lastWrongSection = null;
    }

    public void AddLife()
    {
        lives++;
        GameSceneUI.Instance.SetLives(lives);
        GameSceneUI.Instance.AnimateGainLife();
    }

    public int GetLives()
    {
        return lives;
    }

    private void ClearUndoData()
    {
        hasUndoData = false;
        lastWrongSection = null;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromSections();
    }

    private struct FragmentInfo
    {
        public Sprite sprite;
        public int fragmentId;
        public bool isCorrect;
    }
}
