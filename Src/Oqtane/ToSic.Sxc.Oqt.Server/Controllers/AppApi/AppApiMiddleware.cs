using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public class AppApiMiddleware: IHasLog
    {
        public AppApiMiddleware()
        {
            Log = new Log(HistoryLogName, null, "AppApiMiddleware");
            History.Add(HistoryLogGroup, Log);
        }

        public ILog Log { get; }
        protected string HistoryLogGroup { get; } = "app-api";
        protected string HistoryLogName => "App.api.mdl";

        public async Task Invoke(HttpContext context, [FromServices] AppApiDynamicRouteValueTransformer appApiDynamicRouteValueTransformer)
        {
            var values = await appApiDynamicRouteValueTransformer.TransformAsync(context, context.Request.RouteValues);
            Log.Add($"get values: {values.Count}");

            var routeData = new RouteData(values);

            var actionDescriptorCollectionProvider = context.RequestServices.GetRequiredService<IActionDescriptorCollectionProvider>();

            var actionSelector = context.RequestServices.GetRequiredService<IActionSelector>();

            var routeContext = new RouteContext(context);
            routeContext.RouteData = routeData;

            // default selector can not select correct candidates for app api
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

                var actionInvokerFactory = context.RequestServices.GetRequiredService<IActionInvokerFactory>();

                var actionInvoker = actionInvokerFactory.CreateInvoker(actionContext);

                Log.Add($"invoke app api");
                await actionInvoker.InvokeAsync();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        private static string GetDisplayName(RouteValueDictionary values)
        {
            return $"{values["controllerTypeName"]}.{values["action"]} ({values["dllName"]}.dll)";
        }
    }
}
