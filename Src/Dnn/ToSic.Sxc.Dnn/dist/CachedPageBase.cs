using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Urls;
using DotNetNuke.Framework;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.EditUi;

namespace ToSic.Sxc.Dnn.dist
{
    public class CachedPageBase : CDefault // HACK: inherits dnn default.aspx to preserve correct language cookie
    {
        protected string PageOutputCached(string virtualPath, EditUiResourceSettings settings)
        {
            var key = CacheKey(virtualPath);
            if (!(Cache.Get(key) is string html))
            {
                var path = GetPath(virtualPath);
                html = File.ReadAllText(path);
                html = HtmlDialog.CleanImport(html);
                Cache.Insert(key, html, new CacheDependency(path));
            }

            // portalId should be provided in query string (because of DNN special handling of aspx pages in DesktopModules)
            var portalIdString = Request.QueryString[DnnJsApi.PortalIdParamName];
            var siteId = portalIdString.HasValue() ? Convert.ToInt32(portalIdString) : -1;
            var addOn = $"&{DnnJsApi.PortalIdParamName}={siteId}";

            // pageId should be provided in query string
            var pageIdString = Request.QueryString[HtmlDialog.PageIdInUrl];
            var pageId = pageIdString.HasValue() ? Convert.ToInt32(pageIdString) : -1;
            var siteRoot = GetSiteRoot(pageId, siteId);

            var content = DnnJsApi.GetJsApiJson(pageId, siteRoot);

            var sp = HttpContext.Current.GetScope().ServiceProvider;
            var editUiResources = sp.GetService<EditUiResources>();
            var assets = editUiResources.GetResources(true, siteId, settings);

            var customHeaders = assets.HtmlHead;
            return HtmlDialog.UpdatePlaceholders(html, content, pageId, addOn, customHeaders, "");
        }

        private static string CacheKey(string virtualPath) => $"2sxc-edit-ui-page-{virtualPath}";
        
        internal string GetPath(string virtualPath)
        {
            var path = Server.MapPath(virtualPath);
            if (!File.Exists(path)) throw new Exception("File not found: " + path);
            return path;
        }

        /// <summary>
        /// portalId and pageId context is lost on DesktopModules/ToSIC_SexyContent/dist/...aspx
        /// and DNN Framework can not resolve site root, so we need to handle it by ourselves
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        private static string GetSiteRoot(int pageId, int portalId)
        {
            // this is fallback
            if (pageId == -1) return ServicesFramework.GetServiceFrameworkRoot();
            if (portalId == -1) portalId = PortalController.GetPortalDictionary()[pageId];

            //var cultureCode = LocaleController.Instance.GetCurrentLocale(portalId).Code;
            var cultureCode = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
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
    }
}