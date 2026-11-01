using System.Text.Json;
using DarkNote.Models;
using DarkNote.Storage;

namespace DarkNote.Tests.Storage;

/// <summary>
/// The JSON file format from docs/data-design.md: camelCase names, enums as readable strings,
/// unknown fields ignored, missing fields default, unknown enum values fall back to the default.
/// </summary>
public class StorageJsonTests
{
    [Fact]
    public void Note_IsWrittenInDataDesignFormat()
    {
        var note = new NoteData
        {
            Id = "3f2b8c1e-5a7d-4e2f-9b1c-0d4e6f8a9b2c",
            Name = "Shopping",
            Color = NoteColor.Purple,
            Icon = NoteIcon.BotMessageSquare,
            Font = NoteFont.JetBrainsMono,
            FontSize = FontSizePreset.Large,
            CreatedAt = new DateTime(2026, 11, 1, 10, 15, 0, DateTimeKind.Utc),
        };

        var json = JsonSerializer.Serialize(note, StorageJson.Options);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32());
        Assert.Equal("Shopping", root.GetProperty("name").GetString());
        Assert.Equal("Purple", root.GetProperty("color").GetString());
        Assert.Equal("bot-message-square", root.GetProperty("icon").GetString()); // Lucide name
        Assert.Equal("JetBrainsMono", root.GetProperty("font").GetString());
        Assert.Equal("Large", root.GetProperty("fontSize").GetString());
        Assert.Equal("2026-11-01T10:15:00Z", root.GetProperty("createdAt").GetString());
        Assert.True(root.TryGetProperty("window", out _));
    }

    [Fact]
    public void Note_RoundTrips()
    {
        var note = new NoteData
        {
            Id = "abc",
            Name = "Ideas",
            IsNameManual = true,
            Content = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />",
            Color = NoteColor.Blue,
            Icon = NoteIcon.Lightbulb,
            Font = NoteFont.Caveat,
            FontSize = FontSizePreset.Small,
            IsPinned = true,
            Opacity = 60,
            LastOpacity = 40,
            IsOpen = false,
            Window = new WindowBounds { Left = 520, Top = 140, Width = 320, Height = 280 },
            CreatedAt = new DateTime(2026, 11, 1, 10, 15, 0, DateTimeKind.Utc),
            LastEdited = new DateTime(2026, 11, 1, 10, 20, 31, DateTimeKind.Utc),
        };

        var copy = JsonSerializer.Deserialize<NoteData>(JsonSerializer.Serialize(note, StorageJson.Options), StorageJson.Options)!;

        Assert.Equivalent(note, copy, strict: true);
    }

    [Fact]
    public void Note_MissingFields_GetDefaults()
    {
        var note = JsonSerializer.Deserialize<NoteData>("""{ "name": "Only a name" }""", StorageJson.Options)!;

        Assert.Equal("Only a name", note.Name);
        Assert.Equal(NoteColor.Yellow, note.Color);
        Assert.Equal(NoteIcon.Notebook, note.Icon);
        Assert.Equal(NoteFont.SegoeUI, note.Font);
        Assert.Equal(FontSizePreset.Medium, note.FontSize);
        Assert.Equal(100, note.Opacity);
        Assert.Equal(80, note.LastOpacity);
        Assert.False(note.IsPinned);
    }

    [Fact]
    public void Note_UnknownFields_AreIgnored()
    {
        var note = JsonSerializer.Deserialize<NoteData>(
            """{ "name": "Future", "someNewField": 42, "nested": { "a": [1, 2] } }""", StorageJson.Options)!;

        Assert.Equal("Future", note.Name);
    }

    [Theory]
    [InlineData("""{ "color": "DarkGray" }""")]   // reserved for the Clipboard Note
    [InlineData("""{ "color": "Teal" }""")]       // never existed
    [InlineData("""{ "color": 3 }""")]            // number instead of a name
    [InlineData("""{ "color": null }""")]
    public void Note_UnknownColor_BecomesYellow(string json)
    {
        var note = JsonSerializer.Deserialize<NoteData>(json, StorageJson.Options)!;

        Assert.Equal(NoteColor.Yellow, note.Color);
    }

    [Fact]
    public void Note_UnknownIconFontAndSize_BecomeDefaults()
    {
        var note = JsonSerializer.Deserialize<NoteData>(
            """{ "icon": "copy", "font": "Comic Sans", "fontSize": "Huge" }""", StorageJson.Options)!;

        Assert.Equal(NoteIcon.Notebook, note.Icon); // "copy" belongs to the Clipboard Note only
        Assert.Equal(NoteFont.SegoeUI, note.Font);
        Assert.Equal(FontSizePreset.Medium, note.FontSize);
    }

    [Fact]
    public void EnumNames_AreReadCaseInsensitively()
    {
        var note = JsonSerializer.Deserialize<NoteData>(
            """{ "color": "purple", "icon": "Mail-Open", "font": "caveat" }""", StorageJson.Options)!;

        Assert.Equal(NoteColor.Purple, note.Color);
        Assert.Equal(NoteIcon.MailOpen, note.Icon);
        Assert.Equal(NoteFont.Caveat, note.Font);
    }

    [Fact]
    public void Settings_RecentIcons_UnknownNamesDropped()
    {
        var settings = JsonSerializer.Deserialize<AppSettings>(
            """{ "recentIcons": ["calendar", "not-an-icon", "copy", 7, null, "notebook"] }""", StorageJson.Options)!;

        Assert.Equal([NoteIcon.Calendar, NoteIcon.Notebook], settings.RecentIcons);
    }

    [Fact]
    public void Settings_SortOption_UnknownBecomesLastEdited()
    {
        var settings = JsonSerializer.Deserialize<AppSettings>("""{ "sortOption": "Random" }""", StorageJson.Options)!;

        Assert.Equal(SortOption.LastEdited, settings.SortOption);
    }

    [Fact]
    public void Settings_DefaultsMatchDataDesign()
    {
        var settings = new AppSettings();

        Assert.Equal(1, settings.SchemaVersion);
        Assert.Equal(SortOption.LastEdited, settings.SortOption);
        Assert.Equal(10, settings.ClipboardHistorySize);
        Assert.Empty(settings.RecentIcons);
        Assert.False(settings.TrayHintShown);
        Assert.Equal(400, settings.MainWindow.Width);
        Assert.Equal(600, settings.MainWindow.Height);
    }

    [Fact]
    public void Clipboard_DefaultsMatchDataDesign()
    {
        var clipboard = new ClipboardNoteData();

        Assert.Empty(clipboard.Items);
        Assert.False(clipboard.IsOpen);
        Assert.False(clipboard.IsPinned);
        Assert.Equal(100, clipboard.Opacity);
        Assert.Equal(80, clipboard.LastOpacity);
        Assert.Equal(300, clipboard.Window.Width);
        Assert.Equal(400, clipboard.Window.Height);
    }

    [Fact]
    public void NoteIcon_HasThe21PresetIconsInPrdOrder()
    {
        string[] expected =
        [
            "info", "calendar", "lightbulb", "lock", "database", "folder", "gift", "bot-message-square",
            "code-xml", "notebook", "cat", "mail-open", "fish", "book", "bookmark", "book-text",
            "list", "link", "play", "store", "languages",
        ];

        var names = Enum.GetValues<NoteIcon>()
            .Select(icon => JsonSerializer.Serialize(icon, StorageJson.Options).Trim('"'));

        Assert.Equal(expected, names);
    }
}
