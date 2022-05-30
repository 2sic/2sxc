using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("internal use only")]
    public interface IModuleAndBlockBuilder: IHasLog<IModuleAndBlockBuilder>
    {
        IBlock GetBlock(int pageId, int moduleId);

        IBlock GetBlock<TPlatformModule>(TPlatformModule module, int? pageId) where TPlatformModule : class;
    }
}