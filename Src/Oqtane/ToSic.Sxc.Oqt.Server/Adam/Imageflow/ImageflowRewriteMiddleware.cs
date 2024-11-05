using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Oqt.Server.Adam.Imageflow;

// This middleware is dynamically registered in oqtane imageflow module
// to be executed before main imageflow middleware because we need to
// rewrite query string params for imageflow
internal class ImageflowRewriteMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!ShouldHandleRequest(context) || !context.Request.QueryString.HasValue)
        {
            return next.Invoke(context);
        }

        Console.WriteLine($"ImageflowRewriteMiddleware.Before:{context.Request.QueryString.Value}");

        var qs = UrlHelpers.ParseQueryString(context.Request.QueryString.Value);
        var queryString = "?" + ImageflowRewrite.QueryStringRewrite(qs).NvcToString();
        context.Request.QueryString = new(queryString);

        Console.WriteLine($"ImageflowRewriteMiddleware.After:{context.Request.QueryString.Value}");

        // Call the next delegate/middleware in the pipeline.
        return next(context);
    }

    private static bool ShouldHandleRequest(HttpContext context)
    {
        // If the path is empty or null we don't handle it
        var pathValue = context.Request.Path;
        if (pathValue == null || !pathValue.HasValue)
            return false;

        var path = pathValue.Value;
        if (path == null)
            return false;

        // We handle image request extensions
        return IsImagePath(path);
    }

    private static readonly string[] Suffixes =
    [
        ".png",
        ".jpg",
        ".jpeg",
        ".jfif",
        ".jif",
        ".jfi",
        ".jpe",
        ".gif",
        ".webp"
    ];

    private static bool IsImagePath(string path)
    {
        return Suffixes.Any(suffix => path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
    }
}