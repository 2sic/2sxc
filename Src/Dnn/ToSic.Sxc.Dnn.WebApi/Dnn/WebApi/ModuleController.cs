using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Dnn.WebApi
{
    // support all modules now... 
    [SupportedModules(DnnSupportedModuleNames)]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ModuleController: DnnApiControllerWithFixes
    {
        public ModuleController() : base("Mod") { }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
		public bool Delete(int tabId, int modId)
		{
            Log.A($"delete mod:{modId} on tab:{tabId}");
		    var mc = new DotNetNuke.Entities.Modules.ModuleController();
            mc.DeleteTabModule(tabId, modId, true);
            Log.A("delete completed");
            return true;
		}

	}
}