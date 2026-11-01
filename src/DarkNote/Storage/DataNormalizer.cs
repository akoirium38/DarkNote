using DarkNote.Models;

namespace DarkNote.Storage;

/// <summary>
/// Fixes missing or invalid values after loading (workflow 2.4.3, edge cases 6–8), using the
/// "Validation on load" column of docs/data-design.md. Objects are fixed in place.
/// Off-screen windows are handled later by Remember Window State (2.4.2), which knows the screens.
/// </summary>
public static class DataNormalizer
{
    /// <summary>Missing → "New"; longer than 30 → first 27 characters + "...".</summary>
    public static string FitName(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return DataLimits.DefaultNoteName;
        }

        return name.Length <= DataLimits.NameMaxLength
            ? name
            : name[..(DataLimits.NameMaxLength - DataLimits.NameEllipsis.Length)] + DataLimits.NameEllipsis;
    }

    public static int ClampOpacity(int value) => Math.Clamp(value, DataLimits.OpacityMin, DataLimits.OpacityMax);

    public static void Normalize(AppSettings settings)
    {
        if (!DataLimits.ClipboardHistorySizes.Contains(settings.ClipboardHistorySize))
        {
            settings.ClipboardHistorySize = DataLimits.DefaultClipboardHistorySize;
        }

        settings.RecentIcons = (settings.RecentIcons ?? []).Where(Enum.IsDefined).Distinct().ToList();
        settings.MainWindow = FixWindow(settings.MainWindow,
            DataLimits.MainDefaultWidth, DataLimits.MainDefaultHeight, DataLimits.MainMinWidth, DataLimits.MainMinHeight);
    }

    public static void Normalize(NoteData note, string idFromFileName, DateTime fileCreatedUtc)
    {
        note.Id = idFromFileName;
        note.Name = FitName(note.Name);
        note.Content ??= string.Empty;
        note.Opacity = ClampOpacity(note.Opacity);
        note.LastOpacity = ClampOpacity(note.LastOpacity);
        note.Window = FixWindow(note.Window,
            DataLimits.NoteDefaultWidth, DataLimits.NoteDefaultHeight, DataLimits.NoteMinWidth, DataLimits.NoteMinHeight);

        note.CreatedAt = note.CreatedAt == default ? fileCreatedUtc : ToUtc(note.CreatedAt);
        note.LastEdited = note.LastEdited == default ? note.CreatedAt : ToUtc(note.LastEdited);
    }

    public static void Normalize(ClipboardNoteData clipboard, int historySize)
    {
        clipboard.Items = (clipboard.Items ?? [])
            .Where(item => item is not null && !string.IsNullOrEmpty(item.Text))
            .Select(FixItem)
            .OrderByDescending(item => item.CopiedAt) // stable: equal times keep file order
            .Take(historySize)
            .ToList();

        clipboard.Opacity = ClampOpacity(clipboard.Opacity);
        clipboard.LastOpacity = ClampOpacity(clipboard.LastOpacity);
        clipboard.Window = FixWindow(clipboard.Window,
            DataLimits.ClipboardDefaultWidth, DataLimits.ClipboardDefaultHeight, DataLimits.NoteMinWidth, DataLimits.NoteMinHeight);
    }

    private static ClipboardItem FixItem(ClipboardItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            item.Id = Guid.NewGuid().ToString();
        }

        if (item.Text.Length > DataLimits.ClipboardTextMaxLength)
        {
            item.Text = item.Text[..DataLimits.ClipboardTextMaxLength];
        }

        item.CopiedAt = ToUtc(item.CopiedAt);
        return item;
    }

    private static WindowBounds FixWindow(WindowBounds? window, double defaultWidth, double defaultHeight, double minWidth, double minHeight)
    {
        if (window is null)
        {
            return WindowBounds.OfSize(defaultWidth, defaultHeight);
        }

        window.Width = FixLength(window.Width, defaultWidth, minWidth);
        window.Height = FixLength(window.Height, defaultHeight, minHeight);
        if (window.Left is { } left && !double.IsFinite(left))
        {
            window.Left = null;
        }

        if (window.Top is { } top && !double.IsFinite(top))
        {
            window.Top = null;
        }

        return window;
    }

    private static double FixLength(double value, double defaultValue, double minimum)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            return defaultValue;
        }

        return Math.Max(value, minimum);
    }

    private static DateTime ToUtc(DateTime value) => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value;
}
