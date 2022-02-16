using ToSic.Eav.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks
{
    public interface IModuleAndBlockBuilder: IHasLog<IModuleAndBlockBuilder>
    {
        IModule GetModule(int pageId, int moduleId);

        IBlockBuilder GetBuilder(int pageId, int moduleId);
    }
}