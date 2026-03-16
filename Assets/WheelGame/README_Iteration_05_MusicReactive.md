# README - Iteration 5: Music Reactive System

## What's New in This Iteration

### New Scripts
- **MusicReactor.cs** — Real-time audio spectrum analyzer:
  - Uses `AudioSource.GetSpectrumData()` every frame (256 samples, BlackmanHarris window)
  - Extracts bass energy (first 12 frequency bands) for beat detection
  - Adaptive beat threshold — adjusts to music dynamics, not just a fixed value
  - Beat cooldown (0.2s) prevents double triggers
  - Smoothed energy values for pulse animation
  - `OnBeat` event fired on each detected beat
  - Runtime auto-wires to AudioManager.musicSource if not set in editor
  - Properties: `BassEnergy`, `AverageEnergy`, `NormalizedBass`, `IsPlaying`

- **WheelMusicSync.cs** — Bridges MusicReactor → WheelController:
  - **Rotation speed** varies with average music energy (quiet = 15°/s, loud = 55°/s)
  - **Rotation impulse** on each beat (+12°/s, decays smoothly back)
  - **Scale pulse** driven by bass energy (smooth breathing effect)
  - **Beat pulse** — bigger kick on each beat, then returns
  - **Glow flash** — center glow brightens on beat, then fades
  - `SetSlowMotion()` for booster (Iteration 7) — halves speeds + dampens pulse
  - Runtime auto-finds MusicReactor and WheelController if not wired in editor

### Modified Scripts
- **WheelController.cs** — Added:
  - `SetExternalPulse(float scale)` — lets MusicSync control scale directly
  - `DisableExternalPulse()` — reverts to built-in sine pulse
  - `useExternalPulse` flag — when music plays, MusicSync drives scale; when no music, fallback sine pulse
  - Smooth lerp between current and target external scale
  - All existing code preserved (rotation, sections, center, ResetWheel, PunchScale)
  - All fixes preserved (single-slice, fragment scale 0.25, WheelAdaptive)

### Unchanged Scripts
All scripts from Iterations 1-4.5 unchanged, including:
- Fixed AddressableLoader (Completed callback pattern for auto-released handles)
- BootstrapUI (music required, retry on failure)
- MusicService (try-catch, IsValid checks)
- WheelSection (fragmentBaseScale tracking)
- WheelCenter (green collected fragments)
- GameplayManager (music playback in Start)

## Setup Instructions

### File Placement
```
Assets/WheelGame/
  Scripts/
    MusicReactor.cs           (NEW)
    WheelMusicSync.cs         (NEW)
    WheelController.cs        (MODIFIED — replace)
    Editor/
      Iteration5_MusicReactiveSetup.cs  (NEW)
```

### Scene Setup
1. Open **GameScene**
2. Go to **WheelGame > Iteration 5 - Setup Music Reactive**
3. Click **"Setup Music Reactive System (Iteration 5)"**
4. Save the scene (Ctrl+S)

### What the editor script does:
- Adds MusicReactor component (on AudioManager if present on scene, otherwise standalone)
- Adds WheelMusicSync component on WheelRoot
- Wires WheelController and MusicReactor references
- At runtime, both auto-wire to DontDestroyOnLoad singletons if editor refs are null

## How to Test

### Full flow (with music):
1. Start from **Bootstrap** scene
2. Music downloads → loads → MainMenu → Play → GameScene
3. Music should be playing (from MusicService.PlayGameMusic in GameplayManager)
4. **Watch the wheel:** it should pulse and speed up/slow down with the music
5. On heavy bass hits: wheel should noticeably speed up and kick in scale
6. Center glow flashes on beats
7. During quiet parts: wheel slows down, minimal pulse
8. During loud/energetic parts: wheel spins faster, stronger pulse

### Testing without Bootstrap:
If you Play GameScene directly (no music loaded):
- MusicReactor won't detect any audio → `IsPlaying` returns false
- WheelController falls back to built-in sine pulse (same as before)
- Gameplay works normally, just without music sync

### Tweaking values:
On **MusicReactor** (Inspector):
- `Beat Threshold` — lower = more beats detected, higher = only strong beats
- `Beat Cooldown` — minimum time between beats (0.2s default)
- `Bass Range End` — how many low-frequency bands count as "bass" (12 default)

On **WheelMusicSync** (Inspector):
- `Min/Max Rotation Speed` — range of rotation based on energy
- `Rotation Impulse On Beat` — speed kick per beat
- `Beat Pulse Amount` — scale kick on beat (0.07 default)
- `Energy Pulse Amount` — continuous scale breathing (0.02 default)

## Expected Result
- Wheel rotation speed dynamically follows the music energy
- On each bass beat: noticeable speed impulse + scale punch + glow flash
- Between beats: smooth return to energy-based baseline
- Feels organic and reactive, not robotic
- When music stops or isn't loaded: graceful fallback to simple sine pulse

## Architecture
```
AudioManager (DontDestroyOnLoad)
  └── musicSource (AudioSource) ← plays music from MusicService

MusicReactor (GameScene, wires to AudioManager.musicSource at runtime)
  ├── GetSpectrumData() every frame
  ├── Bass energy → beat detection → OnBeat event
  └── Smoothed energy values for continuous sync

WheelMusicSync (on WheelRoot)
  ├── Listens to MusicReactor.OnBeat → impulse + punch
  ├── Reads MusicReactor.AverageEnergy → rotation speed
  ├── Reads MusicReactor.NormalizedBass → scale pulse
  └── Drives WheelController.SetExternalPulse() + SetRotationSpeed()

WheelController
  ├── External pulse mode (from MusicSync) OR fallback sine pulse
  └── Smooth rotation with SmoothDamp
```
