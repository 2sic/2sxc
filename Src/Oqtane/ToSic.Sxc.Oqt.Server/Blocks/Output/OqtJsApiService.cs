using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Encryption;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output;

internal class OqtJsApiService(
    IAntiforgery antiForgery,
    IHttpContextAccessor http,
    JsApiCacheService jsApiCache,
    AliasResolver aliasResolver,
    RsaCryptographyService rsaCryptographyService)
    : ServiceBase("OqtJsApi", connect: [antiForgery, http, jsApiCache, aliasResolver, rsaCryptographyService]), IJsApiService
{
    public string GetJsApiJson(int? pageId = null, string siteRoot = null, string rvt = null, bool withPublicKey = false) 
        => JsApi.JsApiJson(GetJsApi(pageId, siteRoot, rvt, withPublicKey));

    public JsApi GetJsApi(int? pageId = null, string siteRoot = null, string rvt = null, bool withPublicKey = false)
    {
        return jsApiCache.JsApiJson(
            platform: PlatformType.Oqtane.ToString(),
            pageId: pageId ?? -1,
            siteRoot: SiteRootFn,
            apiRoot: ApiRootFn,
            appApiRoot: SiteRootFn, // without "app/" because the UI will add that later on,
            uiRoot: UiRootFn,
            rvtHeader: Oqtane.Shared.Constants.AntiForgeryTokenHeaderName,
            rvt: RvtFn,
            withPublicKey: withPublicKey,
            secureEndpointPublicKey: SecureEndpointPrimaryKeyFn,
            dialogQuery: null);

        string SiteRootFn() => siteRoot.IsEmpty() ? OqtPageOutput.GetSiteRoot(aliasResolver.Alias) : siteRoot;
        string ApiRootFn() => SiteRootFn() + OqtWebApiConstants.ApiRootNoLanguage + "/";
        string UiRootFn() => OqtConstants.UiRoot + "/";
        string RvtFn() => rvt.IsEmpty() && http?.HttpContext != null ? antiForgery.GetAndStoreTokens(http.HttpContext).RequestToken : rvt;
        string SecureEndpointPrimaryKeyFn() => rsaCryptographyService.PublicKey;
    }

}