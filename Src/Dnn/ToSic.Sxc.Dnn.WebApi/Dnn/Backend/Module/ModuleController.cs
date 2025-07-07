﻿using ToSic.Sxc.Dnn.WebApi.Sys;

namespace ToSic.Sxc.Dnn.Backend.Module;

// support all modules now... 
[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ModuleController() : DnnSxcControllerRoot("Mod")
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