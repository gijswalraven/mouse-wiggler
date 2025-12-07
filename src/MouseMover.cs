using System.Runtime.InteropServices;

namespace MouseWiggler;

/// <summary>
/// Handles mouse movement using Windows API.
/// </summary>
public static class MouseMover
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// Moves the mouse cursor by a small offset and then back.
    /// This subtle movement prevents screen lock without being disruptive.
    /// </summary>
    /// <param name="pixels">Number of pixels to move (default: 1)</param>
    public static void Wiggle(int pixels = 1)
    {
        if (GetCursorPos(out POINT currentPos))
        {
            // Move right
            SetCursorPos(currentPos.X + pixels, currentPos.Y);

            // Small delay to register the movement
            Thread.Sleep(50);

            // Move back to original position
            SetCursorPos(currentPos.X, currentPos.Y);
        }
    }

    /// <summary>
    /// Performs a small circular wiggle pattern.
    /// </summary>
    public static void CircleWiggle(int radius = 2)
    {
        if (GetCursorPos(out POINT originalPos))
        {
            // Move in a small square pattern and return
            int[] offsetsX = { radius, radius, -radius, -radius, 0 };
            int[] offsetsY = { -radius, radius, radius, -radius, 0 };

            for (int i = 0; i < offsetsX.Length; i++)
            {
                SetCursorPos(originalPos.X + offsetsX[i], originalPos.Y + offsetsY[i]);
                Thread.Sleep(25);
            }
        }
    }
}
