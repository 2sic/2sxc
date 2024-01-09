using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.DataSources.Internal;
using static System.StringComparer;
using static ToSic.Eav.DataSource.DataSourceConstants;

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
[InternalApi_DoNotUse_MayChangeWithoutNotice("still wip / finishing specs etc.")]
[VisualQuery(
    ConfigurationType = "", // TODO:
    NameId = "3fe6c215-4c37-45c1-8883-b4b2a47162a7",
    HelpLink = "https://go.2sxc.org/ds-appfiles",
    Icon = Icons.Tree,
    NiceName = "AppFiles",
    Type = DataSourceType.Source,
    Audience = Audience.Advanced,
    UiHint = "Files and folders in the App folder")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppFiles: CustomDataSourceAdvanced
{
    private readonly IDataFactory _dataFactory;
    private readonly AppFilesDataSourceProvider _provider;

    private const string StreamFiles = "Files";
    private const string StreamFolders = "Folders";

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
    public string Root => Configuration.GetThis();

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration(Fallback = "*.*")]
    public string Filter
        => Configuration.GetThis();

    #endregion

    #region Constructor

    [PrivateApi]
    public AppFiles(MyServices services, AppFilesDataSourceProvider provider, IDataFactory dataFactory) : base(services, "CDS.AppFiles")
    {
        ConnectServices(
            _provider = provider,
            _dataFactory = dataFactory
        );

        ProvideOut(() => GetMultiAccess(StreamDefaultName));
        ProvideOut(() => GetMultiAccess(StreamFolders), StreamFolders);
        ProvideOut(() => GetMultiAccess(StreamFiles), StreamFiles);
    }
    #endregion

    /// <summary>
    /// Mini-cache.
    /// Reason is that we can only generate the streams together, so this ensures that after generating them once,
    /// other streams requested at the same time won't re-trigger stream generation.
    /// </summary>
    private IImmutableList<IEntity> GetMultiAccess(string streamName) => _multiAccess.Get(() =>
    {
        var (folders, files) = GetInternal();
        return new Dictionary<string, IImmutableList<IEntity>>(OrdinalIgnoreCase)
        {
            { StreamDefaultName, folders.Concat(files).ToImmutableList() },
            { StreamFolders, folders },
            { StreamFiles, files }
        };
    })[streamName];
    private readonly GetOnce<Dictionary<string, IImmutableList<IEntity>>> _multiAccess = new();

    /// <summary>
    /// Get both the files and folders stream
    /// </summary>
    /// <returns></returns>
    private (IImmutableList<IEntity> folders, IImmutableList<IEntity> files) GetInternal() => Log.Func(l =>
    {
        Configuration.Parse();

        _provider.Configure(zoneId: ZoneId, appId: AppId, onlyFolders: OnlyFolders, onlyFiles: OnlyFiles, root: Root, filter: Filter);

        // Get pages from underlying system/provider
        var (rawFolders, rawFiles) = _provider.GetAll();
        if (!rawFiles.Any() && !rawFolders.Any())
            return ((EmptyList, EmptyList), "null/empty");

        // Convert Folders to Entities
        var folderFactory = _dataFactory.New(options: new DataFactoryOptions(AppFolderDataRaw.Options, appId: AppId));
        var folders = folderFactory.Create(rawFolders);

        // Convert Files to Entities
        var fileFactory = _dataFactory.New(options: new DataFactoryOptions(AppFileDataRaw.Options, appId: AppId),
            // Make sure we share relationships source with folders, as files need folders and folders need files
            relationships: folderFactory.Relationships);
        var files = fileFactory.Create(rawFiles);

        return ((folders, files), $"folders: {folders.Count}, files: {files.Count}");
    });


}