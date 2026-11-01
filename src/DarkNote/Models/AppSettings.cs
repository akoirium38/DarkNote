namespace DarkNote.Models;

/// <summary>App-wide settings, saved in settings.json (docs/data-design.md).</summary>
public sealed class AppSettings
{
    public int SchemaVersion { get; set; } = DataLimits.SchemaVersion;

    public SortOption SortOption { get; set; } = SortOption.LastEdited;

    public WindowBounds MainWindow { get; set; } = WindowBounds.OfSize(DataLimits.MainDefaultWidth, DataLimits.MainDefaultHeight);

    public int ClipboardHistorySize { get; set; } = DataLimits.DefaultClipboardHistorySize;

    /// <summary>Most recently chosen icons first (PRD 2.2.5).</summary>
    public List<NoteIcon> RecentIcons { get; set; } = [];

    public bool TrayHintShown { get; set; }
}
