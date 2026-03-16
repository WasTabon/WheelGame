using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnSceneTransitionStart;
    public event Action OnSceneTransitionEnd;

    [HideInInspector] public int currentLevel = 1;
    [HideInInspector] public int maxUnlockedLevel = 1;

    private int boosterUndo = 5;
    private int boosterSlowmo = 5;
    private int boosterExtraLife = 5;

    public int BoosterUndo => boosterUndo;
    public int BoosterSlowmo => boosterSlowmo;
    public int BoosterExtraLife => boosterExtraLife;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadProgress();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    public void LoadScene(string sceneName, Action onComplete = null)
    {
        OnSceneTransitionStart?.Invoke();
        SceneTransition.Instance.FadeOut(() =>
        {
            SceneManager.LoadScene(sceneName);
            SceneTransition.Instance.FadeIn(() =>
            {
                OnSceneTransitionEnd?.Invoke();
                onComplete?.Invoke();
            });
        });
    }

    public void UseBooster(BoosterType type)
    {
        switch (type)
        {
            case BoosterType.Undo:
                if (boosterUndo > 0) boosterUndo--;
                break;
            case BoosterType.Slowmo:
                if (boosterSlowmo > 0) boosterSlowmo--;
                break;
            case BoosterType.ExtraLife:
                if (boosterExtraLife > 0) boosterExtraLife--;
                break;
        }
        SaveProgress();
    }

    public bool HasBooster(BoosterType type)
    {
        switch (type)
        {
            case BoosterType.Undo: return boosterUndo > 0;
            case BoosterType.Slowmo: return boosterSlowmo > 0;
            case BoosterType.ExtraLife: return boosterExtraLife > 0;
            default: return false;
        }
    }

    public void AddBoosterPack()
    {
        boosterUndo++;
        boosterSlowmo++;
        boosterExtraLife++;
        SaveProgress();
    }

    public void UnlockLevel(int level)
    {
        ProgressManager.UnlockLevel(level);
        maxUnlockedLevel = ProgressManager.GetMaxUnlockedLevel();
    }

    public void SaveLevelStars(int levelNumber, int stars)
    {
        ProgressManager.SaveStars(levelNumber, stars);
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel);
        PlayerPrefs.SetInt("BoosterUndo", boosterUndo);
        PlayerPrefs.SetInt("BoosterSlowmo", boosterSlowmo);
        PlayerPrefs.SetInt("BoosterExtraLife", boosterExtraLife);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        maxUnlockedLevel = ProgressManager.GetMaxUnlockedLevel();
        boosterUndo = PlayerPrefs.GetInt("BoosterUndo", 5);
        boosterSlowmo = PlayerPrefs.GetInt("BoosterSlowmo", 5);
        boosterExtraLife = PlayerPrefs.GetInt("BoosterExtraLife", 5);
    }
}

public enum BoosterType
{
    Undo,
    Slowmo,
    ExtraLife
}
