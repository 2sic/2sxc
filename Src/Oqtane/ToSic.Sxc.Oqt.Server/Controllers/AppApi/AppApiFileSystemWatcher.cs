using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using ToSic.Eav;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;


/// <summary>
/// The AppApiFileSystemWatcher class is ensuring that the 2sxc app's in runtime remains up-to-date with the latest changes to its web APIs by tracking updates in .cs files.
/// FileSystemWatcher monitor changes within the 2sxc application's source files. This detection is crucial for the application's runtime to know when web APIs implementation has changed, 
/// necessitating the removal of the outdated API components from the application's runtime.
/// </summary>
internal class AppApiFileSystemWatcher : IDisposable, IHasLog
{
    private readonly FileSystemWatcher _watcher;

    public static readonly ConcurrentDictionary<string, AppApiCacheItem> CompiledAppApiControllers = new(StringComparer.InvariantCultureIgnoreCase);

    public AppApiFileSystemWatcher(IHostEnvironment hostingEnvironment, ILogStore logStore)
    {
        Log = new Log(HistoryLogName, null, "new AppApiFileSystemWatcher()");
        logStore.Add(HistoryLogGroup, Log);

        var appApiSource = Path.Combine(hostingEnvironment.ContentRootPath, OqtConstants.AppRoot);

        _watcher = new()
        {
            Path = appApiSource,
            NotifyFilter = NotifyFilters.LastWrite
                           | NotifyFilters.FileName
                           | NotifyFilters.DirectoryName,
            Filter = "*.cs",
            IncludeSubdirectories = true
        };

        // Add event handlers.
        _watcher.Changed += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Renamed += OnRenamed;

        // Begin watching.
        Log.A($"Begin watching: {appApiSource}.");
        _watcher.EnableRaisingEvents = true;
    }

    public ILog Log { get; }

    /// <summary>
    /// The group name for log entries in insights.
    /// Helps group various calls by use case.
    /// </summary>
    protected string HistoryLogGroup { get; } = "app-api";

    /// <summary>
    /// The name of the logger in insights.
    /// /// </summary>
    protected string HistoryLogName => "FileSystemWatcher";

    public void Dispose()
    {
        Log.A("Stop watching.");
        _watcher.EnableRaisingEvents = false;
        _watcher?.Dispose();
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        var appApiCacheItem = FlagForRemove(e.FullPath) ?? CheckAppCode(e.FullPath);
        Log.A($"Change type: {e.ChangeType}, file: {e.FullPath}, flag for remove: {appApiCacheItem?.FlagForRemove}.");
    }

    private void OnRenamed(object source, RenamedEventArgs e)
    {
        var appApiCacheItem = FlagForRemove(e.OldFullPath) ?? CheckAppCode(e.OldFullPath);
        Log.A($"Renamed: {e.OldFullPath} to {e.FullPath}, flag for remove: {appApiCacheItem?.FlagForRemove}.");
    }

    private AppApiCacheItem FlagForRemove(string path)
    {
        if (!CompiledAppApiControllers.TryGetValue(path, out var appApiCacheItem)) return null;
        if (!appApiCacheItem.FlagForRemove) appApiCacheItem.FlagForRemove = true;
        return appApiCacheItem;
    }

    private AppApiCacheItem CheckAppCode(string path)
    {
        if (!path.Contains(Constants.AppCode, StringComparison.InvariantCultureIgnoreCase)) return null;
        AppApiCacheItem appApiCacheItem = null;
        foreach (var controller in CompiledAppApiControllers)
        {
            if (!controller.Value.IsAppCode || !path.StartsWith(controller.Value.AppCodePath, StringComparison.InvariantCultureIgnoreCase)) continue;
            appApiCacheItem = FlagForRemove(controller.Key);
        }
        return appApiCacheItem;
    }
}