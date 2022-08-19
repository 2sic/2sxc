using ToSic.Sxc.Context;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output
{
    internal class OqtJsApi
    {
        internal static string GetJsApi(int pageId, string siteRoot, string rvt) =>
            InpageCms.JsApiJson(
                platform: PlatformType.Oqtane.ToString(),
                pageId: pageId,
                siteRoot: siteRoot,
                apiRoot: siteRoot + WebApiConstants.ApiRootWithNoLang + "/",
                appApiRoot: siteRoot, // without "app/" because the UI will add that later on,
                uiRoot: OqtConstants.UiRoot + "/",
                rvtHeader: Oqtane.Shared.Constants.AntiForgeryTokenHeaderName,
                rvt: rvt, 
                dialogQuery: null);
    }
}
