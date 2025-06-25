﻿using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Blocks.Sys.BlockBuilder;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class ModuleAndBlockBuilder(Generator<BlockOfModule> blockGenerator, string logPrefix, object[]? connect = default)
    : ServiceBase($"{logPrefix}.BnMBld", connect: [..connect ?? [], blockGenerator]), IModuleAndBlockBuilder
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

    public IBlock BuildBlock(int pageId, int moduleId)
    {
        var l = Log.Fn<IBlock>($"{pageId}, {moduleId}");
        var module = GetModuleImplementation(pageId, moduleId);
        var ctx = GetContextOfBlock(module, pageId);
        
        var block = blockGenerator.New().GetBlockOfModule(ctx);
        return l.ReturnAsOk(block);
    }

    public IBlock BuildBlock<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class
    {
        var l = Log.Fn<IBlock>($"{module}, {page}");
        var ctx = GetContextOfBlock(module, page);
        var block = blockGenerator.New().GetBlockOfModule(ctx);
        return l.Return(block);
    }

    protected abstract IContextOfBlock GetContextOfBlock(IModule module, int? pageId);

    protected abstract IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId);
}