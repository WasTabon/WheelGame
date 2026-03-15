using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "WheelGame/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public ZodiacData[] zodiacSequence;
    public float threeStarTime;
    public float twoStarTime;
}
