using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Types;
using ToSic.SexyContent.Security;
using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    [AllowAnonymous]
    public class AppQueryController : SxcApiController
    {
        private App _queryApp;
        private bool _useModuleAndCheckModulePermissions = true; // by default, try to check module stuff too

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sApQr");
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]   // will check security internally, so assume no requirements
        // todo warning: had to disable this temporarily, because of a bug in DNN 9.1 which incorrectly handled this!
        //[ValidateAntiForgeryToken]                                          // currently only available for modules, so always require the security token
        public dynamic Query([FromUri] string name, [FromUri] bool includeGuid = false)
        {
            Log.Add($"query name:{name}");
            // use the previously defined query, or just get it from the request (module-mode)
            if (_queryApp == null)
                _queryApp = App;

            var query = GetQueryByName(name);

            var queryConf = query.QueryDefinition;
            var permissionChecker = new PermissionController(_queryApp.ZoneId, _queryApp.AppId, queryConf.EntityGuid, _useModuleAndCheckModulePermissions ? Dnn.Module : null);
            var readAllowed = permissionChecker.UserMay(PermissionGrant.Read);

            var isAdmin = _useModuleAndCheckModulePermissions && DotNetNuke.Security.Permissions.ModulePermissionController.CanAdminModule(Dnn.Module);

            // Only return query if permissions ok
            if (!(readAllowed || isAdmin))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Request not allowed. User does not have read permissions for query '" + name + "'"),
                    ReasonPhrase = "Request not allowed"
                });

            var serializer = new Serializer();
            serializer.IncludeGuid = includeGuid;
            return serializer.Prepare(query);
        }

        [AllowAnonymous]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]   // will check security internally, so assume no requirements
        public dynamic PublicQuery([FromUri] string appPath, [FromUri] string name)
        {
            Log.Add($"public query path:{appPath}, name:{name}");
            _queryApp = new App(PortalSettings, GetCurrentAppIdFromPath(appPath));

            // ensure the queries can be executed (needs configuration provider, usually given in SxcInstance, but we don't hav that here)
            var config = DataSources.ConfigurationProvider.GetConfigProviderForModule(0, _queryApp, null);
            _queryApp.InitData(false, false, config);
            _useModuleAndCheckModulePermissions = false;    // disable module level permission check, as there is no module which can give more permissions

            // now just run the default query check and serializer
            return Query(name);
        }

        private DeferredPipelineQuery GetQueryByName(string name)
        {
            // BETA - TEST-SUPPORT FOR GLOBAL QUERIES
            var tempglobid = "global-beta-";
            if (name.StartsWith(tempglobid))
            {
                var innername = name.Substring(tempglobid.Length);
                return _queryApp.GlobalQueryBeta(innername);
            }

            // Try to find the query, abort if not found
            if (!_queryApp.Query.ContainsKey(name))
                throw new Exception("Can't find Query with name '" + name + "'");

            // Get query, make suer it's the right type...
            if (!(_queryApp.Query[name] is DeferredPipelineQuery query))
                throw new Exception("can't find query with name '" + name + "'");
            return query;
        }
    }
}