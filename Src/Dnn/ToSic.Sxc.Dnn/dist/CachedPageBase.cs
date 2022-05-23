using System;
using System.IO;
using System.Web.Caching;

namespace ToSic.Sxc.Dnn.dist
{
    public class CachedPageBase : System.Web.UI.Page
    {
        protected void PageOutputCached(string virtualPath)
        {
            var key = CacheKey(virtualPath);
            var html = Cache.Get(key);
            if (html == null)
            {
                var path = GetPath(virtualPath);
                html = File.ReadAllText(path);
                Cache.Insert(key, html, new CacheDependency(path));
            }
            Response.Write(html);
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