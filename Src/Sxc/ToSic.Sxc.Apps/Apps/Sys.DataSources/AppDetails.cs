using ToSic.Eav.Context;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.WebApi.Sys.App.Details;
using ToSic.Sxc.Apps.Sys.Work;

namespace ToSic.Sxc.Apps.Sys.DataSources;

[VisualQuery(
    NiceName = "Apps",
    NameId = "53b3fe9b-d689-4b1f-bed1-503cbc898ffc",
    NameIds = ["System.AppDetails"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Apps of the current site")]
[ShowApiWhenReleased(ShowApiMode.Never)]
// ReSharper disable once UnusedMember.Global
public class AppDetails(CustomDataSource.Dependencies services, IAppDetailsService appsInfoSvc, WorkApps workApps, ISite site)
    : CustomDataSource(services, logName: "Sxc.Apps", connect: [appsInfoSvc, site, workApps])
{
    protected override IEnumerable<IRawData> GetDefault()
        => workApps.GetApps(site)
            .Select(app => appsInfoSvc.GetDetails(site, app))
            .ToListOpt();
}
