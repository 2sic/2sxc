using ToSic.Eav.Apps.Inspect.Sys.Query;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.Backend.Admin.Query;

[PrivateApi]
[VisualQuery(
    NiceName = "Query Definition",
    NameId = "214ca49d-bb36-4710-8c40-2a34a3ecb568",
    NameIds = ["System.QueryDefinition"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Definition and parts of a visual query")]
public class QueryDefinition : CustomDataSource
{
    [Configuration]
    public int QueryId => Configuration.GetThis(0);

    public QueryDefinition(Dependencies services, LazySvc<QueryGetDtoWip> query)
        : base(services, "Sxc.QueryDef", connect: [query])
    {
        ProvideOutRaw(() => Definition(query), name: "Definition", options: () => new() { AllowUnknownValueTypes = true });
        ProvideOutRaw(() => Parts(query), name: "DataSources", options: () => new() { AllowUnknownValueTypes = true });
    }

    private IEnumerable<IRawEntity> Definition(LazySvc<QueryGetDtoWip> query)
        =>
        [
            new RawEntity
            {
                Values = query.Value.Get(AppId, QueryId)
                    .Pipeline
                    .ToDictionary(p => p.Key, object? (p) => p.Value)
            }
        ];
    private IEnumerable<IRawEntity> Parts(LazySvc<QueryGetDtoWip> query)
        => query.Value.Get(AppId, QueryId)
            .DataSources
            .Select(IRawEntity (x) => new RawEntity
            {
                Values = x.ToDictionary(p => p.Key, object? (p) => p.Value)
            });
}