using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Apps;

/// <summary>
/// A <em>single-use</em> app-object providing quick simple api to access
/// name, folder, data, metadata etc.
/// </summary>
[PrivateApi("hide implementation - IMPORTANT: was PublicApi_Stable_ForUseInYourCode up to 16.03!")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[method: PrivateApi]
public partial class AppDynamic(LazySvc<GlobalPaths> globalPaths, LazySvc<ICodeDataFactory> cdfLazy, LazySvc<QueryManager> queryManager)
    // Note: If this is ever changed to not inherit from the EavApp, make sure you correct/update the LightSpeed code as well as it checks for this base class
    : ServiceWithSetup<IAppForCode>("App.SxcApp", connect: [globalPaths, cdfLazy, queryManager]),
        IApp, IAppForCode
{

    #region IApp (old dynamic API) Properties

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    string IApp.Name => field ??= MyOptions.AppSpecs.Name;

    #endregion

    /// <inheritdoc />
    [Obsolete]
    [PrivateApi]
    string IApp.AppGuid => MyOptions.AppSpecs.NameId;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    string IApp.NameId => field ??= MyOptions.AppSpecs.NameId;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    string IApp.Folder => field ??= MyOptions.AppSpecs.Folder;


    #region IApp Paths


    /// <inheritdoc cref="IApp.Path" />
    public string Path => field ??= MyOptions.AppPaths.Path;


    /// <inheritdoc cref="IApp.Thumbnail" />
    string? IApp.Thumbnail => _thumbnail.Get(() => AppAssetThumbnail.GetUrl(MyOptions.AppReader, MyOptions.AppPaths, globalPaths));
    private readonly LazyGet<string?> _thumbnail = new();

    /// <inheritdoc cref="IApp.PathShared" />
    public string PathShared => field ??= MyOptions.AppPaths.PathShared;

    [PrivateApi]
    public string PhysicalPath
        => MyOptions.AppPaths.PhysicalPath;

    /// <inheritdoc cref="IApp.PhysicalPathShared" />
    public string PhysicalPathShared => field ??= MyOptions.AppPaths.PhysicalPathShared;

    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePath => field ??= MyOptions.AppPaths.RelativePath;


    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePathShared => field ??= MyOptions.AppPaths.RelativePathShared;


    #endregion

    #region Metadata and Permission Accessors

    /// <inheritdoc />
    public IMetadata Metadata => MyOptions.AppSpecs.Metadata;

    #endregion


    public int ZoneId => MyOptions.ZoneId;

    public int AppId => MyOptions.AppId;

    public IEntity? SettingsEntity => MyOptions.SettingsEntity;

    public IEntity? ResourcesEntity => MyOptions.ResourcesEntity;

    public IAppPaths AppPaths => MyOptions.AppPaths;

    public IAppSpecs AppSpecs => MyOptions.AppSpecs;

    public IAppReader AppReader => MyOptions.AppReader;

    public AppForCodeDataSourceService DataService => MyOptions.DataService;
}