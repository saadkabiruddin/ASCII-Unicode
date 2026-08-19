using ASCII_Unicode.Models;

namespace ASCII_Unicode.Services;

/// <summary>
/// Monitors the conversion mappings JSON file for changes and triggers reloads
/// </summary>
public sealed class FileWatcherService : IHostedService, IDisposable
{
    private readonly ILogger<FileWatcherService> _logger;
    private readonly ConversionMappingsLoader _loader;
    private readonly IHostApplicationLifetime _hostLifetime;
    private readonly string _filePath;
    private FileSystemWatcher? _watcher;
    private readonly object _lockObject = new();
    private volatile bool _isRestarting;

    public FileWatcherService(
        IWebHostEnvironment environment,
        ConversionMappingsLoader loader,
        IHostApplicationLifetime hostLifetime,
        ILogger<FileWatcherService> logger)
    {
        _logger = logger;
        _loader = loader;
        _hostLifetime = hostLifetime;
        _filePath = Path.Combine(environment.ContentRootPath, "conversion-mappings.json");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            var fileName = Path.GetFileName(_filePath);

            if (string.IsNullOrEmpty(directory))
            {
                _logger.LogWarning("Could not determine directory for file watcher");
                return Task.CompletedTask;
            }

            _watcher = new FileSystemWatcher(directory, fileName)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _watcher.Changed += OnMappingsFileChanged;

            _logger.LogInformation("FileWatcher started for {FilePath}", _filePath);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting FileWatcher");
            return Task.CompletedTask;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _watcher?.Dispose();
        _logger.LogInformation("FileWatcher stopped");
        return Task.CompletedTask;
    }

    private void OnMappingsFileChanged(object sender, FileSystemEventArgs e)
    {
        lock (_lockObject)
        {
            if (_isRestarting)
            {
                _logger.LogInformation("Application restart already in progress, ignoring file change event");
                return;
            }

            _isRestarting = true;
        }

        try
        {
            _logger.LogInformation("Conversion mappings file changed: {FilePath}", e.FullPath);

            // Wait a bit to ensure file is fully written
            Task.Delay(500).Wait();

            // Reload the mappings
            _loader.ReloadMappings();
            _logger.LogInformation("Conversion mappings reloaded successfully");

            // Restart the application
            _logger.LogInformation("Triggering application restart due to conversion mappings change");
            RestartApplication();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling conversion mappings file change");
            lock (_lockObject)
            {
                _isRestarting = false;
            }
        }
    }

    private void RestartApplication()
    {
        try
        {
            // Create app_offline.htm to take the app offline
            var appOfflineFile = Path.Combine(
                Path.GetDirectoryName(_filePath) ?? ".",
                "..",
                "app_offline.htm"
            );

            var appOfflinePath = Path.GetFullPath(appOfflineFile);
            var offlineHtml = @"<!DOCTYPE html>
<html>
<head>
    <title>Application Updating</title>
</head>
<body>
    <h1>Application is being updated</h1>
    <p>Conversion mappings have been updated. The application is restarting. Please refresh in a moment.</p>
</body>
</html>";

            _logger.LogInformation("Creating app_offline.htm at {Path}", appOfflinePath);
            File.WriteAllText(appOfflinePath, offlineHtml);

            // Signal the host to stop (which will restart the app when app_offline.htm is removed)
            Task.Delay(2000).ContinueWith(_ =>
            {
                try
                {
                    if (File.Exists(appOfflinePath))
                    {
                        File.Delete(appOfflinePath);
                        _logger.LogInformation("Removed app_offline.htm");
                    }
                    lock (_lockObject)
                    {
                        _isRestarting = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing app_offline.htm");
                    lock (_lockObject)
                    {
                        _isRestarting = false;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating app_offline.htm for restart");
            lock (_lockObject)
            {
                _isRestarting = false;
            }
        }
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}
