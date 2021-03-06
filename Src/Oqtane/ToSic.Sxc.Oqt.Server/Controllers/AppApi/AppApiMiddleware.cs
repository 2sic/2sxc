﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using ToSic.Eav.WebApi.Errors;

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
            catch (HttpExceptionAbstraction e)
            {
                context.Response.StatusCode = e.Status;
                await context.Response.WriteAsync($"{e.Status} - {e.Message}");
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"500 Internal Server Error - {e.Message}");
            }
        }


    }
}