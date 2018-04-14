using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav.Configuration;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [AllowAnonymous]
    public class SystemController : SxcApiController
	{

	    [HttpGet]
	    public IEnumerable<Feature> Features(int appId)
	    {
            // if the user has full edit permissions, he may also get the unpublic features
            // otherwise just the public Ui features
	        var set = AppAndPermissionChecker(appId, null);
	        var includeNonPublic = set.Item2.UserMay(GrantSets.WritePublished);

	        return new Eav.WebApi.SystemController()
                .Features(appId)
                .Where(f => includeNonPublic || f.Public == true);
	    }
        
    }
}