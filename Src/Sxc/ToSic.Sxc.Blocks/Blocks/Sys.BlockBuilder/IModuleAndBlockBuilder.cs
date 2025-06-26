namespace ToSic.Sxc.Blocks.Sys.BlockBuilder;

[PrivateApi("internal use only")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IModuleAndBlockBuilder: IHasLog
{
    IBlock BuildBlock(int pageId, int moduleId);

    IBlock BuildBlock<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class;
}