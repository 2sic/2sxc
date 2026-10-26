using ToSic.Eav.Context;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Users.Permissions;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// This provides other systems with a context
/// Note that it's important to always make this **Scoped**, not transient, as there is some re-use after initialization
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ISxcCurrentContextService: ICurrentContextUserPermissionsService
{
    #region previously ICurrentContextService

    /// <summary>
    /// This is the most basic kind of context. ATM you could also inject it directly,
    /// but we want to introduce the capability of giving a static site or something
    /// without having to write code implementing IContextOfSite
    /// </summary>
    /// <value></value>
    IContextOfSite SiteContext { get; }

    IContextOfApp? AppContextOrNull();

    /// <summary>
    /// Evaluate the supplied context's permissions, including the live/draft preference,
    /// without reading or changing the cached ambient permissions.
    /// </summary>
    EffectivePermissions UserPermissions(IContextOfSite context);

    #endregion

    #region Preiously ISxcAppCurrentContextService

    /// <summary>
    /// Return the block if known, or an app context if not
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    IContextOfApp ShareAppStateOptimized(int appId);

    // 2026-10-20 2dm after refactoring doesn't seem needed any more
    //IContextOfApp SetAppFromPathOrGetExistingBlock(string nameOrPath);

    // Note 2026-10-18 2dm: #NoLookupSideEffects
    //IContextOfApp? SetAppFromPathOrNull(string? nameOrPath);

    // Note 2026-10-18 2dm: #NoLookupSideEffects
    //IContextOfApp SetAppFromNameOrUrlParamsOrUseExistingBlockRequired(string? nameOrPath);

    #endregion

    /// <summary>
    /// Attach a prepared block to the context.
    /// </summary>
    void ShareBlockSpecs(IBlock block);

    /// <summary>
    /// Retrieve the current block - or return null if none.
    /// </summary>
    IBlock? BlockOrNull();

    /// <summary>
    /// Swap in a dynamic view into the context, to render the block differently (for preview)
    /// </summary>
    IBlock SwapBlockView(IView newView);

    IContextOfApp? TryFindAppFromPath(string? nameOrPath);
    IContextOfApp FindAppFromPathNameUrlIdsOrSharedBlock(string? nameOrPath);
    // IContextOfApp? ShareAppContext(IContextOfApp? appContext);
}
