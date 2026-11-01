namespace DarkNote.Models;

/// <summary>Defaults and limits from docs/data-design.md, in one place.</summary>
public static class DataLimits
{
    public const int SchemaVersion = 1;

    public const string DefaultNoteName = "New";
    public const int NameMaxLength = 30;
    public const string NameEllipsis = "...";

    public const int OpacityMin = 20;
    public const int OpacityMax = 100;
    public const int DefaultOpacity = 100;
    public const int DefaultLastOpacity = 80;

    public const int ClipboardTextMaxLength = 100_000;
    public const int DefaultClipboardHistorySize = 10;
    public static readonly IReadOnlyList<int> ClipboardHistorySizes = [5, 10, 20];

    public const double NoteDefaultWidth = 300;
    public const double NoteDefaultHeight = 300;
    public const double NoteMinWidth = 200;
    public const double NoteMinHeight = 150;

    public const double ClipboardDefaultWidth = 300;
    public const double ClipboardDefaultHeight = 400;

    public const double MainDefaultWidth = 400;
    public const double MainDefaultHeight = 600;
    public const double MainMinWidth = 320;
    public const double MainMinHeight = 400;
}
