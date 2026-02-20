using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks.Sys.BlockBuilder;

[PrivateApi("internal use only")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IModuleAndBlockBuilder: IHasLog
{
    IBlock BuildBlock(int pageId, int moduleId);

    IBlock BuildBlock<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class;

    /// <summary>
    /// Get the module specific to each platform.
    /// </summary>
    IModule GetModule(int pageId, int moduleId);
}