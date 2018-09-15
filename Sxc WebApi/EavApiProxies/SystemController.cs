using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav.Configuration;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <inheritdoc />
    /// <summary>
    /// Web API Controller Which Delivers System Information
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [AllowAnonymous]
    public class SystemController : SxcApiControllerBase
    {

        [HttpGet]
        public IEnumerable<Feature> Features(int appId)
            => FeatureListWithPermissionCheck(appId,
                new PermissionsForApp(SxcInstance, appId, Log));

	    internal static IEnumerable<Feature> FeatureListWithPermissionCheck(int appId, PermissionsForApp permCheck)
	    {
            // if the user has full edit permissions, he may also get the unpublic features
            // otherwise just the public Ui features
            //var permCheck = new AppAndPermissions(sxcInstance, appId, log);
	        //if (permCheck.Permissions == null)
	        //    permCheck.GetTypePermissionChecker(null);
	        var includeNonPublic = permCheck.PermissionChecker.UserMay(GrantSets.WritePublished);

	        return Eav.WebApi.SystemController.GetFeatures(appId)
	            .Where(f => includeNonPublic || f.Public == true);
	    }
	}
}