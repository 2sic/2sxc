using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.LookUp.Sys.Engines;

namespace ToSic.Sxc.Apps.Sys.AppDynamic;

/// <summary>
/// Helpers to work with Data Queries.
/// This is a generic manager, since we may need it for classic queries or for TypedQueries within 2sxc.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppDynamicQueries(QueryDefinitionService queryDefSvc, Generator<Query, Query.Options> queryGenerator)
    : ServiceBase("Sxc.DynQrs", connect: [queryDefSvc, queryGenerator])
{

    /// <summary>
    /// Assembles a list of all queries / Queries configured for this app. 
    /// The queries internally are not assembled yet for performance reasons...
    /// ...but will be auto-assembled the moment they are accessed
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, IQuery> AllQueries(IAppIdentity app, ILookUpEngine lookUps)
    {
        var l = Log.Fn<Dictionary<string, IQuery>>();
        var dict = new Dictionary<string, IQuery>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var entQuery in queryDefSvc.AllQueryEntities(app))
        {
            // make sure it doesn't break if two queries have the same name...
            var name = entQuery.GetBestTitle();
            if (string.IsNullOrWhiteSpace(name) || dict.ContainsKey(name!))
                continue;

            dict[name!] = queryGenerator.New(new(app, entQuery, lookUps));
        }
        return l.Return(dict);
    }

}