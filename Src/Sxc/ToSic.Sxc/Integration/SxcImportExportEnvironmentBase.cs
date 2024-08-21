using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Lib.Services;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Integration;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class SxcImportExportEnvironmentBase: EavImportExportEnvironmentBase
{
    #region constructor / DI

    public class MyServices: MyServicesBase
    {
        internal readonly IAppPathsMicroSvc AppPaths;
        internal readonly IAppStates AppStates;
        internal readonly ISite Site;
        internal readonly App NewApp;

        public MyServices(ISite site, App newApp, IAppStates appStates, IAppPathsMicroSvc appPaths)
        {
            ConnectLogs([
                AppPaths = appPaths,
                AppStates = appStates,
                Site = site,
                NewApp = newApp
            ]);
        }
    }


    /// <summary>
    /// DI Constructor
    /// </summary>
    protected SxcImportExportEnvironmentBase(MyServices services, string logName) : base(services.Site, services.AppStates, logName)
    {
        _services = services.ConnectServices(Log);
    }

    private readonly MyServices _services;

    #endregion

    public override string FallbackContentTypeScope => Scopes.Default;

    public override string TemplatesRoot(int zoneId, int appId) 
        => AppPaths(zoneId, appId).PhysicalPath;

    public override string GlobalTemplatesRoot(int zoneId, int appId) 
        => AppPaths(zoneId, appId).PhysicalPathShared;

    private IAppPaths AppPaths(int zoneId, int appId) => _appPaths
        ??= _services.AppPaths.Init(_services.Site, _services.AppStates.Get(new AppIdentity(zoneId, appId)));
    private IAppPaths _appPaths;


}