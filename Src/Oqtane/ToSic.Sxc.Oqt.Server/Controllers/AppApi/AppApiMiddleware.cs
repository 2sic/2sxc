using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public static class AppApiMiddleware
    {
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static async Task InvokeAsync(HttpContext context)
        {
            // Transform route values.
            var appApiDynamicRouteValueTransformer = context.RequestServices.GetService<AppApiDynamicRouteValueTransformer>();
            var values = await appApiDynamicRouteValueTransformer.TransformAsync(context, context.Request.RouteValues);

            // Compile and register dyncode app api controller.
            var appApiControllerManager = context.RequestServices.GetService<AppApiControllerManager>();
            if (!await appApiControllerManager.PrepareController(values)) return;

            // Invoke controller action.
            var appApiActionInvoker = context.RequestServices.GetService<AppApiActionInvoker>();
            await appApiActionInvoker.Invoke(context, values);
        }
    }
}