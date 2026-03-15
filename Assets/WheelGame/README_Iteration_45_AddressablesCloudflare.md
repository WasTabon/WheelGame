# README - Iteration 4.5: Addressables + Cloudflare R2

## What's New in This Iteration

### New Scripts
- **AddressableLoader.cs** — Singleton (DontDestroyOnLoad). Checks internet → initializes Addressables → checks for catalog updates → downloads content with progress events → caches automatically.
- **MusicService.cs** — Singleton (DontDestroyOnLoad). Loads AudioClip from Addressables by key, provides it to AudioManager. PlayGameMusic() / StopGameMusic() for game scenes.
- **BootstrapUI.cs** — Bootstrap scene controller. Progress bar with smooth interpolation, status text, percent display. Retry button appears on failure. After download → loads music → transitions to MainMenu.
- **Iteration45_BootstrapSetup.cs** (Editor) — Creates Bootstrap scene with all UI, managers, and wiring.

### New Scene
- **Bootstrap** — First scene in Build Settings. Downloads music, then auto-transitions to MainMenu.

### Unchanged Scripts
All scripts from Iterations 1-4 remain unchanged. All bug fixes preserved (single-slice, fragment scale 0.25, bounding box slicing, green collected fragments, WheelAdaptive).

---

## PART 1: Cloudflare R2 Setup

### Step 1: Open R2 Dashboard
1. Go to https://dash.cloudflare.com
2. In the left sidebar, click **R2 Object Storage**

### Step 2: Create a Bucket
1. Click **Create bucket**
2. Name it: `wheelgame-assets` (lowercase, no spaces)
3. Choose location: **Automatic** (or closest to your players)
4. Click **Create bucket**

### Step 3: Enable Public Access
1. Open your bucket `wheelgame-assets`
2. Go to **Settings** tab
3. Scroll to **Public access**
4. Click **Allow Access**
5. You'll get a public URL like: `https://pub-XXXXXXXXXXXX.r2.dev`
6. **Save this URL** — you'll need it for Unity Addressables

### Step 4: (Optional) Custom Domain
If you want a custom domain instead of the r2.dev URL:
1. In bucket Settings → **Custom Domains**
2. Click **Connect Domain**
3. Enter your domain (e.g., `assets.yourgame.com`)
4. Follow DNS setup instructions

**Your R2 public URL:** `https://pub-XXXXXXXXXXXX.r2.dev` (save this!)

---

## PART 2: Unity Addressables Setup

### Step 1: Verify Addressables Package
You already have Addressables 1.22.3 installed. Verify:
1. **Window > Package Manager**
2. Check that `com.unity.addressables` version `1.22.3` is listed

### Step 2: Create Addressables Settings
1. **Window > Asset Management > Addressables > Groups**
2. If prompted, click **Create Addressables Settings**
3. This creates `Assets/AddressableAssetsData/` folder

### Step 3: Create Remote Group
1. In the Addressables Groups window, click **Create > Group > Packed Assets**
2. Rename the new group to **RemoteMusic**
3. Select the group, in Inspector set:
   - **Build Path:** RemoteBuildPath
   - **Load Path:** RemoteLoadPath
   - **Bundle Mode:** Pack Together
   - **Bundle Naming:** Use Hash of AssetBundle

### Step 4: Configure Profile with Cloudflare URL
1. In Addressables Groups window, click **Profile: Default** (top toolbar)
2. Click **Manage Profiles**
3. In the profile, set:
   - **RemoteBuildPath:** `ServerData/[BuildTarget]`
   - **RemoteLoadPath:** `https://pub-XXXXXXXXXXXX.r2.dev/[BuildTarget]`
   
   Replace `https://pub-XXXXXXXXXXXX.r2.dev` with YOUR R2 public URL from Part 1.
4. Close the Profiles window

### Step 5: Enable Remote Catalog
1. In Addressables Groups window, select **AddressableAssetSettings** in Project window (Assets/AddressableAssetsData/)
2. In Inspector, find **Catalog** section:
   - Check **Build Remote Catalog**
   - Set **Build Path:** RemoteBuildPath
   - Set **Load Path:** RemoteLoadPath

### Step 6: Mark Your Music as Addressable
1. Find your .wav music file in the Project window
2. Select it → in Inspector, check the **Addressable** checkbox
3. Set its address to: `GameMusic`
4. Drag it into the **RemoteMusic** group (in Addressables Groups window)

### Step 7: Move Default Local Group Assets (Optional)
If you have assets in the Default Local Group that should stay in the build (not on server), leave them there. Only music goes in RemoteMusic.

---

## PART 3: Build and Deploy

