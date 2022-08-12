using System;
using System.IO;
using System.Web.Caching;
using DotNetNuke.Framework;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.dist
{
    public class CachedPageBase : CDefault // HACK: inherits dnn default.aspx to preserve correct language cookie
    {
        protected string PageOutputCached(string virtualPath)
        {
            var key = CacheKey(virtualPath);
            if (!(Cache.Get(key) is string html))
            {
                var path = GetPath(virtualPath);
                html = File.ReadAllText(path);
                html = HtmlDialog.CleanImport(html);
                Cache.Insert(key, html, new CacheDependency(path));
            }

            var pageIdString = Request.QueryString[HtmlDialog.PageIdInUrl];
            var pageId = pageIdString.HasValue() ? Convert.ToInt32(pageIdString) : -1;
            var portalId = Request.QueryString[DnnJsApi.PortalIdParamName];
            var addOn = $"&{DnnJsApi.PortalIdParamName}={portalId}";

            var content = DnnJsApi.GetJsApiJson(pageId);
            return HtmlDialog.UpdatePlaceholders(html, content, pageId, addOn, "", "");
        }

        private static string CacheKey(string virtualPath) => $"2sxc-edit-ui-page-{virtualPath}";
        
        internal string GetPath(string virtualPath)
        {
            var path = Server.MapPath(virtualPath);
            if (!File.Exists(path)) throw new Exception("File not found: " + path);
            return path;
        }
    }
}