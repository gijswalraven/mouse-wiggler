# Release Notes

## v1.1.0

### New Features

- **Single instance check** - Prevents multiple instances from running simultaneously
- **Battery pause** - Automatically pauses wiggling when on battery power (resumes on AC)
- **Improved screen lock prevention** - Uses SendInput API for more reliable prevention

### Bug Fixes

- Fixed screen lock prevention not working reliably on some systems

### Download

Download `MouseWiggler.exe` (~172 KB).

### Requirements

- Windows 10/11 (x64)
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## v1.0.0 - Initial Release

### Features

- System tray application with visual status indicator (green = active, red = paused)
- Toggle wiggling on/off via double-click or context menu
- Configurable intervals: 10s, 30s, 1m, 2m, 5m
- Movement patterns: Simple (back-and-forth) or Circle
- Start with Windows option (launches in disabled mode for safety)
- About dialog with GitHub link

### Download

Download `MouseWiggler.exe` (~169 KB).

### Requirements

- Windows 10/11 (x64)
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### Security Warning

Preventing your screen from locking leaves your computer vulnerable to unauthorized access. Only use this tool when you are physically present.
