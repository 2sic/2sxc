using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.LookUp;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;
using CodeInfoService = ToSic.Eav.Code.InfoSystem.CodeInfoService;

namespace ToSic.Sxc.Apps;

/// <summary>
/// A <em>single-use</em> app-object providing quick simple api to access
/// name, folder, data, metadata etc.
/// </summary>
[PrivateApi("hide implementation - IMPORTANT: was PublicApi_Stable_ForUseInYourCode up to 16.03!")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class App : Eav.Apps.Internal.EavApp, IApp
{
    #region DI Constructors

    [PrivateApi]
    public App(MyServices services, 
        LazySvc<GlobalPaths> globalPaths, 
        LazySvc<IAppPathsMicroSvc> appPathsLazy,
        Generator<IAppStates> appStates,
        Generator<AppConfigDelegate> appConfigDelegate, 
        LazySvc<CodeDataFactory> cdf,
        LazySvc<CodeInfoService> codeChanges)
        : base(services, "App.SxcApp")
    {
        ConnectServices(
            _globalPaths = globalPaths,
            _appPathsLazy = appPathsLazy,
            _appStates = appStates,
            _appConfigDelegate = appConfigDelegate,
            _cdfLazy = cdf.SetInit(asc => asc.SetFallbacks(Site)),
            _codeChanges = codeChanges
        );
    }

    private readonly LazySvc<GlobalPaths> _globalPaths;
    private readonly LazySvc<IAppPathsMicroSvc> _appPathsLazy;
    private readonly Generator<IAppStates> _appStates;
    private readonly Generator<AppConfigDelegate> _appConfigDelegate;
    private readonly LazySvc<CodeInfoService> _codeChanges;
    private readonly LazySvc<CodeDataFactory> _cdfLazy;


    private IAppPaths AppPaths => _appPaths.Get(() => _appPathsLazy.Value.Init(Site, AppStateInt));
    private readonly GetOnce<IAppPaths> _appPaths = new();

    #endregion


    #region IApp Paths


    /// <inheritdoc cref="IApp.Path" />
    public string Path => _path.Get(() => AppPaths.Path);
    private readonly GetOnce<string> _path = new();

    /// <inheritdoc cref="IApp.Thumbnail" />
    public string Thumbnail => (this as IAppTyped).Thumbnail.Url;

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


}