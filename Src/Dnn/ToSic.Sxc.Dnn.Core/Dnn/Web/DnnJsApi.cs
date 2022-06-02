using DotNetNuke.Application;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Edit;

namespace ToSic.Sxc.Dnn.Web
{
    [PrivateApi]
    public class DnnJsApi
    {
        public static string GetJsApiJson(int? pageId = null)
        {
            var siteRoot = ServicesFramework.GetServiceFrameworkRoot();
            if (string.IsNullOrEmpty(siteRoot)) return null;

            var apiRoots = GetApiRoots(siteRoot);
            var portal = PortalSettings.Current;

            var json = InpageCms.JsApiJson(
                platform: PlatformType.Dnn.ToString(),
                pageId: pageId ?? portal.ActiveTab.TabID,
                siteRoot: siteRoot,
                apiRoot: apiRoots.Item1,
                appApiRoot: apiRoots.Item2,
                uiRoot: VirtualPathUtility.ToAbsolute(DnnConstants.SysFolderRootVirtual),
                rvtHeader: DnnConstants.AntiForgeryTokenHeaderName,
                rvt: AntiForgeryToken());

            return json;
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

            return new Tuple<string, string>(apiRoot, appApiRoot);
        }

        private static string AntiForgeryToken()
        {
            var tag = AntiForgery.GetHtml().ToString();
            return GetAttribute(tag, "value");
        }

        private static string GetAttribute(string tag, string attribute)
        {
            return new Regex(@"(?<=\b" + attribute + @"="")[^""]*")
                .Match(tag).Value;
        }
    }
}
