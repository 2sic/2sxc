namespace ToSic.Sxc.Blocks.Internal;

[PrivateApi("internal use only")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IModuleAndBlockBuilder: IHasLog
{
    BlockWithContextProvider GetProvider(int pageId, int moduleId);

    BlockWithContextProvider GetProvider<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class;
}