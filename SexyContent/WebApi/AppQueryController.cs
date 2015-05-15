using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.Security;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    public class AppQueryController : SxcApiController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]   // will check security internally, so assume no requirements
        [ValidateAntiForgeryToken]                                          // currently only available for modules, so always require the security token
        public dynamic Query([FromUri] string name)
        {
            // Try to find the query, abort if not found
            if (!App.Query.ContainsKey(name))
                throw new Exception("Can't find Query with name '" + name + "'");

            // Get query, check what permissions were assigned to the query-definition
            var query = App.Query[name] as DeferredPipelineQuery;
            var queryConf = query.QueryDefinition;
            var permissionChecker = new PermissionController(App.ZoneId, App.AppId, queryConf.EntityGuid, Dnn.Module);
            var readAllowed = permissionChecker.UserMay(PermissionGrant.Read);

            // Only return query if permissions ok
            if (readAllowed)
                return Sxc.Serializer.Prepare(query);
            else
                throw new Exception("Not allowed for query '" + name + "'");
        }



    }
}