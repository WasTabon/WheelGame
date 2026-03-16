# README - Iteration 6: Level System + Stars + Level Select

## What's New in This Iteration

### New Scripts
- **ProgressManager.cs** — Static utility for save/load via PlayerPrefs:
  - `SaveStars(level, stars)` — saves only if better than existing
  - `GetStars(level)` / `GetMaxUnlockedLevel()` / `GetTotalStars()`
  - `ResetAll()` — wipes all progress

- **LevelButton.cs** — Individual level button: number, 3 star icons, lock icon, animated appear

- **LevelSelectPanel.cs** — Scrollable level selection with 30 levels:
  - Levels 1-5: unique LevelData configs
  - Levels 6-30: each mapped to a random config from 1-5 (generated once, saved in PlayerPrefs)
  - `GetConfigForLevel(levelNumber)` — static method used by GameplayManager
  - Vertical ScrollRect with elastic scrolling
  - Total stars counter
  - Back button + dim overlay to close

### Modified Scripts
- **MainMenuUI.cs** — Play button opens LevelSelectPanel instead of loading GameScene directly

- **GameManager.cs** — Added `SaveLevelStars()`, progress uses ProgressManager

- **GameplayManager.cs** — Updated:
  - Uses `LevelSelectPanel.GetConfigForLevel()` to get the right config for any level
  - UI shows actual level number (e.g. "Level 7"), not config number
  - OnLevelComplete saves stars and unlocks next using actual level number

### Unchanged Scripts
All scripts from Iterations 1-5 unchanged, all fixes preserved.

## Level Config System

| Level | Config Source |
|-------|-------------|
| 1 | LevelData 1 (Aries) |
| 2 | LevelData 2 (Taurus) |
| 3 | LevelData 3 (Gemini → Cancer) |
| 4 | LevelData 4 (Leo → Virgo) |
| 5 | LevelData 5 (Libra → Scorpio → Sagittarius) |
| 6-30 | Random from LevelData 1-5 (generated once, saved) |

Example: Level 7 might use config 3 (Gemini → Cancer), Level 12 might use config 1 (Aries).
The mapping is generated randomly on first launch and persists in PlayerPrefs.

## Setup Instructions

### File Placement
```
Assets/WheelGame/
  Scripts/
    ProgressManager.cs              (NEW)
    LevelButton.cs                  (NEW)
    LevelSelectPanel.cs             (NEW)
    MainMenuUI.cs                   (MODIFIED — replace)
    GameManager.cs                  (MODIFIED — replace)
    GameplayManager.cs              (MODIFIED — replace)
    Editor/
      Iteration6_LevelSelectSetup.cs  (NEW)
```

### Scene Setup
1. Open **MainMenu** scene
2. Go to **WheelGame > Iteration 6 - Setup Level Select (MainMenu scene)**
3. Click **"Setup Level Select (Iteration 6)"**
4. Save the scene (Ctrl+S)

## How to Test

### Level Select:
1. MainMenu → click PLAY → scrollable Level Select panel appears
2. Level 1 unlocked, levels 2-30 locked (dark + lock icon)
3. Scroll up/down through 30 levels
4. Click Level 1 → loads GameScene

### Progression:
1. Complete Level 1 → stars earned → Level 2 unlocks
2. Go back to MainMenu → Level Select → Level 1 shows gold stars, Level 2 unlocked
3. Continue through levels — each completion unlocks next

### Star Persistence:
1. Earn stars on several levels
2. Stop and restart Play Mode
3. Stars and unlock state persist (PlayerPrefs)

## Expected Result
- 30 level buttons in a scrollable list
- Lock/unlock visual states
- Gold/grey stars
- Total stars counter at top
- Smooth elastic scrolling
- Config mapping: levels 6-30 play configs from 1-5 randomly
- UI shows actual level number, gameplay uses mapped config
