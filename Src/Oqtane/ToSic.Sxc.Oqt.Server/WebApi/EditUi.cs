using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public class EditUi
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        public static Task PageOutputCached(HttpContext context, IWebHostEnvironment env, string virtualPath)
        {
            context.Response.Headers.Add("test-dev", "2sxc");

            var key = CacheKey(virtualPath);
            if (Cache.Get(key) is not byte[] html)
            {
                var path = Path.Combine(env.WebRootPath, virtualPath);
                if (!File.Exists(path)) throw new FileNotFoundException("File not found: " + path);
                
                html = File.ReadAllBytes(path);
                Cache.Set(key, html, GetCacheItemPolicy(path));
            }
            context.Response.ContentType = "text/html";
            context.Response.Body.WriteAsync(html);

            return Task.CompletedTask;
        }

        private static string CacheKey(string virtualPath) => $"2sxc-edit-ui-page-{virtualPath}";

        private static CacheItemPolicy GetCacheItemPolicy(string filePath)
        {
            var cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { filePath }));
            return cacheItemPolicy;
        }
    }
}
