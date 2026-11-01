using System.IO;
using System.Windows;
using DarkNote.Logging;
using DarkNote.Storage;

namespace DarkNote;

/// <summary>
/// App start: load all data (2.4.3), then show the main page.
/// If the data folder can't be used, show an error and exit, so data is never lost silently.
/// </summary>
public partial class App : Application
{
    public IAppLog Log { get; private set; } = null!;

    public StorageService Storage { get; private set; } = null!;

    /// <summary>Data loaded at start. Later functions (main page, notes, clipboard) build on this.</summary>
    public LoadedData Data { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var dataFolder = StorageService.DefaultDataFolder;
        Log = new FileAppLog(Path.Combine(dataFolder, "logs"));
        Storage = new StorageService(dataFolder, Log);

        try
        {
            Data = Storage.LoadAll();
        }
        catch (DataFolderUnavailableException ex)
        {
            MessageBox.Show(
                $"Cannot access data folder:\n{ex.Folder}\n\n{ex.InnerException?.Message}",
                "DarkNote",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        MainWindow = new MainWindow();
        MainWindow.Show();
    }
}
