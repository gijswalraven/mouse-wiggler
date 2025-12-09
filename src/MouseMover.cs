using System.Runtime.InteropServices;

namespace MouseWiggler;

/// <summary>
/// Handles mouse movement using Windows API.
/// Uses SendInput for authentic user input that prevents screen lock.
/// </summary>
public static class MouseMover
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("kernel32.dll")]
    private static extern uint SetThreadExecutionState(uint esFlags);

    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;
    private const uint ES_DISPLAY_REQUIRED = 0x00000002;

    private const int INPUT_MOUSE = 0;
    private const uint MOUSEEVENTF_MOVE = 0x0001;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public int type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// Prevents Windows from sleeping or turning off the display.
    /// Call this periodically to keep the system awake.
    /// </summary>
    public static void PreventSleep()
    {
        SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
    }

    /// <summary>
    /// Allows Windows to sleep normally again.
    /// </summary>
    public static void AllowSleep()
    {
        SetThreadExecutionState(ES_CONTINUOUS);
    }

    /// <summary>
    /// Moves the mouse cursor by a small offset and then back.
    /// Uses SendInput for authentic user input that prevents screen lock.
    /// </summary>
    /// <param name="pixels">Number of pixels to move (default: 1)</param>
    public static void Wiggle(int pixels = 1)
    {
        // Move right using SendInput (recognized as real user input)
        SendMouseMove(pixels, 0);

        // Small delay to register the movement
        Thread.Sleep(50);

        // Move back to original position
        SendMouseMove(-pixels, 0);
    }

    private static void SendMouseMove(int dx, int dy)
    {
        var input = new INPUT
        {
            type = INPUT_MOUSE,
            mi = new MOUSEINPUT
            {
                dx = dx,
                dy = dy,
                mouseData = 0,
                dwFlags = MOUSEEVENTF_MOVE,
                time = 0,
                dwExtraInfo = IntPtr.Zero
            }
        };

        SendInput(1, [input], Marshal.SizeOf<INPUT>());
    }

    /// <summary>
    /// Performs a small circular wiggle pattern.
    /// Uses SendInput for authentic user input that prevents screen lock.
    /// </summary>
    public static void CircleWiggle(int radius = 2)
    {
        // Move in a small square pattern using relative movements
        // Right-down, left-down, left-up, right-up (back to start)
        int[] deltaX = { radius, 0, -radius, 0 };
        int[] deltaY = { 0, radius, 0, -radius };

        foreach (var i in Enumerable.Range(0, deltaX.Length))
        {
            SendMouseMove(deltaX[i], deltaY[i]);
            Thread.Sleep(25);
        }
    }
}
