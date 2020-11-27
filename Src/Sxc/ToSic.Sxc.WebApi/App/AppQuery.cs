using System;
using System.Collections.Generic;
using System.Net;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.LookUp;


namespace ToSic.Sxc.WebApi.App
{
    public class AppQuery: WebApiBackendBase<AppQuery>
    {
        #region Constructor / DI

        public AppQuery(IServiceProvider serviceProvider) : base(serviceProvider, "Sxc.ApiApQ")
        {

        }
        
        #endregion

        #region In-Container-Context Queries

        internal Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(IContextOfSite context, IBlock block, IApp app, string name, bool includeGuid, string stream, int? appId)
        {
            var wrapLog = Log.Call($"'{name}', inclGuid: {includeGuid}, stream: {stream}");

            // If no app available from context, check if an app-id was supplied in url
            // Note that it may only be an app from the current portal
            // and security checks will run internally
            if (app == null && appId != null)
                app = ServiceProvider.Build<Apps.App>().Init(ServiceProvider, appId.Value, Log, showDrafts: context.User.IsSuperUser);

            var result = BuildQueryAndRun(app, name, stream, includeGuid, context, Log, block?.EditAllowed ?? false);
            wrapLog(null);
            return result;
        }

        #endregion

        #region Public Queries


        internal Dictionary<string, IEnumerable<Dictionary<string, object>>> 
            PublicQuery(IContextOfBlock context, string appPath, string name, string stream, IBlock block)
        {
            var wrapLog = Log.Call($"path:{appPath}, name:{name}");
            if (string.IsNullOrEmpty(name))
                throw HttpException.MissingParam(nameof(name));
            var appIdentity = AppFinder.GetAppIdFromPath(appPath);
            var queryApp = ServiceProvider.Build<Apps.App>().Init(appIdentity,
                ServiceProvider.Build<AppConfigDelegate>().Init(Log).Build(false), Log);

            // now just run the default query check and serializer
            var result = BuildQueryAndRun(queryApp, name, stream, false, context, Log, block?.EditAllowed ?? false);
            wrapLog(null);
            return result;
        }


        #endregion


        private static Dictionary<string, IEnumerable<Dictionary<string, object>>>
            BuildQueryAndRun(IApp app, string name, string stream, bool includeGuid, IContextOfSite context, ILog log,
                bool userMayEdit)
        {
            var wrapLog = log.Call($"name:{name}, withModule:{(context as IContextOfBlock)?.Module.Id}");
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
            var result = serializer.Convert(query, stream?.Split(','));
            wrapLog(null);
            return result;
        }
    }
}
