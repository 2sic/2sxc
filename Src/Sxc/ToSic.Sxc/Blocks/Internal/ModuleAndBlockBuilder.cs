using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class ModuleAndBlockBuilder(Generator<BlockFromModule> blockGenerator, string logPrefix)
    : ServiceBase($"{logPrefix}.BnMBld", connect: [blockGenerator]), IModuleAndBlockBuilder
{
    /// <summary>
    /// Get the module specific to each platform.
    /// </summary>
    protected abstract IModule GetModuleImplementation(int pageId, int moduleId);

    protected void ThrowIfModuleIsNull<TModule>(int pageId, int moduleId, TModule moduleInfo)
    {
        if (moduleInfo != null) return;
        var msg = $"Can't find module {moduleId} on page {pageId}. Maybe you reversed the ID-order?";
        Log.A(msg);
        throw new(msg);
    }

    public BlockWithContextProvider GetProvider(int pageId, int moduleId)
    {
        var l = Log.Fn<BlockWithContextProvider>($"{pageId}, {moduleId}");
        var module = GetModuleImplementation(pageId, moduleId);
        var ctx = GetContextOfBlock(module, pageId);
        
        // 2024-03-11 2dm WIP
        var block = blockGenerator.New().Init(ctx);
        return l.ReturnAsOk(new(block /*, () => blockGenerator.New().Init(ctx)*/));
    }

    public BlockWithContextProvider GetProvider<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class
    {
        var ctx = GetContextOfBlock(module, page);
        var block = blockGenerator.New().Init(ctx);
        return new(block /*, () => blockGenerator.New().Init(ctx) */);
    }

    protected abstract IContextOfBlock GetContextOfBlock(IModule module, int? pageId);

    protected abstract IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId);
}