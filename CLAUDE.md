# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run Commands

```bash
# Build and run for development
dotnet run --project src/MouseWiggler.csproj

# Publish single-file release (~169 KB, requires .NET 8 runtime)
dotnet publish src/MouseWiggler.csproj -c Release -r win-x64 -p:DebugType=none -o publish
```

## Project Structure

```
mouse-wiggler/
├── src/
│   ├── MouseWiggler.csproj
│   ├── Program.cs              # Entry point, handles --disabled flag
│   ├── TrayApplicationContext.cs   # Tray icon, menu, timer, startup logic
│   ├── MouseMover.cs           # P/Invoke to user32.dll
│   ├── WigglerSettings.cs      # Runtime configuration
│   ├── FodyWeavers.xml         # Fody configuration
│   ├── mouse-active.ico        # Embedded resource
│   └── mouse-inactive.ico      # Embedded resource
├── docs/
│   ├── README.md
│   └── ARCHITECTURE.md
├── publish/                    # Single-file output (~169 KB)
└── mouse-wiggler.sln
```

## Architecture

Windows Forms system tray app (no main window):
- `TrayApplicationContext` - NotifyIcon, context menu, timer, startup shortcut management
- `MouseMover` - P/Invoke calls to user32.dll (GetCursorPos/SetCursorPos)
- `WigglerSettings` - Runtime config (interval, pattern, enabled)

## Key Build Settings

- `PublishSingleFile=true` - Bundles DLL into EXE
- `SelfContained=false` - Framework-dependent (~169 KB vs ~154 MB)
- Icons as `EmbeddedResource` - Loaded via `Assembly.GetManifestResourceStream()`

## Command Line Arguments

- `--disabled` - Start with wiggling disabled (used by "Start with Windows" shortcut)
