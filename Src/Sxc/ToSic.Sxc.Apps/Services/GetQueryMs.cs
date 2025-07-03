using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.DataSource.Sys.Query;

namespace ToSic.Sxc.Services.DataServices;

/// <summary>
/// Get-Query Micro Service
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class GetQueryMs: ServiceBase
{
    private readonly LazySvc<QueryManager> _queryManager;
    private readonly DataSourceOptionsMs _optionsMs;

    internal GetQueryMs(LazySvc<QueryManager> queryManager, DataSourceOptionsMs optionsMs, ILog parentLog): base(SxcLogName + ".DtGqMs")
    {
        _queryManager = queryManager;
        _optionsMs = optionsMs;
        this.LinkLog(parentLog);
    }

    public IDataSource? GetQuery(
        string? name = default,
        NoParamOrder noParamOrder = default,
        IDataSourceLinkable? attach = default,
        object? parameters = default)
    {
        var l = Log.Fn<IDataSource>($"{name}, {nameof(parameters)}: {(parameters == null ? "null" : "not null")}");


        // If no in-source was provided, make sure that we create one from the current app
        var fullOptions = _optionsMs.SafeOptions(parameters, null, true /*, options: options*/);

        // #WipAppIdentityOrReader must become not null
        var query = _queryManager.Value.GetQuery(fullOptions.AppIdentityOrReader! /* WIP */, name, fullOptions.LookUp!, 3);

        if (query == null)
            return l.ReturnNull("query was null");

        if (parameters == null)
            return l.Return(query, "query, no parameters");

        var paramsDic = parameters.ObjectToDictionary();
        foreach (var param in paramsDic)
            query.Params(param.Key, param.Value);

        return l.Return(query, "with params");
    }

}