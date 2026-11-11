using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Blocks.Sys.BlockBuilder;

public interface IEnvModuleBuilder
{
    /// <summary>
    /// Get the module specific to each platform.
    /// </summary>
    IModule GetModule(int pageId, int moduleId);

    IContextOfModule GetContextOfBlock(IModule module, int? pageId);
}
