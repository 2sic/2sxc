using System.Collections.Generic;
using System.Net;
using ToSic.Eav;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.WebApi.Errors;

namespace ToSic.Sxc.WebApi.App
{
    public class AppQuery: WebApiBackendBase<AppQuery>
    {
        #region Constructor / DI

        public AppQuery() : base("Sxc.ApiApQ")
        {

        }

        //public AppQuery Init(ILog parentLog)
        //{
        //    Log.LinkTo(parentLog);
        //    return this;
        //}

        #endregion

        #region In-Container-Context Queries

        internal Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(IInstanceContext context, IBlock block, IApp app, string name, bool includeGuid, string stream, int? appId)
        {
            var wrapLog = Log.Call($"'{name}', inclGuid: {includeGuid}, stream: {stream}");
            //var dynamicCode = new DnnDynamicCode().Init(BlockBuilder, Log);
            //var app = blockBuilder.App;

            // If no app available from context, check if an app-id was supplied in url
            // Note that it may only be an app from the current portal
            // and security checks will run internally
            if (app == null && appId != null)
            {
                app = Factory.Resolve<Apps.App>().Init(appId.Value, Log, showDrafts: context.User.IsSuperUser);
                //   Dnn.Factory.App(appId.Value, PortalSettings, false, context.User.IsSuperUser, Log);
                //app = Dnn.Factory.App(appId.Value, PortalSettings, false, context.User.IsSuperUser, Log);
            }

            var result = BuildQueryAndRun(app, name, stream, includeGuid, context, Log, block?.EditAllowed ?? false);
            wrapLog(null);
            return result;
        }

        #endregion

        #region Public Queries


        internal Dictionary<string, IEnumerable<Dictionary<string, object>>> 
            PublicQuery(IInstanceContext context, string appPath, string name, string stream, IBlock block)
        {
            var wrapLog = Log.Call($"path:{appPath}, name:{name}");
            if (string.IsNullOrEmpty(name))
                throw HttpException.MissingParam(nameof(name));
            var appIdentity = AppFinder.GetAppIdFromPath(appPath);
            var queryApp = Factory.Resolve<Apps.App>().Init(appIdentity,
                ConfigurationProvider.Build(false, false), false, Log);

            // now just run the default query check and serializer
            var result = BuildQueryAndRun(queryApp, name, stream, false, context, Log, block?.EditAllowed ?? false);
            wrapLog(null);
            return result;
        }


        #endregion


        internal static Dictionary<string, IEnumerable<Dictionary<string, object>>>
            BuildQueryAndRun(IApp app, string name, string stream, bool includeGuid, IInstanceContext context, ILog log,
                bool userMayEdit)
        {
            var wrapLog = log.Call($"name:{name}, withModule:{context.Container.Id}");
            var query = app.GetQuery(name);

            if (query == null)
            {
                var msg = $"query '{name}' not found";
                wrapLog(msg);
                throw new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found");
            }

            var permissionChecker = Factory.Resolve<AppPermissionCheck>().ForItem( // new DnnPermissionCheck().ForItem(
                context, // new DnnContext(new DnnTenant(), new DnnContainer().Init(module, log), new DnnUser()), 
                appIdentity: app,
                targetItem: query.Definition.Entity, parentLog: log);
            var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

            var isAdmin = context.User.IsAdmin;
            //module != null && DotNetNuke.Security.Permissions
            //              .ModulePermissionController.CanAdminModule(module);

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
