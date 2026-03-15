# README - Iteration 4: Core Gameplay

## What's New in This Iteration

### New Scripts
- **GameplayManager.cs** — Core gameplay controller:
  - Loads LevelData based on GameManager.currentLevel (random after level 5)
  - Distributes correct fragments + random decoys from other zodiacs across 12 sections
  - Handles clicks: correct → fragment flies to center with DOTween, wrong → lose life + shake
  - Sequential zodiac progression (complete one → next appears)
  - Timer counting up during gameplay
  - Win condition (all zodiacs complete) → stars by time
  - Lose condition (0 lives) → game over
  - Subscribes/unsubscribes to WheelSection.OnSectionClicked events properly
  - Click blocking during animations (isProcessingClick flag)

- **WinPanel.cs** — Victory popup:
  - Animated entrance (scale + fade)
  - Stars appear one by one with bounce (gold if earned, grey if not)
  - Shows completion time
  - "NEXT LEVEL" and "MENU" buttons with feedback

- **LosePanel.cs** — Game over popup:
  - Animated entrance with shake
  - "RETRY" (restarts same level) and "MENU" buttons

### Modified Scripts
- **GameSceneUI.cs** — Added:
  - `timerText` field + `SetTimer()` method
  - `winPanel` / `losePanel` fields + `ShowWinPanel()` / `ShowLosePanel()` methods
  - All existing code preserved (top panel, lives, back button, entrance animation)

### Unchanged Scripts
- WheelSection.cs — no changes (isEmpty/isCorrectFragment set by GameplayManager)
- WheelCenter.cs — no changes (SetContour/OnFragmentCollected called by GameplayManager)
- WheelController.cs — no changes
- WheelAdaptive.cs — no changes
- All Iteration 1 scripts — no changes
- All Iteration 3 scripts — no changes
- Single-slice sprite fix preserved
- Correct radius calculations preserved (OUTER_RADIUS_PX=250, INNER_RADIUS_PX=112)

## Setup Instructions

### File Placement
```
Assets/WheelGame/
  Scripts/
    GameplayManager.cs       (NEW)
    WinPanel.cs              (NEW)
    LosePanel.cs             (NEW)
    GameSceneUI.cs           (MODIFIED — replace old file)
    Editor/
      Iteration4_GameplaySetup.cs  (NEW)
```

### Scene Setup
1. Make sure Iteration 3 has been run (zodiac/level assets must exist)
2. Open **GameScene**
3. Go to **WheelGame > Iteration 4 - Setup Core Gameplay**
4. Click **"Setup Core Gameplay (Iteration 4)"**
5. Save the scene (Ctrl+S)

### What the editor script does:
- Creates GameplayManager object, wires WheelController reference
- Finds all LevelData and ZodiacData assets and assigns them to GameplayManager
- Adds TimerText to TopPanel
- Creates WinPanel with stars, time, buttons (hidden by default)
- Creates LosePanel with retry/menu buttons (hidden by default)
- Wires all new references on GameSceneUI

## How to Test

### Basic gameplay:
1. Start from **MainMenu** → click Play (or start GameScene directly)
2. Level 1 loads: Aries (4 correct fragments among 12 sections)
3. Sections now show fragment sprites — some correct, some decoys
4. **Click correct fragment** → it flies to center circle with animation, section empties
5. **Click wrong fragment** → lose life (5→4), red flash, shake
6. Timer counts up in the UI

### Win flow:
1. Collect all correct fragments for current zodiac
2. If level has more zodiacs → sections refill with next zodiac's fragments
3. After all zodiacs → Win Panel appears with stars:
   - 3★ if time < threeStarTime
   - 2★ if time < twoStarTime
   - 1★ otherwise
4. Click "NEXT LEVEL" → loads next level
5. Click "MENU" → returns to MainMenu

### Lose flow:
1. Click 5 wrong sections → 0 lives
2. Lose Panel appears with shake
3. Click "RETRY" → restarts same level (lives restored to 5)
4. Click "MENU" → returns to MainMenu

### Level progression:
- Level 1: Aries (4 fragments)
- Level 2: Taurus (5 fragments)
- Level 3: Gemini → Cancer (2 signs sequential)
- Level 4: Leo → Virgo (2 signs sequential)
- Level 5: Libra → Scorpio → Sagittarius (3 signs sequential)
- After level 5: random level from 1-5

## Expected Result
- Fully playable core gameplay loop
- Fragments appear with staggered animation across sections
- Correct clicks: green flash + fragment flies to center + center punches
- Wrong clicks: red flash + shake + life lost with color flash
- Smooth animated Win/Lose panels
- Timer visible and updating
- Zodiac name and icon update when switching signs
- Progress text shows completed/total zodiacs

## Architecture
```
GameScene
  ├── GameplayManager (loads levels, handles clicks, manages state)
  ├── WheelRoot (WheelController + WheelAdaptive)
  │    └── WheelPivot
  │         ├── Section_00..11 (WheelSection — OnSectionClicked events)
  │         ├── WheelCenter (contour display, fragment collection)
  │         └── Ring
  └── GameCanvas (GameSceneUI)
       ├── TopPanel (zodiac info, lives, timer, back button)
       ├── WinPanel (hidden, shown on victory)
       └── LosePanel (hidden, shown on game over)
```
