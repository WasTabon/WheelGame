using UnityEngine;

public static class ProgressManager
{
    private const string STARS_KEY_PREFIX = "LevelStars_";
    private const string MAX_LEVEL_KEY = "MaxUnlockedLevel";

    public static int GetStars(int levelNumber)
    {
        return PlayerPrefs.GetInt(STARS_KEY_PREFIX + levelNumber, 0);
    }

    public static void SaveStars(int levelNumber, int stars)
    {
        int existing = GetStars(levelNumber);
        if (stars > existing)
        {
            PlayerPrefs.SetInt(STARS_KEY_PREFIX + levelNumber, stars);
            PlayerPrefs.Save();
        }
    }

    public static int GetMaxUnlockedLevel()
    {
        return PlayerPrefs.GetInt(MAX_LEVEL_KEY, 1);
    }

    public static void UnlockLevel(int level)
    {
        int current = GetMaxUnlockedLevel();
        if (level > current)
        {
            PlayerPrefs.SetInt(MAX_LEVEL_KEY, level);
            PlayerPrefs.Save();
        }
    }

    public static int GetTotalStars()
    {
        int total = 0;
        for (int i = 1; i <= 100; i++)
        {
            int s = GetStars(i);
            if (s == 0 && i > GetMaxUnlockedLevel()) break;
            total += s;
        }
        return total;
    }

    public static void ResetAll()
    {
        for (int i = 1; i <= 100; i++)
        {
            PlayerPrefs.DeleteKey(STARS_KEY_PREFIX + i);
        }
        PlayerPrefs.SetInt(MAX_LEVEL_KEY, 1);
        PlayerPrefs.Save();
    }
}
