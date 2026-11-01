namespace DarkNote.Models;

/// <summary>
/// The Clipboard Note, saved in clipboard.json. The first "special note" (PRD Part 5): its name,
/// color and icon are fixed in code; it shares window, pin and opacity fields with regular notes.
/// </summary>
public sealed class ClipboardNoteData
{
    public int SchemaVersion { get; set; } = DataLimits.SchemaVersion;

    /// <summary>Newest first.</summary>
    public List<ClipboardItem> Items { get; set; } = [];

    public bool IsPinned { get; set; }

    public int Opacity { get; set; } = DataLimits.DefaultOpacity;

    public int LastOpacity { get; set; } = DataLimits.DefaultLastOpacity;

    public bool IsOpen { get; set; }

    public WindowBounds Window { get; set; } = WindowBounds.OfSize(DataLimits.ClipboardDefaultWidth, DataLimits.ClipboardDefaultHeight);
}

/// <summary>One captured clipboard text.</summary>
public sealed class ClipboardItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Text { get; set; } = string.Empty;

    /// <summary>UTC time it was captured.</summary>
    public DateTime CopiedAt { get; set; }
}
