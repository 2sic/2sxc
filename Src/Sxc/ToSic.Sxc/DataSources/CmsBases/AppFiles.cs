using System.Collections.Immutable;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Lib.Helpers;
using ToSic.Sxc.DataSources.Internal;
using static System.StringComparer;
using static ToSic.Eav.DataSource.Internal.DataSourceConstants;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of App files and folders from the current platform (Dnn or Oqtane).
///
/// As of now there are no parameters to set.
///
/// To figure out the properties returned and what they match up to, see <see cref="PageDataRaw"/> TODO
/// </summary>
[VisualQuery(
    ConfigurationType = "477d5de4-5ffa-43ef-8553-37354cb27660", // TODO:
    NameId = "3fe6c215-4c37-45c1-8883-b4b2a47162a7",
    HelpLink = "https://go.2sxc.org/ds-appfiles",
    Icon = DataSourceIcons.Tree,
    NiceName = "AppFiles",
    Type = DataSourceType.Source,
    Audience = Audience.Advanced,
    UiHint = "Files and folders in the App folder")]
[PrivateApi("Was till v17 InternalApi_DoNotUse_MayChangeWithoutNotice(still wip / finishing specs etc.)")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppFiles: CustomDataSourceAdvanced
{
    private readonly IDataFactory _dataFactory;
    private readonly AppFilesDataSourceProvider _provider;

    private const string StreamFiles = "Files";
    private const string StreamFolders = "Folders";
    private const string StreamAll = "All";

    #region Configuration properties

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration(Fallback = false)]
    public bool OnlyFolders => Configuration.GetThis(false);

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration(Fallback = false)]
    public bool OnlyFiles => Configuration.GetThis(false);

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration(Fallback = "/")]
    public string RootFolder => Configuration.GetThis();

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration(Fallback = "*.*")]
    public string FileFilter => Configuration.GetThis();

    // TODO: not implemented yet!
    [Configuration(Fallback = false)]
    public bool SearchSubfolders => Configuration.GetThis(false);

    #endregion

    #region Constructor

    [PrivateApi]
    public AppFiles(MyServices services, AppFilesDataSourceProvider provider, IDataFactory dataFactory) : base(services, "CDS.AppFiles", connect: [provider, dataFactory])
    {
        _provider = provider;
        _dataFactory = dataFactory;

        ProvideOut(() => Get(StreamDefaultName));
        ProvideOut(() => Get(StreamFolders), StreamFolders);
        ProvideOut(() => Get(StreamFiles), StreamFiles);
        ProvideOut(() => Get(StreamAll), StreamAll);
    }
    #endregion

    /// <summary>
    /// Mini-cache.
    /// Reason is that we can only generate the streams together, so this ensures that after generating them once,
    /// other streams requested at the same time won't re-trigger stream generation.
    /// </summary>
    private IImmutableList<IEntity> Get(string streamName)
    {
        // check in both values
        var toCheck = RootFolder + " " + FileFilter;
        if (toCheck.Contains(".."))
            return Error.Create(title: "Invalid characters in RootFolder or FileFilter", message: "The sequence '..' is not allowed in the path or file filter.", streamName: streamName);

        var all = GetAll();
        return all.TryGetValue(streamName, out var stream)
            ? stream()
            : Error.TryGetOutFailed(name: streamName);
    }

    /// <summary>
    /// Mini-cache.
    /// Reason is that we can only generate the streams together, so this ensures that after generating them once,
    /// other streams requested at the same time won't re-trigger stream generation.
    /// </summary>
    private IDictionary<string, Func<IImmutableList<IEntity>>> GetAll() => _all.Get(() =>
    {
        var (folders, files) = GetInternal();
        return new(OrdinalIgnoreCase)
        {
            { StreamDefaultName, () => files },
            { StreamAll, () => folders.Concat(files).ToImmutableList() },
            { StreamFolders, () => folders },
            { StreamFiles, () => files }
        };
    });
    private readonly GetOnce<Dictionary<string, Func<IImmutableList<IEntity>>>> _all = new();

    /// <summary>
    /// Get both the files and folders stream
    /// </summary>
    /// <returns></returns>
    private (IImmutableList<IEntity> folders, IImmutableList<IEntity> files) GetInternal()
    {
        var l = Log.Fn<(IImmutableList<IEntity> folders, IImmutableList<IEntity> files)>(timer: true);
        //Configuration.Parse();

        // TODO:
        // must ensure that the folder can't contain ".." characters or anything else to make it go outside of the app folder

        _provider.Configure(zoneId: ZoneId, appId: AppId, onlyFolders: OnlyFolders, onlyFiles: OnlyFiles, root: RootFolder, filter: FileFilter);

        // Get pages from underlying system/provider
        var (rawFolders, rawFiles) = _provider.GetAll();
        if (!rawFiles.Any() && !rawFolders.Any())
            return l.Return((EmptyList, EmptyList), "null/empty");

        // Convert Folders to Entities
        var folderFactory = _dataFactory.New(options: new(AppFolderDataRaw.Options, appId: AppId));
        var folders = folderFactory.Create(rawFolders);

        // Convert Files to Entities
        var fileFactory = _dataFactory.New(options: new(AppFileDataRaw.Options, appId: AppId),
            // Make sure we share relationships source with folders, as files need folders and folders need files
            relationships: folderFactory.Relationships);
        var files = fileFactory.Create(rawFiles);

        return l.Return((folders, files), $"folders: {folders.Count}, files: {files.Count}");
    }


}