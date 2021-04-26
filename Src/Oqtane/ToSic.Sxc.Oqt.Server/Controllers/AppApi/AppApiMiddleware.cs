using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public static class AppApiMiddleware
    {
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Transform route values.
                var appApiDynamicRouteValueTransformer = context.RequestServices.GetService<AppApiDynamicRouteValueTransformer>();
                var values = await appApiDynamicRouteValueTransformer.TransformAsync(context, context.Request.RouteValues);

                // Compile and register dyncode app api controller.
                var appApiControllerManager = context.RequestServices.GetService<AppApiControllerManager>();
                if (!await appApiControllerManager.PrepareController(values))
                    throw new ArgumentException("Error, can't compile controller.");

                // Invoke controller action.
                var appApiActionInvoker = context.RequestServices.GetService<AppApiActionInvoker>();
                await appApiActionInvoker.Invoke(context, values);
            }
            catch (ArgumentException e)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync($"404 Not Found - {e.Message}");
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"500 Internal Server Error - {e.Message}");
            }
        }
    }
}