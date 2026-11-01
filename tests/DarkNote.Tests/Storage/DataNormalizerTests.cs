using DarkNote.Models;
using DarkNote.Storage;

namespace DarkNote.Tests.Storage;

/// <summary>
/// Edge cases 6, 7 and 8 of workflow 2.4.3: missing or invalid values are replaced on load
/// with the nearest valid value or the default from docs/data-design.md.
/// </summary>
public class DataNormalizerTests
{
    private static readonly DateTime FileCreated = new(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);

    // ---------- Note name ----------

    [Theory]
    [InlineData("Shopping", "Shopping")]
    [InlineData("123456789012345678901234567890", "123456789012345678901234567890")] // exactly 30: kept
    [InlineData("1234567890123456789012345678901", "123456789012345678901234567...")] // 31: cut to 27 + "..."
    public void FitName_KeepsUpTo30Characters_ElseCutsTo27PlusEllipsis(string input, string expected)
    {
        Assert.Equal(expected, DataNormalizer.FitName(input));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FitName_MissingName_BecomesNew(string? input)
    {
        Assert.Equal("New", DataNormalizer.FitName(input));
    }

    // ---------- Regular note ----------

    [Fact]
    public void Note_IdFromFileName_WinsOverIdInFile()
    {
        var note = new NoteData { Id = "some-other-id" };

        DataNormalizer.Normalize(note, idFromFileName: "3f2b8c1e-5a7d-4e2f-9b1c-0d4e6f8a9b2c", FileCreated);

        Assert.Equal("3f2b8c1e-5a7d-4e2f-9b1c-0d4e6f8a9b2c", note.Id);
    }

    [Theory]
    [InlineData(5, 20)]     // below range → nearest valid
    [InlineData(19, 20)]
    [InlineData(20, 20)]
    [InlineData(55, 55)]
    [InlineData(100, 100)]
    [InlineData(150, 100)]  // above range → nearest valid
    [InlineData(-3, 20)]
    public void Note_OpacityAndLastOpacity_AreClampedTo20To100(int input, int expected)
    {
        var note = new NoteData { Opacity = input, LastOpacity = input };

        DataNormalizer.Normalize(note, "id", FileCreated);

        Assert.Equal(expected, note.Opacity);
        Assert.Equal(expected, note.LastOpacity);
    }

    [Fact]
    public void Note_LongName_IsCut()
    {
        var note = new NoteData { Name = new string('a', 45) };

        DataNormalizer.Normalize(note, "id", FileCreated);

        Assert.Equal(new string('a', 27) + "...", note.Name);
    }

    [Fact]
    public void Note_NullContent_BecomesEmpty()
    {
        var note = new NoteData { Content = null! };

        DataNormalizer.Normalize(note, "id", FileCreated);

        Assert.Equal(string.Empty, note.Content);
    }

    [Fact]
    public void Note_MissingWindow_GetsNoteDefaultSize()
    {
        var note = new NoteData { Window = null! };

        DataNormalizer.Normalize(note, "id", FileCreated);

        Assert.NotNull(note.Window);
        Assert.Equal(300, note.Window.Width);
        Assert.Equal(300, note.Window.Height);
        Assert.Null(note.Window.Left); // not placed yet: the window decides when it opens
        Assert.Null(note.Window.Top);
    }

    [Theory]
    [InlineData(100, 100, 200, 150)]     // too small → minimum 200 x 150
    [InlineData(250, 180, 250, 180)]     // valid → kept
    [InlineData(0, -5, 300, 300)]        // zero / negative → default
    [InlineData(double.NaN, double.PositiveInfinity, 300, 300)] // not a number → default
    public void Note_WindowSize_IsFixed(double width, double height, double expectedWidth, double expectedHeight)
    {
        var note = new NoteData { Window = new WindowBounds { Left = 10, Top = 20, Width = width, Height = height } };

        DataNormalizer.Normalize(note, "id", FileCreated);

        Assert.Equal(expectedWidth, note.Window.Width);
        Assert.Equal(expectedHeight, note.Window.Height);
        Assert.Equal(10, note.Window.Left);
        Assert.Equal(20, note.Window.Top);
    }

    [Fact]
    public void Note_NotANumberPosition_BecomesNotPlaced()
    {
        var note = new NoteData { Window = new WindowBounds { Left = double.NaN, Top = double.NegativeInfinity, Width = 300, Height = 300 } };

        DataNormalizer.Normalize(note, "id", FileCreated);

        Assert.Null(note.Window.Left);
        Assert.Null(note.Window.Top);
    }

    [Fact]
    public void Note_MissingDates_UseFileCreationTime_ThenCreatedAt()
    {
        var note = new NoteData { CreatedAt = default, LastEdited = default };

        DataNormalizer.Normalize(note, "id", FileCreated);

        Assert.Equal(FileCreated, note.CreatedAt);
        Assert.Equal(FileCreated, note.LastEdited);
    }

    [Fact]
    public void Note_MissingLastEdited_UsesCreatedAt()
    {
        var created = new DateTime(2026, 10, 5, 8, 0, 0, DateTimeKind.Utc);
        var note = new NoteData { CreatedAt = created, LastEdited = default };

        DataNormalizer.Normalize(note, "id", FileCreated);

        Assert.Equal(created, note.CreatedAt);
        Assert.Equal(created, note.LastEdited);
    }

    // ---------- Settings ----------

    [Theory]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(20, 20)]
    [InlineData(0, 10)]
    [InlineData(7, 10)]
    [InlineData(100, 10)]
    public void Settings_ClipboardHistorySize_MustBe5Or10Or20(int input, int expected)
    {
        var settings = new AppSettings { ClipboardHistorySize = input };

        DataNormalizer.Normalize(settings);

        Assert.Equal(expected, settings.ClipboardHistorySize);
    }

