using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Urls;
using DotNetNuke.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Results;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
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

            // this code should be executed only if is called from Default.aspx on DesktopModules/ToSIC_SexyContent/dist/.. 
            var cultureCode = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            var portalId = PortalController.GetPortalDictionary()[pageId.Value]; // also portalID should be provided in query string because od DNN special handling of aspx pages in DesktopModules
            var primaryPortalAlias = PortalAliasController.Instance.GetPortalAliasesByPortalId(portalId)
                .GetAliasByPortalIdAndSettings(portalId, result: null, cultureCode, settings: new FriendlyUrlSettings(portalId));
            var siteRoot = primaryPortalAlias != null ? CleanLeadingPartSiteRoot(primaryPortalAlias.HTTPAlias) : ServicesFramework.GetServiceFrameworkRoot();
            if (string.IsNullOrEmpty(siteRoot)) siteRoot = "/";
            return siteRoot;
        }

        private static string CleanLeadingPartSiteRoot(string path)
        {
            var index = path.IndexOf('/');
            return index <= 0 ? "/" : path.Substring(index).SuffixSlash();
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
