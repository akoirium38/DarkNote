namespace DarkNote.Tests.Storage;

/// <summary>
/// A unique, empty folder under %TEMP% that plays the role of %LOCALAPPDATA%\DarkNote in a test.
/// The folder itself is NOT created, so tests can cover the "first run" case. Deleted on dispose.
/// </summary>
public sealed class TempDataFolder : IDisposable
{
    public TempDataFolder()
    {
        Root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DarkNote.Tests", Guid.NewGuid().ToString("N"));
        Path = System.IO.Path.Combine(Root, "DarkNote");
    }

    /// <summary>Parent folder that owns everything this test creates.</summary>
    public string Root { get; }

    /// <summary>The app data folder (like %LOCALAPPDATA%\DarkNote). Not created up front.</summary>
    public string Path { get; }

    public string NotesPath => System.IO.Path.Combine(Path, "notes");

    public string File(string relativePath) => System.IO.Path.Combine(Path, relativePath);

    /// <summary>Creates the data folder and its notes subfolder.</summary>
    public TempDataFolder Create()
    {
        Directory.CreateDirectory(NotesPath);
        return this;
    }

    public void Write(string relativePath, string content)
    {
        var full = File(relativePath);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(full)!);
        System.IO.File.WriteAllText(full, content);
    }

    public string Read(string relativePath) => System.IO.File.ReadAllText(File(relativePath));

    public bool Exists(string relativePath) => System.IO.File.Exists(File(relativePath));

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, recursive: true);
            }
        }
        catch (IOException)
        {
            // Best effort: a locked file must not fail the test run.
        }
    }
}
