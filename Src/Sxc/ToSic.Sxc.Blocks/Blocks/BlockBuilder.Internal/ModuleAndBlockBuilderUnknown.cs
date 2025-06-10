﻿using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ModuleAndBlockBuilderUnknown: ModuleAndBlockBuilder
{
    public ModuleAndBlockBuilderUnknown(WarnUseOfUnknown<ModuleAndBlockBuilderUnknown> _, Generator<BlockOfModule>bfmGenerator) : base(bfmGenerator, "Unk")
    {
    }

    protected override IModule GetModuleImplementation(int pageId, int moduleId) => throw new NotSupportedException();

    protected override IContextOfBlock GetContextOfBlock(IModule module, int? pageId) => throw new NotSupportedException();

    protected override IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId) => throw new NotSupportedException();
}