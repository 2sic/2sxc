using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oqtane.Repository;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public class AppApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AppApiMiddleware> _logger;

        public AppApiMiddleware(RequestDelegate next, ILogger<AppApiMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context, [FromServices] AppApiDynamicRouteValueTransformer appApiDynamicRouteValueTransformer)
        {
            var s = new Stopwatch();
            s.Start();

            // stv: POC!!!

            var values = AppApiMiddlewareExtension.GetValues(context);
            values = await appApiDynamicRouteValueTransformer.TransformAsync(context, values);
            _logger.LogInformation($"stv app api values: { values}");
            var routeData = new RouteData(values);

            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName != null && a.FullName.StartsWith($"{values["dllName"]}"));
            var controllerType = assembly.GetTypes().First();
            var actionDescriptor = new ControllerActionDescriptor();
            actionDescriptor.ControllerTypeInfo = controllerType.GetTypeInfo();
            actionDescriptor.ControllerName = controllerType.Name;
            actionDescriptor.MethodInfo = controllerType.GetMethod("Hello");
            actionDescriptor.ActionName = actionDescriptor.MethodInfo.Name;

            var actionContext = new ActionContext(context, routeData, actionDescriptor);

            var actionInvokerFactory = context.RequestServices.GetRequiredService<IActionInvokerFactory>();

            var actionInvoker = actionInvokerFactory.CreateInvoker(actionContext);

            await actionInvoker.InvokeAsync();

            s.Stop();
            var result = s.ElapsedMilliseconds;
            _logger.LogInformation($"stv time needed: { result}");

            // execute the rest of the pipeline
            await _next(context);
        }
    }
}
