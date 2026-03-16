# README - Iteration 10: Final Polish

## What's New in This Iteration

### New Scripts
- **FXManager.cs** — Procedural particle effects (no prefabs needed):
  - Creates 4 ParticleSystem at runtime using Sprites/Default shader (Built-in 2D compatible)
  - **Correct click**: Green sparkles burst from section position
  - **Wrong click**: Red particles scatter from section
  - **Collect/Zodiac complete**: Gold explosion at center
  - **Star earned**: Gold sparkles at star position on WinPanel
  - All particles auto-deactivate after lifetime

- **HapticFeedback.cs** — iOS haptic wrapper:
  - `Light()` — correct click, star earned
  - `Medium()` — booster use
  - `Heavy()` — wrong click
  - `Success()` — zodiac complete, level complete
  - `Error()` — level failed
  - Uses native UIImpactFeedbackGenerator via .mm plugin
  - Safe no-op in Editor and non-iOS platforms

- **HapticFeedback.mm** — iOS native Objective-C plugin:
  - UIImpactFeedbackGenerator (Light/Medium/Heavy)
  - UINotificationFeedbackGenerator (Success/Warning/Error)
  - UISelectionFeedbackGenerator
  - Place in Assets/WheelGame/Plugins/iOS/

- **UIJuice.cs** — Extra UI feel:
  - **Low life warning**: When lives ≤ 2, lives text pulses between white and red, container gently scales
  - **Timer color**: Normal (purple) → Warning yellow (past 3★ threshold) → Danger red (past 2★ threshold)
  - Resets when lives restored above threshold

### Modified Scripts
- **GameplayManager.cs** — Added calls throughout:
  - Correct click: `HapticFeedback.Light()` + `FXManager.PlayCorrect()`
  - Fragment reaches center: `FXManager.PlayCollect()`
  - Wrong click: `HapticFeedback.Heavy()` + `FXManager.PlayWrong()` + `UIJuice.UpdateLivesWarning()`
  - Zodiac complete: `HapticFeedback.Success()` + `FXManager.PlayCollect()`
  - Level complete: `HapticFeedback.Success()`
  - Timer update: `UIJuice.UpdateTimerColor()`
  - Level start: `UIJuice.SetStarThresholds()` + `UIJuice.UpdateLivesWarning()`

- **WinPanel.cs** — Each earned star triggers `HapticFeedback.Light()` + `FXManager.PlayStar()`

### Unchanged Scripts
All other scripts preserved including all fixes from iterations 1-8.

## Setup Instructions

### File Placement
```
Assets/WheelGame/
  Scripts/
    FXManager.cs                    (NEW)
    HapticFeedback.cs               (NEW)
    UIJuice.cs                      (NEW)
    GameplayManager.cs              (MODIFIED — replace)
    WinPanel.cs                     (MODIFIED — replace)
    Editor/
      Iteration10_PolishSetup.cs    (NEW)
  Plugins/
    iOS/
      HapticFeedback.mm             (NEW — iOS native plugin)
```

### Scene Setup
1. Open **GameScene**
2. Go to **WheelGame > Iteration 10 - Setup Final Polish (GameScene)**
3. Click **"Setup Final Polish (Iteration 10)"**
4. Save the scene (Ctrl+S)

### What the editor script does:
- Creates FXManager object (particle systems are generated in code at runtime)
- Adds UIJuice to GameCanvas, wires livesText, livesContainer, timerText from GameSceneUI

## How to Test

### Particles:
1. Start a level
2. Click correct section → green sparkles burst from the section
3. Fragment flies to center → gold explosion at center
4. Click wrong section → red particles scatter
5. Complete all zodiacs → gold explosion at center
6. Win Panel → each earned star has gold sparkles

### Haptics (iOS device only):
1. Correct click → light tap
2. Wrong click → heavy thump
3. Zodiac/level complete → success vibration
4. Star appear → light tap
5. (In Editor, haptics are no-op — test on device)

### UIJuice:
1. Start a level, lose lives until 2 remain
2. Lives text should pulse red/white, container gently breathes
3. Gain life (Extra Life booster) → pulse stops when above 2
4. Watch timer: starts purple, turns yellow after 3★ time, turns red after 2★ time

### Particle Material:
All particles use `Sprites/Default` shader — compatible with Built-in 2D render pipeline on iOS. No additional materials needed.

## Expected Result
- Every action has visual particle feedback
- iOS devices have haptic feedback on every interaction
- Low lives create urgency with pulsing UI
- Timer color guides player on star thresholds
- Game feels juicy, responsive, and polished

## Complete Game Flow (All Iterations)
```
Bootstrap
  └── Download music (Addressables + Cloudflare R2)
       └── MainMenu
            ├── Play → Level Select (30 levels, scroll)
            │    └── Select Level → GameScene
            │         ├── Wheel rotates to music (MusicReactor + WheelMusicSync)
            │         ├── Click sections (correct/wrong with FX + haptics)
            │         ├── Boosters (Undo / Slowmo / +1 Life)
            │         ├── Complete zodiacs → Win Panel (stars + FX)
            │         │    ├── Next Level
            │         │    └── Menu
            │         └── Lose → Lose Panel
            │              ├── Retry
            │              └── Menu
            ├── Settings (Music/SFX volume)
            └── Shop (IAP booster pack)
```

## All Iterations Summary
| # | What | Status |
|---|------|--------|
| 1 | Foundation + Main Menu | ✅ |
| 2 | Wheel Visual + Rotation | ✅ + fixes |
| 3 | Zodiac Generation + ScriptableObjects | ✅ + fixes |
| 4 | Core Gameplay | ✅ + fixes |
| 4.5 | Addressables + Cloudflare R2 | ✅ + fixes |
| 5 | Music Reactive System | ✅ |
| 6 | Level System + Stars + Level Select | ✅ + fixes |
| 7 | Boosters | ✅ |
| 8 | Shop + IAP | ✅ |
| 10 | Final Polish (particles, haptics, UI juice) | ✅ |
