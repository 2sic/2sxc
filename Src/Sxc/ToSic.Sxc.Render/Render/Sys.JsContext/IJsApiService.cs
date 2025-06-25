namespace ToSic.Sxc.Render.Sys.JsContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IJsApiService
{
    string GetJsApiJson(int? pageId = null, string? siteRoot = null, string? rvt = null, bool withPublicKey = false);

    JsApi GetJsApi(int? pageId, string? siteRoot, string? rvt, bool withPublicKey = false);
}