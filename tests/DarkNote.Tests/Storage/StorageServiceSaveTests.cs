using DarkNote.Logging;
using DarkNote.Models;
using DarkNote.Storage;
using NSubstitute;

namespace DarkNote.Tests.Storage;

/// <summary>
/// Workflow 2.4.3 "save one file": write to .tmp, then replace. Edge cases 9 and 10.
/// </summary>
public sealed class StorageServiceSaveTests : IDisposable
{
    private readonly TempDataFolder _folder = new();
    private readonly IAppLog _log = Substitute.For<IAppLog>();
    private readonly StorageService _service;

    public StorageServiceSaveTests()
    {
        _folder.Create();
        _service = new StorageService(_folder.Path, _log);
    }

    public void Dispose() => _folder.Dispose();

    // ---------- Happy path ----------

    [Fact]
    public void SaveNote_WritesNotesIdJson_AndLeavesNoTmp()
    {
        var note = new NoteData { Id = "n1", Name = "Saved" };

        var ok = _service.SaveNote(note);

        Assert.True(ok);
        Assert.True(_folder.Exists(@"notes\n1.json"));
        Assert.False(_folder.Exists(@"notes\n1.json.tmp"));
        Assert.Contains("\"name\": \"Saved\"", _folder.Read(@"notes\n1.json"));
    }

    [Fact]
    public void SaveNote_ReplacesExistingFile()
    {
        _folder.Write(@"notes\n1.json", """{ "name": "Old" }""");

        _service.SaveNote(new NoteData { Id = "n1", Name = "New name" });

        Assert.Contains("New name", _folder.Read(@"notes\n1.json"));
        Assert.DoesNotContain("Old", _folder.Read(@"notes\n1.json"));
    }

    [Fact]
    public void SaveSettings_And_SaveClipboard_WriteTheirFiles()
    {
        Assert.True(_service.SaveSettings(new AppSettings { SortOption = SortOption.Icon }));
        Assert.True(_service.SaveClipboard(new ClipboardNoteData { IsOpen = true }));

        Assert.Contains("\"sortOption\": \"Icon\"", _folder.Read("settings.json"));
        Assert.Contains("\"isOpen\": true", _folder.Read("clipboard.json"));
        Assert.False(_folder.Exists("settings.json.tmp"));
        Assert.False(_folder.Exists("clipboard.json.tmp"));
    }

    [Fact]
    public void SavedFiles_LoadBackTheSameData()
    {
        var settings = new AppSettings { SortOption = SortOption.Color, ClipboardHistorySize = 20, RecentIcons = [NoteIcon.Fish], TrayHintShown = true };
        var clipboard = new ClipboardNoteData
        {
            Items = [new ClipboardItem { Id = "c1", Text = "copied", CopiedAt = new DateTime(2026, 11, 1, 10, 30, 0, DateTimeKind.Utc) }],
            IsPinned = true,
            Opacity = 70,
        };
        var note = new NoteData
        {
            Id = "n1",
            Name = "Round trip",
            Color = NoteColor.Green,
            Window = new WindowBounds { Left = 50, Top = 60, Width = 310, Height = 290 },
            CreatedAt = new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc),
            LastEdited = new DateTime(2026, 11, 1, 9, 5, 0, DateTimeKind.Utc),
        };

        _service.SaveSettings(settings);
        _service.SaveClipboard(clipboard);
        _service.SaveNote(note);
        var data = new StorageService(_folder.Path, _log).LoadAll();

        Assert.Equivalent(settings, data.Settings, strict: true);
        Assert.Equivalent(clipboard, data.Clipboard, strict: true);
        Assert.Equivalent(note, Assert.Single(data.Notes), strict: true);
    }

    // ---------- Edge case 9: write fails ----------

    [Fact]
    public void SaveNote_TargetLocked_ReturnsFalse_OldFileUntouched_NoTmpLeft_Logged()
    {
        _folder.Write(@"notes\n1.json", """{ "name": "Last good" }""");

        bool ok;
        using (new FileStream(_folder.File(@"notes\n1.json"), FileMode.Open, FileAccess.Read, FileShare.None))
        {
            ok = _service.SaveNote(new NoteData { Id = "n1", Name = "Can't save" });
        }

        Assert.False(ok);
        Assert.Equal("""{ "name": "Last good" }""", _folder.Read(@"notes\n1.json"));
        Assert.False(_folder.Exists(@"notes\n1.json.tmp"));
        _log.ReceivedWithAnyArgs().Error(default!, default);
    }

    [Fact]
    public void SaveSettings_TmpCannotBeWritten_ReturnsFalse_OldFileUntouched_Logged()
    {
        _folder.Write("settings.json", """{ "sortOption": "Name" }""");
        _folder.Write("settings.json.tmp", "locked by antivirus");

        bool ok;
        using (new FileStream(_folder.File("settings.json.tmp"), FileMode.Open, FileAccess.Read, FileShare.None))
        {
            ok = _service.SaveSettings(new AppSettings { SortOption = SortOption.Color });
        }

        Assert.False(ok);
        Assert.Equal("""{ "sortOption": "Name" }""", _folder.Read("settings.json"));
        _log.ReceivedWithAnyArgs().Error(default!, default);
    }

    [Fact]
    public void SaveNote_AfterAFailedSave_RetrySucceeds()
    {
        var note = new NoteData { Id = "n1", Name = "Retry me" };
        using (new FileStream(_folder.File(@"notes\n1.json.tmp"), FileMode.Create, FileAccess.Write, FileShare.None))
        {
            Assert.False(_service.SaveNote(note));
        }

        Assert.True(_service.SaveNote(note)); // autosave retries later (2.4.1)
        Assert.Contains("Retry me", _folder.Read(@"notes\n1.json"));
    }

    [Fact]
    public void SaveNote_NotesFolderDeletedWhileRunning_IsRecreated()
    {
        Directory.Delete(_folder.NotesPath, recursive: true);

        Assert.True(_service.SaveNote(new NoteData { Id = "n1" }));
        Assert.True(_folder.Exists(@"notes\n1.json"));
    }

    // ---------- Edge case 10: notes saving at the same time ----------

    [Fact]
    public async Task SaveNote_ManyNotesInParallel_AllSaved()
    {
        var notes = Enumerable.Range(0, 20).Select(i => new NoteData { Id = $"n{i}", Name = $"Note {i}" }).ToList();

        var results = await Task.WhenAll(notes.Select(n => Task.Run(() => _service.SaveNote(n))));

        Assert.All(results, Assert.True);
        var loaded = new StorageService(_folder.Path, _log).LoadAll().Notes;
        Assert.Equal(notes.Select(n => n.Name).Order(), loaded.Select(n => n.Name).Order());
    }
}
