using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;

namespace ToSic.Sxc.Apps.Sys;

public partial class AppForCodeShared
{
    /// <summary>
    /// Options
    /// </summary>
    /// <param name="ReplaceSite"></param>
    /// <param name="AppIdentity"></param>
    /// <param name="DataSpecs"></param>
    public record Options(ISite? ReplaceSite, IAppIdentityPure AppIdentity, AppDataConfigSpecs? DataSpecs);

    /// <summary>
    /// Dependencies
    /// </summary>
    [PrivateApi]
    public record Dependencies(
        IZoneMapper ZoneMapper,
        ISite Site,
        IAppReaderFactory AppReaders,
        IAppPathsMicroSvc AppPathSvc,
        Generator<AppForCodeDataSourceService, AppForCodeDataSourceService.Options> AppDataSvc)
        : DependenciesBase(connect: [ZoneMapper, Site, AppReaders, AppPathSvc, AppDataSvc]);
}