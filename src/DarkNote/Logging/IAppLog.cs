namespace DarkNote.Logging;

/// <summary>Minimal app log. Logging must never throw or crash the app.</summary>
public interface IAppLog
{
    void Info(string message);

    void Warn(string message);

    void Error(string message, Exception? exception = null);
}
