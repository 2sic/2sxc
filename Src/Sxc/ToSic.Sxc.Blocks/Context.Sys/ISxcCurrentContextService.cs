using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// This provides other systems with a context
/// Note that it's important to always make this **Scoped**, not transient, as there is some re-use after initialization
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ISxcCurrentContextService: ICurrentContextService, ISxcAppCurrentContextService
{
    /// <summary>
    /// Attach a prepared block to the context.
    /// </summary>
    void AttachBlock(IBlock block);

    /// <summary>
    /// Retrieve the current block - or return null if none.
    /// </summary>
    IBlock? BlockOrNull();

    /// <summary>
    /// Swap in a dynamic view into the context, to render the block differently (for preview)
    /// </summary>
    IBlock SwapBlockView(IView newView);
}