using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Blocks.Sys.BlockBuilder;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ModuleAndBlockBuilderUnknown: ModuleAndBlockBuilder
{
    public ModuleAndBlockBuilderUnknown(WarnUseOfUnknown<ModuleAndBlockBuilderUnknown> _, Generator<BlockOfModule>bfmGenerator) : base(bfmGenerator, "Unk")
    { }

    public override IModule GetModule(int pageId, int moduleId)
        => throw new NotSupportedException();

    protected override IContextOfBlock GetContextOfBlock(IModule module, int? pageId)
        => throw new NotSupportedException();

    protected override IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId)
        => throw new NotSupportedException();
}