using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.WebApi.Sys.ImportExport;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "Apps Pending Initialization",
    NameId = "746f371d-6fd4-4834-a559-cb4a1ae2ec9e",
    NameIds = ["System.AppsPendingInitialization"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.System,
    UiHint = "App packages waiting to be initialized")]
public class AppsPendingInitialization : CustomDataSource
{
    [Configuration(Field = "ZoneId")]
    public int OfZoneId => Configuration.GetThis(ZoneId);

    public AppsPendingInitialization(Dependencies services, LazySvc<ImportApp> importApp)
        : base(services, "Sxc.PendingApps", connect: [importApp])
        => ProvideOutRaw(() => Get(importApp), options: Options);

    private IEnumerable<PendingAppDto> Get(LazySvc<ImportApp> importApp)
        => importApp.Value.GetPendingApps(OfZoneId);

    private static DataFactoryOptions Options() => new() { TypeName = "PendingApp", AllowUnknownValueTypes = true };
}
