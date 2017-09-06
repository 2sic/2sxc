using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.DataSources;
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

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]   // will check security internally, so assume no requirements
        [ValidateAntiForgeryToken]                                          // currently only available for modules, so always require the security token
        public dynamic Query([FromUri] string name)//, [FromUri] string appPath = null)
        {
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

            return new Serializer().Prepare(query);
        }

        [AllowAnonymous]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]   // will check security internally, so assume no requirements
        public dynamic PublicQuery([FromUri] string appPath, [FromUri] string name)
        {
            _queryApp = new App(PortalSettings, GetCurrentAppIdFromPath(appPath));

            // ensure the queries can be executed (needs configuration provider, usually given in SxcInstance, but we don't hav that here)
            var config = DataSources.ConfigurationProvider.GetConfigProviderForModule(0, _queryApp, null);
            _queryApp.InitData(false, new Environment.Dnn7.PagePublishing().IsVersioningEnabled(this.ActiveModule.ModuleID), config);
            _useModuleAndCheckModulePermissions = false;    // disable module level permission check, as there is no module which can give more permissions

            // now just run the default query check and serializer
            return Query(name);
        }

        private DeferredPipelineQuery GetQueryByName(string name)
        {
            // Try to find the query, abort if not found
            if (!_queryApp.Query.ContainsKey(name))
                throw new Exception("Can't find Query with name '" + name + "'");

            // Get query, check what permissions were assigned to the query-definition
            var query = _queryApp.Query[name] as DeferredPipelineQuery;
            if (query == null)
                throw new Exception("can't find query with name '" + name + "'");
            return query;
        }
    }
}