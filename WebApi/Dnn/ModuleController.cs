using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.Dnn
{
	// support all modules now... [SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [AllowAnonymous]
    public class ModuleController: DnnApiControllerWithFixes
    {

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
		public bool Delete(int tabId, int modId)
		{
		    var mc = new DotNetNuke.Entities.Modules.ModuleController();
            mc.DeleteTabModule(tabId, modId, true);
            return true;
		}

	}
}