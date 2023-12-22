using System.Collections.Concurrent;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output;

internal class OqtJsApiService : ServiceBase, IJsApiService
{
    private readonly IAntiforgery _antiForgery;
    private readonly IHttpContextAccessor _http;
    private readonly JsApiCache _jsApiCache;
    private readonly SiteStateInitializer _siteStateInitializer;

    public OqtJsApiService(IAntiforgery antiForgery, IHttpContextAccessor http, JsApiCache jsApiCache, SiteStateInitializer siteStateInitializer) : base("OqtJsApi")
    {
        ConnectServices(
            _antiForgery = antiForgery,
            _http = http,
            _jsApiCache = jsApiCache,
            _siteStateInitializer = siteStateInitializer
        );
    }

    public string GetJsApiJson(int? pageId = null, string siteRoot = null, string rvt = null) 
        => InpageCms.JsApiJson(GetJsApi(pageId, siteRoot, rvt));

    public JsApi GetJsApi(int? pageId = null, string siteRoot = null, string rvt = null)
    {
        string SiteRootFn() => siteRoot.IsEmpty() ? OqtPageOutput.GetSiteRoot(_siteStateInitializer?.InitializedState) : siteRoot;
        string ApiRootFn() => SiteRootFn() + OqtWebApiConstants.ApiRootWithNoLang + "/";
        string UiRootFn() => OqtConstants.UiRoot + "/";
        string RvtFn() => rvt.IsEmpty() && _http?.HttpContext != null ? _antiForgery.GetAndStoreTokens(_http.HttpContext).RequestToken : rvt;

        return _jsApiCache.JsApiJson(
            platform: PlatformType.Oqtane.ToString(),
            pageId: pageId ?? -1,
            siteRoot: SiteRootFn,
            apiRoot: ApiRootFn,
            appApiRoot: SiteRootFn, // without "app/" because the UI will add that later on,
            uiRoot: UiRootFn,
            rvtHeader: Oqtane.Shared.Constants.AntiForgeryTokenHeaderName,
            rvt: RvtFn,
            dialogQuery: null);
    }

    private ConcurrentDictionary<int, JsApi> _cache;
}