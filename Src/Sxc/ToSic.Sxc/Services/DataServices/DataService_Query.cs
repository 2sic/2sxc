using ToSic.Eav;
using ToSic.Eav.DataSource;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services
{
    internal partial class DataService
    {
        public IDataSource GetQuery(string name = default,
            string noParamOrder = Parameters.Protector,
            IDataSourceLinkable attach = default,
            object parameters = default)
        {
            var l = Log.Fn<IDataSource>($"{name}, {nameof(parameters)}: {(parameters == null ? "null" : "not null")}");

            Protect(noParamOrder, $"{nameof(attach)}, {nameof(parameters)}");

            // If no in-source was provided, make sure that we create one from the current app
            var fullOptions = SafeOptions(parameters, null, true /*, options: options*/);

            var query = _queryManager.Value.GetQuery(fullOptions.AppIdentity, name, fullOptions.LookUp, 3);
            
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
}
