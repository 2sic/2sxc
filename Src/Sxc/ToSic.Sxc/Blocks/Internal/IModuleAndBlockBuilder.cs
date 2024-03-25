namespace ToSic.Sxc.Blocks.Internal;

[PrivateApi("internal use only")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IModuleAndBlockBuilder: IHasLog
{
    IBlock BuildBlock(int pageId, int moduleId);

    IBlock BuildBlock<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class;
}