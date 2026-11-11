using DotNetNuke.Entities.Modules;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn;

public class DnnModuleContextFactory(IContextOfBlockFactory blockCtxFactory, DnnSiteFactory siteFactory, DnnPageFactory pageFactory, DnnModuleFactory moduleFactory)
    : ServiceBase("Dnn.CtxMod", connect: [blockCtxFactory, siteFactory, pageFactory, moduleFactory])
{
    private ILog ParentLog => (Log as Log)?.Parent ?? Log;

    public IContextOfModule InitDnnSiteModuleAndBlockContext(ModuleInfo dnnModule, int? pageId = null)
    {
        var l = Log.Fn<IContextOfModule>($"{nameof(pageId)}: {pageId}, {nameof(dnnModule.ModuleID)}: {dnnModule.ModuleID}");

        l.A($"Will try-swap module info of {dnnModule.ModuleID} into site");
        var newSite = siteFactory.CreateFromModuleAndShare(dnnModule, ParentLog);

        l.A($"Page with pageOverride: {pageId}");
        var page = pageFactory.Create(newSite.GetContents()?.ActiveTab, pageId);

        l.A("Will init module");
        var module = moduleFactory.Get(dnnModule);

        // Assemble the final context
        var contextWithModuleAndSite = blockCtxFactory.Create(newSite, page, module);
        return l.ReturnAsOk(contextWithModuleAndSite);
    }
}
