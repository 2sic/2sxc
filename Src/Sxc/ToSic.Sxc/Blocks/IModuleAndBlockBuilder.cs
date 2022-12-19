using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("internal use only")]
    public interface IModuleAndBlockBuilder: IHasLog
    {
        IBlock GetBlock(int pageId, int moduleId);

        IBlock GetBlock<TPlatformModule>(TPlatformModule module, int? pageId) where TPlatformModule : class;
    }
}