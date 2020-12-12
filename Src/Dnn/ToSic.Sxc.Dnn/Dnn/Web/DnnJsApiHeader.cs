using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Edit;

namespace ToSic.Sxc.Dnn.Web
{
    [PrivateApi]
    public class DnnJsApiHeader: HasLog
    {
        public DnnJsApiHeader(ILog parentLog) : base("Dnn.JsApiH", parentLog)
        {
        }

        public bool AddHeaders()
        {
            var wrapLog = Log.Call<bool>();
            // ensure we only do this once
            if (MarkAddedAndReturnIfAlreadyDone()) return wrapLog("already", false);
            var siteRoot = ServicesFramework.GetServiceFrameworkRoot();
            if (string.IsNullOrEmpty(siteRoot)) return wrapLog("no path", false);

            var dnnVersion = DotNetNukeContext.Current.Application.Version.Major;
            var apiRoot = siteRoot + (dnnVersion < 9
                ? $"desktopmodules/{InpageCms.ExtensionPlaceholder}/api/"
                : $"api/{InpageCms.ExtensionPlaceholder}/");

            var portal = PortalSettings.Current;
            var json = InpageCms.JsApiJson(
                portal.ActiveTab.TabID, 
                siteRoot, 
                apiRoot, 
                AntiForgeryToken(),
                VirtualPathUtility.ToAbsolute(DnnConstants.SysFolderRootVirtual));

            HtmlPage.AddMeta(InpageCms.MetaName, json);
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
