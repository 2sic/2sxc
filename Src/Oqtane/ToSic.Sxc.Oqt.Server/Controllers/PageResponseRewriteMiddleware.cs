using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server.Controllers;

// TODO: @STV WHAT IS THIS FOR? IT LOOKS LIKE IT WAS JUST A TEST?
// Or will we need this for CSP headers etc.?
/// <summary>
/// WIP: response rewrite for razor pages (CSP, meta, etc...)
/// </summary>
internal class PageResponseRewriteMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Add("test-dev-page-middleware", "2sxc");

        // Call the next delegate/middleware in the pipeline.
        await next(context);
    }
}

public static class PageMiddlewareMiddlewareExtensions
{
    public static IApplicationBuilder UsePageResponseRewriteMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PageResponseRewriteMiddleware>();
    }
}