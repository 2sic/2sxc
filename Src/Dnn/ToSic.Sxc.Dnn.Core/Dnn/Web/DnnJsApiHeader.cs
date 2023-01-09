using DotNetNuke.Common;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Edit;

namespace ToSic.Sxc.Dnn.Web
{
    [PrivateApi]
    public class DnnJsApiHeader: ServiceBase
    {
        public DnnJsApiHeader() : base("Dnn.JsApiH")
        {
        }

        public bool AddHeaders()
        {
            var wrapLog = Log.Fn<bool>();
            // ensure we only do this once
            if (MarkAddedAndReturnIfAlreadyDone()) return wrapLog.ReturnFalse("already");
            
            var json = DnnJsApi.GetJsApiJson();
            if (json == null) return wrapLog.ReturnFalse("no path");

#pragma warning disable CS0618
            HtmlPage.AddMeta(InpageCms.MetaName, json);
#pragma warning restore CS0618
            return wrapLog.ReturnTrue("added");
        }
  
        private const string KeyToMarkAdded = "2sxcApiHeadersAdded";

        private static bool MarkAddedAndReturnIfAlreadyDone()
        {
            var alreadyAdded = HttpContextSource.Current.Items.Contains(KeyToMarkAdded);
            HttpContextSource.Current.Items[KeyToMarkAdded] = true;
            return alreadyAdded;
        }
    }
}
