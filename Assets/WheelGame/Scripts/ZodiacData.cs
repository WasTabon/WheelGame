using UnityEngine;

[CreateAssetMenu(fileName = "NewZodiac", menuName = "WheelGame/Zodiac Data")]
public class ZodiacData : ScriptableObject
{
    public string zodiacName;
    public Sprite iconSprite;
    public Sprite contourSprite;
    public Sprite[] fragmentSprites;
    public int fragmentCount;
}
