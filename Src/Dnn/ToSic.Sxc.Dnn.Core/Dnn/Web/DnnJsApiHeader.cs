using System;
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
using ToSic.Sxc.Context;
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
            var wrapLog = Log.Fn<bool>();
            // ensure we only do this once
            if (MarkAddedAndReturnIfAlreadyDone()) return wrapLog.ReturnFalse("already");
            var siteRoot = ServicesFramework.GetServiceFrameworkRoot();
            if (string.IsNullOrEmpty(siteRoot)) return wrapLog.ReturnFalse("no path");

            var apiRoots = GetApiRoots(siteRoot);

            
            var portal = PortalSettings.Current;
            var json = InpageCms.JsApiJson(
                platform: PlatformType.Dnn.ToString(),
                pageId: portal.ActiveTab.TabID, 
                siteRoot: siteRoot, 
                apiRoot: apiRoots.Item1, 
                appApiRoot: apiRoots.Item2,
                uiRoot: VirtualPathUtility.ToAbsolute(DnnConstants.SysFolderRootVirtual),
                rvtHeader: DnnConstants.AntiForgeryTokenHeaderName,
                rvt: AntiForgeryToken());

#pragma warning disable CS0618
            HtmlPage.AddMeta(InpageCms.MetaName, json);
#pragma warning restore CS0618
            return wrapLog.ReturnTrue("added");
        }

        internal static Tuple<string, string> GetApiRoots(string siteRoot = null)
        {
            siteRoot = siteRoot ?? ServicesFramework.GetServiceFrameworkRoot();
            var dnnVersion = DotNetNukeContext.Current.Application.Version.Major;
            var apiRoot = siteRoot + (dnnVersion < 9
                ? $"desktopmodules/{InpageCms.ExtensionPlaceholder}/api/"
                : $"api/{InpageCms.ExtensionPlaceholder}/");

            // appApiRoot is the same as apiRoot - the UI will add "app" to it later on 
            // but app-api root shouldn't contain generic modules-name, as it's always 2sxc
            var appApiRoot = apiRoot;
            appApiRoot = appApiRoot.Replace(InpageCms.ExtensionPlaceholder, "2sxc");

            return new Tuple<string,string>(apiRoot, appApiRoot);
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
