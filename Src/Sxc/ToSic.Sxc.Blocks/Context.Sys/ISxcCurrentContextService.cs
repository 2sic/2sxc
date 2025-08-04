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
    /// Return the block or throw an error
    /// </summary>
    IContextOfBlock BlockContextRequired();

    /// <summary>
    /// Return the block if known, or null if not
    /// </summary>
    /// <returns>The current block or null</returns>
    IContextOfBlock? BlockContextOrNull();

    void AttachBlock(IBlock block);

    IBlock? BlockOrNull();

    IBlock BlockRequired();

    IBlock SwapBlockView(IView newView);
}