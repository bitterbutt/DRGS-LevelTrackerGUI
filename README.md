# DRGS Level Tracker GUI

A BepInEx IL2CPP plugin for Deep Rock Galactic: Survivor that overlays a compact, dynamically updating resource/block tracker on the screen. The GUI replaces prior console output and can be toggled in-game.

Original resource tracking plugin by [potatodock](https://www.nexusmods.com/deeprockgalacticsurvivor/mods/12).

## Features

- Toggle GUI with in-game "Rock & Stone" input
- Center-left overlay with compact rows: `BLOCKTYPE: COUNT | TOTAL`
- Dynamic width/height based on content; absolute-position IMGUI (no GUILayout)
- Persists window position when dragged; configurable open-at-start behavior
- Reads live data from the plugin's `BlockTracker`

## Building

```powershell
dotnet restore LevelTrackerGUI
dotnet build LevelTrackerGUI
```

- Output: `LevelTrackerGUI.dll`
- Target: copy to `<GameRoot>\BepInEx\plugins\`

## Installation

1. Ensure the game is set up with BepInEx IL2CPP.
2. Copy `LevelTrackerGUI.dll` to `<GameRoot>\BepInEx\plugins\`.
3. Start the game; the plugin loads automatically.

Note: If rebuilding while the game is running, the DLL may be locked. Close the game before copying/replacing the DLL.

## Usage

- Toggle: Use the game's "Rock & Stone" input to show/hide the GUI.
- Position: Drag the window to your preferred location; position persists.
- Display: Rows show block/resource type name on the left, and right-aligned quantities (count and total) on the right.

## Configuration

Config entries (via BepInEx config) control startup behavior and window position:

- `General.OpenAtStart`: Start with the GUI visible (true/false).
- `Window.PosX` / `Window.PosY`: Persisted window position when dragged; defaults to center-left with vertical centering if unset.

## Technical Details

- Harmony patches hook gameplay events to populate and refresh data (`OnExitDropPod`, `OnEnterDropPod`, and the Rock & Stone input for toggling).
- Unity IMGUI is used with `GUI.Window` / `GUI.Label` and absolute coordinates to avoid IL2CPP stripping of `GUILayout` APIs.
- Data source: `BlockTracker.List` exposed for the GUI to render; console logging is minimized/disabled for the overlay.
- Formatting: Name padding via `Utils.Extend()`; quantities right-justified for consistent alignment.

## Development Notes

- Built as a single BepInEx IL2CPP plugin under `LevelTrackerGUI/`.
- Avoid IMGUI layout helpers that may be stripped under IL2CPP (e.g., `GUILayout.BeginScrollView`, `GUILayout.Space`).
- Window sizing is computed from content lengths and row heights without runtime IMGUI measurements.
