using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.Json;
using DarkNote.Logging;
using DarkNote.Models;

namespace DarkNote.Storage;

/// <summary>Everything loaded at start.</summary>
public sealed record LoadedData(AppSettings Settings, ClipboardNoteData Clipboard, IReadOnlyList<NoteData> Notes);

/// <summary>
/// Storage Structure (PRD 2.4.3, workflow docs/workflows/2.4.3-storage-structure.md).
/// Loads all data on start and saves one file at a time safely: write &lt;file&gt;.tmp, then replace.
/// All other functions read and write data through this class.
/// </summary>
public sealed class StorageService(string dataFolder, IAppLog log)
{
    private const string TmpExtension = ".tmp";
    private const string CorruptExtension = ".corrupt";
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    // One lock per file, so two saves of the same file never share a .tmp. Different files save in parallel.
    private readonly ConcurrentDictionary<string, object> _fileLocks = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>%LOCALAPPDATA%\DarkNote</summary>
    public static string DefaultDataFolder { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkNote");

    public string DataFolder { get; } = dataFolder;

    public string NotesFolder => Path.Combine(DataFolder, "notes");

    public string SettingsPath => Path.Combine(DataFolder, "settings.json");

    public string ClipboardPath => Path.Combine(DataFolder, "clipboard.json");

    public string NotePath(string noteId)
    {
        if (string.IsNullOrWhiteSpace(noteId) || noteId.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || noteId is "." or "..")
        {
            throw new ArgumentException($"Invalid note id '{noteId}'.", nameof(noteId));
        }

        return Path.Combine(NotesFolder, noteId + ".json");
    }

    // ---------------- Load on start ----------------

    /// <summary>Loads settings, the Clipboard Note and all notes. Damaged files are kept as .corrupt and skipped.</summary>
    /// <exception cref="DataFolderUnavailableException">The data folder can't be created or read.</exception>
    public LoadedData LoadAll()
    {
        EnsureFolders();
        DeleteLeftoverTmpFiles(DataFolder);
        DeleteLeftoverTmpFiles(NotesFolder);

        var settings = ReadFile<AppSettings>(SettingsPath) ?? new AppSettings();
        DataNormalizer.Normalize(settings);

        var clipboard = ReadFile<ClipboardNoteData>(ClipboardPath) ?? new ClipboardNoteData();
        DataNormalizer.Normalize(clipboard, settings.ClipboardHistorySize);

        var notes = new List<NoteData>();
        foreach (var path in EnumerateFiles(NotesFolder, ".json"))
        {
            var note = ReadFile<NoteData>(path);
            if (note is null)
            {
                continue;
            }

            DataNormalizer.Normalize(note, Path.GetFileNameWithoutExtension(path), SafeCreationTimeUtc(path));
            notes.Add(note);
        }

        log.Info($"Loaded {notes.Count} note(s) from '{DataFolder}'.");
        return new LoadedData(settings, clipboard, notes);
    }

    private void EnsureFolders()
    {
        try
        {
            Directory.CreateDirectory(DataFolder);
            Directory.CreateDirectory(NotesFolder);
            // Reading the folder proves we have access, not just that it exists.
            _ = Directory.EnumerateFileSystemEntries(DataFolder).Any();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
        {
            log.Error($"Cannot access data folder '{DataFolder}'.", ex);
            throw new DataFolderUnavailableException(DataFolder, ex);
        }
    }

    private void DeleteLeftoverTmpFiles(string folder)
    {
        foreach (var tmp in EnumerateFiles(folder, TmpExtension))
        {
            try
            {
                File.Delete(tmp);
                log.Info($"Deleted leftover temp file '{tmp}'.");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                log.Warn($"Could not delete leftover temp file '{tmp}': {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Reads one JSON file. Missing → null. Damaged → renamed to .corrupt, logged, null.
    /// Can't be read (locked, no access) → logged, null, file left as is (it may be fine).
    /// </summary>
    private T? ReadFile<T>(string path)
        where T : class
    {
        if (!File.Exists(path))
        {
            return null;
        }

        string json;
        try
        {
            json = File.ReadAllText(path, Encoding.UTF8);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            log.Error($"Could not read '{path}'; using defaults for it this time.", ex);
            return null;
        }

        try
        {
            var value = JsonSerializer.Deserialize<T>(json, StorageJson.Options);
            if (value is not null)
            {
                return value;
            }
        }
        catch (JsonException ex)
        {
            KeepAsCorrupt(path, ex.Message);
            return null;
        }

        KeepAsCorrupt(path, "the file holds no data object");
        return null;
    }

    private void KeepAsCorrupt(string path, string reason)
    {
        try
        {
            var target = path + CorruptExtension;
            for (var i = 1; File.Exists(target); i++)
            {
                target = $"{path}.{i}{CorruptExtension}";
            }

            File.Move(path, target);
            log.Warn($"Damaged file '{path}' ({reason}); kept as '{Path.GetFileName(target)}', using defaults.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            log.Error($"Damaged file '{path}' ({reason}) could not be renamed; skipped.", ex);
        }
    }

    // ---------------- Save one file ----------------

    public bool SaveSettings(AppSettings settings) => SaveFile(SettingsPath, settings);

    public bool SaveClipboard(ClipboardNoteData clipboard) => SaveFile(ClipboardPath, clipboard);

    public bool SaveNote(NoteData note) => SaveFile(NotePath(note.Id), note);

    /// <summary>
    /// Writes &lt;path&gt;.tmp, then replaces &lt;path&gt; with it. On failure the old file is untouched,
    /// the .tmp is removed, the error is logged and false is returned (autosave retries later).
    /// </summary>
    private bool SaveFile<T>(string path, T value)
    {
        var tmp = path + TmpExtension;
        lock (_fileLocks.GetOrAdd(path, _ => new object()))
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var json = JsonSerializer.Serialize(value, StorageJson.Options);

                using (var stream = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var bytes = Utf8NoBom.GetBytes(json);
                    stream.Write(bytes);
                    stream.Flush(flushToDisk: true);
                }

                File.Move(tmp, path, overwrite: true);
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                log.Error($"Could not save '{path}'; changes stay in memory.", ex);
                TryDelete(tmp);
                return false;
            }
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Left for the next start, which deletes leftover .tmp files.
        }
    }

    // ---------------- Helpers ----------------

    /// <summary>Files whose extension is exactly <paramref name="extension"/> (so "a.json.corrupt" is not a ".json").</summary>
    private static IEnumerable<string> EnumerateFiles(string folder, string extension) =>
        Directory.EnumerateFiles(folder, "*" + extension)
            .Where(f => string.Equals(Path.GetExtension(f), extension, StringComparison.OrdinalIgnoreCase))
            .ToList();

    private static DateTime SafeCreationTimeUtc(string path)
    {
        try
        {
            return File.GetCreationTimeUtc(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return DateTime.UtcNow;
        }
    }
}
