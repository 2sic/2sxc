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


            var anyValuesCount = GetAnyValuesCount(values);

            // our custom selector for app api methods
            var candidates = actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(
                i => string.Equals(i.DisplayName, displayName, StringComparison.OrdinalIgnoreCase)
                                    && i.Parameters.Count(p => p?.BindingInfo?.BindingSource?.DisplayName !="Body") == anyValuesCount
            ).ToList();

            Log.Add(candidates.Count > 0
                ? $"ok, have candidates: {candidates.Count}"
                : $"error, missing candidates: {candidates.Count}");

            try
            {
                Log.Add($"actionDescriptor SelectBestCandidate");
                var actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);

                var actionContext = new ActionContext(context, routeData, actionDescriptor);

                //string template = actionContext.ActionDescriptor.AttributeRouteInfo.Template;
                //string queryString = actionContext.HttpContext.Request.QueryString.Value;
                //var routes = new RouteData(context.Request.RouteValues);
                ////var routes = context.GetRouteData();
                //var id = context.GetRouteValue("id");
                //var id2 = routes?.Values["id"]?.ToString();

                //var controllerContext = new ControllerContext(actionContext);

                //if (!controllerContext.ActionDescriptor.RouteValues.TryGetValue("area", out var area))
                //    controllerContext.ActionDescriptor.RouteValues.Add("area", (string)values["area"]);

                // Map catch all route values as endpoint params.
                NaiveRouteParametersMapping(values, actionDescriptor, routeData);

                //var area = controllerContext.ActionDescriptor.RouteValues["area"];
                //var actionName = controllerContext.ActionDescriptor.ActionName;
                //var controllerName = controllerContext.ActionDescriptor.ControllerName;

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

        private static void NaiveRouteParametersMapping(RouteValueDictionary values, ActionDescriptor actionDescriptor, RouteData routeData)
        {
            var any = (string) values["any"];
            if (string.IsNullOrWhiteSpace(any)) return;

            var anyValues = any.Split("/").ToList();
            for (var i = 0; i < actionDescriptor.Parameters.Count; i++)
            {
                var parameter = actionDescriptor.Parameters[i];
                if (anyValues.Count > i) routeData.Values.TryAdd(parameter.Name, anyValues[i]);
            }
        }

        private static int GetAnyValuesCount(RouteValueDictionary values)
        {
            var any = (string)values["any"];
            return string.IsNullOrWhiteSpace(any) ? 0 : any.Split("/").ToList().Count;
        }

        private static string GetDisplayName(RouteValueDictionary values)
        {
            return $"{values["controllerTypeName"]}.{values["action"]} ({values["dllName"]}.dll)";
        }
    }
}
