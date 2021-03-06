﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public class AppApiFileSystemWatcher : IDisposable, IHasLog
    {
        private readonly FileSystemWatcher _watcher;

        public readonly ConcurrentDictionary<string, bool> CompiledAppApiControllers = new(StringComparer.OrdinalIgnoreCase);

        public AppApiFileSystemWatcher(IHostEnvironment hostingEnvironment)
        {
            Log = new Log(HistoryLogName, null, "new AppApiFileSystemWatcher()");
            History.Add(HistoryLogGroup, Log);

            var appApiSource = Path.Combine(hostingEnvironment.ContentRootPath, OqtConstants.AppRoot);

            _watcher = new FileSystemWatcher
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
            Log.Add($"Begin watching: {appApiSource}.");
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
            Log.Add("Stop watching.");
            _watcher.EnableRaisingEvents = false;
            _watcher?.Dispose();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Log.Add($"Change type: {e.ChangeType}, file: {e.FullPath}, update: {CompiledAppApiControllers.TryUpdate(e.FullPath, true, false)}.");
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Log.Add($"Renamed: {e.OldFullPath} to {e.FullPath}, update: {CompiledAppApiControllers.TryUpdate(e.OldFullPath, true, false)}.");
        }
    }
}
