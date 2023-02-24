using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Helpers;
using ToSic.Eav.ImportExport;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using static System.IO.Path;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Base class to provide data to the AppFiles DataSource.
    ///
    /// Must be overriden in each platform.
    /// </summary>
    public abstract class AppFilesDataSourceProvider : ServiceBase<AppFilesDataSourceProvider.MyServices>
    {
        public class MyServices : MyServicesBase
        {
            public ZipExport ZipExport { get; }
            public IAppStates AppStates { get; }
            public LazySvc<App> AppLazy { get; }

            /// <summary>
            /// Note that we will use Generators for safety, because in rare cases the dependencies could be re-used to create a sub-data-source
            /// </summary>
            public MyServices(
                LazySvc<App> appLazy,
                IAppStates appStates,
                ZipExport zipExport
            )
            {
                ConnectServices(
                    AppStates = appStates,
                    AppLazy = appLazy,
                    ZipExport = zipExport
                );
            }
        }

        protected AppFilesDataSourceProvider(MyServices services, string logName) : base(services, logName)
        {
        }

        public AppFilesDataSourceProvider Configure(
            string noParamOrder = Eav.Parameters.Protector,
            int zoneId = default,
            int appId = default,
            bool onlyFolders = default,
            bool onlyFiles = default,
            string root = default,
            string filter = default
        ) => Log.Func($"a:{appId}; z:{zoneId}, onlyFolders:{onlyFolders}, onlyFiles:{onlyFiles}, root:{root}, filter:{filter}", l =>
        {
            _onlyFolders = onlyFolders;
            _onlyFiles = onlyFiles;
            _root = root.TrimPrefixSlash().Backslash();
            _filter = filter;
            _currentApp = GetApp(zoneId, appId);
            _fileManager = GetZipExport(_currentApp).FileManager;
            _fileManager.SetFolder(_currentApp.PhysicalPath, _root);
            return this;
        });

        private bool _onlyFolders;
        private bool _onlyFiles;
        private string _root;
        private string _filter;
        private IApp _currentApp;
        private FileManager _fileManager;

        /// <summary>
        /// FYI: The filters are not actually implemented yet.
        /// So the core data source doesn't have settings to configure this
        /// </summary>
        /// <returns></returns>
        public List<AppFileDataNewBase> GetInternal() => Log.Func(l =>
        {
            var result = new List<AppFileDataNewBase>();
            result.AddRange(Folders);
            result.AddRange(Files);
            return (result, $"found:{result.Count}");
        });

        public List<AppFileDataNew> Files => _files.Get(Log, l =>
        {
            var files = new List<AppFileDataNew>();
            if (!_onlyFolders)
            {
                files = _fileManager.GetAllTransferableFiles(_filter)
                    .Select(p => new FileInfo(p))
                    .Select(f =>
                    {
                        var fullName = FullNameWithoutAppFolder(f.FullName, _currentApp, _root);
                        return new AppFileDataNew
                        {
                            Name = $"{GetFileNameWithoutExtension(f.FullName)}{f.Extension}",
                            Extension = f.Extension,
                            FullName = fullName,
                            ParentFolderInternal = fullName.BeforeLast("/"),
                            Path = fullName.BeforeLast("/") + "/",
                            Size = f.Length,
                            Created = f.CreationTime,
                            Modified = f.LastWriteTime
                        };
                    })
                    .ToList();
            }
            else
                l.A($"onlyFolders:{_onlyFolders}");

            return (files, $"files:{files.Count}");
        });
        private readonly GetOnce<List<AppFileDataNew>> _files = new GetOnce<List<AppFileDataNew>>();

        public List<AppFolderDataNew> Folders => _folders.Get(Log, l =>
        {
            var folders = new List<AppFolderDataNew>();
            if (!_onlyFiles)
            {
                folders = _fileManager.GetAllTransferableFolders(/*filter*/)
                    .Select(p => new DirectoryInfo(p))
                    .Select(d =>
                    {
                        var fullName = FullNameWithoutAppFolder(d.FullName, _currentApp, _root);
                        return new AppFolderDataNew
                        {
                            Name = $"{GetFileNameWithoutExtension(d.FullName)}{d.Extension}",
                            FullName = fullName,
                            ParentFolderInternal = fullName.BeforeLast("/"),
                            Path = fullName.BeforeLast("/") + "/",
                            Created = d.CreationTime,
                            Modified = d.LastWriteTime
                        };
                    })
                    .ToList();
                folders.Insert(0, _rootFolder);
            }
            else
                l.A($"onlyFiles:{_onlyFiles}");

            return (folders, $"found:{folders.Count}");
        });
        private readonly GetOnce<List<AppFolderDataNew>> _folders = new GetOnce<List<AppFolderDataNew>>();

        private readonly AppFolderDataNew _rootFolder = new AppFolderDataNew
        {
            Name = "Root",
            FullName = "",
            ParentFolderInternal = "/", // it should not be empty for root folder (it is expected that its children should have empty ParentFolderInternal)
            Path = "/", // this is to show to end 
            Created = DateTime.MinValue,
            Modified = DateTime.MinValue
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentApp"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private static string FullNameWithoutAppFolder(string path, IApp currentApp, string root)
        {
            var name = path?.Replace(Combine(currentApp.PhysicalPath, root), string.Empty);
            //var isFolder = GetAttributes(filePath).HasFlag(FileAttributes.Directory);
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            if (currentApp.PhysicalPathShared != null) 
                name = name.Replace(Combine(currentApp.PhysicalPathShared, root), string.Empty);
            return name.ForwardSlash();
        }

        private ZipExport GetZipExport(IApp app)
            => Services.ZipExport.Init(app.ZoneId, app.AppId, app.Folder, app.PhysicalPath, app.PhysicalPathShared);

        private IApp GetApp(int zoneId, int appId)
            => Services.AppLazy.Value.Init(GetAppState(zoneId, appId), null);

        private AppState GetAppState(int zoneId, int appId)
            => Services.AppStates.Get(new AppIdentity(zoneId, appId));
    }
}
