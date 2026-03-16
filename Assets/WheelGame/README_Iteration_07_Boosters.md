# README - Iteration 7: Boosters

## What's New in This Iteration

### New Scripts
- **BoosterManager.cs** — Booster logic controller:
  - **Undo**: Reverts last wrong click, restores 1 life. Only available if a wrong click happened and no correct click since.
  - **Slowmo**: Slows wheel rotation, sets music pitch to 0.6x, adds AudioLowPassFilter (800Hz) for slowmo audio feel. Lasts 5 seconds, then smoothly returns to normal with DOTween.
  - **Extra Life**: Adds +1 life immediately.
  - `OnBoostersChanged` event for UI refresh.
  - Resets slowmo on level end/retry/menu.

- **BoosterUI.cs** — 3 booster buttons at bottom of screen:
  - Each shows icon + label + remaining count.
  - Greyed out (alpha 0.4) when unavailable (count = 0 or condition not met).
  - Punch animation on use.
  - Slowmo button has timer fill that drains during slowmo.
  - Slides in from bottom on level start.
  - Auto-refreshes after each booster use and after wrong clicks.

### Modified Scripts
- **GameplayManager.cs** — Added:
  - Undo tracking: saves last wrong click, clears on correct click or new zodiac
  - `HasUndoData()`, `ExecuteUndo()`, `AddLife()`, `GetLives()`
  - Resets slowmo on Retry/NextLevel/GoToMenu
  - Refreshes BoosterUI after wrong clicks

- **GameSceneUI.cs** — Added `boosterUI` reference field.

### Unchanged Scripts
All other scripts preserved including all fixes (single-slice, fragment scale 0.25, bounding box slicing, green collected fragments, Addressable Completed callbacks, music required, ScrollView level select, config map).

## Setup Instructions

### File Placement
```
Assets/WheelGame/
  Scripts/
    BoosterManager.cs             (NEW)
    BoosterUI.cs                  (NEW)
    GameplayManager.cs            (MODIFIED — replace)
    GameSceneUI.cs                (MODIFIED — replace)
    Editor/
      Iteration7_BoosterSetup.cs  (NEW)
```

### Scene Setup
1. Open **GameScene**
2. Go to **WheelGame > Iteration 7 - Setup Boosters (GameScene)**
3. Click **"Setup Boosters (Iteration 7)"**
4. Save the scene (Ctrl+S)

## How to Test

### Undo Booster:
1. Start a level, click a wrong section (lose 1 life)
2. Bottom panel: Undo button should be active
3. Click Undo → life restored, count decreases by 1
4. Click a correct section → Undo becomes unavailable (can only undo last wrong click)

### Slowmo Booster:
1. Click Slowmo button → wheel slows down
2. Music pitch drops to 0.6x, sound gets muffled (low pass filter)
3. Timer fill on button drains over 5 seconds
4. After 5 seconds → smoothly returns to normal speed and audio
5. Can't use Slowmo again while active

### Extra Life Booster:
1. Click +1 Life → lives increase by 1
2. Green flash on lives counter
3. Works any time during gameplay

### Counts:
- Each booster starts with 5 uses (from GameManager)
- Count shows on button, updates after each use
- When count reaches 0 → button greyed out

## Expected Result
- 3 booster buttons at bottom of game screen
- Slide-in animation on level start
- Active/inactive visual states
- Undo restores life from last wrong click only
- Slowmo creates immersive slow-motion with audio filter
- Extra Life is simple +1
- All counts persist (saved in GameManager/PlayerPrefs)

## UI Layout
```
GameCanvas
  ├── TopPanel (zodiac info, lives, timer)
  ├── BoosterPanel (NEW — bottom)
  │    ├── UndoButton (icon + "UNDO" + count)
  │    ├── SlowmoButton (icon + "SLOW" + count + timer fill)
  │    └── ExtraLifeButton (icon + "+1 HP" + count)
  ├── WinPanel (hidden)
  └── LosePanel (hidden)
```
