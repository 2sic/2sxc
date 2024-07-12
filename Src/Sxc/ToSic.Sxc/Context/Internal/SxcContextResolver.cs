using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class SxcContextResolver(
    LazySvc<AppIdResolver> appIdResolverLazy,
    Generator<IContextOfSite> siteCtxGenerator,
    Generator<IContextOfApp> appCtxGenerator,
    Lazy<IFeaturesService> featuresService,
    LazySvc<IHttp> http)
    : ContextResolver(siteCtxGenerator, appCtxGenerator, "Sxc.CtxRes", connect: [appIdResolverLazy, siteCtxGenerator, appCtxGenerator, featuresService, http]), ISxcContextResolver
{
    private const string CookieTemplate = "app-{0}-data-preview";
    private const string CookieLive = "live";

    /// <summary>
    /// Get the best possible context which can give us insights about the user permissions.
    ///
    /// TODO: WIP - requires that if an app is to be used, it was accessed before - not yet perfect...
    /// </summary>
    /// <returns></returns>
    public EffectivePermissions UserPermissions() => _ctxUserPerm ??= GetUserPermissions();
    private EffectivePermissions _ctxUserPerm;

    /// <summary>
    /// Figure out user permissions based on block-context, app-context or site-context.
    /// In addition, (new 17.10) figure out if a cookie is set to show live or draft data.
    /// </summary>
    /// <returns></returns>
    private EffectivePermissions GetUserPermissions()
    {
        var perms = (BlockContextOrNull() ?? AppOrNull() ?? Site())?.Permissions;
        if (perms == null) return new(false);
        if (!perms.ShowDraftData) return perms;

        // Check if an all-apps cookie is set
        return CookieExpectsLive("*") 
            ? new(perms.IsSiteAdmin, perms.IsContentAdmin, perms.IsContentEditor, showDrafts: false) :
            perms;

        // Check if a cookie is set to this specific app
        // 2024-06-03 ATM this doesn't work, because the initial access
        // to get the view etc. already needs to know this, and at that time the block isn't created yet
        // would need quite a bit of work to get it right, so commented out for now.
        //if (blockOrAppCtx != null && CookieExpectsLive(blockOrAppCtx.AppState.AppId.ToString()))
        //    return new(perms.IsSiteAdmin, perms.IsContentAdmin, perms.IsContentEditor,
        //        showDrafts: false);

        bool CookieExpectsLive(string app) => http?.Value.GetCookie(string.Format(CookieTemplate, app)) == CookieLive;
    }

    public IContextOfApp SetAppOrNull(string nameOrPath)
    {
        if (string.IsNullOrWhiteSpace(nameOrPath)) return null;
        var zoneId = Site().Site.ZoneId;
        var appId = appIdResolverLazy.Value.GetAppIdFromPath(zoneId, nameOrPath, false);
        return appId <= Eav.Constants.AppIdEmpty
            ? null
            : SetApp(new AppIdentity(zoneId, appId));
    }

    #region Blocks

    public void AttachBlock(IBlock block)
    {
        _block = block;
        AppContextFromAppOrBlock = _block?.Context;
    }
    private IBlock _block;

    public IBlock BlockOrNull() => _block;

    public IBlock BlockRequired() => BlockOrNull()
                                     ?? throw new("Block required but missing. It was not attached");

    public IContextOfBlock BlockContextRequired() => BlockContextOrNull()
                                                     ?? throw new("Block context required but not known. It was not attached.");

    public IContextOfBlock BlockContextOrNull() => _block?.Context;


    #endregion
}