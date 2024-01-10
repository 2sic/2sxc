using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Lib.Logging;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.Dnn.Backend.Module;

// support all modules now... 
[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ModuleController() : DnnApiControllerWithFixes("Mod")
{
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