### Step 1: Build Addressable Bundles
1. **Window > Asset Management > Addressables > Groups**
2. Click **Build > New Build > Default Build Script**
3. Wait for build to complete
4. Check the output folder: `ServerData/iOS/` (or your build target)
5. You should see files like:
   - `remotemusic_XXXX.bundle`
   - `catalog_XXXX.hash`
   - `catalog_XXXX.json`

### Step 2: Upload to Cloudflare R2
1. Go to Cloudflare Dashboard → R2 → your bucket `wheelgame-assets`
2. Click **Upload**
3. Create a folder matching your build target: `iOS` (case sensitive!)
4. Upload ALL files from `ServerData/iOS/` into the `iOS` folder in R2
5. Final structure in R2 should be:
```
wheelgame-assets/
  iOS/
    remotemusic_assets_all_XXXXXXXX.bundle
    catalog_2024.XX.XX.hash
    catalog_2024.XX.XX.json
```

### Step 3: Verify Upload
Open your browser and navigate to:
`https://pub-XXXXXXXXXXXX.r2.dev/iOS/catalog_2024.XX.XX.json`

If you see JSON content — everything is uploaded correctly.

---

## PART 4: Unity Scene Setup

### File Placement
```
Assets/WheelGame/
  Scripts/
    AddressableLoader.cs       (NEW)
    MusicService.cs            (NEW)
    BootstrapUI.cs             (NEW)
    Editor/
      Iteration45_BootstrapSetup.cs  (NEW)
```

### Scene Setup
1. Go to **WheelGame > Iteration 4.5 - Setup Bootstrap Scene**
2. Click **"Create Bootstrap Scene File"**
3. Open the newly created **Assets/Scenes/Bootstrap.unity**
4. Click **"Setup Bootstrap Scene (Iteration 4.5)"**
5. Click **"Update Build Settings (Bootstrap first)"**
6. Save the scene (Ctrl+S)

### MusicService Configuration
1. Find the MusicService object in Bootstrap scene
2. In Inspector, set **Game Music Key** to: `GameMusic` (must match the Addressable address from Part 2 Step 6)

### Remove Managers from MainMenu Scene (IMPORTANT)
Since managers now spawn in Bootstrap scene with DontDestroyOnLoad:
1. Open MainMenu scene
2. If GameManager, AudioManager, SceneTransition objects exist — they will be duplicated! The singletons handle this (destroy duplicates), but it's cleaner to:
   - Leave them in MainMenu as fallback for testing MainMenu directly
   - The singleton pattern prevents duplicates automatically

---

## PART 5: Using Music in Game

After the music is downloaded and loaded, GameplayManager (or any script) can play it:

```csharp
// In GameplayManager.Start() or similar:
if (MusicService.Instance != null && MusicService.Instance.IsLoaded)
{
    MusicService.Instance.PlayGameMusic();
}
```

The AudioManager.musicSource will be used for playback — the same source that the Music Reactive System (Iteration 5) will analyze with GetSpectrumData().

---

## PART 6: Testing

### Test 1: With Internet (first run)
1. Build Settings: Bootstrap → MainMenu → GameScene
2. Play from Bootstrap scene
3. Expected: progress bar fills, status updates, auto-transitions to MainMenu
4. Console should show: "MusicService: Loaded GameMusic clip, length: Xs"

### Test 2: Without Internet (simulate)
1. In AddressableLoader.cs, temporarily change the internet check:
   ```csharp
   // Temporary test: simulate no internet
   if (true) // was: Application.internetReachability == NetworkReachability.NotReachable
   ```
2. Play from Bootstrap scene
3. Expected: "No internet connection" message, Retry button appears
4. Revert the change after testing

### Test 3: Cached (second run)
1. Play from Bootstrap again (after successful first run)
2. Expected: "Music already cached" → fast transition (no re-download)

### Test 4: Editor Shortcut
For faster iteration, you can test GameScene directly:
- GameScene still works without music (MusicService just won't play)
- Bootstrap is only needed for the full download flow

---

## PART 7: Updating Music Later

When you want to change the music track:
1. Replace the .wav file in Unity (keep the same Addressable address `GameMusic`)
2. Rebuild Addressables: **Build > Update a Previous Build**
3. Upload new files from `ServerData/iOS/` to R2 (overwrite old files)
4. Players will auto-download the new version on next app launch (catalog check)

---

## Flow Summary
```
Bootstrap Scene
  ├── AddressableLoader (check internet → check catalog → download)
  ├── MusicService (load AudioClip from Addressables)
  └── BootstrapUI (progress bar → status → auto-transition)
       │
       ├── [Success] → Load music → Transition to MainMenu
       └── [Fail] → Show "Retry" button → tap → retry download
```

## Build Settings Order
```
0: Bootstrap    (loading scene, entry point)
1: MainMenu     (main menu)
2: GameScene    (gameplay)
```
