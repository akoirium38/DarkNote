using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace DarkNote.Logging;

/// <summary>
/// Appends log lines to &lt;folder&gt;\darknote.log. When the file grows past 1 MB it is moved to
/// darknote.old.log (one old file kept). Any failure to write is swallowed.
/// </summary>
public sealed class FileAppLog(string folder) : IAppLog
{
    private const long MaxFileSize = 1024 * 1024;
    private readonly Lock _gate = new();

    public string FilePath { get; } = Path.Combine(folder, "darknote.log");

    public void Info(string message) => Write("INFO", message, null);

    public void Warn(string message) => Write("WARN", message, null);

    public void Error(string message, Exception? exception = null) => Write("ERROR", message, exception);

    private void Write(string level, string message, Exception? exception)
    {
        var line = string.Create(CultureInfo.InvariantCulture, $"{DateTime.UtcNow:O} [{level}] {message}");
        if (exception is not null)
        {
            line += Environment.NewLine + "    " + exception;
        }

        Debug.WriteLine(line);
        try
        {
            lock (_gate)
            {
                Directory.CreateDirectory(folder);
                var info = new FileInfo(FilePath);
                if (info.Exists && info.Length > MaxFileSize)
                {
                    File.Move(FilePath, Path.Combine(folder, "darknote.old.log"), overwrite: true);
                }

                File.AppendAllText(FilePath, line + Environment.NewLine);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Logging must never break the app.
        }
    }
}
