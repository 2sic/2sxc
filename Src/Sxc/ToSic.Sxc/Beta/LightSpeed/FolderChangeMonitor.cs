using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Caching.Hosting;
using System.Text;
using System.Threading;

namespace ToSic.Sxc.Beta.LightSpeed
{
    // based on https://raw.githubusercontent.com/microsoft/referencesource/master/System.Runtime.Caching/System/Caching/HostFileChangeMonitor.cs
    public class FolderChangeMonitor : FileChangeMonitor
    {
        private const int MaxCharCountOfLongConvertedToHexadecimalString = 16;
        private static IFileChangeNotificationSystem _folderChangeNotificationSystem;
        private object _fcnState;

        public override ReadOnlyCollection<string> FilePaths => _folderPaths;
        private readonly ReadOnlyCollection<string> _folderPaths;
        
        public override string UniqueId => _uniqueId;
        private string _uniqueId;
        
        public override DateTimeOffset LastModified => _lastModified;
        private DateTimeOffset _lastModified;

        public FolderChangeMonitor(IList<string> folderPaths)
        {
            if (folderPaths == null || folderPaths.Count == 0) throw new ArgumentException("Empty collection: folderPaths");

            _folderPaths = folderPaths.ToList().AsReadOnly();
            InitFcn();
            InitDisposableMembers();
        }

        private static void InitFcn()
        {
            if (_folderChangeNotificationSystem != null) return;
            Interlocked.CompareExchange(ref _folderChangeNotificationSystem, new FolderChangeNotificationSystem(), null);
        }

        private void InitDisposableMembers()
        {
            var dispose = true;
            try
            {
                string uniqueId = null;
                if (_folderPaths.Count == 1)
                {
                    var path = _folderPaths[0];
                    _folderChangeNotificationSystem.StartMonitoring(path, new OnChangedCallback(OnChanged), out _fcnState, out var lastWrite, out var fileSize);
                    uniqueId = path + lastWrite.UtcDateTime.Ticks.ToString("X", CultureInfo.InvariantCulture) + fileSize.ToString("X", CultureInfo.InvariantCulture);
                    _lastModified = lastWrite;
                }
                else
                {
                    var capacity = _folderPaths.Sum(path => path.Length + (2 * MaxCharCountOfLongConvertedToHexadecimalString));
                    var fcnState = new Hashtable(_folderPaths.Count);
                    _fcnState = fcnState;
                    var sb = new StringBuilder(capacity);
                    foreach (var path in _folderPaths)
                    {
                        if (fcnState.Contains(path)) continue;
                        _folderChangeNotificationSystem.StartMonitoring(path, new OnChangedCallback(OnChanged), out var state, out var lastWrite, out var fileSize);
                        fcnState[path] = state;
                        sb.Append(path);
                        sb.Append(lastWrite.UtcDateTime.Ticks.ToString("X", CultureInfo.InvariantCulture));
                        sb.Append(fileSize.ToString("X", CultureInfo.InvariantCulture));
                        if (lastWrite > _lastModified) _lastModified = lastWrite;
                    }
                    uniqueId = sb.ToString();
                }
                _uniqueId = uniqueId;
                dispose = false;
            }
            finally
            {
                InitializationComplete();
                if (dispose)
                {
                    Dispose();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing || _folderChangeNotificationSystem == null || _folderPaths == null || _fcnState == null) return;
            
            if (_folderPaths.Count > 1)
            {
                var fcnState = _fcnState as Hashtable;
                foreach (var path in _folderPaths)
                {
                    if (string.IsNullOrEmpty(path)) continue;
                    
                    var state = fcnState[path];
                    if (state != null) _folderChangeNotificationSystem.StopMonitoring(path, state);
                }
            }
            else
            {
                var path = _folderPaths[0];
                if (path != null && _fcnState != null) _folderChangeNotificationSystem.StopMonitoring(path, _fcnState);
            }
        }
    }
}
