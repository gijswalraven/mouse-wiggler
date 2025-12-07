using System.Reflection;

namespace MouseWiggler;

public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly WigglerSettings _settings;
    private readonly Icon _activeIcon;
    private readonly Icon _inactiveIcon;
    private bool _enabled;

    public TrayApplicationContext(bool startDisabled = false)
    {
        _settings = new WigglerSettings();
        _enabled = startDisabled ? false : _settings.Enabled;

        _activeIcon = LoadEmbeddedIcon("MouseWiggler.mouse-active.ico");
        _inactiveIcon = LoadEmbeddedIcon("MouseWiggler.mouse-inactive.ico");

        _trayIcon = new NotifyIcon
        {
            Icon = _enabled ? _activeIcon : _inactiveIcon,
            Text = "Mouse Wiggler",
            Visible = true,
            ContextMenuStrip = CreateContextMenu()
        };

        _trayIcon.DoubleClick += (s, e) => ToggleEnabled();

        _timer = new System.Windows.Forms.Timer
        {
            Interval = _settings.IntervalSeconds * 1000
        };
        _timer.Tick += Timer_Tick;

        if (_enabled)
        {
            _timer.Start();
        }

        UpdateTrayIcon();

        // Update existing startup shortcut to ensure it has --disabled flag
        UpdateStartupShortcutIfExists();
    }

    private static Icon LoadEmbeddedIcon(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            return new Icon(stream);
        }
        return SystemIcons.Application;
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();

        var enableItem = new ToolStripMenuItem("Enabled", null, (s, e) => ToggleEnabled())
        {
            Checked = _enabled,
            Name = "enableItem"
        };
        menu.Items.Add(enableItem);

        menu.Items.Add(new ToolStripSeparator());

        var intervalMenu = new ToolStripMenuItem("Interval");
        foreach (var seconds in new[] { 10, 30, 60, 120, 300 })
        {
            var label = seconds < 60 ? $"{seconds}s" : $"{seconds / 60}m";
            var item = new ToolStripMenuItem(label, null, (s, e) => SetInterval(seconds))
            {
                Checked = _settings.IntervalSeconds == seconds
            };
            intervalMenu.DropDownItems.Add(item);
        }
        menu.Items.Add(intervalMenu);

        var patternMenu = new ToolStripMenuItem("Pattern");
        patternMenu.DropDownItems.Add(new ToolStripMenuItem("Simple", null, (s, e) => SetPattern(false))
        {
            Checked = !_settings.UseCirclePattern
        });
        patternMenu.DropDownItems.Add(new ToolStripMenuItem("Circle", null, (s, e) => SetPattern(true))
        {
            Checked = _settings.UseCirclePattern
        });
        menu.Items.Add(patternMenu);

        menu.Items.Add(new ToolStripSeparator());

        var startupItem = new ToolStripMenuItem("Start with Windows", null, (s, e) => ToggleStartup())
        {
            Checked = IsInStartup(),
            Name = "startupItem"
        };
        menu.Items.Add(startupItem);

        menu.Items.Add(new ToolStripSeparator());

        menu.Items.Add(new ToolStripMenuItem("About", null, (s, e) => ShowAbout()));
        menu.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) => Exit()));

        return menu;
    }

    private static void ShowAbout()
    {
        var result = MessageBox.Show(
            "Mouse Wiggler\n\n" +
            "Prevents screen lock by periodically moving the mouse cursor.\n\n" +
            "GitHub: github.com/gijswalraven/mouse-wiggler\n\n" +
            "Click OK to open the repository.",
            "About Mouse Wiggler",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Information);

        if (result == DialogResult.OK)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/gijswalraven/mouse-wiggler",
                UseShellExecute = true
            });
        }
    }

    private static string StartupShortcutPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            "MouseWiggler.lnk");

    private static bool IsInStartup() => File.Exists(StartupShortcutPath);

    private void ToggleStartup()
    {
        if (IsInStartup())
        {
            File.Delete(StartupShortcutPath);
        }
        else
        {
            CreateShortcut(StartupShortcutPath, Application.ExecutablePath);
        }
        UpdateStartupMenu();
    }

    /// <summary>
    /// Recreates the startup shortcut if it exists (to update arguments).
    /// Call this after updating the app to ensure --disabled flag is present.
    /// </summary>
    private static void UpdateStartupShortcutIfExists()
    {
        if (File.Exists(StartupShortcutPath))
        {
            File.Delete(StartupShortcutPath);
            CreateShortcut(StartupShortcutPath, Application.ExecutablePath);
        }
    }

    private void UpdateStartupMenu()
    {
        if (_trayIcon.ContextMenuStrip?.Items["startupItem"] is ToolStripMenuItem item)
        {
            item.Checked = IsInStartup();
        }
    }

    private static void CreateShortcut(string shortcutPath, string targetPath)
    {
        // Use PowerShell to create shortcut (avoids COM interop issues in .NET Core)
        // Start with --disabled flag so it doesn't wiggle until user enables it
        var script = $@"
            $ws = New-Object -ComObject WScript.Shell
            $s = $ws.CreateShortcut('{shortcutPath}')
            $s.TargetPath = '{targetPath}'
            $s.Arguments = '--disabled'
            $s.WorkingDirectory = '{Path.GetDirectoryName(targetPath)}'
            $s.Description = 'Mouse Wiggler'
            $s.Save()
        ";

        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -Command \"{script.Replace("\"", "\\\"")}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(psi);
        process?.WaitForExit();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_enabled)
        {
            if (_settings.UseCirclePattern)
            {
                MouseMover.CircleWiggle(_settings.PixelDistance);
            }
            else
            {
                MouseMover.Wiggle(_settings.PixelDistance);
            }
        }
    }

    private void ToggleEnabled()
    {
        _enabled = !_enabled;

        if (_enabled)
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }

        UpdateTrayIcon();
        UpdateMenuChecked();
    }

    private void SetInterval(int seconds)
    {
        _settings.IntervalSeconds = seconds;
        _timer.Interval = seconds * 1000;
        UpdateIntervalMenu();
    }

    private void SetPattern(bool useCircle)
    {
        _settings.UseCirclePattern = useCircle;
        UpdatePatternMenu();
    }

    private void UpdateTrayIcon()
    {
        _trayIcon.Icon = _enabled ? _activeIcon : _inactiveIcon;
        _trayIcon.Text = _enabled ? "Mouse Wiggler (Active)" : "Mouse Wiggler (Paused)";
    }

    private void UpdateMenuChecked()
    {
        if (_trayIcon.ContextMenuStrip?.Items["enableItem"] is ToolStripMenuItem item)
        {
            item.Checked = _enabled;
        }
    }

    private void UpdateIntervalMenu()
    {
        if (_trayIcon.ContextMenuStrip?.Items[2] is ToolStripMenuItem intervalMenu)
        {
            foreach (ToolStripMenuItem item in intervalMenu.DropDownItems)
            {
                var seconds = item.Text switch
                {
                    "10s" => 10,
                    "30s" => 30,
                    "60s" or "1m" => 60,
                    "2m" => 120,
                    "5m" => 300,
                    _ => 0
                };
                item.Checked = _settings.IntervalSeconds == seconds;
            }
        }
    }

    private void UpdatePatternMenu()
    {
        if (_trayIcon.ContextMenuStrip?.Items[3] is ToolStripMenuItem patternMenu)
        {
            if (patternMenu.DropDownItems[0] is ToolStripMenuItem simple)
                simple.Checked = !_settings.UseCirclePattern;
            if (patternMenu.DropDownItems[1] is ToolStripMenuItem circle)
                circle.Checked = _settings.UseCirclePattern;
        }
    }

    private void Exit()
    {
        _timer.Stop();
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        Application.Exit();
    }
}
