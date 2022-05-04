using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Caching.Hosting;

namespace ToSic.Sxc.Web.LightSpeed
{
    //  based on https://github.com/microsoft/referencesource/blob/master/System.Runtime.Caching/System/Caching/FileChangeNotificationSystem.cs
    public class FolderChangeNotificationSystem : IFileChangeNotificationSystem
    {
        private readonly Hashtable _dirMonitors;
        private readonly object _lock;
        public FolderChangeNotificationSystem()
        {
            _dirMonitors = Hashtable.Synchronized(new Hashtable(StringComparer.OrdinalIgnoreCase));
            _lock = new object();
        }

        public class DirectoryMonitor
        {
            public FileSystemWatcher FileSystemWatcher;
        }

        public class FolderChangeEventTarget
        {
            //private readonly string _folderName;
            private readonly OnChangedCallback _onChangedCallback;
            public FileSystemEventHandler ChangedHandler { get; }
            public ErrorEventHandler ErrorHandler { get; }
            public RenamedEventHandler RenamedHandler { get; }
            public FolderChangeEventTarget(/*string folderName, */OnChangedCallback onChangedCallback)
            {
                //_folderName = folderName;
                _onChangedCallback = onChangedCallback;
                ChangedHandler = new FileSystemEventHandler(this.OnChanged);
                ErrorHandler = new ErrorEventHandler(this.OnError);
                RenamedHandler = new RenamedEventHandler(this.OnRenamed);
            }

            private void OnChanged(object sender, FileSystemEventArgs e)
            {
                //if (EqualsIgnoreCase(_folderName, e.Name))
                    _onChangedCallback(null);
            }

            private void OnError(object sender, ErrorEventArgs e)
            {
                _onChangedCallback(null);
            }

            private void OnRenamed(object sender, RenamedEventArgs e)
            {
                //if (EqualsIgnoreCase(_folderName, e.Name) || EqualsIgnoreCase(_folderName, e.OldName))
                    _onChangedCallback(null);
            }
            
            //private static bool EqualsIgnoreCase(string s1, string s2)
            //{
            //    if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
            //        return true;
            //    if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            //        return false;
            //    if (s2.Length != s1.Length)
            //        return false;
            //    return 0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase);
            //}
        }
        
        public void StartMonitoring(string dirPath, OnChangedCallback onChangedCallback, out object state, out DateTimeOffset lastWriteTime, out long fileSize)
        {
            if (dirPath == null) throw new ArgumentNullException("dirPath");
            if (onChangedCallback == null) throw new ArgumentNullException("onChangedCallback");
            
            var directoryInfo = new DirectoryInfo(dirPath);
            if (!(_dirMonitors[dirPath] is DirectoryMonitor dirMon))
            {
                lock (_lock)
                {
                    dirMon = _dirMonitors[dirPath] as DirectoryMonitor ?? new DirectoryMonitor
                    {
                        FileSystemWatcher = new FileSystemWatcher(dirPath)
                        {
                            NotifyFilter = NotifyFilters.FileName
                                           | NotifyFilters.DirectoryName
                                           | NotifyFilters.Size
                                           | NotifyFilters.LastWrite,
                            IncludeSubdirectories = true,
                            EnableRaisingEvents = true
                        }
                    };
                    _dirMonitors[dirPath] = dirMon;
                }
            }

            var target = new FolderChangeEventTarget(/*directoryInfo.Name, */onChangedCallback);

            lock (dirMon)
            {
                dirMon.FileSystemWatcher.Changed += target.ChangedHandler;
                dirMon.FileSystemWatcher.Created += target.ChangedHandler;
                dirMon.FileSystemWatcher.Deleted += target.ChangedHandler;
                dirMon.FileSystemWatcher.Error += target.ErrorHandler;
                dirMon.FileSystemWatcher.Renamed += target.RenamedHandler;
            }

            state = target;
            lastWriteTime = directoryInfo.LastWriteTime;
            fileSize = (directoryInfo.Exists) ? GetDirectorySize(directoryInfo) : -1;
        }

        private static long GetDirectorySize(DirectoryInfo directoryInfo) => 
            directoryInfo.GetFiles("*.*",SearchOption.AllDirectories).Sum(f => f.Length);

        public void StopMonitoring(string dirPath, object state)
        {
            if (dirPath == null) throw new ArgumentNullException("dirPath");
            if (state == null) throw new ArgumentNullException("state");
            if (!(state is FolderChangeEventTarget target)) throw new ArgumentException("target is null");
            if (!(_dirMonitors[dirPath] is DirectoryMonitor dirMon)) return;
            lock (dirMon)
            {
                dirMon.FileSystemWatcher.Changed -= target.ChangedHandler;
                dirMon.FileSystemWatcher.Created -= target.ChangedHandler;
                dirMon.FileSystemWatcher.Deleted -= target.ChangedHandler;
                dirMon.FileSystemWatcher.Error -= target.ErrorHandler;
                dirMon.FileSystemWatcher.Renamed -= target.RenamedHandler;
            }
        }
    }
}
