using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class Iteration3_ZodiacSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 3 - Generate Zodiacs & Levels")]
    public static void ShowWindow()
    {
        GetWindow<Iteration3_ZodiacSetup>("Iteration 3 - Zodiac Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 3 - Zodiac & Level Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("This will generate:\n" +
                         "- 12 zodiac contour sprites (geometric placeholders)\n" +
                         "- Fragments for each zodiac (4-5 pieces)\n" +
                         "- 12 icon placeholders\n" +
                         "- 12 ZodiacData ScriptableObjects\n" +
                         "- 5 LevelData ScriptableObjects", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Generate All Zodiac Assets (Iteration 3)", GUILayout.Height(40)))
        {
            GenerateAll();
        }
    }

    private static string spritePath = "Assets/WheelGame/GeneratedSprites/Zodiacs";
    private static string dataPath = "Assets/WheelGame/Data";
    private const int CONTOUR_SIZE = 256;
    private const int ICON_SIZE = 128;
    private const float LINE_THICKNESS = 4f;

    private struct ZodiacDef
    {
        public string name;
        public int fragmentCount;
    }

    private static ZodiacDef[] zodiacs = new ZodiacDef[]
    {
        new ZodiacDef { name = "Aries", fragmentCount = 4 },
        new ZodiacDef { name = "Taurus", fragmentCount = 5 },
        new ZodiacDef { name = "Gemini", fragmentCount = 4 },
        new ZodiacDef { name = "Cancer", fragmentCount = 5 },
        new ZodiacDef { name = "Leo", fragmentCount = 4 },
        new ZodiacDef { name = "Virgo", fragmentCount = 5 },
        new ZodiacDef { name = "Libra", fragmentCount = 4 },
        new ZodiacDef { name = "Scorpio", fragmentCount = 5 },
        new ZodiacDef { name = "Sagittarius", fragmentCount = 4 },
        new ZodiacDef { name = "Capricorn", fragmentCount = 5 },
        new ZodiacDef { name = "Aquarius", fragmentCount = 4 },
        new ZodiacDef { name = "Pisces", fragmentCount = 5 },
    };

    private static void GenerateAll()
    {
        EnsureDirectories();
        GenerateAllSprites();
        AssetDatabase.Refresh();
        SetAllSpriteImportSettings();
        AssetDatabase.Refresh();
        CreateScriptableObjects();
        Debug.Log("[Iteration 3] All zodiac assets generated!");
    }

    private static void EnsureDirectories()
    {
        if (!Directory.Exists(spritePath)) Directory.CreateDirectory(spritePath);
        if (!Directory.Exists(dataPath + "/Zodiacs")) Directory.CreateDirectory(dataPath + "/Zodiacs");
        if (!Directory.Exists(dataPath + "/Levels")) Directory.CreateDirectory(dataPath + "/Levels");
    }

    private static void GenerateAllSprites()
    {
        for (int i = 0; i < zodiacs.Length; i++)
        {
            Texture2D contour = GenerateContourTexture(i);
            SaveTexture(contour, zodiacs[i].name + "_Contour");

            GenerateFragments(contour, i);

            Texture2D icon = GenerateIconTexture(i);
            SaveTexture(icon, zodiacs[i].name + "_Icon");

            Object.DestroyImmediate(contour);
            Object.DestroyImmediate(icon);
        }
        Debug.Log("[Iteration 3] All sprites generated at " + spritePath);
    }

    private static Texture2D GenerateContourTexture(int zodiacIndex)
    {
        Texture2D tex = new Texture2D(CONTOUR_SIZE, CONTOUR_SIZE, TextureFormat.RGBA32, false);
        ClearTexture(tex, Color.clear);

        Color lineColor = new Color(0.85f, 0.78f, 1f, 0.9f);
        float cx = CONTOUR_SIZE / 2f;
        float cy = CONTOUR_SIZE / 2f;

        switch (zodiacIndex)
        {
            case 0: DrawAries(tex, cx, cy, lineColor); break;
            case 1: DrawTaurus(tex, cx, cy, lineColor); break;
            case 2: DrawGemini(tex, cx, cy, lineColor); break;
            case 3: DrawCancer(tex, cx, cy, lineColor); break;
            case 4: DrawLeo(tex, cx, cy, lineColor); break;
            case 5: DrawVirgo(tex, cx, cy, lineColor); break;
            case 6: DrawLibra(tex, cx, cy, lineColor); break;
            case 7: DrawScorpio(tex, cx, cy, lineColor); break;
            case 8: DrawSagittarius(tex, cx, cy, lineColor); break;
            case 9: DrawCapricorn(tex, cx, cy, lineColor); break;
            case 10: DrawAquarius(tex, cx, cy, lineColor); break;
            case 11: DrawPisces(tex, cx, cy, lineColor); break;
        }

        tex.Apply();
        return tex;
    }

    private static void DrawAries(Texture2D tex, float cx, float cy, Color c)
    {
        DrawArc(tex, cx - 35, cy + 20, 35, 0, 180, c);
        DrawArc(tex, cx + 35, cy + 20, 35, 0, 180, c);
        DrawLine(tex, cx, cy + 55, cx, cy - 60, c);
    }

    private static void DrawTaurus(Texture2D tex, float cx, float cy, Color c)
    {
        DrawArc(tex, cx, cy - 10, 45, 0, 360, c);
        DrawArc(tex, cx - 30, cy + 55, 25, 20, 160, c);
        DrawArc(tex, cx + 30, cy + 55, 25, 20, 160, c);
    }

    private static void DrawGemini(Texture2D tex, float cx, float cy, Color c)
    {
        DrawLine(tex, cx - 25, cy + 60, cx - 25, cy - 60, c);
        DrawLine(tex, cx + 25, cy + 60, cx + 25, cy - 60, c);
        DrawArc(tex, cx, cy + 60, 40, 200, 340, c);
        DrawArc(tex, cx, cy - 60, 40, 20, 160, c);
    }

    private static void DrawCancer(Texture2D tex, float cx, float cy, Color c)
    {
        DrawArc(tex, cx - 25, cy + 15, 25, 0, 360, c);
        DrawArc(tex, cx + 25, cy - 15, 25, 0, 360, c);
        DrawArc(tex, cx, cy + 15, 55, 190, 360, c);
        DrawArc(tex, cx, cy - 15, 55, 10, 180, c);
    }

    private static void DrawLeo(Texture2D tex, float cx, float cy, Color c)
    {
        DrawArc(tex, cx - 20, cy - 25, 30, 0, 360, c);
        DrawArc(tex, cx + 15, cy + 20, 20, 270, 450, c);
        DrawLine(tex, cx + 15, cy + 40, cx + 40, cy + 55, c);
        DrawLine(tex, cx + 40, cy + 55, cx + 25, cy + 70, c);
    }

    private static void DrawVirgo(Texture2D tex, float cx, float cy, Color c)
    {
        DrawLine(tex, cx - 50, cy - 50, cx - 50, cy + 30, c);
        DrawArc(tex, cx - 35, cy + 30, 15, 0, 180, c);
        DrawLine(tex, cx - 20, cy + 30, cx - 20, cy - 20, c);
        DrawArc(tex, cx - 5, cy - 20, 15, 0, 180, c);
        DrawLine(tex, cx + 10, cy - 20, cx + 10, cy + 30, c);
        DrawArc(tex, cx + 30, cy + 10, 20, 270, 430, c);
    }

    private static void DrawLibra(Texture2D tex, float cx, float cy, Color c)
    {
        DrawLine(tex, cx - 55, cy - 40, cx + 55, cy - 40, c);
        DrawLine(tex, cx - 55, cy - 55, cx + 55, cy - 55, c);
        DrawArc(tex, cx, cy + 5, 45, 200, 340, c);
        DrawLine(tex, cx, cy + 5, cx, cy - 40, c);
    }

    private static void DrawScorpio(Texture2D tex, float cx, float cy, Color c)
    {
        DrawLine(tex, cx - 50, cy - 50, cx - 50, cy + 30, c);
        DrawArc(tex, cx - 35, cy + 30, 15, 0, 180, c);
        DrawLine(tex, cx - 20, cy + 30, cx - 20, cy - 20, c);
        DrawArc(tex, cx - 5, cy - 20, 15, 0, 180, c);
        DrawLine(tex, cx + 10, cy - 20, cx + 10, cy + 30, c);
        DrawLine(tex, cx + 10, cy + 30, cx + 40, cy + 55, c);
        DrawLine(tex, cx + 40, cy + 55, cx + 30, cy + 45, c);
        DrawLine(tex, cx + 40, cy + 55, cx + 40, cy + 42, c);
    }

    private static void DrawSagittarius(Texture2D tex, float cx, float cy, Color c)
    {
        DrawLine(tex, cx - 45, cy - 45, cx + 45, cy + 45, c);
        DrawLine(tex, cx + 45, cy + 45, cx + 20, cy + 45, c);
        DrawLine(tex, cx + 45, cy + 45, cx + 45, cy + 20, c);
        DrawLine(tex, cx - 15, cy + 15, cx + 15, cy - 15, c);
    }

    private static void DrawCapricorn(Texture2D tex, float cx, float cy, Color c)
    {
        DrawLine(tex, cx - 40, cy - 40, cx - 40, cy + 30, c);
        DrawArc(tex, cx - 20, cy + 30, 20, 0, 180, c);
        DrawLine(tex, cx, cy + 30, cx, cy - 10, c);
        DrawArc(tex, cx + 25, cy - 30, 25, 180, 360, c);
        DrawArc(tex, cx + 35, cy - 50, 15, 0, 270, c);
    }

    private static void DrawAquarius(Texture2D tex, float cx, float cy, Color c)
    {
        for (int row = 0; row < 2; row++)
        {
            float y = cy + 15 - row * 35;
            for (int i = 0; i < 3; i++)
            {
                float x1 = cx - 55 + i * 37;
                float x2 = x1 + 18;
                float x3 = x2 + 19;
                DrawLine(tex, x1, y, x2, y + 15, c);
                DrawLine(tex, x2, y + 15, x3, y, c);
            }
        }
    }

    private static void DrawPisces(Texture2D tex, float cx, float cy, Color c)
    {
        DrawArc(tex, cx - 40, cy, 40, 270, 450, c);
        DrawArc(tex, cx + 40, cy, 40, 90, 270, c);
        DrawLine(tex, cx - 55, cy, cx + 55, cy, c);
    }

    private static void GenerateFragments(Texture2D contour, int zodiacIndex)
    {
        int fragCount = zodiacs[zodiacIndex].fragmentCount;
        Color[] contourPixels = contour.GetPixels();

        float fragHeight = (float)CONTOUR_SIZE / fragCount;

        for (int f = 0; f < fragCount; f++)
        {
            Texture2D fragTex = new Texture2D(CONTOUR_SIZE, CONTOUR_SIZE, TextureFormat.RGBA32, false);
            Color[] fragPixels = new Color[CONTOUR_SIZE * CONTOUR_SIZE];

            int yStart = Mathf.RoundToInt(f * fragHeight);
            int yEnd = Mathf.RoundToInt((f + 1) * fragHeight);

            float fadeSize = 3f;

            for (int y = 0; y < CONTOUR_SIZE; y++)
            {
                for (int x = 0; x < CONTOUR_SIZE; x++)
                {
                    int idx = y * CONTOUR_SIZE + x;
                    if (y >= yStart && y < yEnd)
                    {
                        Color c = contourPixels[idx];
                        float distFromEdge = Mathf.Min(y - yStart, yEnd - 1 - y);
                        if (distFromEdge < fadeSize && fragCount > 1)
                        {
                            c.a *= distFromEdge / fadeSize;
                        }
                        fragPixels[idx] = c;
                    }
                    else
                    {
                        fragPixels[idx] = Color.clear;
                    }
                }
            }

            fragTex.SetPixels(fragPixels);
            fragTex.Apply();
            SaveTexture(fragTex, zodiacs[zodiacIndex].name + "_Frag_" + f);
            Object.DestroyImmediate(fragTex);
        }
    }

    private static Texture2D GenerateIconTexture(int zodiacIndex)
    {
        Texture2D tex = new Texture2D(ICON_SIZE, ICON_SIZE, TextureFormat.RGBA32, false);
        ClearTexture(tex, Color.clear);

        float cx = ICON_SIZE / 2f;
        float cy = ICON_SIZE / 2f;
        float r = ICON_SIZE / 2f - 4f;

        Color bgColor = Color.HSVToRGB((zodiacIndex * 30f) / 360f, 0.4f, 0.45f);
        bgColor.a = 0.9f;
        DrawFilledCircle(tex, cx, cy, r, bgColor);

        Color borderColor = Color.HSVToRGB((zodiacIndex * 30f) / 360f, 0.3f, 0.7f);
        borderColor.a = 1f;
        DrawCircleOutline(tex, cx, cy, r, borderColor, 3f);

        Color textColor = Color.white;
        string label = (zodiacIndex + 1).ToString();
        DrawNumber(tex, cx, cy, label, textColor);

        tex.Apply();
        return tex;
    }

    private static void DrawLine(Texture2D tex, float x1, float y1, float x2, float y2, Color color)
    {
        float dist = Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        int steps = Mathf.Max(Mathf.CeilToInt(dist * 2), 1);

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float x = Mathf.Lerp(x1, x2, t);
            float y = Mathf.Lerp(y1, y2, t);
            DrawDot(tex, x, y, LINE_THICKNESS, color);
        }
    }

    private static void DrawArc(Texture2D tex, float cx, float cy, float radius, float startDeg, float endDeg, Color color)
    {
        float arcLength = Mathf.Abs(endDeg - startDeg) * Mathf.Deg2Rad * radius;
        int steps = Mathf.Max(Mathf.CeilToInt(arcLength * 0.5f), 8);

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float angle = Mathf.Lerp(startDeg, endDeg, t) * Mathf.Deg2Rad;
            float x = cx + Mathf.Cos(angle) * radius;
            float y = cy + Mathf.Sin(angle) * radius;
            DrawDot(tex, x, y, LINE_THICKNESS, color);
        }
    }

    private static void DrawDot(Texture2D tex, float px, float py, float radius, Color color)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(px - radius));
        int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(px + radius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(py - radius));
        int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(py + radius));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = x - px;
                float dy = y - py;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                if (dist <= radius)
                {
                    float alpha = color.a;
                    if (dist > radius - 1.5f)
                        alpha *= (radius - dist) / 1.5f;

                    Color existing = tex.GetPixel(x, y);
                    Color blended = BlendColor(existing, new Color(color.r, color.g, color.b, alpha));
                    tex.SetPixel(x, y, blended);
                }
            }
        }
    }

    private static void DrawFilledCircle(Texture2D tex, float cx, float cy, float radius, Color color)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(cx - radius));
        int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(cx + radius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(cy - radius));
        int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(cy + radius));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                if (dist <= radius)
                {
                    float alpha = color.a;
                    if (dist > radius - 2f)
                        alpha *= (radius - dist) / 2f;
                    tex.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
            }
        }
    }

    private static void DrawCircleOutline(Texture2D tex, float cx, float cy, float radius, Color color, float thickness)
    {
        float arcLength = 2f * Mathf.PI * radius;
        int steps = Mathf.CeilToInt(arcLength);

        for (int i = 0; i <= steps; i++)
        {
            float angle = ((float)i / steps) * Mathf.PI * 2f;
            float x = cx + Mathf.Cos(angle) * radius;
            float y = cy + Mathf.Sin(angle) * radius;
            DrawDot(tex, x, y, thickness, color);
        }
    }

    private static void DrawNumber(Texture2D tex, float cx, float cy, string number, Color color)
    {
        float size = 18f;
        float startX = cx - (number.Length * size * 0.35f);

        foreach (char ch in number)
        {
            DrawDigit(tex, startX, cy, ch - '0', size, color);
            startX += size * 0.7f;
        }
    }

    private static void DrawDigit(Texture2D tex, float x, float y, int digit, float size, Color color)
    {
        float hs = size * 0.4f;
        float vs = size * 0.5f;
        float t = 2.5f;

        bool[] segments = GetSegments(digit);

        if (segments[0]) DrawLineThick(tex, x - hs, y + vs, x + hs, y + vs, t, color);
        if (segments[1]) DrawLineThick(tex, x + hs, y + vs, x + hs, y, t, color);
        if (segments[2]) DrawLineThick(tex, x + hs, y, x + hs, y - vs, t, color);
        if (segments[3]) DrawLineThick(tex, x - hs, y - vs, x + hs, y - vs, t, color);
        if (segments[4]) DrawLineThick(tex, x - hs, y, x - hs, y - vs, t, color);
        if (segments[5]) DrawLineThick(tex, x - hs, y + vs, x - hs, y, t, color);
        if (segments[6]) DrawLineThick(tex, x - hs, y, x + hs, y, t, color);
    }

    private static bool[] GetSegments(int digit)
    {
        bool[][] table = new bool[][]
        {
            new bool[] { true, true, true, true, true, true, false },
            new bool[] { false, true, true, false, false, false, false },
            new bool[] { true, true, false, true, true, false, true },
            new bool[] { true, true, true, true, false, false, true },
            new bool[] { false, true, true, false, false, true, true },
            new bool[] { true, false, true, true, false, true, true },
            new bool[] { true, false, true, true, true, true, true },
            new bool[] { true, true, true, false, false, false, false },
            new bool[] { true, true, true, true, true, true, true },
            new bool[] { true, true, true, true, false, true, true },
        };
        if (digit < 0 || digit > 9) return table[0];
        return table[digit];
    }

    private static void DrawLineThick(Texture2D tex, float x1, float y1, float x2, float y2, float thickness, Color color)
    {
        float dist = Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        int steps = Mathf.Max(Mathf.CeilToInt(dist), 1);

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float x = Mathf.Lerp(x1, x2, t);
            float y = Mathf.Lerp(y1, y2, t);
            DrawDot(tex, x, y, thickness, color);
        }
    }

    private static Color BlendColor(Color bg, Color fg)
    {
        float a = fg.a + bg.a * (1f - fg.a);
        if (a < 0.001f) return Color.clear;
        float r = (fg.r * fg.a + bg.r * bg.a * (1f - fg.a)) / a;
        float g = (fg.g * fg.a + bg.g * bg.a * (1f - fg.a)) / a;
        float b = (fg.b * fg.a + bg.b * bg.a * (1f - fg.a)) / a;
        return new Color(r, g, b, a);
    }

    private static void ClearTexture(Texture2D tex, Color color)
    {
        Color[] pixels = new Color[tex.width * tex.height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
        tex.SetPixels(pixels);
    }

    private static void SaveTexture(Texture2D tex, string name)
    {
        byte[] bytes = tex.EncodeToPNG();
        string path = spritePath + "/" + name + ".png";
        File.WriteAllBytes(path, bytes);
    }

    private static void SetAllSpriteImportSettings()
    {
        string[] files = Directory.GetFiles(spritePath, "*.png");
        foreach (string file in files)
        {
            string assetPath = file.Replace("\\", "/");
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
        }
    }

    private static void CreateScriptableObjects()
    {
        ZodiacData[] allZodiacs = new ZodiacData[12];

        for (int i = 0; i < zodiacs.Length; i++)
        {
            string assetPath = dataPath + "/Zodiacs/" + zodiacs[i].name + ".asset";
            ZodiacData zd = AssetDatabase.LoadAssetAtPath<ZodiacData>(assetPath);

            if (zd == null)
            {
                zd = ScriptableObject.CreateInstance<ZodiacData>();
                AssetDatabase.CreateAsset(zd, assetPath);
            }

            zd.zodiacName = zodiacs[i].name;
            zd.fragmentCount = zodiacs[i].fragmentCount;
            zd.contourSprite = LoadZodiacSprite(zodiacs[i].name + "_Contour");
            zd.iconSprite = LoadZodiacSprite(zodiacs[i].name + "_Icon");

            zd.fragmentSprites = new Sprite[zodiacs[i].fragmentCount];
            for (int f = 0; f < zodiacs[i].fragmentCount; f++)
            {
                zd.fragmentSprites[f] = LoadZodiacSprite(zodiacs[i].name + "_Frag_" + f);
            }

            EditorUtility.SetDirty(zd);
            allZodiacs[i] = zd;
        }

        CreateLevelData(1, new ZodiacData[] { allZodiacs[0] }, 30f, 60f);
        CreateLevelData(2, new ZodiacData[] { allZodiacs[1] }, 35f, 70f);
        CreateLevelData(3, new ZodiacData[] { allZodiacs[2], allZodiacs[3] }, 50f, 90f);
        CreateLevelData(4, new ZodiacData[] { allZodiacs[4], allZodiacs[5] }, 55f, 100f);
        CreateLevelData(5, new ZodiacData[] { allZodiacs[6], allZodiacs[7], allZodiacs[8] }, 80f, 140f);

        AssetDatabase.SaveAssets();
        Debug.Log("[Iteration 3] Created 12 ZodiacData + 5 LevelData assets");
    }

    private static void CreateLevelData(int levelNum, ZodiacData[] sequence, float threeStar, float twoStar)
    {
        string assetPath = dataPath + "/Levels/Level_" + levelNum.ToString("00") + ".asset";
        LevelData ld = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);

        if (ld == null)
        {
            ld = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(ld, assetPath);
        }

        ld.levelNumber = levelNum;
        ld.zodiacSequence = sequence;
        ld.threeStarTime = threeStar;
        ld.twoStarTime = twoStar;

        EditorUtility.SetDirty(ld);
    }

    private static Sprite LoadZodiacSprite(string name)
    {
        string path = spritePath + "/" + name + ".png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
            Debug.LogWarning("[Iteration 3] Sprite not found: " + path);
        return sprite;
    }
}
