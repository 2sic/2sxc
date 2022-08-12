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
        public const string PortalIdParamName = "portalId";

        public static string GetJsApiJson(int? pageId = null)
        {
            var siteRoot = GetSiteRoot(pageId);
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

        internal static string GetSiteRoot(int? pageId)
        {
            // this one is wrong for child portals, because it returns parent portal settings (where that pageId is missing)
            var portalSettings = PortalSettings.Current;
            
            // get correct PortalSettings based on pageId
            if (pageId.HasValue)
            {
                var portalDic = PortalController.GetPortalDictionary();
                if (portalDic != null && portalDic.ContainsKey(pageId.Value))
                    portalSettings = new PortalSettings(portalDic[pageId.Value]);
            }
            
            var defaultPortalAlias = portalSettings.DefaultPortalAlias;

            // fallback when default portal alias is not defined
            if (string.IsNullOrEmpty(defaultPortalAlias))
            {
                // getting siteRoot from request context in case of child portal sometimes return siteRoot for parent portal
                var siteRoot = ServicesFramework.GetServiceFrameworkRoot();
                // fallback value when request context is missing
                if (string.IsNullOrEmpty(siteRoot)) siteRoot = "/"; 
                return siteRoot;
            }

            // clean leading domain part from portal alias
            var index = defaultPortalAlias.IndexOf('/');
            if (index > 0)
            {
                defaultPortalAlias = defaultPortalAlias.Substring(index);
                if (!defaultPortalAlias.EndsWith("/")) defaultPortalAlias += "/";
            }
            else
                defaultPortalAlias = "/";

            return defaultPortalAlias;
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
