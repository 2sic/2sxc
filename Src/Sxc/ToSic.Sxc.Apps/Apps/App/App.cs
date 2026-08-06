using ToSic.Eav.Apps.Sys.Paths;
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
    SxcAppBase.Dependencies services,
    LazySvc<GlobalPaths> globalPaths,
    LazySvc<ICodeDataFactory> cdfLazy,
    LazySvc<CodeInfoService> codeChanges,
    IAppPathsMicroSvc pathFactoryTemp)
    // Note: If this is ever changed to not inherit from the EavApp, make sure you correct/update the LightSpeed code as well as it checks for this base class
    : SxcAppBase(services, "App.SxcApp", connect: [globalPaths, cdfLazy, codeChanges, pathFactoryTemp]),
        IAppWithInternal
{
    #region Special objects

    IAppReader IAppWithInternal.AppReader => AppReaderInt;

    private ICodeDataFactory Cdf => field ??= cdfLazy.SetInit(obj => obj.SetFallbacks(MySite)).Value;


    private IAppPaths AppPaths => field ??= pathFactoryTemp.Get(AppReaderInt, MySite);

    #endregion


    #region IApp Paths


    /// <inheritdoc cref="IApp.Path" />
    public string Path => field ??= AppPaths.Path;


    /// <inheritdoc cref="IApp.Thumbnail" />
    public string? Thumbnail => _thumbnail.Get(() => new AppAssetThumbnail(AppReaderInt, AppPaths, globalPaths).Url);
    private readonly GetOnce<string?> _thumbnail = new();

    /// <inheritdoc cref="IApp.PathShared" />
    public string PathShared => field ??= AppPaths.PathShared;

    /// <inheritdoc cref="IApp.PhysicalPathShared" />
    public string PhysicalPathShared => field ??= AppPaths.PhysicalPathShared;

    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePath => field ??= AppPaths.RelativePath;


    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePathShared => field ??= AppPaths.RelativePathShared;


    #endregion

    #region Special internal properties for the IAppTyped wrapper. It will need these properties, but they are protected

    internal IAppPaths AppPathsForTyped => AppPaths;
    internal IAppReader AppReaderForTyped => AppReaderInt;

    internal TResult BuildDataForTyped<TDataSource, TResult>() where TDataSource : TResult where TResult : class, IDataSource
        => BuildData<TDataSource, TResult>();

    #endregion

}