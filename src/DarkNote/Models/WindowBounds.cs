namespace DarkNote.Models;

/// <summary>
/// Saved window position and size ("WindowState" in docs/data-design.md; renamed in code so it
/// doesn't clash with WPF's System.Windows.WindowState). Device-independent units.
/// </summary>
public sealed class WindowBounds
{
    /// <summary>Left edge, or null when the window hasn't been placed yet (it decides when it opens).</summary>
    public double? Left { get; set; }

    /// <summary>Top edge, or null when the window hasn't been placed yet.</summary>
    public double? Top { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public static WindowBounds OfSize(double width, double height) => new() { Width = width, Height = height };
}