    [Fact]
    public void Settings_RecentIcons_DuplicatesRemoved_OrderKept()
    {
        var settings = new AppSettings { RecentIcons = [NoteIcon.Calendar, NoteIcon.Notebook, NoteIcon.Calendar, NoteIcon.Cat] };

        DataNormalizer.Normalize(settings);

        Assert.Equal([NoteIcon.Calendar, NoteIcon.Notebook, NoteIcon.Cat], settings.RecentIcons);
    }

    [Fact]
    public void Settings_MissingParts_GetDefaults()
    {
        var settings = new AppSettings { MainWindow = null!, RecentIcons = null! };

        DataNormalizer.Normalize(settings);

        Assert.Equal(400, settings.MainWindow.Width);
        Assert.Equal(600, settings.MainWindow.Height);
        Assert.Null(settings.MainWindow.Left); // centered when the main page opens
        Assert.Empty(settings.RecentIcons);
    }

    [Fact]
    public void Settings_MainWindowTooSmall_GetsMainPageMinimum()
    {
        var settings = new AppSettings { MainWindow = new WindowBounds { Width = 100, Height = 100 } };

        DataNormalizer.Normalize(settings);

        Assert.Equal(320, settings.MainWindow.Width);
        Assert.Equal(400, settings.MainWindow.Height);
    }

    // ---------- Clipboard Note ----------

    [Fact]
    public void Clipboard_Items_EmptyRemoved_NewestFirst_TrimmedToHistorySize()
    {
        var t = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);
        var clipboard = new ClipboardNoteData
        {
            Items =
            [
                new ClipboardItem { Id = "a", Text = "old", CopiedAt = t },
                new ClipboardItem { Id = "b", Text = "", CopiedAt = t.AddMinutes(9) },
                new ClipboardItem { Id = "c", Text = "newest", CopiedAt = t.AddMinutes(5) },
                new ClipboardItem { Id = "d", Text = "middle", CopiedAt = t.AddMinutes(3) },
                new ClipboardItem { Id = "e", Text = null!, CopiedAt = t.AddMinutes(8) },
            ],
        };

        DataNormalizer.Normalize(clipboard, historySize: 2);

        Assert.Equal(["c", "d"], clipboard.Items.Select(i => i.Id));
    }

    [Fact]
    public void Clipboard_ItemTextOver100000Characters_IsCut()
    {
        var clipboard = new ClipboardNoteData
        {
            Items = [new ClipboardItem { Id = "a", Text = new string('x', 100_005), CopiedAt = DateTime.UtcNow }],
        };

        DataNormalizer.Normalize(clipboard, historySize: 10);

        Assert.Equal(100_000, clipboard.Items[0].Text.Length);
    }

    [Fact]
    public void Clipboard_ItemWithoutId_GetsNewId()
    {
        var clipboard = new ClipboardNoteData
        {
            Items = [new ClipboardItem { Id = null!, Text = "hello", CopiedAt = DateTime.UtcNow }],
        };

        DataNormalizer.Normalize(clipboard, historySize: 10);

        Assert.True(Guid.TryParse(clipboard.Items[0].Id, out _));
    }

    [Fact]
    public void Clipboard_MissingParts_GetDefaults_AndOpacityClamped()
    {
        var clipboard = new ClipboardNoteData { Items = null!, Window = null!, Opacity = 0, LastOpacity = 500 };

        DataNormalizer.Normalize(clipboard, historySize: 10);

        Assert.Empty(clipboard.Items);
        Assert.Equal(300, clipboard.Window.Width);
        Assert.Equal(400, clipboard.Window.Height);
        Assert.Equal(20, clipboard.Opacity);
        Assert.Equal(100, clipboard.LastOpacity);
    }
}
