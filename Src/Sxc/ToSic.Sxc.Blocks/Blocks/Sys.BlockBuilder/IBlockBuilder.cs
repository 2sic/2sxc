using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks.Sys.BlockBuilder;

[PrivateApi("internal use only")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IBlockBuilder: IHasLog
{
    IBlock BuildBlock(int pageId, int moduleId);

}
