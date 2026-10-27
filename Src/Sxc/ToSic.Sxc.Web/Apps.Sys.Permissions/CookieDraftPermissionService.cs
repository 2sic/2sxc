using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sxc.Web.Sys.Http;
using ToSic.Sys.Users.Permissions;

namespace ToSic.Sxc.Context.Sys;

public class DraftOverrideFromCookieService(LazySvc<IHttp> http) : ServiceBase("Sxc.CtxCookPerm"), IDraftOverrideService
{
    private const string CookieTemplate = "app-{0}-data-preview";
    private const string CookieLive = "live";

    /// <inheritdoc/>
    public EffectivePermissions UserPermissions(EffectivePermissions permissions)
    {
        if (!permissions.ShowDraftData)
            return permissions;

        // Check if an all-apps cookie is set
        return CookieExpectsLive("*")
            ? permissions with { ShowDraftData = false }
            : permissions;

        // Check if a cookie is set to this specific app
        // 2024-06-03 ATM this doesn't work, because the initial access
        // to get the view etc. already needs to know this, and at that time the block isn't created yet
        // would need quite a bit of work to get it right, so commented out for now.
        //if (blockOrAppCtx != null && CookieExpectsLive(blockOrAppCtx.AppState.AppId.ToString()))
        //    return new(perms.IsSiteAdmin, perms.IsContentAdmin, perms.IsContentEditor,
        //        showDrafts: false);

        bool CookieExpectsLive(string app) => http?.Value.GetCookie(string.Format(CookieTemplate, app)) == CookieLive;
    }

}
