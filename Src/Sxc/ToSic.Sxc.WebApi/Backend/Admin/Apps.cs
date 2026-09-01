using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.App;

namespace ToSic.Eav.WebApi.Sys.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "Apps",
    NameId = "53b3fe9b-d689-4b1f-bed1-503cbc898ffc",
    NameIds = ["System.Apps"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Apps of the current site"
)]
public class Apps : CustomDataSource
{
    public Apps(
        Dependencies services,
        LazySvc<AppsBackend> appsBackend)
        : base(services, logName: "Sxc.Apps", connect: [appsBackend])
    {
        ProvideOutRaw(appsBackend.Value.Apps);
    }
}
