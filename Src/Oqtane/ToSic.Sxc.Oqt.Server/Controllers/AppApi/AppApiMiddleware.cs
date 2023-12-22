using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

// TODO: @STV - PLS EXPLAIN what this does / what it's for
internal static class AppApiMiddleware
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    internal static async Task InvokeAsync(HttpContext context)
    {
        // Transform route values.
        var appApiDynamicRouteValueTransformer = context.RequestServices.GetService<AppApiDynamicRouteValueTransformer>();
        var values = await appApiDynamicRouteValueTransformer.TransformAsync(context, context.Request.RouteValues);

        // Compile and register dyncode app api controller.
        var appApiControllerManager = context.RequestServices.GetService<AppApiControllerManager>();
        if (!await appApiControllerManager.PrepareController(values))
            throw new HttpExceptionAbstraction(HttpStatusCode.NotFound, "Error, can't compile controller.", "Not Found");

        // Provide ActionContext.
        var appApiActionContext = context.RequestServices.GetService<AppApiActionContext>();
        var actionContext = appApiActionContext.Provide(context, values);

        async Task InvokeActionAfterAuthorization(HttpContext context)
        {
            // Invoke controller action.
            var appApiActionInvoker = context.RequestServices.GetService<AppApiActionInvoker>();
            await appApiActionInvoker.Invoke(actionContext);
        }

        // Check security.
        var appApiAuthorization = context.RequestServices.GetService<AppApiAuthorization>().Init(InvokeActionAfterAuthorization);
        await appApiAuthorization.Invoke(actionContext);
    }
}