using System;
using System.Collections.Concurrent;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Web.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsApiCache : ServiceBase
{
    private const string JsApiKey = "JsApi";
    private readonly IHttp _http;
    private ConcurrentDictionary<int, JsApi> _cache;

    public JsApiCache(IHttp http) : base("JsApi")
    {
        ConnectServices(_http = http);
    }

    public JsApi JsApiJson(string platform,
        int pageId,
        Func<string> siteRoot,
        Func<string> apiRoot,
        Func<string> appApiRoot,
        Func<string> uiRoot,
        string rvtHeader,
        Func<string> rvt,
        string dialogQuery = null // these are any platform specific url query params to the dialog; can be null
    )
    {
        if (_cache == null)
        {
            if (_http.Current.Items[JsApiKey] != null)
                _cache = (ConcurrentDictionary<int, JsApi>)_http.Current.Items[JsApiKey];
            if (_cache == null)
            {
                _cache = new ConcurrentDictionary<int, JsApi>();
                _http.Current.Items[JsApiKey] = _cache;
            }
        }

        if (_cache.TryGetValue(pageId, out var jsApi))
            return jsApi;

        jsApi = new JsApi
        {
            platform = platform.ToLowerInvariant(),
            page = pageId,
            root = siteRoot.Invoke(),
            api = apiRoot.Invoke(),
            appApi = appApiRoot.Invoke(),
            uiRoot = uiRoot.Invoke(),
            rvtHeader = rvtHeader,
            rvt = rvt.Invoke(),
            dialogQuery = dialogQuery
        };
        _cache.AddOrUpdate(pageId, jsApi, (key, value) => jsApi);
            
        return jsApi;
    }
}