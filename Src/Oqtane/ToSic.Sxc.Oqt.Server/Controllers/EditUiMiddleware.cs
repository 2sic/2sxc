using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Oqtane.Repository;
using ToSic.Sxc.Oqt.Server.Blocks.Output;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public class EditUiMiddleware
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        public static Task PageOutputCached(HttpContext context, IWebHostEnvironment env, string virtualPath)
        {
            context.Response.Headers.Add("test-dev", "2sxc");

            var key = CacheKey(virtualPath);
            if (Cache.Get(key) is not string html)
            {
                var path = Path.Combine(env.WebRootPath, virtualPath);
                if (!File.Exists(path)) throw new FileNotFoundException("File not found: " + path);

                var bytesInFile = File.ReadAllBytes(path);
                html = Encoding.Default.GetString(bytesInFile);
                html = HtmlDialog.CleanImport(html);
                Cache.Set(key, html, GetCacheItemPolicy(path));
            }

            //var html = Encoding.Default.GetString(bytes);

            // inject JsApi to html content
            var pageIdString = context.Request.Query[HtmlDialog.PageIdInUrl];
            var pageId = !string.IsNullOrEmpty(pageIdString) ? Convert.ToInt32(pageIdString) : -1;

            var siteStateInitializer = context.RequestServices.GetService<SiteStateInitializer>();
            
            // find siteId from pageId (if provided)
            if (pageId != -1)
            {
                // siteState need to be initialized for DB connection to get siteId from pageId
                var _ = siteStateInitializer?.InitializedState;
                var pages = context.RequestServices.GetRequiredService<IPageRepository>();
                var page = pages.GetPage(pageId, false);
                siteStateInitializer?.InitIfEmpty(page?.SiteId);
            }

            var siteRoot = OqtPageOutput.GetSiteRoot(siteStateInitializer?.InitializedState);
            var antiForgery = context.RequestServices.GetRequiredService<IAntiforgery>();
            var tokenSet = antiForgery.GetAndStoreTokens(context);
            var rsvt = tokenSet.RequestToken;
            var content = OqtJsApi.GetJsApi(pageId, siteRoot, rsvt);
            html = HtmlDialog.UpdatePlaceholders(html, content, pageId, "", "", $"<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"{rsvt}\" >");

            var bytes = Encoding.Default.GetBytes(html);

            // html response
            context.Response.ContentType = "text/html";
            //context.Response.Body.Write(Encoding.Unicode.GetBytes(html));
            context.Response.Body.WriteAsync(bytes);

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
