using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Data.Raw;
using ToSic.Sxc.Backend.ImportExport;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "App Statistics",
    NameId = "2bb27fae-fc52-412c-91f0-3276a4567dec",
    NameIds = ["System.AppStatistics"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Export statistics for the current app"
)]
// ReSharper disable once UnusedMember.Global
public class AppStatistics(CustomDataSource.Dependencies services, ExportAppInfo exportAppLazy)
    : CustomDataSource(services, logName: "Sxc.AppStats", connect: [exportAppLazy])
{
    [Configuration(Field = "ZoneId")]
    public int OfZoneId => Configuration.GetThis(ZoneId);

    protected override IEnumerable<IRawData> GetDefault()
        => [exportAppLazy.GetAppInfo(new AppIdentity(OfZoneId, AppId))];
}
