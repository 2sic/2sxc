using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Blocks;

internal class OqtModuleBuilder(Generator<IModule> modGen, OqtContextOfBlockFactory blockCtxFac, Generator<IModuleRepository> modRepoGen, RequestHelper requestHelper)
    : ServiceBase(OqtConstants.OqtLogPrefix, connect: [modGen, blockCtxFac, modRepoGen, requestHelper]),
        IEnvModuleBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageId">not required in Oqtane</param>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    public IModule GetModule(int pageId, int moduleId)
    {
        var oqtModule = modRepoGen.New().GetModule(moduleId)
            ?? throw new($"Can't find module {moduleId} on page {pageId}. Maybe you reversed the ID-order?");
        var module = ((OqtModule) modGen.New()).Init(oqtModule);
        return module;
    }

    public IContextOfModule GetContextOfBlock(IModule module, int? pageId)
        => GetContextOfBlock2((module as OqtModule)?.GetContents(), pageId);

    private IContextOfModule GetContextOfBlock2(Module module, int? pageId)
    {
        var l = Log.Fn<IContextOfModule>();
        if (module == null)
            throw new ArgumentNullException(nameof(module));

        l.A($"Module: {module.ModuleId}");
        var context = blockCtxFac.Create(requestHelper.TryGetId(ContextConstants.PageIdKey), module);
        var url = context.Page.Url;
        return l.Return(context, url);
    }

}
