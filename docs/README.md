# Mouse Wiggler

A Windows system tray application that prevents screen lock by periodically moving the mouse cursor.

> **Security Warning**: Preventing your screen from locking leaves your computer vulnerable to unauthorized access. Only use this tool when you are physically present and can monitor your workstation. Never use it in shared or public environments. Your organization's security policies may prohibit disabling screen lock.

## Features

- **System Tray Icon**: Lives in the Windows notification area with visual status indicator
- **Toggle On/Off**: Double-click the tray icon or use the context menu
- **Configurable Interval**: Choose between 10s, 30s, 1m, 2m, or 5m
- **Movement Patterns**: Simple (back-and-forth) or Circle pattern
- **Visual Feedback**: Green icon when active, red X icon when paused
- **Single File**: Distributable as a single 166 KB executable

## Usage

1. Run `MouseWiggler.exe`
2. The application appears in the system tray (notification area)
3. **Double-click** the icon to toggle enable/disable
4. **Right-click** for options menu:
   - Enable/Disable toggle
   - Interval selection
   - Movement pattern selection
   - Exit

## System Requirements

- Windows 10/11 (x64)
- .NET 8.0 Desktop Runtime

## Download

Get the latest `MouseWiggler.exe` from the `publish` folder (~166 KB).

## Building from Source

```bash
# Build and run for development
dotnet run --project src/MouseWiggler.csproj

# Publish single-file release (framework-dependent, ~166 KB)
dotnet publish src/MouseWiggler.csproj -c Release -r win-x64 -o publish
```
