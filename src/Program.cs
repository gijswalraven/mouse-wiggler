using MouseWiggler;

const string mutexName = "MouseWiggler_SingleInstance";

using var mutex = new Mutex(true, mutexName, out bool isNewInstance);

if (!isNewInstance)
{
    // Another instance is already running
    MessageBox.Show(
        "Mouse Wiggler is already running.\n\nCheck the system tray.",
        "Mouse Wiggler",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    return;
}

ApplicationConfiguration.Initialize();

var startDisabled = args.Contains("--disabled");
Application.Run(new TrayApplicationContext(startDisabled));
