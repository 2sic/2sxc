﻿using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Blocks;

internal class OqtModuleAndBlockBuilder(
    Generator<IModule> moduleGenerator,
    Generator<IContextOfBlock> contextGenerator,
    Generator<BlockOfModule> blockGenerator,
    Generator<IModuleRepository> moduleRepositoryGenerator,
    RequestHelper requestHelper)
    : ModuleAndBlockBuilder(blockGenerator, OqtConstants.OqtLogPrefix,
        connect: [moduleGenerator, contextGenerator, moduleRepositoryGenerator, requestHelper])
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageId">not required in Oqtane</param>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    protected override IModule GetModuleImplementation(int pageId, int moduleId)
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
        var context = contextGenerator.New();
        l.A("Will init module");
        ((OqtModule) context.Module).Init(oqtModule);
        return l.Return(InitPageOnly(context, pageId));
    }

    private IContextOfBlock InitPageOnly(IContextOfBlock context, int? pageId)
    {
        // TODO: try to use the pageId if given, would usually only be used in inner-content / IRenderService scenarios
        var l = Log.Fn<IContextOfBlock>();
        // Collect / assemble page information
        context.Page.Init(requestHelper.TryGetId(ContextConstants.PageIdKey));
        var url = context.Page.Url;
        return l.Return(context, url);
    }
}