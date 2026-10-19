using ToSic.Sxc.Blocks.Sys;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Context.Sys.Module;

public class ModuleFactoryUnknown(WarnUseOfUnknown<ModuleFactoryUnknown> _) : IModuleFactory
{
    public IModule Get(int moduleId) =>
        new ModuleRecord<object>(moduleId, false, new BlockIdentifier(0, 0, "unknown", Guid.Empty, Guid.Empty))
        {
            Contents = null
        };
}