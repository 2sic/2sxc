using DotNetNuke.Entities.Modules;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Dnn;

internal class DnnModuleBuilder(LazySvc<DnnModuleContextFactory> ctxOfModFac, DnnModuleFactory moduleFactory)
    : ServiceBase(DnnConstants.LogName + "MnBBld", connect: [ctxOfModFac, moduleFactory]),
        IEnvModuleBuilder
{
    public IModule GetModule(int pageId, int moduleId)
    {
        var l = Log.Fn<IModule>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
        var moduleInfo = new ModuleController().GetModule(moduleId, pageId, false)
                         ?? throw new($"Can't find module {moduleId} on page {pageId}. Maybe you reversed the ID-order?");

        l.A($"Page Id on DNN Module: {moduleInfo.TabID} - should be {pageId}");

        var module = moduleFactory.Get(moduleInfo);

        return l.Return(module, $"Page Id on IModule: {module.BlockIdentifier} - should be {pageId}");
    }

    public IContextOfModule GetContextOfBlock(IModule module, int? pageId)
        => GetContextOfBlock((module as IWrapper<ModuleInfo>)?.GetContents(), pageId);


    private IContextOfModule GetContextOfBlock(ModuleInfo module, int? pageId)
    {
        if (module == null)
            throw new ArgumentNullException(nameof(module));
        Log.A($"Module: {module.ModuleID}");

        return ctxOfModFac.Value.InitDnnSiteModuleAndBlockContext(module, pageId);
    }
}
