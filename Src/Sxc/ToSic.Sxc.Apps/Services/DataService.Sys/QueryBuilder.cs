using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.LookUp.Sys.Engines;

namespace ToSic.Sxc.Services.DataService.Sys;

/// <summary>
/// Helpers to work with Data Queries.
/// This is a generic manager, since we may need it for classic queries or for TypedQueries within 2sxc.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class QueryBuilder<TQuery>(QueryDefinitionService queryDefSvc, Generator<TQuery, Query.Options> queryGenerator)
    : ServiceBase("Sxc.QryMan", connect: [queryDefSvc, queryGenerator])
    where TQuery : Query
{
    
    public TQuery? TryBuild(IAppIdentity appIdentity, string? queryNameOrGuid, ILookUpEngine lookUps, int recurseParents = 0)
    {
        var l = Log.Fn<TQuery>($"{queryNameOrGuid}, recurse: {recurseParents}");
        var qEntity = queryDefSvc.TryGetQueryEntity(appIdentity, queryNameOrGuid, recurseParents);
        if (qEntity == null)
            return l.ReturnNull("not found");
        var query = queryGenerator.New(new(appIdentity, qEntity, lookUps));
        return l.Return(query, "found");
    }

}