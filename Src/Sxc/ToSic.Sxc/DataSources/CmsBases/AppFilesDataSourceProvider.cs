using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Helpers;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;
using static System.IO.Path;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Base class to provide data to the AppFiles DataSource.
    ///
    /// Must be overriden in each platform.
    /// </summary>
    public abstract class AppFilesDataSourceProvider: ServiceBase<AppFilesDataSourceProvider.Dependencies>
    {
        public class Dependencies : ServiceDependencies
        {
            public ZipExport ZipExport { get; }
            public IAppStates AppStates { get; }
            public LazySvc<App> AppLazy { get; }

            /// <summary>
            /// Note that we will use Generators for safety, because in rare cases the dependencies could be re-used to create a sub-data-source
            /// </summary>
            public Dependencies(
                LazySvc<App> appLazy, 
                IAppStates appStates,
                ZipExport zipExport
            )
            {
                AddToLogQueue(
                    AppStates = appStates,
                    AppLazy = appLazy,
                    ZipExport = zipExport
                );
            }
        }

        protected AppFilesDataSourceProvider(Dependencies services, string logName) : base(services ,logName)
        {
        }

        /// <summary>
        /// FYI: The filters are not actually implemented yet.
        /// So the core data source doesn't have settings to configure this
        /// </summary>
        /// <returns></returns>
        public List<AppFileInfo> GetAppFilesInternal(
            string noParamOrder = Eav.Parameters.Protector,
            int zoneId = default,
            int appId = default,
            bool onlyFolders = default,
            bool onlyFiles = default,
            string root = default,
            string filter = default
        ) => Log.Func($"app:{appId}; zone:{zoneId}", l =>
        {
            var currentApp = GetApp(zoneId, appId);
            var fileManager = GetZipExport(currentApp).FileManager;
            var prepRoot = root.TrimPrefixSlash().Backslash();
            fileManager.SetFolder(currentApp.PhysicalPath, prepRoot);

            var id = 0;

            var folders = new List<AppFileInfo>();
            var result = new List<AppFileInfo>();

            if (!onlyFiles)
            {
                folders = fileManager.GetAllTransferableFolders(/*filter*/)
                    .Select(p => new DirectoryInfo(p))
                    .Select(d => new AppFileInfo
                    {
                        Id = ++id,
                        Guid = Guid.NewGuid(),
                        Title = $"{GetFileNameWithoutExtension(d.FullName)}{d.Extension}",
                        Name = GetFileNameWithoutExtension(d.FullName),
                        Extension = d.Extension,
                        FullName = NameInAppFolder(d.FullName, currentApp, prepRoot),
                        Folder = FolderInAppFolder(d.FullName, currentApp, prepRoot),
                        IsFolder = true,
                        Size = 0,
                        Created = d.CreationTime,
                        Modified = d.LastWriteTime
                    })
                    .ToList();
                folders.ForEach(f => f.ParentId = (folders.FirstOrDefault(d =>  d.FullName == f.Folder && f.Title != "<root>")?.Id ?? 0));
                l.A($"folders: {folders.Count}");
                result.AddRange(folders);
            }
            else
                l.A($"onlyFiles: {onlyFiles}");

            if (!onlyFolders)
            {
                var files = fileManager.GetAllTransferableFiles(filter)
                    .Select(p => new FileInfo(p))
                    .Select(f => new AppFileInfo
                    {
                        Id = ++id,
                        Guid = Guid.NewGuid(),
                        Title = $"{GetFileNameWithoutExtension(f.FullName)}{f.Extension}",
                        Name = GetFileNameWithoutExtension(f.FullName),
                        Extension = f.Extension,
                        FullName = NameInAppFolder(f.FullName, currentApp, prepRoot),
                        Folder = FolderInAppFolder(f.FullName, currentApp, prepRoot),
                        ParentId = 0,
                        IsFolder = false,
                        Size = f.Length,
                        Created = f.CreationTime,
                        Modified = f.LastWriteTime
                    })
                    .ToList();
                files.ForEach(f => f.ParentId = (folders.FirstOrDefault(d => d.FullName == f.Folder)?.Id ?? 0));
                l.A($"files: {files.Count}");
                result.AddRange(files);
            }
            else
                l.A($"onlyFolders: {onlyFolders}");

            return (result, $"found {result.Count}");
        });

        private static string NameInAppFolder(string filePath, IApp currentApp, string root)
        {
            var name = filePath?.Replace(Combine(currentApp.PhysicalPath, root), string.Empty);
            //var isFolder = GetAttributes(filePath).HasFlag(FileAttributes.Directory);
            if (string.IsNullOrEmpty(name)) 
                return root;
            if (currentApp.PhysicalPathShared != null) name = name.Replace(Combine(currentApp.PhysicalPathShared, root), string.Empty);
            return name.ForwardSlash();
        }

        private static string FolderInAppFolder(string filePath, IApp currentApp, string root)
        {
            var folder = GetDirectoryName(filePath)?.Replace(Combine(currentApp.PhysicalPath, root), string.Empty);
            if (string.IsNullOrEmpty(folder)) 
                return root;
            if (currentApp.PhysicalPathShared != null) folder = folder.Replace(Combine(currentApp.PhysicalPathShared, root), string.Empty);
            return folder.ForwardSlash();
        }

        private ZipExport GetZipExport(IApp app) 
            => Services.ZipExport.Init(app.ZoneId, app.AppId, app.Folder, app.PhysicalPath, app.PhysicalPathShared);

        private IApp GetApp(int zoneId, int appId) 
            => Services.AppLazy.Value.Init(GetAppState(zoneId, appId), null);

        private AppState GetAppState(int zoneId, int appId) 
            => Services.AppStates.Get(new AppIdentity(zoneId, appId));
    }
}
