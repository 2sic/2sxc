using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.Sxc.Dnn.WebApi
{
    // support all modules now... 
    [SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class ModuleController: DnnApiControllerWithFixes
    {
        public ModuleController() : base("Mod") { }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
		public bool Delete(int tabId, int modId)
		{
            Log.Add($"delete mod:{modId} on tab:{tabId}");
		    var mc = new DotNetNuke.Entities.Modules.ModuleController();
            mc.DeleteTabModule(tabId, modId, true);
            Log.Add("delete completed");
            return true;
		}

	}
}