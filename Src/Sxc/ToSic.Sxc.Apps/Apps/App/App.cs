﻿using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps.Internal.Assets;
using ToSic.Sxc.Data.Internal;
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
    EavApp.MyServices services,
    LazySvc<GlobalPaths> globalPaths,
    LazySvc<ICodeDataFactory> cdfLazy,
    LazySvc<CodeInfoService> codeChanges,
    IAppPathsMicroSvc pathFactoryTemp)
    // Note: If this is ever changed to not inherit from the EavApp, make sure you correct/update the LightSpeed code as well as it checks for this base class
    : EavApp(services, "App.SxcApp", connect: [globalPaths, cdfLazy, codeChanges, pathFactoryTemp]), IApp
{
    #region Special objects

    private ICodeDataFactory Cdf => _cdf.Get(() => cdfLazy.SetInit(obj => obj.SetFallbacks(Site)).Value);
    private readonly GetOnce<ICodeDataFactory> _cdf = new();


    private IAppPaths AppPaths => _appPaths.Get(() => pathFactoryTemp.Get(AppReaderInt, Site));
    private readonly GetOnce<IAppPaths> _appPaths = new();

    #endregion


    #region IApp Paths


    /// <inheritdoc cref="IApp.Path" />
    public string Path => _path.Get(() => AppPaths.Path);
    private readonly GetOnce<string> _path = new();

    /// <inheritdoc cref="IApp.Thumbnail" />
    public string Thumbnail => _thumbnail.Get(() => new AppAssetThumbnail(AppReaderInt, AppPaths, globalPaths).Url);
    private readonly GetOnce<string> _thumbnail = new();

    /// <inheritdoc cref="IApp.PathShared" />
    public string PathShared => _pathShared.Get(() => AppPaths.PathShared);
    private readonly GetOnce<string> _pathShared = new();

    /// <inheritdoc cref="IApp.PhysicalPathShared" />
    public string PhysicalPathShared => _physicalPathGlobal.Get(() => AppPaths.PhysicalPathShared);
    private readonly GetOnce<string> _physicalPathGlobal = new();

    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePath => _relativePath.Get(() => AppPaths.RelativePath);
    private readonly GetOnce<string> _relativePath = new();


    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePathShared => _relativePathShared.Get(() => AppPaths.RelativePathShared);
    private readonly GetOnce<string> _relativePathShared = new();


    #endregion

    #region Special internal properties for the IAppTyped wrapper. It will need these properties, but they are protected

    internal IAppPaths AppPathsForTyped => AppPaths;
    internal IAppReader AppReaderForTyped => AppReaderInt;
    internal IEntity AppSettingsForTyped => AppSettings;
    internal IEntity AppResourcesForTyped => AppResources;

    internal TResult BuildDataForTyped<TDataSource, TResult>() where TDataSource : TResult where TResult : class, IDataSource
        => BuildData<TDataSource, TResult>();

    #endregion

}