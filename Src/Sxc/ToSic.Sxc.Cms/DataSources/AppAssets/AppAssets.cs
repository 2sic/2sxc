using System.Collections.Immutable;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;

using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Data.Sys.Entities.Sources;
using ToSic.Sxc.Cms.Assets;
using ToSic.Sxc.Cms.Assets.Sys;
using ToSic.Sxc.DataSources.Sys.AppAssets;
using static System.StringComparer;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of App files and folders from the current platform (Dnn or Oqtane).
/// </summary>
/// <remarks>
///
/// This provides 4 streams:
///
/// * All: Stream containing both files and folders
/// * Default: All files <see cref="IFileModel"/>
/// * Files: All Files <see cref="IFileModel"/>
/// * Folders: All folders <see cref="IFolderModel"/>
///
/// To figure out the properties returned and what they match up to, see <see cref="IFileModel"/> and <see cref="IFolderModel"/>.
/// 
/// History
/// 
/// * Started v18.02 for the first time - in the Picker Source App Assets.
/// * officially documented for v19.00, but API not fully final/stable, names may change.
/// </remarks>
[VisualQuery(
    NiceName = "App Assets",
    Type = DataSourceType.Source,
    ConfigurationType = "477d5de4-5ffa-43ef-8553-37354cb27660",
    NameId = "3fe6c215-4c37-45c1-8883-b4b2a47162a7",
    HelpLink = "https://go.2sxc.org/ds-appassets",
    Icon = DataSourceIcons.Tree,
    Audience = Audience.Advanced,
    UiHint = "Files and folders in the App folder")]
[PublicApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppAssets: CustomDataSource
{
    private readonly AppAssetsDataSourceProvider _appAssetsSource;

    private const string StreamFiles = "Files";
    private const string StreamFolders = "Folders";
    private const string StreamAll = "All";

    #region Configuration properties

    /// <summary>
    /// The root folder to start from, beginning in the app root.
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration(Fallback = "/")]
    public string RootFolder => Configuration.GetThis(fallback: "/");

    /// <summary>
    /// The file name filter, such as "*.jpg".
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration(Fallback = "*.*")]
    public string FileFilter => Configuration.GetThis(fallback: "*.*");

    // TODO: not implemented yet!
    [PrivateApi("TODO: NOT IMPLEMENTED YET")]
    [Configuration(Fallback = false)]
    public bool SearchSubfolders => Configuration.GetThis(false);

    private AppAssetsGetSpecs Specs => new()
    {
        RootFolder = RootFolder,
        FileFilter = FileFilter,
        //SearchSubfolders = SearchSubfolders
    };

    #endregion

    #region Constructor

    [PrivateApi]
    public AppAssets(Dependencies services, AppAssetsDataSourceProvider appAssetsSource) : base(services, "CDS.AppFiles", connect: [appAssetsSource])
    {
        _appAssetsSource = appAssetsSource;

        ProvideOut(GetFiles, options: FilesOptions);
        ProvideOut(GetFolders, name: StreamFolders, options: FoldersOptions);
        ProvideOut(() => Out[DataSourceConstants.StreamDefaultName].List, StreamFiles);
        ProvideOut(GetAll, StreamAll);
    }
    #endregion

    private readonly LazyLookup<object, IEntity> _relationships = new();

    private DataFactoryOptions FilesOptions() => new()
    {
        AppId = AppId,
        IdSeed = -1,
        Relationships = _relationships,
    };

    private DataFactoryOptions FoldersOptions() => new()
    {
        AppId = AppId,
        IdSeed = -100001,
        Relationships = _relationships,
    };

    private object GetFiles()
    {
        if (InvalidPathOrFilter())
            return InvalidPathError(DataSourceConstants.StreamDefaultName);

        EnsureOtherStream(StreamFolders);
        return Raw.Files;
    }

    private object GetFolders()
    {
        if (InvalidPathOrFilter())
            return InvalidPathError(StreamFolders);

        EnsureOtherStream(DataSourceConstants.StreamDefaultName);
        return Raw.Folders;
    }

    private IEnumerable<IEntity> GetAll()
        => Out[StreamFolders].List
            .Concat(Out[DataSourceConstants.StreamDefaultName].List)
            .ToImmutableOpt();

    private bool InvalidPathOrFilter() => (RootFolder + " " + FileFilter).Contains("..");

    private IImmutableList<IEntity> InvalidPathError(string streamName)
        => Error.Create(
            title: "Invalid characters in RootFolder or FileFilter",
            message: "The sequence '..' is not allowed in the path or file filter.",
            streamName: streamName);

    private void EnsureOtherStream(string streamName)
    {
        if (_preparingRelationships)
            return;

        try
        {
            _preparingRelationships = true;
            _ = Out[streamName].List;
        }
        finally
        {
            _preparingRelationships = false;
        }
    }
    private bool _preparingRelationships;

    private (IImmutableList<FolderModelRaw> Folders, IImmutableList<FileModelRaw> Files) Raw
        => _raw.Get(GetRaw)!;
    private readonly LazyGet<(IImmutableList<FolderModelRaw> Folders, IImmutableList<FileModelRaw> Files)> _raw = new();

    private (IImmutableList<FolderModelRaw> Folders, IImmutableList<FileModelRaw> Files) GetRaw()
    {
        var l = Log.Fn<(IImmutableList<FolderModelRaw>, IImmutableList<FileModelRaw>)>(timer: true);

        var specs = Specs with
        {
            AppId = Specs.AppId == int.MinValue ? AppId : Specs.AppId,
            ZoneId = Specs.ZoneId == int.MinValue ? ZoneId : Specs.ZoneId,
        };

        _appAssetsSource.Configure(specs);
        var (folders, files) = _appAssetsSource.GetAll();

        var rawFolders = folders.ToImmutableOpt();
        var rawFiles = files.ToImmutableOpt();
        return l.Return((rawFolders, rawFiles), $"folders: {rawFolders.Count}, files: {rawFiles.Count}");
    }

}