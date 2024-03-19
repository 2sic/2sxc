using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Repository;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Caching;
using ToSic.Sxc.Oqt.Server.Blocks.Output;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Web.Internal.EditUi;
using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Oqt.Server.Controllers;

internal class EditUiMiddleware
{
    public static Task PageOutputCached(HttpContext context, IWebHostEnvironment env, string virtualPath, EditUiResourceSettings settings, MemoryCacheService cache)
    {
        context.Response.Headers.Add("test-dev", "2sxc");

        var path = Path.Combine(env.WebRootPath, virtualPath);
        var html = cache.GetOrBuildEditUiMiddlewareHtml(path, () =>
        {
            if (!File.Exists(path)) throw new FileNotFoundException($"File not found: {path}");
            return HtmlDialog.CleanImport(Encoding.Default.GetString(File.ReadAllBytes(path)));
        });

        // inject JsApi to html content
        var pageIdString = context.Request.Query[HtmlDialog.PageIdInUrl];
        var pageId = !string.IsNullOrEmpty(pageIdString) ? Convert.ToInt32(pageIdString) : -1;

        var sp = context.RequestServices;
        var siteStateInitializer = sp.GetService<SiteStateInitializer>();
            
        // find siteId from pageId (if provided)
        var siteId = 1; // TODO: @STV - why do we have the site with all the null checks?
        if (pageId != -1)
        {
            // siteState need to be initialized for DB connection to get siteId from pageId
            var _ = siteStateInitializer?.InitializedState;
            var pages = sp.GetRequiredService<IPageRepository>();
            var page = pages.GetPage(pageId, false);
            siteStateInitializer?.InitIfEmpty(page?.SiteId);
            siteId = page?.SiteId ?? siteId;
        }

        var siteRoot = OqtPageOutput.GetSiteRoot(siteStateInitializer?.InitializedState);
        var antiForgery = sp.GetRequiredService<IAntiforgery>();
        var tokenSet = antiForgery.GetAndStoreTokens(context);
        var rvt = tokenSet.RequestToken;

        var oqtJsApi = sp.GetRequiredService<IJsApiService>();
        var content = oqtJsApi.GetJsApiJson(pageId, siteRoot, rvt);

        // New feature to get resources
        var htmlHead = "";
        try
        {
            var editUiResources = sp.GetRequiredService<EditUiResources>();
            var assets = editUiResources.GetResources(true, siteId, settings);
            htmlHead = assets.HtmlHead;
        }
        catch { /* ignore */ }

        html = HtmlDialog.UpdatePlaceholders(html, content, pageId, "", htmlHead, $"<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"{rvt}\" >");

        var bytes = Encoding.Default.GetBytes(html);

        // html response
        context.Response.ContentType = "text/html";
        //context.Response.Body.Write(Encoding.Unicode.GetBytes(html));
        context.Response.Body.WriteAsync(bytes);

        return Task.CompletedTask;
    }
}