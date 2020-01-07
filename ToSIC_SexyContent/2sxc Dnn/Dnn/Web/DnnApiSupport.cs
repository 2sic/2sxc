using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using ToSic.Eav.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Dnn.Web
{
    [PrivateApi]
    public class DnnApiSupport
    {
        private const string MetaName = "_jsApi";
        private const string ExtensionPlaceholder = "{extension}";

        public void AddHeaders()
        {
            // ensure we only do this once
            if (MarkAddedAndReturnIfAlreadyDone()) return;

            var pageId = PortalSettings.Current.ActiveTab.TabID.ToString(CultureInfo.InvariantCulture);
            var path = ServicesFramework.GetServiceFrameworkRoot();
            if (string.IsNullOrEmpty(path)) return;

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
        }

        private const string KeyToMarkAdded = "2sxcApiHeadersAdded";

        private static bool MarkAddedAndReturnIfAlreadyDone()
        {
            var alreadyAdded = HttpContextSource.Current.Items.Contains(KeyToMarkAdded);
            HttpContextSource.Current.Items[KeyToMarkAdded] = true;
            return alreadyAdded;
        }

        public string AntiForgeryToken()
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
