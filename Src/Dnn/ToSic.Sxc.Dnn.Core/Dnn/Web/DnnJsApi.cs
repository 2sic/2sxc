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
        public const string PortalIdParamName = "portalId";

        public static string GetJsApiJson(int? pageId = null, string siteRoot = null)
        {
            // pageId and siteRoot are normally null when called from razor, api, custom cs
            // pageId and siteRoot are provided only in very special case for EditUI in /DesktopModules/.../...aspx

            siteRoot = siteRoot ?? ServicesFramework.GetServiceFrameworkRoot();
            var apiRoots = GetApiRoots(siteRoot);

            var json = InpageCms.JsApiJson(
                platform: PlatformType.Dnn.ToString(),
                pageId: pageId ?? PortalSettings.Current.ActiveTab.TabID,
                siteRoot: siteRoot,
                apiRoot: apiRoots.Item1,
                appApiRoot: apiRoots.Item2,
                uiRoot: VirtualPathUtility.ToAbsolute(DnnConstants.SysFolderRootVirtual),
                rvtHeader: DnnConstants.AntiForgeryTokenHeaderName,
                rvt: AntiForgeryToken(),
                dialogQuery: $"{PortalIdParamName}={PortalSettings.Current.PortalId}"
                );

            return json;
        }

        internal static Tuple<string, string> GetApiRoots(string siteRoot = null)
        {
            siteRoot = siteRoot ?? ServicesFramework.GetServiceFrameworkRoot();
            var apiRoot = siteRoot + $"api/{InpageCms.ExtensionPlaceholder}/";
            
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
