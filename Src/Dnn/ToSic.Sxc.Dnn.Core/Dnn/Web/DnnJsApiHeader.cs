using DotNetNuke.Common;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Dnn.Web;

internal class DnnJsApiHeader(IJsApiService dnnJsApiService, ILog parentLog = null) : HelperBase(parentLog, "Dnn.JsApiH")
{
    public bool AddHeaders()
    {
        var l = Log.Fn<bool>();
        // ensure we only do this once
        if (MarkAddedAndReturnIfAlreadyDone()) return l.ReturnFalse("already");

        var json = dnnJsApiService.GetJsApiJson(pageId: null, siteRoot: null, rvt: null, withPublicKey: false);
        if (json == null) return l.ReturnFalse("no path");

#pragma warning disable CS0618
        HtmlPage.AddMeta(JsApi.MetaName, json);
#pragma warning restore CS0618
        return l.ReturnTrue("added");
    }
  
    private const string KeyToMarkAdded = "2sxcApiHeadersAdded";

    private static bool MarkAddedAndReturnIfAlreadyDone()
    {
        var alreadyAdded = HttpContextSource.Current.Items.Contains(KeyToMarkAdded);
        HttpContextSource.Current.Items[KeyToMarkAdded] = true;
        return alreadyAdded;
    }
}