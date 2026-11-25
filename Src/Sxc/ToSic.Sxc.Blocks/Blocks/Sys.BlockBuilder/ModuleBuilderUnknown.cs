using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Blocks.Sys.BlockBuilder;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ModuleBuilderUnknown: IEnvModuleBuilder
{
    public ModuleBuilderUnknown(WarnUseOfUnknown<ModuleBuilderUnknown> _)
    { }

    public IModule GetModule(int pageId, int moduleId)
        => throw new NotSupportedException();

    public IContextOfModule GetContextOfBlock(IModule module, int? pageId)
        => throw new NotSupportedException();

    public IContextOfModule GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId)
        => throw new NotSupportedException();
}