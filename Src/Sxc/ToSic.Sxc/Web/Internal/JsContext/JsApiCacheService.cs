using System.Collections.Concurrent;
using ToSic.Lib.Services;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.Web.Internal.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsApiCacheService(IHttp http) : ServiceBase("JsApi", connect: [http])
{
    private const string JsApiKey = "JsApi";

    /// <summary>
    /// Create the JsAPI - or get from cache.
    /// This is why we provide functions for most properties, so they don't need to be accessed if not needed.
    /// </summary>
    /// <returns></returns>
    public JsApi JsApiJson(
        string platform,
        int pageId,
        Func<string> siteRoot,
        Func<string> apiRoot,
        Func<string> appApiRoot,
        Func<string> uiRoot,
        string rvtHeader,
        Func<string> rvt,
        bool withPublicKey,
        Func<string> secureEndpointPublicKey,
        string dialogQuery = null // these are any platform specific url query params to the dialog; can be null
    )
    {
        // Minor hack: if with secure endpoint, use negative page-id for cache
        var cacheKey = withPublicKey ? -pageId : pageId;

        if (Cache.TryGetValue(cacheKey, out var jsApi))
            return jsApi;

        jsApi = new()
        {
            Platform = platform.ToLowerInvariant(),
            Page = pageId,
            Root = siteRoot.Invoke(),
            Api = apiRoot.Invoke(),
            AppApi = appApiRoot.Invoke(),
            UiRoot = uiRoot.Invoke(),
            RvtHeader = rvtHeader,
            Rvt = rvt.Invoke(),
            DialogQuery = dialogQuery,
            PublicKey = withPublicKey ? secureEndpointPublicKey.Invoke() : null,
        };
        Cache.AddOrUpdate(cacheKey, jsApi, (key, value) => jsApi);
            
        return jsApi;
    }

    private ConcurrentDictionary<int, JsApi> Cache => field ??= GetCache();

    private ConcurrentDictionary<int, JsApi> GetCache()
    {
        if (http.Current.Items[JsApiKey] is ConcurrentDictionary<int, JsApi> cache)
            return cache;
        cache = new();
        http.Current.Items[JsApiKey] = cache;
        return cache;
    }


}