using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

// TODO: @STV - PLS EXPLAIN what this does / what it's for
internal class AppApiActionContext : IHasLog
{
    public AppApiActionContext(ILogStore logStore)
    {
        Log = new Log(HistoryLogName, null, "AppApiActionContext");
        logStore.Add(HistoryLogGroup, Log);
    }

    public ILog Log { get; }
    protected string HistoryLogGroup { get; } = "app-api";
    protected static string HistoryLogName => "ActionContext";

    public ActionContext Provide(HttpContext context, RouteValueDictionary values)
    {
        Log.A($"get values: {values.Count}");

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
        Log.A($"app-api: {displayName}");

        List<ActionDescriptor> candidates;
        // our custom selector for app api methods

        candidates = actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(
            i => i.DisplayName != null
                 && (string.Equals(i.DisplayName, displayName, StringComparison.OrdinalIgnoreCase)
                     || i.DisplayName.EndsWith($".{displayName}", StringComparison.OrdinalIgnoreCase)) // try to find match in case we have unknown namespaces on controller name for WepApi method 
                                                                                                       // Ensure to have at least one HttpMethods attribute on the app api endpoint.
                 && i.ActionConstraints != null
                 && i.ActionConstraints.Any(c => (c.ToString() ?? "").Contains("HttpMethodActionConstraint"))
        ).ToList();

        Log.A(candidates.Count > 0
            ? $"ok, have candidates: {candidates.Count}"
            : $"error, missing candidates: {candidates.Count}, can't find right method for action: {values["action"]} on controller: {values["controller"]}.");

        if (candidates.Count == 0) throw new HttpExceptionAbstraction(HttpStatusCode.NotFound, $"Can't find right method for action: {values["action"]} on controller: {values["controllerTypeName"]}.", "Not Found");

        Log.A($"actionDescriptor SelectBestCandidate");
        var actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);

        var actionContext = new ActionContext(context, routeData, actionDescriptor);

        // Map query string values as endpoint parameters.
        MapQueryStringValuesAsEndpointParameters(actionContext, actionDescriptor, routeData);

        return actionContext;
    }

    private static void MapQueryStringValuesAsEndpointParameters(ActionContext actionContext, ActionDescriptor actionDescriptor, RouteData routeData)
    {
        foreach (var t in actionDescriptor.Parameters)
        {
            var key = t.Name;
            var value = actionContext.HttpContext.Request.Query[key];
            if (!string.IsNullOrEmpty(value)) routeData.Values.TryAdd(key, value);
        }
    }

    private static string GetDisplayName(RouteValueDictionary values)
        => $"{values["controllerTypeName"]}.{values["action"]} ({GetDllName(values)})";

    private static string GetDllName(RouteValueDictionary values)
    {
        if (AppApiFileSystemWatcher.CompiledAppApiControllers.TryGetValue((string)values["apiFile"], out var appApiCacheItem))
            if (appApiCacheItem.IsAppCode)
                return appApiCacheItem.DllName;
        return $"{values["dllName"]}.dll";
    }
}