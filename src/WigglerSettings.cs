namespace MouseWiggler;

/// <summary>
/// Configuration settings for the mouse wiggler service.
/// </summary>
public class WigglerSettings
{
    public const string SectionName = "Wiggler";

    /// <summary>
    /// Interval between mouse movements in seconds.
    /// Default: 30 seconds (well under typical 1-5 minute lock timeouts).
    /// </summary>
    public int IntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Number of pixels to move the mouse.
    /// </summary>
    public int PixelDistance { get; set; } = 4;

    /// <summary>
    /// Whether to use circle wiggle pattern instead of simple back-and-forth.
    /// </summary>
    public bool UseCirclePattern { get; set; } = false;

    /// <summary>
    /// Whether the service is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Whether to automatically pause when running on battery power.
    /// </summary>
    public bool PauseOnBattery { get; set; } = true;
}
