namespace ToSic.Sxc.Blocks.Internal;

[PrivateApi("internal use only")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IModuleAndBlockBuilder: IHasLog
{
    IBlock BuildBlock(int pageId, int moduleId);

    IBlock BuildBlock<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class;
}