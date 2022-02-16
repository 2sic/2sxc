using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("internal use only")]
    public interface IModuleAndBlockBuilder: IHasLog<IModuleAndBlockBuilder>
    {
        // 2022-02-16 2dm - ATM not needed, maybe we'll reactivate if ever requested
        //IModule GetModule(int pageId, int moduleId);
        //IBlockBuilder GetBuilder(int pageId, int moduleId);
        //IBlock GetBlock(IModule module);

        IBlock GetBlock(int pageId, int moduleId);

        IBlock GetBlock<TPlatformModule>(TPlatformModule module) where TPlatformModule : class;
    }
}