namespace DarkNote.Storage;

/// <summary>
/// The app data folder can't be created or read (workflow 2.4.3, edge case 2).
/// The app shows an error and exits, so data is never lost silently.
/// </summary>
public sealed class DataFolderUnavailableException(string folder, Exception innerException)
    : Exception($"Cannot access data folder '{folder}'.", innerException)
{
    public string Folder { get; } = folder;
}
