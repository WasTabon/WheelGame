# README - Iteration 2: Wheel Visual + Base Rotation

## What's New in This Iteration

### New Scripts
- **WheelController.cs** — Manages the wheel: continuous rotation with variable speed, smooth pulse animation (scale breathing), rotation impulse support. Singleton with references to all 12 sections and center.
- **WheelSection.cs** — Individual wheel section component. Click detection via PolygonCollider2D (pie-slice shaped), visual feedback for correct/wrong clicks (green flash / red shake), fragment display with animated appear/disappear.
- **WheelCenter.cs** — Center circle: displays zodiac contour sprite, glow pulse animation (looping fade), punch animation on fragment collection, complete animation.
- **GameSceneUI.cs** — Game scene overlay UI: top panel with zodiac icon (placeholder), zodiac name, level text, lives counter with heart icon, progress text, back button to return to MainMenu.
- **Iteration2_GameSceneSetup.cs** (Editor) — Two-step setup: generates all sprites, then builds the entire game scene.

### Generated Sprites (in Assets/WheelGame/GeneratedSprites/)
- `WheelSection.png` — Pie-slice shape for each of the 12 sections (with gaps between slices)
- `WheelSectionHighlight.png` — Semi-transparent overlay for section highlight effects
- `WheelCenter.png` — Solid circle for the center area
- `WheelCenterGlow.png` — Soft-edge glow circle for center pulse effect
- `WheelRing.png` — Thin ring outline for visual polish around the wheel
- `FragmentPlaceholder.png` — Small circle placeholder for fragment display in sections
- `ContourPlaceholder.png` — Larger circle placeholder for zodiac contour in center

### Unchanged from Iteration 1
- GameManager.cs — no changes
- AudioManager.cs — no changes
- SceneTransition.cs — no changes
- MainMenuUI.cs — no changes
- SettingsPanel.cs — no changes
- UIAnimations.cs — no changes
- ButtonFeedback.cs — no changes

## Setup Instructions

### File Placement
Copy/overwrite the `Assets/WheelGame/` folder. New files:
```
Assets/WheelGame/
  Scripts/
    WheelController.cs
    WheelSection.cs
    WheelCenter.cs
    GameSceneUI.cs
    Editor/
      Iteration1_MainMenuSetup.cs  (unchanged)
      Iteration2_GameSceneSetup.cs (NEW)
```

### Scene Setup
1. Open the **GameScene** (created in Iteration 1)
2. Go to **WheelGame > Iteration 2 - Setup Game Scene**
3. Click **"1. Generate Sprites (Iteration 2)"** — creates all sprites in GeneratedSprites folder
4. Wait for Unity to reimport assets
5. Click **"2. Setup Game Scene (Iteration 2)"** — creates wheel, sections, center, and UI
6. Save the scene (Ctrl+S)

## How to Test
1. Open GameScene and enter Play Mode
2. **Wheel** should be visible with 12 colored pie-slice sections arranged in a circle
3. **Wheel rotates** smoothly and continuously (base speed ~25 deg/sec)
4. **Wheel pulses** subtly (slight scale breathing)
5. **Center circle** has a soft glow that fades in and out
6. **Top panel** shows zodiac icon placeholder, name "Aries", "Level 1", lives "5", and progress "0 / 4"
7. **Click on sections** — they animate (punch scale) when clicked
8. **Back button** — fades out and returns to MainMenu
9. From MainMenu, **Play button** transitions to GameScene smoothly

## Expected Result
- Visually complete wheel with 12 distinct colored sections around a glowing center
- Smooth continuous rotation
- Subtle scale pulse (breathing effect)
- Clickable sections with feedback animation
- Clean top UI panel with all game info placeholders
- Smooth transitions between MainMenu and GameScene

## Architecture
```
WheelRoot (WheelController)
  └── WheelPivot (rotates)
       ├── Section_00 (WheelSection + SpriteRenderer + Collider)
       │    ├── Highlight (SpriteRenderer)
       │    └── Fragment (SpriteRenderer)
       ├── Section_01 ... Section_11
       ├── WheelCenter (WheelCenter + SpriteRenderer)
       │    ├── Glow (SpriteRenderer)
       │    └── Contour (SpriteRenderer)
       └── Ring (SpriteRenderer)

GameCanvas (GameSceneUI)
  └── TopPanel
       ├── BackButton
       ├── ZodiacContainer (Icon + Name + Level)
       ├── LivesContainer (Heart + Count)
       └── ProgressText
```
