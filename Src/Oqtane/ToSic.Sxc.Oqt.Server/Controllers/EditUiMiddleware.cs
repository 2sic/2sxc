using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Repository;
using System.Text;
using ToSic.Sxc.Oqt.Server.Blocks.Output;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Render.Sys.JsContext;
using ToSic.Sxc.Web.Sys.EditUi;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Oqt.Server.Controllers;

internal class EditUiMiddleware
{
    private const int UnknownId = -1;

    public static Task PageOutputCached(HttpContext context, IWebHostEnvironment env, string virtualPath, EditUiResourceSettings settings)
    {
        context.Response.Headers.TryAdd("test-dev", "2sxc");

        var sp = context.RequestServices;

        var key = CacheKey(virtualPath);
        var memoryCacheService = sp.GetService<MemoryCacheService>();
        if (!memoryCacheService.TryGet<string>(key, out var html))
        {
            var path = Path.Combine(env.WebRootPath, virtualPath);
            if (!File.Exists(path)) throw new FileNotFoundException("File not found: " + path);

            var bytesInFile = File.ReadAllBytes(path);
            html = Encoding.Default.GetString(bytesInFile);
            html = HtmlDialog.CleanImport(html);
            memoryCacheService.Set(key, html, p => p.WatchFiles([path]));
        }

        var pageId = GetPageId(context);
        var tenantId = GetTenantId(context);
        var aliasId = GetAliasId(context);
        var addOn = $"{HtmlDialog.TenantIdInUrl}={tenantId}&{HtmlDialog.AliasIdInUrl}={aliasId}";

        // find siteId from pageId (if provided)
        var aliasResolver = sp.GetService<AliasResolver>();

        // 1. (keep order of lines)
        var siteId = EnsureCorrectAliasAndGetSiteIdFromPageId(tenantId, aliasId, pageId, aliasResolver, sp);

        // New feature to get resources
        var htmlHead = "";
        try
        {
            htmlHead = sp.GetRequiredService<EditUiResources>().GetResources(true, siteId, settings).HtmlHead;
        }
        catch { /* ignore */ }

        // 2. (keep order of lines)
        var siteRoot = OqtPageOutput.GetSiteRoot(aliasResolver.Alias);

        var rvt = sp.GetRequiredService<IAntiforgery>().GetAndStoreTokens(context).RequestToken;

        var withPublicKey = WithPublicKey(context);

        // inject JsApi to html content
        var content = sp.GetRequiredService<IJsApiService>().GetJsApiJson(pageId, siteRoot, rvt, withPublicKey);

        html = HtmlDialog.UpdatePlaceholders(html, content, pageId, addOn, htmlHead, $"<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"{rvt}\" >");

        var bytes = Encoding.Default.GetBytes(html);

        // html response
        context.Response.ContentType = "text/html";
        //context.Response.Body.Write(Encoding.Unicode.GetBytes(html));
        context.Response.Body.WriteAsync(bytes);

        return Task.CompletedTask;
    }

    private static string CacheKey(string virtualPath) => $"ToSic.Sxc.Oqt.Server.Controllers.{nameof(EditUiMiddleware)}:{virtualPath}";
    
    private static bool WithPublicKey(HttpContext context)
    {
        // 'wpk' should be provided in query string
        var withPublicKeyString = context.Request.Query[HtmlDialog.WithPublicKey];
        return !string.IsNullOrEmpty(withPublicKeyString) && Convert.ToBoolean(withPublicKeyString);
    }

    /// <summary>
    /// find siteId from pageId (if provided)
    /// initialize SiteState for DB connection
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="aliasId"></param>
    /// <param name="pageId"></param>
    /// <param name="aliasResolver"></param>
    /// <param name="sp"></param>
    /// <returns>siteId or NULL</returns>
    /// <remarks>internally find correct alias and store it in SiteState.Alias for reuse</remarks>
    private static int? EnsureCorrectAliasAndGetSiteIdFromPageId(int tenantId, int aliasId, int pageId, AliasResolver aliasResolver, IServiceProvider sp)
    {
        if (tenantId == UnknownId) return null;
        if (aliasId == UnknownId) return null;
        if (pageId == UnknownId) return null;

        // FIX: No database provider has been configured for this DbContext. A provider can be configured by overriding the 'DbContext.OnConfiguring' method or by using 'AddDbContext' on the application service provider.
        var _ = aliasResolver?.GetAndStoreAlias(aliasId);// do not remove or reorder this line (it will initialize SiteState for DB connection)

        if (aliasResolver?.Alias.TenantId != tenantId) return null;

        var pages = sp.GetRequiredService<IPageRepository>();
        var page = pages.GetPage(pageId, false); // SiteState need to be initialized for DB connection

        aliasResolver?.InitIfEmpty(page?.SiteId); // store correct alias in SiteState.Alias

        return page?.SiteId;
    }

    private static int GetQueryParameterAsInt(HttpContext context, string parameterName)
    {
        var parameterValue = context.Request.Query[parameterName];
        return !string.IsNullOrEmpty(parameterValue)
            ? Convert.ToInt32(parameterValue)
            : UnknownId;
    }

    private static int GetTenantId(HttpContext context) => GetQueryParameterAsInt(context, HtmlDialog.TenantIdInUrl);

    private static int GetAliasId(HttpContext context) => GetQueryParameterAsInt(context, HtmlDialog.AliasIdInUrl);

    private static int GetPageId(HttpContext context) => GetQueryParameterAsInt(context, HtmlDialog.PageIdInUrl);
}