using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IJsApiService
{
    string GetJsApiJson(int? pageId = null, string siteRoot = null, string rvt = null);
    JsApi GetJsApi(int? pageId = null, string siteRoot = null, string rvt = null);
}