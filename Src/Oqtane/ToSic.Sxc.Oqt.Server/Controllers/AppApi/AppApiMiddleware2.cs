using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public class AppApiMiddleware2
    {
        public async Task Invoke(HttpContext context, [FromServices] AppApiDynamicRouteValueTransformer appApiDynamicRouteValueTransformer)
        {
            // stv: POC!!!
            var values = await appApiDynamicRouteValueTransformer.TransformAsync(context, context.Request.RouteValues);

            var endpoint = context.GetEndpoint();
            //var actionDescriptor = endpoint.Metadata.GetMetadata<ActionDescriptor>();
            //var actionContext = new ActionContext
            //{
            //    ActionDescriptor = actionDescriptor,
            //    HttpContext = context
            //};
            //var invokerFactory = context.RequestServices.GetRequiredService<IActionInvokerFactory>();
            //var invoker = invokerFactory.CreateInvoker(actionContext);
            //await invoker.InvokeAsync();


            //var values = await appApiDynamicRouteValueTransformer.TransformAsync(context, context.Request.RouteValues);
            //var routeData = new RouteData(values);

            //var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName != null && a.FullName.StartsWith($"{values["dllName"]}"));
            //var controllerType = assembly.GetTypes().First();

            //var actionDescriptorCollectionProvider = context.RequestServices.GetRequiredService<IActionDescriptorCollectionProvider>();

            //// This is working because custom dyncode app api controller is already added/registered with _partManager.ApplicationParts.Add(...).
            //// TODO: Find how to directly load actionDescriptors from dyncode assembly (without depending on _partManager.ApplicationParts.Add())
            //var actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items
            //    .Where(x => (x as ControllerActionDescriptor)?.ControllerTypeInfo == controllerType);

            //var routes = actionDescriptors.Select(x => new {
            //    Action = x.RouteValues["Action"],
            //    Controller = x.RouteValues["Controller"],
            //    Name = x.AttributeRouteInfo.Name,
            //    Template = x.AttributeRouteInfo.Template,
            //    Constrains = x.ActionConstraints
            //}).ToList();

            //// Dummy action selector.
            ////var actionDescriptor = actionDescriptors.First();
            //// Naive action selector.
            //// TODO: have to use one that will account for method, parameters, etc...
            //var actionDescriptor = actionDescriptors.First(d =>
            //    d.RouteValues.TryGetValue("action", out var value)
            //    && string.Compare(value, values["action"]?.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase) == 0);





            //var actionContext = new ActionContext(context, routeData, actionDescriptor);

            //var actionInvokerFactory = context.RequestServices.GetRequiredService<IActionInvokerFactory>();

            //var actionInvoker = actionInvokerFactory.CreateInvoker(actionContext);

            //await actionInvoker.InvokeAsync();
        }
    }
}
