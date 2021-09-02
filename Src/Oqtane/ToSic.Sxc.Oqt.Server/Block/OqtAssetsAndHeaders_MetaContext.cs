using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Block
{
    [PrivateApi]
    public partial class OqtAssetsAndHeaders
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
            var wrapLog = Log.Call<string>();

            var pageId = Parent?.Page.PageId ?? -1;
            var siteRoot = GetSiteRoot(_siteState);
            var apiRoot = siteRoot + WebApiConstants.ApiRoot + "/";
            var appApiRoot = siteRoot; // without "app/" because the UI will add that later on
            var result = InpageCms.JsApiJson(
                platform: PlatformType.Oqtane.ToString(),
                pageId: pageId, 
                siteRoot: siteRoot, 
                apiRoot: apiRoot, 
                appApiRoot: appApiRoot, 
                uiRoot: OqtConstants.UiRoot + "/",
                rvtHeader: Oqtane.Shared.Constants.AntiForgeryTokenHeaderName,
                rvt: AntiForgeryToken());
            return wrapLog("ok", result);
        }

        public string ContextMetaName => InpageCms.MetaName;

        /// <summary>
        /// Empty / Fake Anti-Forgery Token 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 2021-05-11 the Token given by IAntiforgery is always wrong, so better keep it empty
        /// Reason is probably that at this moment the render-request is running in another HttpContext
        /// </remarks>
        private string AntiForgeryToken() => "";

    }
}
