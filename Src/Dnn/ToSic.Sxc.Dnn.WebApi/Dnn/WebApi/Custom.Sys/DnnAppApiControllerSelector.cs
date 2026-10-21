using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using ToSic.Sxc.Dnn.Backend.Sys;

namespace ToSic.Sxc.Dnn.WebApi.Custom.Sys;

/// <summary>
/// This is a .net Framework controller selector to handle dynamically compiled controllers inside 2sxc apps.
/// </summary>
/// <remarks>
/// It will be registered in the ServicesContainer to override the previous selector.
/// It will
/// 
/// 1. check if it's responsible (based on url)
/// 2. and if yes, hand off to <see cref="DnnAppApiControllerService"/> to compile / run the app-specific api controllers
/// 3. otherwise hand processing back to next api controller up-stream
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("only made public for docs")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnAppApiControllerSelector(HttpConfiguration configuration) : IHttpControllerSelector
{
    /// <summary>
    /// Registration in the services so it can handle the requests.
    /// </summary>
    /// <remarks>
    /// Will remember the previous selector and return that if it doesn't match.
    /// </remarks>
    public static void Register()
    {
        var config = GlobalConfiguration.Configuration;
        var previousSelector = config.Services.GetService(typeof(IHttpControllerSelector)) as IHttpControllerSelector;
        config.Services.Replace(typeof(IHttpControllerSelector), new DnnAppApiControllerSelector(config) { PreviousSelector = previousSelector });
    }

    /// <summary>
    /// Remember the previous selector that was replaced.
    /// </summary>
    private IHttpControllerSelector PreviousSelector { get; init; }

    /// <inheritdoc/>
    public IDictionary<string, HttpControllerDescriptor> GetControllerMapping() => PreviousSelector.GetControllerMapping();

    /// <summary>
    /// List of root routes this selector should consider - for quick matching.
    /// </summary>
    private static readonly string[] RoutesQuickMatch =
    [
        "desktopmodules/2sxc/api/app-api/", // old routes, dnn 7/8 & dnn 9
        "api/2sxc/app-api/"
    ];

    /// <summary>
    /// Patterns to check if the route is to be served.
    /// </summary>
    /// <remarks>
    /// new in 2sxc 9.34 #1651 - added "([^/]+\/)?" to allow an optional edition parameter
    /// </remarks>
    private static readonly Regex[] RoutesAdvMatch =
    [
        new(@"desktopmodules\/2sxc\/api\/app\/[^/]+\/([^/]+\/)?api", RegexOptions.Compiled),
        new(@"api\/2sxc\/app\/[^/]+\/([^/]+\/)?api", RegexOptions.Compiled)
    ];

    /// <inheritdoc/>
    public HttpControllerDescriptor SelectController(HttpRequestMessage request)
    {
        // Do this once and early, to be really sure we always use the same one
        var sp = DnnStaticDi.GetPageScopedServiceProvider();

        // Log this lookup and add to history for insights
        var uriToLog = request?.RequestUri?.AbsoluteUri;
        var log = LogFactory.Create("Sxc.Http", null, uriToLog);
        AddToInsightsHistory(sp, uriToLog, log);

        var l = log.Fn<HttpControllerDescriptor>();

        if (!IsSxcOrEavRequest(request))
            return l.Return(PreviousSelector.SelectController(request), $"not 2sxc request, use upstream ${nameof(HttpControllerDescriptor)}");

        // 2024-03-21 New: offload all the work to a separate class, to use more normal DI for most of the code
        var appControllerSelectorSvc = sp.Build<DnnAppApiControllerService>(log);
        try
        {
            var newDescriptor = appControllerSelectorSvc.FindAndBuild(configuration, request);
            return l.Return(newDescriptor, $"found descriptor for '{newDescriptor?.ControllerName}'. Will hand that over to .net");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            throw;
        }
    }

    /// <summary>
    /// Verify if this request is one which should be handled by this system
    /// </summary>
    /// <param name="request"></param>
    /// <returns>true if we want to handle it</returns>
    private static bool IsSxcOrEavRequest(HttpRequestMessage request)
    {
        var routeData = request.GetRouteData();
        var routeTemplateLower = routeData.Route.RouteTemplate.ToLowerInvariant();
        var simpleMatch = RoutesQuickMatch.Any(routeTemplateLower.Contains);
        if (simpleMatch)
            return true;

        var rexMatch = RoutesAdvMatch.Any(a => a.IsMatch(routeTemplateLower));
        return rexMatch;

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
