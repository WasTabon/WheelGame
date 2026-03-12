# README - Iteration 1: Foundation + Main Menu

## What's in this iteration
- **GameManager** — Singleton (DontDestroyOnLoad). Manages game progress, booster counts, scene transitions, save/load via PlayerPrefs.
- **AudioManager** — Singleton (DontDestroyOnLoad). Handles music and SFX playback, volume control with fade in/out, crossfade between clips.
- **SceneTransition** — Singleton (DontDestroyOnLoad). Full-screen black fade overlay for smooth scene transitions.
- **MainMenuUI** — Controls main menu: animated title entrance, staggered button appearance, navigation to Settings/Shop/Play.
- **SettingsPanel** — Animated panel with Music and SFX sliders. Slides up from bottom with dim overlay. Tap dim or Close to dismiss.
- **UIAnimations** — Static utility class with DOTween helpers: ScalePunch, BounceIn/Out, FadeIn/Out, SlideIn/Out, StaggeredAppear.
- **ButtonFeedback** — Component for tactile button feedback: scale down on press, punch on click, plays SFX.

## Setup Instructions

### Prerequisites
1. Unity 2022.3.62f with Built-in 2D Render Pipeline
2. DOTween Free installed (from Asset Store)
3. TextMeshPro imported (Window > TextMeshPro > Import TMP Essential Resources)

### File Placement
Copy the `Assets/WheelGame/` folder into your project's `Assets/` folder:
```
Assets/
  WheelGame/
    Scripts/
      GameManager.cs
      AudioManager.cs
      SceneTransition.cs
      MainMenuUI.cs
      SettingsPanel.cs
      UIAnimations.cs
      ButtonFeedback.cs
      Editor/
        Iteration1_MainMenuSetup.cs
```

### Scene Setup
1. Open (or create) your MainMenu scene
2. Go to **WheelGame > Iteration 1 - Setup Main Menu** in the Unity menu bar
3. Click **"Setup Main Menu Scene (Iteration 1)"** — this creates all UI objects and managers
4. Click **"Create GameScene (empty placeholder)"** — creates an empty GameScene for Play button navigation
5. Click **"Add Scenes to Build Settings"** — registers both scenes
6. Save the scene (Ctrl+S)

### Important: DOTween Setup
After importing DOTween, go to **Tools > Demigiant > DOTween Utility Panel** and click **"Setup DOTween"**.

## How to Test
1. Enter Play Mode on the MainMenu scene
2. **Title** should fade in and slide down from above
3. **Buttons** (Play, Settings, Shop) should appear one by one with bounce animation
4. **Click any button** — it should scale down on press, then punch on release
5. **Settings button** — opens settings panel with slide-up animation and dim overlay
6. **Music/SFX sliders** — adjust values (saved to PlayerPrefs)
7. **Close button or tap dim** — panel slides back down
8. **Play button** — screen fades to black, loads GameScene (empty), then fades back in

## Expected Result
- Polished main menu with smooth DOTween animations
- Tactile button feedback (press/release/click)
- Settings panel with animated open/close
- Working scene transition with fade
- All managers persist across scene loads (DontDestroyOnLoad)

## What Changed
First iteration — everything is new.
