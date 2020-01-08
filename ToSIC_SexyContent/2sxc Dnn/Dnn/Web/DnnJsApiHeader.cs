using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Dnn.Web
{
    [PrivateApi]
    public class DnnJsApiHeader: HasLog
    {
        private const string MetaName = "_jsApi";
        private const string ExtensionPlaceholder = "{extension}";

        public DnnJsApiHeader(ILog parentLog) : base("Dnn.JsApiH", parentLog)
        {
        }

        public bool AddHeaders()
        {
            var wrapLog = Log.Call<bool>();
            // ensure we only do this once
            if (MarkAddedAndReturnIfAlreadyDone()) return wrapLog("already", false);

            var pageId = PortalSettings.Current.ActiveTab.TabID.ToString(CultureInfo.InvariantCulture);
            var path = ServicesFramework.GetServiceFrameworkRoot();
            if (string.IsNullOrEmpty(path)) return wrapLog("no path", false);

            var dnnVersion = DotNetNukeContext.Current.Application.Version.Major;
            var apiRoot = path + (dnnVersion < 9
                ? $"desktopmodules/{ExtensionPlaceholder}/api/"
                : $"api/{ExtensionPlaceholder}/");

            var json = "{"
                       + $"\"page\": {pageId},"
                       + $"\"root\": \"{path}\","
                       + $"\"api\": \"{apiRoot}\","
                       + $"\"rvt\": \"{AntiForgeryToken()}\""
                       + "}";

            HtmlPage.AddMeta(MetaName, json);
            return wrapLog("added", true);
        }

        private const string KeyToMarkAdded = "2sxcApiHeadersAdded";

        private static bool MarkAddedAndReturnIfAlreadyDone()
        {
            var alreadyAdded = HttpContextSource.Current.Items.Contains(KeyToMarkAdded);
            HttpContextSource.Current.Items[KeyToMarkAdded] = true;
            return alreadyAdded;
        }

        private string AntiForgeryToken()
        {
            if (_antiForgeryToken != null) return _antiForgeryToken;

            var tag = AntiForgery.GetHtml().ToString();
            _antiForgeryToken = GetAttribute(tag, "value");
            return _antiForgeryToken;
        }
        private string _antiForgeryToken;

        private static string GetAttribute(string tag, string attribute)
        {
            return new Regex(@"(?<=\b" + attribute + @"="")[^""]*")
                .Match(tag).Value;
        }
    }
}
