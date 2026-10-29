using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Blocks;

internal class OqtModuleAndBlockBuilder(
    Generator<IModule> moduleGenerator,
    OqtContextOfBlockFactory contextOfBlockFactory,
    Generator<BlockOfModule> blockGenerator,
    Generator<IModuleRepository> moduleRepositoryGenerator,
    RequestHelper requestHelper)
    : ModuleAndBlockBuilder(blockGenerator, OqtConstants.OqtLogPrefix,
        connect: [moduleGenerator, contextOfBlockFactory, moduleRepositoryGenerator, requestHelper])
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageId">not required in Oqtane</param>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    public override IModule GetModule(int pageId, int moduleId)
    {
        var oqtModule = moduleRepositoryGenerator.New().GetModule(moduleId);
        ThrowIfModuleIsNull(pageId, moduleId, oqtModule);
        var module = ((OqtModule) moduleGenerator.New()).Init(oqtModule);
        return module;
    }

    protected override IContextOfBlock GetContextOfBlock(IModule module, int? pageId)
        => GetContextOfBlock((module as OqtModule)?.GetContents(), pageId);

    protected override IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId)
    {
        var l = Log.Fn<IContextOfBlock>();
        if (module == null) throw new ArgumentNullException(nameof(module));

        var oqtModule = module switch
        {
            Module oModule => oModule,
            PageModule oPageModule => oPageModule.Module,
            _ => throw new ArgumentException("Given data is not a module")
        };

        l.A($"Module: {oqtModule.ModuleId}");
        var initializedCtx = InitOqtSiteModuleAndBlockContext(oqtModule, pageId);
        return l.ReturnAsOk(initializedCtx);
    }


    private IContextOfBlock InitOqtSiteModuleAndBlockContext(Module oqtModule, int? pageId)
    {
        var l = Log.Fn<IContextOfBlock>();
        l.A("Will init module");
        // TODO: try to use the pageId if given, would usually only be used in inner-content / IRenderService scenarios
        // Collect / assemble page information
        var context = contextOfBlockFactory.Create(requestHelper.TryGetId(ContextConstants.PageIdKey), oqtModule);
        var url = context.Page.Url;
        return l.Return(context, url);
    }
}
