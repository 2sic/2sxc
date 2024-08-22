using ToSic.Eav.DataSource;
using static ToSic.Eav.DataSource.Internal.DataSourceConstants;

namespace ToSic.Sxc.Apps;

partial class App
{
    /// <summary>
    /// Accessor to queries. Use like:
    /// - App.Query.Count
    /// - App.Query.ContainsKey(...)
    /// - App.Query["One Event"].List
    /// </summary>
    /// <inheritdoc />
    public IDictionary<string, IQuery> Query
    {
        get
        {
            if (_queries != null) return _queries;

            if (ConfigurationProvider == null)
                throw new("Can't use app-queries, because the necessary configuration provider hasn't been initialized. Call InitData first.");
            return _queries = Services.QueryManager.Value.AllQueries(this, ConfigurationProvider);
        }
    }
    private IDictionary<string, IQuery> _queries;

    /// <inheritdoc />
    public IQuery GetQuery(string name)
    {
        // Try to find the local query, abort if not found
        // Not final implementation, but goal is to properly cascade from AppState to parent
        if (Query.TryGetValue(name, out var value) && value is ToSic.Eav.DataSource.Internal.Query.Query query)
            return query;

        // Try to find query definition - while also checking parent apps
        var qEntity = Services.QueryManager.Value.GetQuery(AppReaderInt, name, ConfigurationProvider, recurseParents: 3);

        return qEntity ?? throw new((IsGlobalQuery(name) ? "Global " : "") + "Query not Found!");
    }
}