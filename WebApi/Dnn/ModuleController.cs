using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.Dnn
{
    // support all modules now... 
    [SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ModuleController: DnnApiControllerWithFixes
    {

		[HttpGet]
        // 2016-12-09 testing - had to disable this because the additional security brock in dnn9
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
		public bool Delete(int tabId, int modId)
		{
		    var mc = new DotNetNuke.Entities.Modules.ModuleController();
            mc.DeleteTabModule(tabId, modId, true);
            return true;
		}

	}
}