using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.WebApi.App
{
    /// <inheritdoc />
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    [AllowAnonymous]
    public class AppQueryController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Sxc.ApQrCt");
        }

        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query([FromUri] string name, [FromUri] bool includeGuid = false, [FromUri] string stream = null, [FromUri] int? appId = null)
        {
            var wrapLog = Log.Call($"'{name}', inclGuid: {includeGuid}, stream: {stream}");
            var context = new DnnDynamicCode(BlockBuilder, Log);
            var app = BlockBuilder.App;

            // If no app available from context, check if an app-id was supplied in url
            // Note that it may only be an app from the current portal
            // and security checks will run internally
            if (app == null && appId != null)
                app = Dnn.Factory.App(appId.Value, PortalSettings, false, UserInfo?.IsSuperUser ?? false, Log);

            var result = BuildQueryAndRun(app, name, stream, includeGuid, context.Dnn.Module, Log, BlockBuilder?.UserMayEdit ?? false);
            wrapLog(null);
            return result;
        }


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery([FromUri] string appPath, [FromUri] string name, [FromUri] string stream = null)
        {
            var wrapLog = Log.Call($"path:{appPath}, name:{name}");
            var appIdentity = AppFinder.GetCurrentAppIdFromPath(appPath);
            var queryApp = Factory.Resolve<Apps.App>().Init(appIdentity,
                ConfigurationProvider.Build(false, false), false, Log);

            // now just run the default query check and serializer
            var result = BuildQueryAndRun(queryApp, name, stream, false, null, Log, BlockBuilder?.UserMayEdit ?? false);
            wrapLog(null);
            return result;
        }


        private static Dictionary<string, IEnumerable<Dictionary<string, object>>> 
            BuildQueryAndRun(IApp app, string name, string stream, bool includeGuid, ModuleInfo module, ILog log, 
                bool userMayEdit)
        {
            var wrapLog = log.Call($"name:{name}, withModule:{module?.ModuleID}");
            var query = app.GetQuery(name);

            if (query == null)
            {
                var msg = $"query '{name}' not found";
                wrapLog(msg);
                throw HttpErr(HttpStatusCode.NotFound, "query not found", msg);
            }

            var permissionChecker = new DnnPermissionCheck(log, targetItem: query.Definition.Entity, 
                instance: new DnnContainer().Init(module, log), appIdentity: app);
            var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

            var isAdmin = module != null && DotNetNuke.Security.Permissions
                              .ModulePermissionController.CanAdminModule(module);

            // Only return query if permissions ok
            if (!(readExplicitlyAllowed || isAdmin))
            {
                var msg = $"Request not allowed. User does not have read permissions for query '{name}'";
                wrapLog(msg);
                throw HttpErr(HttpStatusCode.Unauthorized, "Request not allowed", msg);
            }

            var serializer = new DataToDictionary(userMayEdit) { WithGuid = includeGuid };
            var result = serializer.Convert(query, stream?.Split(','));
            wrapLog(null);
            return result;
        }

        private static HttpResponseException HttpErr(HttpStatusCode status, string title, string msg) =>
            new HttpResponseException(new HttpResponseMessage(status)
            {
                Content = new StringContent(msg),
                ReasonPhrase = title
            });
    }
}