using ToSic.Eav.Logging;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("internal use only")]
    public interface IModuleAndBlockBuilder: IHasLog<IModuleAndBlockBuilder>
    {
        IBlock GetBlock(int pageId, int moduleId);

        IBlock GetBlock<TPlatformModule>(TPlatformModule module, int? pageId) where TPlatformModule : class;
    }
}