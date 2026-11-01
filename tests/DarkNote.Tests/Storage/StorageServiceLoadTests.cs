using DarkNote.Logging;
using DarkNote.Models;
using DarkNote.Storage;
using NSubstitute;

namespace DarkNote.Tests.Storage;

/// <summary>
/// Workflow 2.4.3 "load on start": happy path and edge cases 1–8.
/// </summary>
public sealed class StorageServiceLoadTests : IDisposable
{
    private readonly TempDataFolder _folder = new();
    private readonly IAppLog _log = Substitute.For<IAppLog>();

    private StorageService CreateService() => new(_folder.Path, _log);

    public void Dispose() => _folder.Dispose();

    // ---------- Happy path ----------

    [Fact]
    public void LoadAll_ReadsSettingsClipboardAndEveryNote()
    {
        _folder.Create();
        _folder.Write("settings.json", """{ "schemaVersion": 1, "sortOption": "Name", "clipboardHistorySize": 5 }""");
        _folder.Write("clipboard.json", """{ "items": [ { "id": "a", "text": "hello", "copiedAt": "2026-11-01T10:30:00Z" } ], "isOpen": true }""");
        _folder.Write(@"notes\11111111-1111-1111-1111-111111111111.json", """{ "name": "First", "color": "Red" }""");
        _folder.Write(@"notes\22222222-2222-2222-2222-222222222222.json", """{ "name": "Second", "icon": "cat" }""");

        var data = CreateService().LoadAll();

        Assert.Equal(SortOption.Name, data.Settings.SortOption);
        Assert.Equal(5, data.Settings.ClipboardHistorySize);
        Assert.True(data.Clipboard.IsOpen);
        Assert.Equal("hello", Assert.Single(data.Clipboard.Items).Text);
        Assert.Equal(2, data.Notes.Count);
        Assert.Contains(data.Notes, n => n.Name == "First" && n.Color == NoteColor.Red);
        Assert.Contains(data.Notes, n => n.Name == "Second" && n.Icon == NoteIcon.Cat);
        _log.DidNotReceiveWithAnyArgs().Error(default!, default);
    }

    // ---------- Edge case 1: first run ----------

    [Fact]
    public void LoadAll_FirstRun_CreatesFolderAndNotesSubfolder_AndUsesDefaults()
    {
        Assert.False(Directory.Exists(_folder.Path));

        var data = CreateService().LoadAll();

        Assert.True(Directory.Exists(_folder.Path));
        Assert.True(Directory.Exists(_folder.NotesPath));
        Assert.Equivalent(new AppSettings(), data.Settings);
        Assert.Equivalent(new ClipboardNoteData(), data.Clipboard);
        Assert.Empty(data.Notes);
    }

    [Fact]
    public void LoadAll_FolderExistsButNotesSubfolderMissing_CreatesIt()
    {
        Directory.CreateDirectory(_folder.Path);

        CreateService().LoadAll();

        Assert.True(Directory.Exists(_folder.NotesPath));
    }

    // ---------- Edge case 2: data folder can't be created ----------

    [Fact]
    public void LoadAll_FolderCannotBeCreated_ThrowsDataFolderUnavailable_AndLogs()
    {
        // A regular FILE sits where the parent folder should be, so the data folder can't be created.
        Directory.CreateDirectory(_folder.Root);
        var blocker = Path.Combine(_folder.Root, "blocker");
        File.WriteAllText(blocker, "not a folder");
        var service = new StorageService(Path.Combine(blocker, "DarkNote"), _log);

        Assert.Throws<DataFolderUnavailableException>(() => service.LoadAll());
        _log.ReceivedWithAnyArgs().Error(default!, default);
    }

    // ---------- Edge case 3: damaged settings / clipboard ----------

    [Theory]
    [InlineData("{ this is not json")]
    [InlineData("")]                       // empty file
    [InlineData("null")]                   // valid JSON but no object
    [InlineData("[1, 2, 3]")]              // wrong root type
    [InlineData("""{ "clipboardHistorySize": "ten" }""")] // wrong value type
    public void LoadAll_DamagedSettings_RenamedToCorrupt_AndDefaultsUsed(string damaged)
    {
        _folder.Create();
        _folder.Write("settings.json", damaged);

        var data = CreateService().LoadAll();

        Assert.Equivalent(new AppSettings(), data.Settings);
        Assert.False(_folder.Exists("settings.json"));
        Assert.True(_folder.Exists("settings.json.corrupt"));
        Assert.Equal(damaged, _folder.Read("settings.json.corrupt")); // kept for recovery
        _log.ReceivedWithAnyArgs().Warn(default!);
    }

    [Fact]
    public void LoadAll_DamagedClipboard_RenamedToCorrupt_AndDefaultsUsed()
    {
        _folder.Create();
        _folder.Write("clipboard.json", "{{{");

        var data = CreateService().LoadAll();

        Assert.Equivalent(new ClipboardNoteData(), data.Clipboard);
        Assert.True(_folder.Exists("clipboard.json.corrupt"));
        Assert.False(_folder.Exists("clipboard.json"));
    }

