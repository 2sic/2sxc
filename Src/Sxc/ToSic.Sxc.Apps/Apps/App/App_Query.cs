using ToSic.Eav.Apps.Sys;
using ToSic.Eav.DataSource.Sys;
using ToSic.Sxc.Apps.Sys;

namespace ToSic.Sxc.Apps;

partial class App
{
    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public IAppData Data => field ??= MyOptions.DataService.BuildData<AppDataWithCrud, IAppData>(this.PureIdentity());

    /// <summary>
    /// Accessor to queries. Use like:
    /// - App.Query.Count
    /// - App.Query.ContainsKey(...)
    /// - App.Query["One Event"].List
    /// </summary>
    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public IDictionary<string, IQuery> Query
    {
        get
        {
            if (field != null)
                return field;

            var le = MyOptions.DataService.LookUpEngine;
            if (le == null)
                throw new("Can't use app-queries, because the necessary configuration provider hasn't been initialized. Call InitData first.");
            return field = queryManager.Value.AllQueries(this, le);
        }
    }

    /// <inheritdoc />
    public IQuery GetQuery(string name)
    {
        // Try to find the local query, abort if not found
        // Not final implementation, but goal is to properly cascade from AppState to parent
        if (Query.TryGetValue(name, out var query) && query != null)
            return query;

        // Try to find query definition - while also checking parent apps
        var qEntity = queryManager.Value.TryGetQuery(MyOptions.AppReader, name, MyOptions.DataService.LookUpEngine, recurseParents: 3);

        return qEntity ?? throw new((DataSourceConstantsInternal.IsGlobalQuery(name) ? "Global " : "") + "Query not Found!");
    }
}