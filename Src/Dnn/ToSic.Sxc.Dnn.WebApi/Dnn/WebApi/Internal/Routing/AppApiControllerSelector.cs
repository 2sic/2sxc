using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Dnn.Backend.Sys;

namespace ToSic.Sxc.Dnn.WebApi;

/// <inheritdoc />
/// <summary>
/// This controller will check if it's responsible (based on url)
/// ...and if yes, compile / run the app-specific api controllers
/// ...otherwise hand processing back to next api controller up-stream
/// </summary>
internal class AppApiControllerSelector(HttpConfiguration configuration) : IHttpControllerSelector
{
    public IHttpControllerSelector PreviousSelector { get; set; }

    public IDictionary<string, HttpControllerDescriptor> GetControllerMapping() => PreviousSelector.GetControllerMapping();

    private static readonly string[] AllowedRoutes = [
        "desktopmodules/2sxc/api/app-api/", // old routes, dnn 7/8 & dnn 9
        "api/2sxc/app-api/"
    ]; 

    // new in 2sxc 9.34 #1651 - added "([^/]+\/)?" to allow an optional edition parameter
    private static readonly string[] RegExRoutes =
    [
        @"desktopmodules\/2sxc\/api\/app\/[^/]+\/([^/]+\/)?api",
        @"api\/2sxc\/app\/[^/]+\/([^/]+\/)?api"
    ];

    /// <summary>
    /// Verify if this request is one which should be handled by this system
    /// </summary>
    /// <param name="request"></param>
    /// <returns>true if we want to handle it</returns>
    private static bool IsSxcOrEavRequest(HttpRequestMessage request)
    {
        var routeData = request.GetRouteData();
        var routeTemplateLower = routeData.Route.RouteTemplate.ToLowerInvariant();
        var simpleMatch = AllowedRoutes.Any(routeTemplateLower.Contains);
        if (simpleMatch)
            return true;

        var rexMatch = RegExRoutes.Any(a => new Regex(a, RegexOptions.None).IsMatch(routeTemplateLower));
        return rexMatch;

    }

    public HttpControllerDescriptor SelectController(HttpRequestMessage request)
    {
        // Do this once and early, to be really sure we always use the same one
        var sp = DnnStaticDi.GetPageScopedServiceProvider();

        // Log this lookup and add to history for insights
        var uriToLog = request?.RequestUri?.AbsoluteUri;
        var log = new Log("Sxc.Http", null, uriToLog);
        AddToInsightsHistory(sp, uriToLog, log);

        var l = log.Fn<HttpControllerDescriptor>();

        if (!IsSxcOrEavRequest(request))
            return l.Return(PreviousSelector.SelectController(request), $"not 2sxc request, use upstream ${nameof(HttpControllerDescriptor)}");

        // 2024-03-21 New: offload all the work to a separate class, to use more normal DI for most of the code
        var appControllerSelectorSvc = sp.Build<AppApiControllerSelectorService>(log);
        try
        {
            var newDescriptor = appControllerSelectorSvc.SelectController(configuration, request);
            return l.Return(newDescriptor, $"found descriptor for '{newDescriptor?.ControllerName}'. Will hand that over to .net");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            throw;
        }
    }



    private static void AddToInsightsHistory(IServiceProvider sp, string urlOrNull, ILog log)
    {
        // Note: This should never error, but it's too important risk breaking just for logging
        try
        {
            var addToHistory = true;
            if (InsightsController.InsightsLoggingEnabled)
                addToHistory = urlOrNull?.Contains(InsightsController.InsightsUrlFragment) ?? true;
            
            if (addToHistory) sp.Build<ILogStore>().Add("http-request", log);
        }
        catch { /* ignore */ }
    }
}