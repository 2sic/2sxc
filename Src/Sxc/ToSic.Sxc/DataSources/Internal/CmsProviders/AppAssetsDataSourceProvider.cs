using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Helpers;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using static System.IO.Path;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the AppFiles DataSource.
///
/// Must be overriden in each platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppAssetsDataSourceProvider(AppAssetsDataSourceProvider.MyServices services)
    : ServiceBase<AppAssetsDataSourceProvider.MyServices>(services, $"{SxcLogName}.AppFls")
{
    public class MyServices(IAppReaderFactory appReaders, IAppPathsMicroSvc appPathMicroSvc, Generator<FileManager> fileManagerGenerator)
        : MyServicesBase(connect: [appReaders, appPathMicroSvc, fileManagerGenerator])
    {
        /// <summary>
        /// Note that we will use Generators for safety, because in rare cases the dependencies could be re-used to create a sub-data-source
        /// </summary>
        internal Generator<FileManager> FileManagerGenerator { get; } = fileManagerGenerator;

        internal IAppPathsMicroSvc AppPathMicroSvc { get; } = appPathMicroSvc;

        internal IAppReaderFactory AppReaders { get; } = appReaders;
    }

    public AppAssetsDataSourceProvider Configure(
        NoParamOrder noParamOrder = default,
        int zoneId = default,
        int appId = default,
        bool onlyFolders = default,
        bool onlyFiles = default,
        string root = default,
        string filter = default
    ) {
        var l = Log.Fn<AppAssetsDataSourceProvider>($"a:{appId}; z:{zoneId}, onlyFolders:{onlyFolders}, onlyFiles:{onlyFiles}, root:{root}, filter:{filter}");
        _onlyFolders = onlyFolders;
        _onlyFiles = onlyFiles;
        _root = root.TrimPrefixSlash().Backslash();
        _filter = filter;

        var appState = Services.AppReaders.Get(new AppIdentity(zoneId, appId));
        _appPaths = Services.AppPathMicroSvc.Get(appState);
        
        _fileManager = Services.FileManagerGenerator.New().SetFolder(appId, _appPaths.PhysicalPath, _root);
        return l.Return(this);
    }

    private bool _onlyFolders;
    private bool _onlyFiles;
    private string _root;
    private string _filter;
    private FileManager _fileManager;
    private IAppPaths _appPaths;

    /// <summary>
    /// FYI: The filters are not actually implemented yet.
    /// So the core data source doesn't have settings to configure this
    /// </summary>
    /// <returns></returns>
    public (List<AppFolderDataRaw> Folders, List<AppFileDataRaw> Files) GetAll()
        => Log.Func(l => (Folders, Files));

    public List<AppFileDataRaw> Files => _files.GetM(Log, l =>
    {
        var files = new List<AppFileDataRaw>();
        if (!_onlyFolders)
        {
            var pathsFromRoot = PreparePaths(_appPaths, "");

            files = _fileManager.GetAllTransferableFiles(_filter)
                .Select(p => new FileInfo(p))
                .Select(f =>
                {
                    // 2024-10-07 2dm - had to fix because the path became shorter when requesting files from a subfolder
                    // but when asking for app files/folders it must always assume/start with the root of the app
                    //var fullName = FullNameWithoutAppFolder(f.FullName, _appPaths, _root);
                    var fullNameFromAppRoot = FullNameWithoutAppFolder(f.FullName, pathsFromRoot);
                    return new AppFileDataRaw
                    {
                        Name = $"{GetFileNameWithoutExtension(f.FullName)}{f.Extension}",
                        Extension = f.Extension,
                        FullName = fullNameFromAppRoot,
                        ParentFolderInternal = fullNameFromAppRoot.BeforeLast("/"),
                        Path = fullNameFromAppRoot.BeforeLast("/") + "/",

                        // TODO convert characters for safe HTML
                        Url = $"{_appPaths.Path}/{fullNameFromAppRoot}",
                        UrlRelative = fullNameFromAppRoot,


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
    private readonly GetOnce<List<AppFileDataRaw>> _files = new();

    public List<AppFolderDataRaw> Folders => _folders.GetM(Log, l =>
    {
        var folders = new List<AppFolderDataRaw>();
        if (!_onlyFiles)
        {
            var pathsFromRoot = PreparePaths(_appPaths, "");

            folders = _fileManager.GetAllTransferableFolders(/*filter*/)
                .Select(p => new DirectoryInfo(p))
                .Select(d =>
                {
                    // 2024-10-07 2dm - had to fix because the path became shorter when requesting files from a subfolder
                    // but when asking for app files/folders it must always assume/start with the root of the app
                    //var fullNameRelative = FullNameWithoutAppFolder(d.FullName, _appPaths, _root);
                    var fullNameFromAppRoot = FullNameWithoutAppFolder(d.FullName, pathsFromRoot);
                    return new AppFolderDataRaw
                    {
                        Name = $"{GetFileNameWithoutExtension(d.FullName)}{d.Extension}",
                        FullName = fullNameFromAppRoot,
                        ParentFolderInternal = fullNameFromAppRoot.BeforeLast("/"),
                        Path = fullNameFromAppRoot.BeforeLast("/") + "/",
                        Created = d.CreationTime,
                        Modified = d.LastWriteTime

                        // TODO: URL / UrlRelative
                    };
                })
                .ToList();
            folders.Insert(0, RootFolder);
        }
        else
            l.A($"onlyFiles:{_onlyFiles}");

        return (folders, $"found:{folders.Count}");
    });
    private readonly GetOnce<List<AppFolderDataRaw>> _folders = new();

    private static readonly AppFolderDataRaw RootFolder = new()
    {
        Name = "Root",
        FullName = "",
        ParentFolderInternal = "/", // it should not be empty for root folder (it is expected that its children should have empty ParentFolderInternal)
        Path = "/", // this is to show to end 
        Created = DateTime.MinValue,
        Modified = DateTime.MinValue
    };

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private static string FullNameWithoutAppFolder(string path, (string AppSitePath, bool HasShared, string AppSharedPath) paths)
    {
        if (path == null)
            return string.Empty;

        var name = path.Replace(paths.AppSitePath, string.Empty);
        //var isFolder = GetAttributes(filePath).HasFlag(FileAttributes.Directory);
        if (string.IsNullOrEmpty(name))
            return string.Empty;
        if (paths.HasShared) 
            name = name.Replace(paths.AppSharedPath, string.Empty);
        return name.ForwardSlash();
    }

    private static (string AppSitePath, bool HasShared, string AppSharedPath) PreparePaths(IAppPaths appPaths, string root)
    {
        var hasShared = appPaths.PhysicalPathShared != null;
        return (Combine(appPaths.PhysicalPath, root), hasShared, hasShared ? Combine(appPaths.PhysicalPathShared, root) : "");
    }
}