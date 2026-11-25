using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Razor.Blade;
using ToSic.Sxc.Cms.Assets.Sys;
using ToSic.Sys.Work;

namespace ToSic.Sxc.DataSources.Sys.AppAssets;

/// <summary>
/// Base class to provide data to the AppFiles DataSource.
///
/// Must be overriden in each platform.
/// </summary>
/// <param name="fileManagerGenerator">Note that we will use Generators for safety, because in rare cases the dependencies could be re-used to create a sub-data-source</param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppAssetsDataSourceProvider(IAppReaderFactory appReaders, IAppPathsMicroSvc appPathMicroSvc, Generator<AppFileManager> fileManagerGenerator)
    : ServiceWithSetup<AppAssetsDataSourceProvider.Options>($"{SxcLogName}.AppFls", connect: [appReaders, appPathMicroSvc, fileManagerGenerator])
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public record Options : IWorkSpecs
    {
        public int AppId { get; init; } = int.MinValue;
        public int ZoneId { get; init; } = int.MinValue;
        public string? RootFolder { get; init; } = "/";
        public string? FileFilter { get; init; } = "*.*";
    }

    private string? Root => field ??= MyOptions.RootFolder.TrimPrefixSlash().ToSystemPath();
    private string? Filter => field ??= MyOptions.FileFilter;
    private AppFileManager AppFileManager => field ??= fileManagerGenerator.New().SetFolder(MyOptions.AppId, AppPaths.PhysicalPath, Root);

    private IAppPaths AppPaths =>
        field ??= appPathMicroSvc.Get(appReaders.Get(new AppIdentity(MyOptions.ZoneId, MyOptions.AppId)));

    /// <summary>
    /// FYI: The filters are not actually implemented yet.
    /// So the core data source doesn't have settings to configure this
    /// </summary>
    /// <returns></returns>
    public (List<FolderModelRaw> Folders, List<FileModelRaw> Files) GetAll()
        => Log.Quick(() => (Folders, Files));

    public List<FileModelRaw> Files => field ??= GetFiles();

    public List<FileModelRaw> GetFiles()
    {
        var l = Log.Fn<List<FileModelRaw>>();
        var pathsFromRoot = PreparePaths(AppPaths, "");

        var files = AppFileManager.GetAllTransferableFiles(Filter)
            .Select(p => new FileInfo(p))
            .Select(f =>
            {
                var fullNameFromAppRoot = FullNameWithoutAppFolder(f.FullName, pathsFromRoot);
                var name = Path.GetFileNameWithoutExtension(f.FullName);
                var path = fullNameFromAppRoot.TrimPrefixSlash();
                return new FileModelRaw
                {
                    Name = name,
                    Extension = f.Extension.TrimStart('.'), // Extension is without the dot
                    FullName = $"{name}{f.Extension}",
                    ParentFolderInternal = path.BeforeLast("/").SuffixSlash(),
                    Path = path,
                    // TODO convert characters for safe HTML
                    Url = $"{AppPaths.Path}{fullNameFromAppRoot}",

                    Size = (int)f.Length,
                    Created = f.CreationTime,
                    Modified = f.LastWriteTime
                };
            })
            .ToList();

        return l.Return(files, $"files:{files.Count}");
    }

    public List<FolderModelRaw> Folders => field ??= GetFolders();

    private List<FolderModelRaw> GetFolders()
    {
        var l = Log.Fn<List<FolderModelRaw>>();
        var pathsFromRoot = PreparePaths(AppPaths, "");

        var folders = AppFileManager.GetAllTransferableFolders(/*filter*/)
            .Select(p => new DirectoryInfo(p))
            .Select(d => ToFolderData(d, pathsFromRoot))
            .ToList();

        // if the root is just "/" then we need to add the root folder, otherwise not
        var root = new DirectoryInfo(
            $"{AppPaths.PhysicalPath}/{Root}"
                .FlattenMultipleForwardSlashes()
                .TrimLastSlash()
        );
        folders.Insert(0, ToFolderData(root, pathsFromRoot) with
        {
            Name = "",                  // Make name blank, since it's the root folder
            ParentFolderInternal = "",  // reset the ParentFolder, otherwise the root thinks it's a subfolder of itself
        });

        return l.Return(folders, $"found:{folders.Count}");
    }

    private FolderModelRaw ToFolderData(DirectoryInfo d, PreparedPaths pathsFromRoot)
    {
        var fullNameFromAppRoot = FullNameWithoutAppFolder(d.FullName, pathsFromRoot);

        var name = Path.GetFileName(d.FullName);

        return new()
        {
            Name = name,
            FullName = name,
            ParentFolderInternal = fullNameFromAppRoot.TrimPrefixSlash().BeforeLast("/").SuffixSlash(),
            Path = fullNameFromAppRoot.TrimPrefixSlash().SuffixSlash(),
            Created = d.CreationTime,
            Modified = d.LastWriteTime,
            Url = $"{AppPaths.Path}{fullNameFromAppRoot}",
        };
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private static string FullNameWithoutAppFolder(string? path, PreparedPaths paths)
    {
        if (path == null)
            return string.Empty;

        var name = path.Replace(paths.AppSitePath, string.Empty);
        if (string.IsNullOrEmpty(name))
            return string.Empty;
        if (paths.HasShared) 
            name = name.Replace(paths.AppSharedPath, string.Empty);
        return name.ForwardSlash();
    }

    private static PreparedPaths PreparePaths(IAppPaths appPaths, string root)
    {
        var hasShared = appPaths.PhysicalPathShared != null! /* paranoid */;
        var appSharedPath = hasShared
            ? Path.Combine(appPaths.PhysicalPathShared!, root)
            : "";
        return new(Path.Combine(appPaths.PhysicalPath, root), hasShared, appSharedPath);
    }

    private record PreparedPaths(string AppSitePath, bool HasShared, string AppSharedPath);
}
