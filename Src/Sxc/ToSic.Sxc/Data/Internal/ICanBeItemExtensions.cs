using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Data.Internal;

public static class ICanBeItemExtensions
{
    public static IBlock TryGetBlock(this ICanBeItem item)
        => item?.TryGetBlockContext() as IBlock;
}