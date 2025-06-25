using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys;

namespace ToSic.Sxc.Render.Internal;

public static class ICanBeItemExtensions
{
    public static IBlock GetRequiredBlockForRender(this ICanBeItem item)
        => item?.TryGetBlock() as IBlock
           ?? throw new NullReferenceException(
               "Tried to get the block off of this item but got null, can't do inner render as expected");
}