# Architecture

## Overview

Mouse Wiggler is a Windows Forms application that runs entirely in the system tray without a main window. It publishes as a single ~166 KB executable with embedded icons.

## Components

### Program.cs
Entry point that initializes Windows Forms and runs the `TrayApplicationContext`.

### TrayApplicationContext.cs
Main application logic:
- Creates and manages the `NotifyIcon` (system tray icon)
- Handles context menu creation and events
- Manages the wiggle timer
- Loads icons from embedded resources
- Switches icons based on enabled/disabled state

### MouseMover.cs
Static class that performs mouse cursor movement using Windows API:
- `GetCursorPos` - Gets current cursor position
- `SetCursorPos` - Sets cursor position
- `Wiggle()` - Simple back-and-forth movement
- `CircleWiggle()` - Square pattern movement

### WigglerSettings.cs
Configuration class with default values:
- `IntervalSeconds` (default: 30)
- `PixelDistance` (default: 1)
- `UseCirclePattern` (default: false)
- `Enabled` (default: true)

## Data Flow

```
Timer Tick
    |
    v
WigglerSettings (check Enabled)
    |
    v
MouseMover.Wiggle() or CircleWiggle()
    |
    v
Windows API (user32.dll)
    |
    v
Cursor moves and returns
```

## Build Configuration

The project uses these key settings for single-file publishing:
- `PublishSingleFile=true` - Bundles DLL into EXE
- `SelfContained=false` - Framework-dependent (small size)
- Icons embedded as `EmbeddedResource` (no external .ico files)
- `GenerateDependencyFile=false` - No .deps.json needed

## Icons

Icons are embedded in the assembly and loaded via `Assembly.GetManifestResourceStream()`:
- `MouseWiggler.mouse-active.ico` - White cursor with green circle (enabled)
- `MouseWiggler.mouse-inactive.ico` - Gray cursor with red X (disabled)
