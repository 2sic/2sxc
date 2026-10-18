using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Dnn;

internal class DnnModuleAndBlockBuilder(
    Generator<IModule> moduleGenerator,
    Generator<IContextOfBlock> contextGenerator,
    DnnSiteFactory siteFactory,
    Generator<BlockOfModule> blockGenerator)
    : ModuleAndBlockBuilder(blockGenerator, DnnConstants.LogName, connect: [moduleGenerator, contextGenerator, siteFactory])
{
    private ILog ParentLog => (Log as Log)?.Parent ?? Log;


    public override IModule GetModule(int pageId, int moduleId)
    {
        var l = Log.Fn<IModule>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
        var moduleInfo = new ModuleController().GetModule(moduleId, pageId, false);

        l.A($"Page Id on DNN Module: {moduleInfo.TabID} - should be {pageId}");

        ThrowIfModuleIsNull(pageId, moduleId, moduleInfo);
        var module = ((DnnModule)moduleGenerator.New()).Init(moduleInfo);

        return l.Return(module, $"Page Id on IModule: {module.BlockIdentifier} - should be {pageId}");
    }

    protected override IContextOfBlock GetContextOfBlock(IModule module, int? pageId)
        => GetContextOfBlock((module as DnnModule)?.GetContents(), pageId);


    protected override IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId)
    {
        if (module == null)
            throw new ArgumentNullException(nameof(module));
        if (module is not ModuleInfo dnnModule)
            throw new ArgumentException("Given data is not a module");
        Log.A($"Module: {dnnModule.ModuleID}");

        var initializedCtx = InitDnnSiteModuleAndBlockContext(dnnModule, pageId);
        return initializedCtx;
    }

    private IContextOfBlock InitDnnSiteModuleAndBlockContext(ModuleInfo dnnModule, int? pageId)
    {
        var l = Log.Fn<IContextOfBlock>($"{nameof(pageId)}: {pageId}, {nameof(dnnModule.ModuleID)}: {dnnModule.ModuleID}");
        var context = contextGenerator.New();
        l.A($"Will try-swap module info of {dnnModule.ModuleID} into site");
        var newSite = siteFactory.CreateFromModuleAndShare(dnnModule, ParentLog);
        l.A("Will init module");
        ((DnnModule)context.Module).Init(dnnModule);
        var contextWithModuleAndSite = context.ResetWip(ParentLog, newSite, context.Module);
        return l.ReturnAsOk(InitPageOnly(contextWithModuleAndSite, pageId));
    }

    private IContextOfBlock InitPageOnly(IContextOfBlock context, int? pageId)
    {
        var l = Log.Fn<IContextOfBlock>($"{nameof(pageId)}: {pageId}");
        // Collect / assemble page information
        var activeTab = (context.Site as IWrapper<PortalSettings>)?.GetContents()?.ActiveTab;
        var page = (DnnPage)context.Page;
        var url = page.InitPageIdAndUrl(activeTab, pageId);
        return l.Return(context, url);
    }

}