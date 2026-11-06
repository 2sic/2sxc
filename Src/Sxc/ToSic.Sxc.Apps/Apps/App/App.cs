using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Data.Sys.Factory;
using CodeInfoService = ToSic.Sys.Code.InfoSystem.CodeInfoService;

namespace ToSic.Sxc.Apps;

/// <summary>
/// A <em>single-use</em> app-object providing quick simple api to access
/// name, folder, data, metadata etc.
/// </summary>
[PrivateApi("hide implementation - IMPORTANT: was PublicApi_Stable_ForUseInYourCode up to 16.03!")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[method: PrivateApi]
public partial class App(
    AppForCodeShared.Dependencies services,
    LazySvc<GlobalPaths> globalPaths,
    LazySvc<ICodeDataFactory> cdfLazy,
    LazySvc<CodeInfoService> codeChanges,
    LazySvc<QueryManager> queryManager)
    // Note: If this is ever changed to not inherit from the EavApp, make sure you correct/update the LightSpeed code as well as it checks for this base class
    : SxcAppBase(services, "App.SxcApp", connect: [globalPaths, cdfLazy, codeChanges, queryManager]),
        IApp, IAppForCode
{

    #region IApp (old dynamic API) Properties

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public string Name => field ??= AppSource.AppSpecs.Name;


    private IAppForCode AppSource => field ??= this;

    #endregion

    /// <inheritdoc />
    [Obsolete]
    [PrivateApi]
    public string AppGuid => NameId;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public string NameId => field ??= AppSource.AppSpecs.NameId;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public string Folder => field ??= AppSource.AppSpecs.Folder;


    #region IApp Paths


    /// <inheritdoc cref="IApp.Path" />
    public string Path => field ??= AppSource.AppPaths.Path;


    /// <inheritdoc cref="IApp.Thumbnail" />
    public string? Thumbnail => _thumbnail.Get(() => new AppAssetThumbnail(AppSource.AppReader, AppSource.AppPaths, globalPaths).Url);
    private readonly LazyGet<string?> _thumbnail = new();

    /// <inheritdoc cref="IApp.PathShared" />
    public string PathShared => field ??= AppSource.AppPaths.PathShared;

    [PrivateApi]
    public string PhysicalPath
        => AppSource.AppPaths.PhysicalPath;

    /// <inheritdoc cref="IApp.PhysicalPathShared" />
    public string PhysicalPathShared => field ??= AppSource.AppPaths.PhysicalPathShared;

    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePath => field ??= AppSource.AppPaths.RelativePath;


    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePathShared => field ??= AppSource.AppPaths.RelativePathShared;


    #endregion

    #region Metadata and Permission Accessors

    /// <inheritdoc />
    public IMetadata Metadata => AppSource.AppSpecs.Metadata;

    #endregion


}