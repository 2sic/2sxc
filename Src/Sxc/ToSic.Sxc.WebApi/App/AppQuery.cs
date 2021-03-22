using System;
using System.Collections.Generic;
using System.Net;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.LookUp;


namespace ToSic.Sxc.WebApi.App
{
    public class AppQuery: WebApiBackendBase<AppQuery>
    {
        private const string AllStreams = "*";

        #region Constructor / DI

        public AppQuery(IServiceProvider serviceProvider, IContextResolver ctxResolver) : base(serviceProvider, "Sxc.ApiApQ")
        {
            _ctxResolver = ctxResolver;
        }
        
        private readonly IContextResolver _ctxResolver;
        #endregion

        #region In-Container-Context Queries

        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(int? appId, string name, bool includeGuid, string stream)
        {
            var wrapLog = Log.Call($"'{name}', inclGuid: {includeGuid}, stream: {stream}");

            var appCtx = appId != null ? _ctxResolver.BlockOrApp(appId.Value) : _ctxResolver.BlockRequired();

            // If no app available from context, check if an app-id was supplied in url
            // Note that it may only be an app from the current portal
            // and security checks will run internally
            var app = ServiceProvider.Build<Apps.App>().Init(ServiceProvider, appCtx.AppState.AppId, Log, appCtx.UserMayEdit);

            var result = BuildQueryAndRun(app, name, stream, includeGuid, appCtx, Log, appCtx.UserMayEdit);
            wrapLog(null);
            return result;
        }

        #endregion

        #region Public Queries


        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery(string appPath, string name, string stream)
        {
            var wrapLog = Log.Call($"path:{appPath}, name:{name}, stream: {stream}");
            if (string.IsNullOrEmpty(name))
                throw HttpException.MissingParam(nameof(name));

            var appCtx = _ctxResolver.AppOrBlock(appPath);

            var queryApp = ServiceProvider.Build<Apps.App>().Init(appCtx.AppState,
                ServiceProvider.Build<AppConfigDelegate>().Init(Log).Build(appCtx.UserMayEdit), Log);

            // now just run the default query check and serializer
            var result = BuildQueryAndRun(queryApp, name, stream, false, appCtx, Log, appCtx.UserMayEdit);
            wrapLog(null);
            return result;
        }


        #endregion


        private static Dictionary<string, IEnumerable<Dictionary<string, object>>>
            BuildQueryAndRun(IApp app, string name, string stream, bool includeGuid, IContextOfSite context, ILog log,
                bool userMayEdit)
        {
            var wrapLog = log.Call($"name:{name}, stream:{stream}, withModule:{(context as IContextOfBlock)?.Module.Id}");
            var query = app.GetQuery(name);

            if (query == null)
            {
                var msg = $"query '{name}' not found";
                wrapLog(msg);
                throw new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found");
            }

            var permissionChecker = context.ServiceProvider.Build<AppPermissionCheck>().ForItem(context, appIdentity: app,
                targetItem: query.Definition.Entity, parentLog: log);
            var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

            var isAdmin = context.User.IsAdmin;

            // Only return query if permissions ok
            if (!(readExplicitlyAllowed || isAdmin))
            {
                var msg = $"Request not allowed. User does not have read permissions for query '{name}'";
                wrapLog(msg);
                throw new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, msg, "Request not allowed");
            }

            var serializer = new DataToDictionary(userMayEdit) { WithGuid = includeGuid };
            if (stream == AllStreams) stream = null;
            var result = serializer.Convert(query, stream);
            wrapLog(null);
            return result;
        }
    }
}
