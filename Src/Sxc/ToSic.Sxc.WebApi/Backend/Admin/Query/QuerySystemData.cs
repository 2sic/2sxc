using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.SysData;

namespace ToSic.Sxc.Backend.Admin.Query;

[PrivateApi]
[VisualQuery(NiceName = "Query Definition", NameId = "214ca49d-bb36-4710-8c40-2a34a3ecb568", NameIds = ["System.QueryDefinition"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Definition and parts of a visual query")]
public class QueryDefinition : CustomDataSource
{
    [Configuration]
    public int QueryId => Configuration.GetThis(0);

    public QueryDefinition(Dependencies services, LazySvc<QueryControllerReal> query)
        : base(services, "Sxc.QueryDef", connect: [query])
    {
        ProvideOutRaw(() => Definition(query), name: "Definition", options: Options);
        ProvideOutRaw(() => Parts(query), name: "DataSources", options: Options);
    }

    private IEnumerable<IRawEntity> Definition(LazySvc<QueryControllerReal> query)
        => [new RawEntity { Values = query.Value.Get(AppId, QueryId).Pipeline.ToDictionary(p => p.Key, p => (object?)p.Value) }];
    private IEnumerable<IRawEntity> Parts(LazySvc<QueryControllerReal> query)
        => query.Value.Get(AppId, QueryId).DataSources.Select(x => (IRawEntity)new RawEntity { Values = x.ToDictionary(p => p.Key, p => (object?)p.Value) });
    private static DataFactoryOptions Options() => new() { AutoId = true, AllowUnknownValueTypes = true };
}

[PrivateApi]
[VisualQuery(NiceName = "Data Sources", NameId = "2de41d89-3cb9-480a-ac3c-40f77fd3af4e", NameIds = ["System.DataSources"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Internal, UiHint = "Data sources available to visual query")]
public class DataSources : CustomDataSource
{
    [Configuration(Field = "ZoneId")]
    public int OfZoneId => Configuration.GetThis(ZoneId);

    public DataSources(Dependencies services, LazySvc<QueryControllerReal> query)
        : base(services, "Sxc.DataSources", connect: [query])
        => ProvideOutRaw(() => SysDataRaw.Many(query.Value.DataSources(new AppIdentity(OfZoneId, AppId))), options: Options);

    private static DataFactoryOptions Options() => new() { AutoId = true, TitleField = "Name", TypeName = "DataSource", AllowUnknownValueTypes = true };
}
