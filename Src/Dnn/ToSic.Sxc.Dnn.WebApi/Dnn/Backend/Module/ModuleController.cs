using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

namespace ToSic.Sxc.Dnn.Backend.Module;

// support all modules now... 
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ModuleController : DnnApiController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.Mod");

    //private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion


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