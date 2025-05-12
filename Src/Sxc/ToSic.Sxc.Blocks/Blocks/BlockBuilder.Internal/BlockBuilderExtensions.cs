using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Blocks.BlockBuilder.Internal;

public static class BlockBuilderExtensions
{
    public static IBlockBuilder GetBlockBuilder(this IBlock block)
        => (IBlockBuilder)block.BlockBuilder;
}