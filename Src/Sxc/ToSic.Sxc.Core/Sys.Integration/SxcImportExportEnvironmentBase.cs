using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.ImportExport.Integration;

namespace ToSic.Sxc.Sys.Integration;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class SxcImportExportEnvironmentBase: EavImportExportEnvironmentBase
{
    #region constructor / DI

    public class Dependencies(ISite site, IAppReaderFactory appReaders, IAppsCatalog appsCatalog, IAppPathsMicroSvc appPaths)
        : DependenciesBase(connect: [site, appReaders, appPaths])
    {
        internal readonly IAppPathsMicroSvc AppPaths = appPaths;
        internal readonly IAppsCatalog AppsCatalog = appsCatalog;
        internal readonly IAppReaderFactory AppReaders = appReaders;
        internal readonly ISite Site = site;
    }


    /// <summary>
    /// DI Constructor
    /// </summary>
    protected SxcImportExportEnvironmentBase(Dependencies services, string logName) : base(services.Site, services.AppsCatalog, logName)
    {
        _services = services.ConnectServices(Log);
    }

    private readonly Dependencies _services;

    #endregion

    public override string FallbackContentTypeScope => ScopeConstants.Default;

    public override string TemplatesRoot(int zoneId, int appId) 
        => AppPaths(zoneId, appId).PhysicalPath;

    public override string GlobalTemplatesRoot(int zoneId, int appId) 
        => AppPaths(zoneId, appId).PhysicalPathShared;

    private IAppPaths AppPaths(int zoneId, int appId) => _appPaths
        ??= _services.AppPaths.Get(_services.AppReaders.Get(new AppIdentity(zoneId, appId)), _services.Site);
    private IAppPaths? _appPaths;


}