using ToSic.Eav.Context;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Context;
using ToSic.Sys.Users.Permissions;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// This provides other systems with a context
/// Note that it's important to always make this **Scoped**, not transient, as there is some re-use after initialization
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ISxcCurrentContextService: IContextCompleteService, IContextOfSiteService, IContextService
{
    #region previously ICurrentContextService

    /// <summary>
    /// Evaluate the supplied context's permissions, including the live/draft preference,
    /// without reading or changing the cached ambient permissions.
    /// </summary>
    EffectivePermissions UserPermissions(IContextOfSite context);

    #endregion

    #region Previously ISxcAppCurrentContextService

    /// <summary>
    /// Return the block if known, or an app context if not
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    IContextOfApp ShareAppStateOptimized(int appId);

    #endregion

    /// <summary>
    /// Attach a prepared block to the context.
    /// </summary>
    void ShareBlockSpecs(IBlock block);


    /// <summary>
    /// Swap in a dynamic view into the context, to render the block differently (for preview)
    /// </summary>
    IBlock SwapBlockView(IView newView);

}

/// <summary>
/// Service to provide the complete context.
/// </summary>
public interface IContextCompleteService: IContextOfSiteService
{
    /// <summary>
    /// Retrieve the current app with reader etc. or null if not available.
    /// </summary>
    /// <remarks>
    /// Can be standalone (App only) or with a block (App + Block).
    /// </remarks>
    IContextOfApp? AppContextOrNull { get; }

    /// <summary>
    /// Retrieve the current block - or return null if block is not known.
    /// </summary>
    IBlock? BlockOrNull { get; }
}