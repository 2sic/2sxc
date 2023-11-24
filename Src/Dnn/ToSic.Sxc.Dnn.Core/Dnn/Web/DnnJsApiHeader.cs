using DotNetNuke.Common;
using System.ComponentModel;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Web
{
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class DnnJsApiHeader: HelperBase
    {
        private readonly IJsApiService _dnnJsApiService;

        public DnnJsApiHeader(IJsApiService dnnJsApiService, ILog parentLog = null) : base(parentLog, "Dnn.JsApiH")
        {
            _dnnJsApiService = dnnJsApiService;
        }

        public bool AddHeaders()
        {
            var l = Log.Fn<bool>();
            // ensure we only do this once
            if (MarkAddedAndReturnIfAlreadyDone()) return l.ReturnFalse("already");

            var json = _dnnJsApiService.GetJsApiJson();
            if (json == null) return l.ReturnFalse("no path");

#pragma warning disable CS0618
            HtmlPage.AddMeta(InpageCms.MetaName, json);
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
}
