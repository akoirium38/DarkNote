namespace DarkNote.Models;

/// <summary>One regular note, saved in notes\&lt;id&gt;.json (docs/data-design.md).</summary>
public sealed class NoteData
{
    public int SchemaVersion { get; set; } = DataLimits.SchemaVersion;

    /// <summary>Note id (a GUID string). On load, the file name wins over the value in the file.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = DataLimits.DefaultNoteName;

    public bool IsNameManual { get; set; }

    /// <summary>FlowDocument XAML; empty means an empty document.</summary>
    public string Content { get; set; } = string.Empty;

    public NoteColor Color { get; set; } = NoteColor.Yellow;

    public NoteIcon Icon { get; set; } = NoteIcon.Notebook;

    public NoteFont Font { get; set; } = NoteFont.SegoeUI;

    public FontSizePreset FontSize { get; set; } = FontSizePreset.Medium;

    public bool IsPinned { get; set; }

    /// <summary>Body opacity in percent, 20–100.</summary>
    public int Opacity { get; set; } = DataLimits.DefaultOpacity;

    /// <summary>Opacity to restore when transparency is toggled back on, 20–100.</summary>
    public int LastOpacity { get; set; } = DataLimits.DefaultLastOpacity;

    public bool IsOpen { get; set; } = true;

    public WindowBounds Window { get; set; } = WindowBounds.OfSize(DataLimits.NoteDefaultWidth, DataLimits.NoteDefaultHeight);

    /// <summary>UTC. Left unset (default) when missing from the file; fixed on load. Set by Create New Note (2.2.1).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC. Left unset (default) when missing from the file; fixed on load.</summary>
    public DateTime LastEdited { get; set; }
}
