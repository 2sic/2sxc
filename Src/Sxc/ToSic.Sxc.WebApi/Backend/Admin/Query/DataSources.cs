using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.Backend.Admin.Query;

[PrivateApi]
[VisualQuery(
    NiceName = "Data Sources",
    NameId = "2de41d89-3cb9-480a-ac3c-40f77fd3af4e",
    NameIds = ["System.DataSources"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Data sources available to visual query")]
public class DataSources : CustomDataSource
{
    /// <summary>
    /// The configuration for which zone to look in.
    /// Cannot be named `ZoneId` as that property already exists.
    /// </summary>
    [Configuration(Field = "ZoneId")]
    public int OfZoneId => Configuration.GetThis(ZoneId);

    public DataSources(Dependencies services, LazySvc<QueryControllerReal> query)
        : base(services, "Sxc.DataSources", connect: [query])
        => ProvideOutRaw(() => query.Value.DataSources(new AppIdentity(OfZoneId, AppId)));
}