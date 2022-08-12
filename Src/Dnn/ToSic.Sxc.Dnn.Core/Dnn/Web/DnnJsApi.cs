using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using System;
using System.Linq;
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
            var apiRoot = siteRoot + $"api/{InpageCms.ExtensionPlaceholder}/";
            
            // appApiRoot is the same as apiRoot - the UI will add "app" to it later on 
            // but app-api root shouldn't contain generic modules-name, as it's always 2sxc
            var appApiRoot = apiRoot;
            appApiRoot = appApiRoot.Replace(InpageCms.ExtensionPlaceholder, "2sxc");

            return new Tuple<string, string>(apiRoot, appApiRoot);
        }


        // commented out because it is not handling correctly portal alias with localization
        internal static string GetSiteRoot(int? pageId)
        {
            // pageId should be provided only in very special case for EditUI
            // it should be null when called from razor or api
            if (!pageId.HasValue) return ServicesFramework.GetServiceFrameworkRoot();

            // this one is wrong for child portals, because it returns parent portal settings (where that pageId is missing)
            var portalSettings = PortalSettings.Current;

            var portalDic = PortalController.GetPortalDictionary();
            if (portalDic == null
                || !portalDic.ContainsKey(pageId.Value)
                || portalDic[pageId.Value] == portalSettings.PortalId)
                return ServicesFramework.GetServiceFrameworkRoot();
            

            var portalId = portalDic[pageId.Value];
            var cultureCode = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            var portalAliases = PortalAliasController.Instance.GetPortalAliasesByPortalId(portalId).ToList();
            var portalAlias = portalAliases.Where(a => (string.Compare(a.CultureCode, cultureCode, StringComparison.OrdinalIgnoreCase) == 0 || string.IsNullOrEmpty(a.CultureCode))
                                                       && a.PortalID == portalId)
                .OrderByDescending(a => a.IsPrimary)
                .ThenByDescending(a => a.CultureCode)
                .ThenByDescending(a => a.BrowserType)
                .FirstOrDefault();

            string path;
            if (portalAlias != null)
            {
                path = portalAlias.HTTPAlias;
            }
            else
            {
                // fallback #1
                // get correct PortalSettings based on pageId
                portalSettings = new PortalSettings(portalDic[pageId.Value]);
                path = portalSettings.PortalAlias.HTTPAlias;

                // fallback #2 when default portal alias is not defined
                if (string.IsNullOrEmpty(path))
                {
                    // getting siteRoot from request context in case of child portal sometimes return siteRoot for parent portal
                    var siteRoot = ServicesFramework.GetServiceFrameworkRoot();
                    // fallback value when request context is missing
                    if (string.IsNullOrEmpty(siteRoot)) siteRoot = "/";
                    return siteRoot;
                }
            }

            // clean leading domain part from portal alias
            var index = path.IndexOf('/');
            if (index > 0)
            {
                path = path.Substring(index);
                if (!path.EndsWith("/")) path += "/";
            }
            else
                path = "/";

            return path;
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
