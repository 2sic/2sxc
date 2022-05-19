using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi;

namespace ToSic.Sxc.Dnn.WebApi
{
    // support all modules now... 
    [SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class ModuleController: DnnApiControllerWithFixes<DummyControllerReal>
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