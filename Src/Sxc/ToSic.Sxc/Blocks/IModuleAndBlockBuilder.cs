using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Blocks;

[PrivateApi("internal use only")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IModuleAndBlockBuilder: IHasLog
{
    BlockWithContextProvider GetProvider(int pageId, int moduleId);

    BlockWithContextProvider GetProvider<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class;
}