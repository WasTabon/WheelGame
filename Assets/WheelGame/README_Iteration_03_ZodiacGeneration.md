# README - Iteration 3: Zodiac Sprite Generation + ScriptableObjects

## What's New in This Iteration

### New Scripts
- **ZodiacData.cs** — ScriptableObject storing: zodiac name, icon sprite (replaceable placeholder), contour sprite, array of fragment sprites, fragment count.
- **LevelData.cs** — ScriptableObject storing: level number, array of ZodiacData (sequential signs to complete), star time thresholds (3★ and 2★).
- **Iteration3_ZodiacSetup.cs** (Editor) — One-button generator that creates all zodiac assets.

### Generated Sprites (in Assets/WheelGame/GeneratedSprites/Zodiacs/)
For each of the 12 zodiac signs:
- `[Name]_Contour.png` — 256x256 geometric symbol drawn with lines/arcs (placeholder, replaceable)
- `[Name]_Frag_0.png` through `[Name]_Frag_N.png` — Horizontal strips of the contour, each fragment is a piece of the full symbol
- `[Name]_Icon.png` — 128x128 colored circle with number (placeholder for zodiac icon in top panel, replaceable via ScriptableObject)

### Generated ScriptableObjects
**12 ZodiacData** (in Assets/WheelGame/Data/Zodiacs/):
- Aries (4 fragments), Taurus (5), Gemini (4), Cancer (5), Leo (4), Virgo (5)
- Libra (4), Scorpio (5), Sagittarius (4), Capricorn (5), Aquarius (4), Pisces (5)

**5 LevelData** (in Assets/WheelGame/Data/Levels/):
| Level | Signs | 3★ Time | 2★ Time |
|-------|-------|---------|---------|
| 1 | Aries | < 30s | < 60s |
| 2 | Taurus | < 35s | < 70s |
| 3 | Gemini → Cancer | < 50s | < 90s |
| 4 | Leo → Virgo | < 55s | < 100s |
| 5 | Libra → Scorpio → Sagittarius | < 80s | < 140s |

### Unchanged from Previous Iterations
All scripts from Iteration 1 and 2 remain unchanged, including:
- Fixed single-slice pie sprite generation (no modulo bug)
- WheelAdaptive for screen scaling
- Correct radius calculations (OUTER_RADIUS_PX=250, INNER_RADIUS_PX=112, PPU=100)

## Setup Instructions

### File Placement
Add these new files to your project:
```
Assets/WheelGame/
  Scripts/
    ZodiacData.cs          (NEW)
    LevelData.cs           (NEW)
    Editor/
      Iteration3_ZodiacSetup.cs  (NEW)
```

### Asset Generation
1. Go to **WheelGame > Iteration 3 - Generate Zodiacs & Levels** in the Unity menu
2. Click **"Generate All Zodiac Assets (Iteration 3)"**
3. Wait for Unity to process (sprite import + ScriptableObject creation)
4. Check `Assets/WheelGame/GeneratedSprites/Zodiacs/` for sprites
5. Check `Assets/WheelGame/Data/Zodiacs/` for ZodiacData assets
6. Check `Assets/WheelGame/Data/Levels/` for LevelData assets

### Replacing Art Later
To replace placeholder art with real zodiac artwork:
1. Open any ZodiacData asset in the Inspector
2. Drag your icon sprite into the "Icon Sprite" field
3. Drag your contour sprite into the "Contour Sprite" field
4. Drag fragment sprites into the "Fragment Sprites" array
5. That's it — the game will use the new art

## How to Test
1. Run the generator button
2. Navigate to `Assets/WheelGame/Data/Zodiacs/` and click on any ZodiacData asset
3. Verify in Inspector: name is set, sprites are assigned, fragment count matches
4. Navigate to `Assets/WheelGame/Data/Levels/` and click on any LevelData asset
5. Verify: level number, zodiac sequence, star thresholds are correct
6. Open any contour sprite in `GeneratedSprites/Zodiacs/` — should show a unique geometric symbol
7. Fragment sprites should be horizontal slices of their parent contour

## Expected Result
- 12 unique zodiac symbols as contour sprites (line drawings)
- Each zodiac has correctly sliced fragment sprites
- 12 colored icon placeholders (numbered circles)
- 12 ZodiacData assets with all references wired
- 5 LevelData assets with correct zodiac sequences and star thresholds
- No changes to any scenes

## Zodiac Symbols (Placeholder Descriptions)
- **Aries**: Ram horns (two arcs) with vertical line
- **Taurus**: Circle with horns on top
- **Gemini**: Two vertical lines with connecting arcs
- **Cancer**: Two circles with sweeping curves
- **Leo**: Circle with loop tail
- **Virgo**: M-shape with curved tail
- **Libra**: Two horizontal lines with arc above
- **Scorpio**: M-shape with arrow tail
- **Sagittarius**: Diagonal arrow
- **Capricorn**: Curved V with loop
- **Aquarius**: Two rows of zigzag waves
- **Pisces**: Two facing arcs with horizontal line

## What Changed Since Last Iteration
- New: ZodiacData.cs, LevelData.cs, Iteration3_ZodiacSetup.cs
- No existing scripts modified
- No scene changes
