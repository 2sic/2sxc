using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Security;
using ToSic.Sxc.WebApi;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <inheritdoc cref="ISystemController" />
    /// <summary>
    /// Web API Controller Which Delivers System Information
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class SystemController : SxcApiControllerBase, ISystemController
    {

        [HttpGet]
        public IEnumerable<Feature> Features(int appId)
        {
            // some dialogs don't have an app-id yet, because no app has been selected
            // in this case, use the app-id of the content-app for feature-permission check
            if (appId == 0)
            {
                var environment = Factory.Resolve<IEnvironmentFactory>().Environment(Log);
                var zoneId = environment.ZoneMapper.GetZoneId(PortalSettings.PortalId);
                appId = new ZoneRuntime(zoneId, Log).DefaultAppId;
            }

            return FeatureListWithPermissionCheck(appId,
                new MultiPermissionsApp(CmsBlock, appId, Log));
        }


        internal static IEnumerable<Feature> FeatureListWithPermissionCheck(int appId, MultiPermissionsApp permCheck)
	    {
            // if the user has full edit permissions, he may also get the unpublic features
            // otherwise just the public Ui features
            //var permCheck = new AppAndPermissions(sxcInstance, appId, log);
	        //if (permCheck.Permissions == null)
	        //    permCheck.GetTypePermissionChecker(null);
	        var includeNonPublic = permCheck.UserMayOnAll(GrantSets.WritePublished);

	        return Eav.Configuration.Features.Ui
                .Where(f => includeNonPublic || f.Public == true);
	    }
	}
}