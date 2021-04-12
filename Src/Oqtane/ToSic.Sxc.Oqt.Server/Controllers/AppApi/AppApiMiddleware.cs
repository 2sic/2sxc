using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public class AppApiMiddleware : IHasLog
    {
        public AppApiMiddleware()
        {
            Log = new Log(HistoryLogName, null, "AppApiMiddleware");
            History.Add(HistoryLogGroup, Log);
        }

        public ILog Log { get; }
        protected string HistoryLogGroup { get; } = "app-api";
        protected static string HistoryLogName => "Middleware";

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static async Task UseAppApi(HttpContext context)
        {
            // Transform route values.
            var appApiDynamicRouteValueTransformer = context.RequestServices.GetService<AppApiDynamicRouteValueTransformer>();
            var values = await appApiDynamicRouteValueTransformer.TransformAsync(context, context.Request.RouteValues);

            // Compile and register dyncode app api controller.
            var appApiControllerManager = context.RequestServices.GetService<AppApiControllerManager>();
            if (!await appApiControllerManager.PrepareController(values)) return;

            // Invoke controller action.
            await new AppApiMiddleware().Invoke(context, values);
        }

        private async Task Invoke(HttpContext context, RouteValueDictionary values)
        {
            Log.Add($"get values: {values.Count}");

            var routeData = new RouteData(values);

            var actionDescriptorCollectionProvider = context.RequestServices.GetRequiredService<IActionDescriptorCollectionProvider>();

            var actionSelector = context.RequestServices.GetRequiredService<IActionSelector>();

            var routeContext = new RouteContext(context)
            {
                RouteData = routeData
            };

            // default selector can not select correct candidates from dyncode app api
            //var candidates = actionSelector.SelectCandidates(routeContext);

            var displayName = GetDisplayName(values);
            Log.Add($"app-api: {displayName}");

            // our custom selector for app api methods
            var candidates = actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(
                i => string.Equals(i.DisplayName, displayName, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            Log.Add(candidates.Count > 0
                ? $"ok, have candidates: {candidates.Count}"
                : $"error, missing candidates: {candidates.Count}");

            try
            {
                Log.Add($"actionDescriptor SelectBestCandidate");
                var actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);

                var actionContext = new ActionContext(context, routeData, actionDescriptor);

                // Map query string values as endpoint parameters.
                MapQueryStringValuesAsEndpointParameters(actionContext, actionDescriptor, routeData);

                var actionInvokerFactory = context.RequestServices.GetRequiredService<IActionInvokerFactory>();

                var actionInvoker = actionInvokerFactory.CreateInvoker(actionContext);

                Log.Add($"invoke app api action");
                await actionInvoker.InvokeAsync();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        private static void MapQueryStringValuesAsEndpointParameters(ActionContext actionContext, ActionDescriptor actionDescriptor, RouteData routeData)
        {
            foreach (var t in actionDescriptor.Parameters)
            {
                var key = t.Name;
                var value = actionContext.HttpContext.Request.Query[key];
                routeData.Values.TryAdd(key, value);
            }
        }

        private static string GetDisplayName(RouteValueDictionary values)
        {
            return $"{values["controllerTypeName"]}.{values["action"]} ({values["dllName"]}.dll)";
        }
    }
}
