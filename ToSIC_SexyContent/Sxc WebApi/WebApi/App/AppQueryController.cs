using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Serializers;

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
            Log.Rename("2sApQr");
        }

        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query([FromUri] string name, [FromUri] bool includeGuid = false, [FromUri] string stream = null)
        {
            var context = GetContext(CmsBlock, Log);
            return BuildQueryAndRun(CmsBlock.App, name, stream, includeGuid, context.Dnn.Module, Log, CmsBlock);
        }


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery([FromUri] string appPath, [FromUri] string name, [FromUri] string stream = null)
        {
            Log.Add($"public query path:{appPath}, name:{name}");
            var appIdentity = AppFinder.GetCurrentAppIdFromPath(appPath);
            var queryApp = new Apps.App(new Tenant(PortalSettings), appIdentity.ZoneId, appIdentity.AppId,
                ConfigurationProvider.Build(false, false), false, Log);

            // now just run the default query check and serializer
            return BuildQueryAndRun(queryApp, name, stream, false, null, Log, CmsBlock);
        }


        private static Dictionary<string, IEnumerable<Dictionary<string, object>>> BuildQueryAndRun(IApp app, string name, string stream, bool includeGuid, ModuleInfo module, ILog log, /*SxcBlock*/ICmsBlock cms)
        {
            log.Add($"build and run query name:{name}, with module:{module?.ModuleID}");
            var query = app.GetQuery(name);

            if (query == null)
                throw HttpErr(HttpStatusCode.NotFound, "query not found", $"query '{name}' not found");

            var permissionChecker = new DnnPermissionCheck(log, targetItem: query.QueryDefinition, 
                instance: new Container(module), appIdentity: app);
            var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

            var isAdmin = module != null && DotNetNuke.Security.Permissions
                              .ModulePermissionController.CanAdminModule(module);

            // Only return query if permissions ok
            if (!(readExplicitlyAllowed || isAdmin))
                throw HttpErr(HttpStatusCode.Unauthorized, "Request not allowed", $"Request not allowed. User does not have read permissions for query '{name}'");
            
            var serializer = new Serializer(cms) { IncludeGuid = includeGuid };
            return serializer.Prepare(query, stream?.Split(','));
        }

        private static HttpResponseException HttpErr(HttpStatusCode status, string title, string msg)
        {
            return new HttpResponseException(new HttpResponseMessage(status)
            {
                Content = new StringContent(msg),
                ReasonPhrase = title
            });
        }
    }
}