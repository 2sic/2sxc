using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("internal use only")]
    public interface IModuleAndBlockBuilder: IHasLog
    {
        BlockWithContextProvider GetProvider(int pageId, int moduleId);

        BlockWithContextProvider GetProvider<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class;
    }
}