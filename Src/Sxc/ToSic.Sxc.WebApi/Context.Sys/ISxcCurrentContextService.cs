using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Context;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// This provides other systems with a context
/// Note that it's important to always make this **Scoped**, not transient, as there is some re-use after initialization
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ISxcCurrentContextService: IContextCompleteService, IContextOfSiteService, IContextService
{
    /// <summary>
    /// Return the block if known, or an app context if not
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    IContextOfApp ShareAppStateOptimized(int appId);

    /// <summary>
    /// Attach a prepared block to the context.
    /// </summary>
    void ShareBlockSpecs(IBlock block);


    /// <summary>
    /// Swap in a dynamic view into the context, to render the block differently (for preview)
    /// </summary>
    IBlock SwapBlockView(IView newView);

}
