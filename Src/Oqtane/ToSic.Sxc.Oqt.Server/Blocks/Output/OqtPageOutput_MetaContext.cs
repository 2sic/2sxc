using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output;

[PrivateApi]
partial class OqtPageOutput
{
    /// <summary>
    /// Determines if the context header is needed
    /// </summary>
    public bool AddContextMeta => AddJsCore || AddJsEdit;

    /// <summary>
    /// Will return the meta-header which the $2sxc client needs for context, page id, request verification token etc.
    /// </summary>
    /// <returns></returns>
    public string ContextMetaContents()
    {
        var l = Log.Fn<string>();
        var pageId = Parent?.Page.PageId ?? -1;
        var siteRoot = GetSiteRoot(siteState.Alias);
        var rvt = AntiForgeryToken();
        var result = jsApiService.GetJsApiJson(pageId, siteRoot, rvt);
        return l.ReturnAsOk(result);
    }

    /// <summary>
    /// Empty / Fake Anti-Forgery Token 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// 2021-05-11 the Token given by IAntiforgery is always wrong, so better keep it empty
    /// Reason is probably that at this moment the render-request is running in another HttpContext
    /// </remarks>
    private string AntiForgeryToken() => "";

    public string ContextMetaName => JsApi.MetaName;
}