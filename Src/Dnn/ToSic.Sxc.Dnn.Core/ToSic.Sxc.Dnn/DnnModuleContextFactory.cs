using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn;

public class DnnModuleContextFactory(IContextOfBlockFactory blockCtxFactory, DnnSiteFactory siteFactory, DnnPageFactory pageFactory, DnnModuleFactory moduleFactory, DnnSiteShareWip siteShare)
    : ServiceBase("Dnn.CtxMod", connect: [blockCtxFactory, siteFactory, pageFactory, moduleFactory, siteShare])
{

    public IContextOfModule InitDnnSiteModuleAndBlockContext(ModuleInfo dnnModule, int? pageId = null)
    {
        var l = Log.Fn<IContextOfModule>($"{nameof(pageId)}: {pageId}, {nameof(dnnModule.ModuleID)}: {dnnModule.ModuleID}");

        l.A($"Will try-swap module info of {dnnModule.ModuleID} into site");
        var parentLog = (Log as Log)?.Parent ?? Log;
        var newSite = CreateFromModuleAndShare(dnnModule, parentLog);

        l.A($"Page with pageOverride: {pageId}");
        var page = pageFactory.Create(newSite.GetContents()?.ActiveTab, pageId);

        l.A("Will init module");
        var module = moduleFactory.Get(dnnModule);

        // Assemble the final context
        var contextWithModuleAndSite = blockCtxFactory.Create(newSite, page, module);
        return l.ReturnAsOk(contextWithModuleAndSite);
    }

    
    /// <summary>
    /// Create site object and broadcast it through dependency injection.
    /// </summary>
    /// <param name="module"></param>
    /// <param name="extLog"></param>
    /// <returns></returns>
    internal DnnSite CreateFromModuleAndShare(ModuleInfo module, ILog extLog)
    {
        var l = extLog.Fn<DnnSite>($"Owner Site: {module?.OwnerPortalID}, Current Site: {module?.PortalID}");

        if (module == null)
            return l.Return(siteShare.TempSiteContents, "no module");
        if (module.OwnerPortalID < 0)
            return l.Return(siteShare.TempSiteContents, "no change, owner < 0");
        if (module.OwnerPortalID == siteShare.TempSiteContents.ZoneId)
            return l.Return(siteShare.TempSiteContents, "no change, owner = temp site");

        // Note: this does look a bit fishy, since creating the object will probably be missing
        // some properties; should probably go through some kind of portal-settings manager or something?
        var modulePortalSettings = new PortalSettings(module.OwnerPortalID);
        var result = siteFactory.CreateFromPortal(modulePortalSettings);

        // Share/Broadcast the new site object, as it will apply to everything now
        // This is how it used to work before v22
        // It's not a good implementation as of now, but the complexity to change this is huge.
        siteShare.TempSiteContents = result;

        return l.Return(result, "updated with portal settings of module");
    }

}