using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services.DataServices
{
    /// <summary>
    /// Get-Query Micro Service
    /// </summary>
    internal class GetQueryMs: ServiceBase
    {
        private readonly LazySvc<QueryManager> _queryManager;
        private readonly DataSourceOptionsMs _optionsMs;

        internal GetQueryMs(LazySvc<QueryManager> queryManager, DataSourceOptionsMs optionsMs, ILog parentLog): base(Constants.SxcLogName + ".DtGqMs")
        {
            this._queryManager = queryManager;
            this._optionsMs = optionsMs;
            this.LinkLog(parentLog);
        }

        public IDataSource GetQuery(string name = default,
            string noParamOrder = Protector,
            IDataSourceLinkable attach = default,
            object parameters = default)
        {
            var l = Log.Fn<IDataSource>($"{name}, {nameof(parameters)}: {(parameters == null ? "null" : "not null")}");

            Protect(noParamOrder, $"{nameof(attach)}, {nameof(parameters)}");

            // If no in-source was provided, make sure that we create one from the current app
            var fullOptions = _optionsMs.SafeOptions(parameters, null, true /*, options: options*/);

            var query = _queryManager.Value.GetQuery(fullOptions.AppIdentity, name, fullOptions.LookUp, 3);

            if (query == null) return l.ReturnNull("query was null");

            if (parameters == null) return l.Return(query, "query, no parameters");

            var paramsDic = parameters.ObjectToDictionary();
            foreach (var param in paramsDic)
                query.Params(param.Key, param.Value);

            return l.Return(query, "with params");
        }

    }
}
