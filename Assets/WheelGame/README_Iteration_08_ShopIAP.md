# README - Iteration 8: Shop + IAP

## What's New in This Iteration

### New Scripts
- **IAPManager.cs** — Shop panel controller + IAP callbacks:
  - `Show()` / `Hide()` — animated open/close with dim overlay
  - `OnBuyClicked()` — shows loading overlay on buy button
  - `OnPurchaseComplete(Product)` — called by IAP Button: adds booster pack via GameManager, hides loading, shows success message, refreshes counts
  - `OnPurchaseFailed(Product, PurchaseFailureDescription)` — hides loading, shows error message
  - `OnProductFetched(Product)` — sets localized price text
  - Displays current booster counts (Undo, Slowmo, Extra Life)
  - Loading overlay prevents double-clicks during purchase

### Modified Scripts
- **MainMenuUI.cs** — Added `shopPanel` (IAPManager) reference. Shop button now opens the shop panel.

### Unchanged Scripts
All scripts from Iterations 1-7 unchanged, all fixes preserved.

## Setup Instructions

### File Placement
```
Assets/WheelGame/
  Scripts/
    IAPManager.cs                 (NEW)
    MainMenuUI.cs                 (MODIFIED — replace)
    Editor/
      Iteration8_ShopSetup.cs     (NEW)
```

### Prerequisites
1. Install **Unity IAP** package: Window > Package Manager > Unity IAP
2. Enable In-App Purchasing: Services > In-App Purchasing > Enable

### Scene Setup
1. Open **MainMenu** scene
2. Go to **WheelGame > Iteration 8 - Setup Shop (MainMenu scene)**
3. Click **"Setup Shop (Iteration 8)"**
4. Save the scene

### Manual Wiring (IMPORTANT — after running editor script)

The editor script creates the UI but you need to manually add the IAP Button:

1. **Select BuyButton** (inside ShopPanel > PanelBg > BuyButton)
2. **Add Component** → search for **IAP Button** (from Unity Purchasing)
3. Set **Product ID**: `com.wheelgame.boosterpack`
4. Set **Product Type**: Consumable
5. In IAP Button's events, wire:
   - **On Purchase Complete** → drag ShopPanel object → `IAPManager.OnPurchaseComplete`
   - **On Purchase Failed** → drag ShopPanel object → `IAPManager.OnPurchaseFailed`
   - **On Product Fetched** → drag ShopPanel object → `IAPManager.OnProductFetched`
6. On the BuyButton's regular **OnClick** event, add:
   - drag ShopPanel object → `IAPManager.OnBuyClicked`

### IAP Catalog Setup
1. **Window > Unity IAP > IAP Catalog**
2. Add product:
   - **ID**: `com.wheelgame.boosterpack`
   - **Type**: Consumable
   - **Title**: Booster Pack
   - **Description**: +1 Undo, +1 Slowmo, +1 Extra Life

### App Store Connect Setup (for iOS)
1. In App Store Connect, create an In-App Purchase:
   - Type: Consumable
   - Product ID: `com.wheelgame.boosterpack`
   - Price: choose your tier
2. Submit for review along with your app

## How to Test

### Editor Testing (without real IAP):
1. MainMenu → click SHOP → shop panel slides in
2. Shows current booster counts
3. Shows buy button with price
4. Click buy → loading overlay appears
5. (In editor, IAP will fail since no real store — failure message shows, loading hides)
6. Close button or tap dim → panel closes

### Device Testing:
1. Build to iOS device
2. Sign in with Sandbox tester account
3. Shop → Buy → Apple payment sheet → Confirm
4. On success: "+1 Undo +1 Slowmo +1 Life" message, counts update
5. On failure: "Purchase failed" message

### Loading Overlay:
- Appears when Buy is clicked (prevents double-tap)
- Disappears on success or failure
- Covers only the buy button area

## UI Structure
```
MainMenuCanvas
  ├── ... (existing elements)
  └── ShopPanel (NEW)
       ├── Dim (tap to close)
       └── PanelBg
            ├── Title "BOOSTER SHOP"
            ├── Subtitle "Get +1 of each booster!"
            ├── UndoRow (icon + label + count)
            ├── SlowmoRow (icon + label + count)
            ├── ExtraLifeRow (icon + label + count)
            ├── PackLabel "Booster Pack"
            ├── PackDesc "+1 Undo • +1 Slowmo • +1 Life"
            ├── BuyButton (← add IAP Button here)
            │    ├── PriceText "$0.99"
            │    └── LoadingOverlay (hidden, blocks clicks)
            │         └── LoadingText "Loading..."
            ├── StatusText (success/error messages)
            └── CloseButton "CLOSE"
```

## Purchase Flow
```
User taps BUY
  → OnBuyClicked() → LoadingOverlay shows
  → IAP Button triggers Apple/Google payment
  
  → Success:
     → OnPurchaseComplete()
     → GameManager.AddBoosterPack() (+1 each)
     → LoadingOverlay hides
     → Status: "+1 Undo +1 Slowmo +1 Life" (green)
     → Counts refresh
  
  → Failure:
     → OnPurchaseFailed()
     → LoadingOverlay hides
     → Status: "Purchase failed" (red)
```
