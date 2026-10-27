using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.DataSources.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Sys.Http;
using ToSic.Sys.Context;
using ToSic.Sys.Context.Sys;
using ToSic.Sys.Users;
using ToSic.Sys.Users.Permissions;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// Main context resolver - provides context information about the site, app, block, permissions etc.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal partial class SxcCurrentContextService(
    //LazySvc<AppIdResolver> appIdResolverLazy,
    ISite site, // IMPORTANT: Never import the IContextOfSiteService itself, since this class is registered to implement it.
    IUser user,
    Generator<IContextOfAppFactory> appCtxGenerator,
    LazySvc<IContextOfBlockFactory> blockCtxFactory,
    LazySvc<IFeaturesService> featuresService,
    LazySvc<IAppReaderFactory> appReaderFactory,
    LazySvc<BlockDataSourceFactory> bdsFactoryLazy,
    LazySvc<IHttp> http)
    : ServiceBase("Sxc.CtxRes",
        connect: [appCtxGenerator, blockCtxFactory, featuresService, http, appReaderFactory, bdsFactoryLazy]),
        ISxcCurrentContextService
{
    private const string CookieTemplate = "app-{0}-data-preview";
    private const string CookieLive = "live";

    public IContextOfUser Context => SiteContext;

    IContextOfSite IContextOfSiteService.Context => SiteContext;


    private IContextOfSite SiteContext => (IContextOfSite?)AppContextOrNull ??
                                          (field ??= new ContextOfSite(site, user) { Permissions = UserPermissions(new ContextOfSite(site, user)) });

    /// <summary>
    /// Get the best possible context which can give us insights about the user permissions.
    ///
    /// TODO: WIP - requires that if an app is to be used, it was accessed before - not yet perfect...
    /// </summary>
    /// <returns></returns>
    /// <summary>
    /// Figure out user permissions based on block-context, app-context or site-context.
    /// In addition, (new 17.10) figure out if a cookie is set to show live or draft data.
    /// </summary>
    /// <returns></returns>
    public EffectivePermissions UserPermissions() => _ctxUserPerm ??= UserPermissions(SiteContext);
    private EffectivePermissions? _ctxUserPerm;

    /// <inheritdoc />
    public EffectivePermissions UserPermissions(IContextOfSite context)
    {
        var perms = context.Permissions;
        //if (perms == null)
        //    return new(false);
        if (!perms.ShowDraftData)
            return perms;

        // Check if an all-apps cookie is set
        return CookieExpectsLive("*") 
            ? perms with { ShowDraftData = false }
            : perms;

        // Check if a cookie is set to this specific app
        // 2024-06-03 ATM this doesn't work, because the initial access
        // to get the view etc. already needs to know this, and at that time the block isn't created yet
        // would need quite a bit of work to get it right, so commented out for now.
        //if (blockOrAppCtx != null && CookieExpectsLive(blockOrAppCtx.AppState.AppId.ToString()))
        //    return new(perms.IsSiteAdmin, perms.IsContentAdmin, perms.IsContentEditor,
        //        showDrafts: false);

        bool CookieExpectsLive(string app) => http?.Value.GetCookie(string.Format(CookieTemplate, app)) == CookieLive;
    }

    private T CtxWithUpdatedDrafting<T>(T origCtx) where T: IContextOfSite
    {
        var updated = UserPermissions(origCtx);
        if (origCtx is ContextOfBlock asBlockCtx)
            return (T)(IContextOfBlock)(asBlockCtx with { Permissions = updated });

        if (origCtx is ContextOfApp asAppCtx)
            return (T)(IContextOfApp)(asAppCtx with { Permissions = updated });

        if (origCtx is ContextOfSite asSiteCtx)
            return (T)(IContextOfSite)(asSiteCtx with { Permissions = updated });

        throw new NullReferenceException("Context is not Block or App, cannot update permissions view.");
        //var blockCtx = origCtx as ContextOfBlock
        //               ?? throw new NullReferenceException("Block context is not attached, cannot swap view.");
        //var ctxWithUpdatedDrafting = blockCtx with { Permissions = UserPermissions(blockCtx) };
        //return ctxWithUpdatedDrafting;
    }


}