    [Fact]
    public void LoadAll_CorruptCopyAlreadyExists_OlderCopyIsKept()
    {
        _folder.Create();
        _folder.Write("settings.json.corrupt", "first damage");
        _folder.Write("settings.json", "second damage");

        CreateService().LoadAll();

        Assert.Equal("first damage", _folder.Read("settings.json.corrupt"));
        Assert.Equal("second damage", _folder.Read("settings.json.1.corrupt"));
    }

    // ---------- Edge case 4: one damaged note ----------

    [Fact]
    public void LoadAll_OneDamagedNote_IsRenamedAndSkipped_OthersLoad()
    {
        _folder.Create();
        _folder.Write(@"notes\good-1.json", """{ "name": "Good 1" }""");
        _folder.Write(@"notes\bad.json", """{ "name": "Bad", """);
        _folder.Write(@"notes\good-2.json", """{ "name": "Good 2" }""");

        var data = CreateService().LoadAll();

        Assert.Equal(["Good 1", "Good 2"], data.Notes.Select(n => n.Name).Order());
        Assert.True(_folder.Exists(@"notes\bad.json.corrupt"));
        Assert.False(_folder.Exists(@"notes\bad.json"));
        _log.ReceivedWithAnyArgs().Warn(default!);
    }

    [Fact]
    public void LoadAll_OnlyJsonFilesInNotesFolderAreLoaded()
    {
        _folder.Create();
        _folder.Write(@"notes\a.json", """{ "name": "A" }""");
        _folder.Write(@"notes\old.json.corrupt", """{ "name": "Corrupt copy" }""");
        _folder.Write(@"notes\readme.txt", "hello");

        var data = CreateService().LoadAll();

        Assert.Equal("A", Assert.Single(data.Notes).Name);
    }

    // ---------- Edge case 5: leftover .tmp files ----------

    [Fact]
    public void LoadAll_LeftoverTmpFiles_AreDeleted_OriginalsKept()
    {
        _folder.Create();
        _folder.Write("settings.json", """{ "sortOption": "Color" }""");
        _folder.Write("settings.json.tmp", """{ "sortOption": "Na""");
        _folder.Write("clipboard.json.tmp", "half");
        _folder.Write(@"notes\n1.json", """{ "name": "Last good" }""");
        _folder.Write(@"notes\n1.json.tmp", """{ "name": "Half writ""");

        var data = CreateService().LoadAll();

        Assert.False(_folder.Exists("settings.json.tmp"));
        Assert.False(_folder.Exists("clipboard.json.tmp"));
        Assert.False(_folder.Exists(@"notes\n1.json.tmp"));
        Assert.Equal(SortOption.Color, data.Settings.SortOption);
        Assert.Equal("Last good", Assert.Single(data.Notes).Name);
    }

    // ---------- Edge cases 6–8: values fixed on load ----------

    [Fact]
    public void LoadAll_NoteValuesAreNormalized_AndIdComesFromFileName()
    {
        _folder.Create();
        _folder.Write(@"notes\3f2b8c1e-5a7d-4e2f-9b1c-0d4e6f8a9b2c.json",
            """{ "id": "different", "name": "This name is much longer than thirty characters", "opacity": 5, "color": "DarkGray" }""");

        var note = Assert.Single(CreateService().LoadAll().Notes);

        Assert.Equal("3f2b8c1e-5a7d-4e2f-9b1c-0d4e6f8a9b2c", note.Id);
        Assert.Equal("This name is much longer th...", note.Name);
        Assert.Equal(20, note.Opacity);
        Assert.Equal(NoteColor.Yellow, note.Color);
    }

    [Fact]
    public void LoadAll_ClipboardIsTrimmedToSettingsHistorySize()
    {
        _folder.Create();
        _folder.Write("settings.json", """{ "clipboardHistorySize": 5 }""");
        var items = string.Join(",", Enumerable.Range(0, 8).Select(i =>
            $$"""{ "id": "i{{i}}", "text": "t{{i}}", "copiedAt": "2026-11-01T10:{{i:00}}:00Z" }"""));
        _folder.Write("clipboard.json", $$"""{ "items": [ {{items}} ] }""");

        var clipboard = CreateService().LoadAll().Clipboard;

        Assert.Equal(["i7", "i6", "i5", "i4", "i3"], clipboard.Items.Select(i => i.Id));
    }

    [Fact]
    public void LoadAll_NoteWithoutDates_UsesFileCreationTime()
    {
        _folder.Create();
        _folder.Write(@"notes\n1.json", """{ "name": "No dates" }""");
        var created = File.GetCreationTimeUtc(_folder.File(@"notes\n1.json"));

        var note = Assert.Single(CreateService().LoadAll().Notes);

        Assert.Equal(created, note.CreatedAt);
        Assert.Equal(created, note.LastEdited);
    }

    [Fact]
    public void DefaultDataFolder_IsDarkNoteInLocalAppData()
    {
        var expected = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkNote");

        Assert.Equal(expected, StorageService.DefaultDataFolder);
    }
}
