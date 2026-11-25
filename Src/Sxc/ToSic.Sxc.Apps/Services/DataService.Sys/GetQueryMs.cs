using ToSic.Eav.DataSource.Query.Sys;

namespace ToSic.Sxc.Services.DataService.Sys;

/// <summary>
/// Get-Query Micro Service
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class GetQueryMs<TQuery>(LazySvc<QueryBuilder<TQuery>> queryBuilder)
    : ServiceWithSetup<DataSourceOptionsMs>(SxcLogName + ".DtGqMs", connect: [queryBuilder])
    where TQuery : Query
{
    public TQuery? GetQuery(
        string? name = default,
        NoParamOrder npo = default,
        IDataSourceLinkable? attach = default,
        object? parameters = default)
    {
        var l = Log.Fn<TQuery>($"{name}, {nameof(parameters)}: {(parameters == null ? "null" : "not null")}");

        // If no in-source was provided, make sure that we create one from the current app
        var fullOptions = MyOptions.SafeOptions(parameters, null, true);

        // #WipAppIdentityOrReader must become not null
        var query = queryBuilder.Value.TryBuild(fullOptions.AppIdentityOrReader!, name, fullOptions.LookUp!, 3);

        // no query / parameters, exit early
        if (query == null)
            return l.ReturnNull("query was null");

        if (parameters == null)
            return l.Return(query, "query, no parameters");

        // Transfer parameters
        var paramsDic = parameters.ObjectToDictionary();
        foreach (var param in paramsDic)
            query.Params(param.Key, param.Value);

        return l.Return(query, "with params");
    }

}