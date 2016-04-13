using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.Dnn
{
	[SupportedModules("2sxc,2sxc-app")]
	public class ModuleController: DnnApiController
	{

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
		public bool Delete(int tabId, int modId)
		{
		    var mc = DotNetNuke.Entities.Modules.ModuleController.Instance;
            mc.DeleteTabModule(tabId, modId, true);
            return true;
		}

	}
}