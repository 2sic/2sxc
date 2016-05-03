using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.Security;

// Beta / test - doesn't work yet! todo!
namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    /// 
    // todo: ensure that high-permitted users get the query even if not specified
    //[SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
    [AllowAnonymous]
    public class AppWithoutModuleQueryController  : SxcApiController
    {
        private App _queryApp;

        //[HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]   // will check security internally, so assume no requirements
        //[SupportedModules("2sxc,2sxc-app")]
        //[ValidateAntiForgeryToken]                                          // currently only available for modules, so always require the security token
        //public dynamic Query([FromUri] string name)
        //{
        //    if (_queryApp == null)
        //        _queryApp = App;

        //    var query = GetQueryByName(name);

        //    var queryConf = query.QueryDefinition;
        //    var permissionChecker = new PermissionController(_queryApp.ZoneId, _queryApp.AppId, queryConf.EntityGuid, Dnn.Module);
        //    var readAllowed = permissionChecker.UserMay(PermissionGrant.Read);

        //    var isAdmin = DotNetNuke.Security.Permissions.ModulePermissionController.CanAdminModule(Dnn.Module);

        //    // Only return query if permissions ok
        //    if (readAllowed || isAdmin)
        //        return Sxc.Serializer.Prepare(query);
        //    else
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
        //        {
        //            Content = new StringContent("Request not allowed. User does not have read permissions for query '" + name + "'"),
        //            ReasonPhrase = "Request not allowed"
        //        }); 
        //}

        [AllowAnonymous]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]   // will check security internally, so assume no requirements
        public dynamic PublicQuery([FromUri] string appname, [FromUri] string name)
        {
            // todo: get app from appname
            var zid = ZoneHelpers.GetZoneID(PortalSettings.PortalId);
            if(zid == null)
                throw new Exception("zone not found");
            var aid = AppHelpers.GetAppIdFromGuidName(zid.Value, appname);
            _queryApp = new App(PortalSettings, aid);
            return "ok!";// Query(name);

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