using ToSic.Eav.WebApi.Sys.Routing;
using ToSic.Sxc.Dnn.Backend;
using ToSic.Sxc.Dnn.Backend.Admin;
using ToSic.Sxc.Dnn.Backend.App;
using ToSic.Sxc.Dnn.Backend.Cms;
using ToSic.Sxc.Dnn.Backend.Module;
using ToSic.Sxc.Dnn.Backend.Sys;
using ToSic.Sxc.Dnn.WebApi.Custom.Sys;
using AppRoots = ToSic.Sxc.Dnn.WebApi.Routing.Sys.DnnRoutingAppRoots;
using RouteTokens = ToSic.Sxc.Dnn.WebApi.Routing.Sys.DnnRouteTokens;
using ControllerNames = ToSic.Sxc.Dnn.WebApi.Routing.Sys.DnnRoutingControllerNames;

namespace ToSic.Sxc.Dnn.WebApi.Routing.Sys;

/// <summary>
/// Register all the WebApi routes served by 2sxc.
/// Will be run by Dnn because it implements <see cref="IServiceRouteMapper"/>.
/// </summary>
/// <remarks>
/// Routes:
///
/// * starting with eav means it's a rather low-level admin function, always needs an AppId
///   * eav
///   * eav-*
/// * starting with app means that it's an app-specific action, more for the JS developers working with content
///   * app-content  will do basic content-actions like get one, edit, update, delete
///   * app-query    will try to request a query
///   * app-api      will call custom c# web-apis of a specific app
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("only made public for docs")]
[ShowApiWhenReleased(ShowApiMode.Never)]
// ReSharper disable once UnusedMember.Global
public class DnnRouteRegistration : IServiceRouteMapper
{
    // Dnn Module Name used in the route
    private const string Mod2Sxc = "2sxc";

    // Route Concept

    private static readonly string[] StdNsWebApi = [typeof(AppDataController).Namespace];
    private static readonly string[] AdamNamespaces = [typeof(AdamController).Namespace];
    private IMapRoute _mapRouteManager;

    private static readonly object AppContentDefaults = new
    {
        controller = ControllerNames.AppData,
        id = RouteParameter.Optional
    };

    /// <summary>
    /// Register the routes when called by Dnn.
    /// </summary>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public void RegisterRoutes(IMapRoute mapRouteManager)
    {
        _mapRouteManager = mapRouteManager;

        #region new API routes after 08.10

        // ADAM routes
        AddWithDefaults("adam-auto", $"{AppRoots.AppAutoContent}/{RouteTokens.TypeGuidField}", ControllerNames.Adam, AdamNamespaces);
        AddWithDefaults("adam2-auto", $"{AppRoots.AppAutoContent}/{RouteTokens.TypeGuidFieldAction}", ControllerNames.Adam, AdamNamespaces);
        AddWithDefaults("adam3-auto", $"{AppRoots.AppAutoData}/{RouteTokens.TypeGuidField}", ControllerNames.Adam, AdamNamespaces); // new, v13
        AddWithDefaults("adam4-auto", $"{AppRoots.AppAutoData}/{RouteTokens.TypeGuidFieldAction}", ControllerNames.Adam, AdamNamespaces); // new, v13

        // App Content routes - for GET/DELETE/PUT entities using REST
        // 1. Type and null or int-id
        // 2. Type and guid-id
        var idNullOrNumConstraints = ConstraintForIdEmptyOrNumber();
        foreach (var part in AppRoots.AppContents)
        {
            AddWithConstraints($"2sxc-{part.Name}",      $"{part.Path}/{RouteTokens.TypeAndId}", AppContentDefaults, idNullOrNumConstraints, StdNsWebApi);
            AddWithDefaults($"2sxc-guid-{part.Name}", $"{part.Path}/{RouteTokens.TypeAndGuid}",  ControllerNames.AppData, StdNsWebApi);
        }

        // App-API routes - for the custom code API calls of an app
        foreach (var part in AppRoots.AppAutoNamed)
            AddBasic($"app-api{part.Name}", $"{part.Path}/{RouteTokens.RouteApiControllerAction}", StdNsWebApi); // new, v08.10+


        // App-Query routes - to access designed queries
        // new routes, v08.10+
        foreach (var part in AppRoots.AppQueryRoots)
        {
            AddWithDefaults($"2sxc-auto-{part.Name}",        $"{part.Path}/{RouteTokens.Name}", ControllerNames.AppQuery, StdNsWebApi);
            AddWithDefaults($"2sxc-auto-slash{part.Name}",   $"{part.Path}/{RouteTokens.Name}/", ControllerNames.AppQuery, StdNsWebApi);
            AddWithDefaults($"2sxc-auto-stream{part.Name}",  $"{part.Path}/{RouteTokens.Name}/{RouteTokens.Stream}", ControllerNames.AppQuery, StdNsWebApi);
        }
        #endregion


        #region New routes in 2sxc 11.06+ which should replace most previous internal routes

        // /Sys/ Part 1: Special update v13 - all the insights-commands go through "Details?view=xyz
        // It's important that this comes first, otherwise the second /sys/ will capture this as well
        _mapRouteManager.MapHttpRoute(Mod2Sxc, "2sxc-sys-new", $"{Areas.Sys}/Insights/{{View}}",
            new
            {
                controller = "Insights",
                action = nameof(InsightsController.Details)
            }, [typeof(InsightsController).Namespace]);

        // /Sys/ Part 2: All others
        AddBasic("2sxc-sys", $"{Areas.Sys}/{RouteTokens.ControllerAndAction}", [typeof(InstallController).Namespace]);
        AddBasic("2sxc-cms", $"{Areas.Cms}/{RouteTokens.ControllerAndAction}", [typeof(BlockController).Namespace]);
        AddBasic("2sic-admin", $"{Areas.Admin}/{RouteTokens.ControllerAndAction}", [typeof(TypeController).Namespace]);
        // Specific admin route to support folder segment for AppController.Extensions
        AddBasic($"2sxc-admin-app-extensions-{RouteTokens.Name}", url: $"{Areas.Admin}/{RouteTokens.ControllerAndAction}/{RouteTokens.Name}", namespaces: [typeof(AppController).Namespace]);

        #endregion

        // v21 - app cache/Lightspeed
        AddBasic("2sxc-app-cache-auto", $"{AppRoots.AppAuto}/{RouteTokens.ControllerAndAction}", [typeof(CacheController).Namespace]);
        AddBasic("2sxc-app-cache-name", $"{AppRoots.AppNamed}/{RouteTokens.ControllerAndAction}", [typeof(CacheController).Namespace]);

        // DNN: System calls to dnn - this is just for module delete
        AddBasic("dnn", $"dnn/{RouteTokens.ControllerAndAction}", [typeof(ModuleController).Namespace]);


        // Add custom service locator into the chain of service-locators
        // this is needed to enable custom API controller lookup for the app-api
        DnnAppApiControllerSelector.Register();

        // Attempt to add another Module Resolver to the list which will work with the header PageId instead of TabId
        GlobalConfiguration.Configuration.AddTabAndModuleInfoProvider(new ModifiedTabAndModuleInfoProvider());
    }

    /// <summary>
    /// Generate a constraint which only matches an ID parameter which is either empty or contains only digits.
    /// </summary>
    /// <returns></returns>
    private static object ConstraintForIdEmptyOrNumber() => new { id = @"^\d*$" };

    #region "Add" shorthands

    /// <summary>
    /// Add just with namespaces
    /// </summary>
    private void AddBasic(string name, string url, string[] namespaces)
        => _mapRouteManager.MapHttpRoute(Mod2Sxc, name, url, namespaces);

    /// <summary>
    /// Add WD - "With Defaults"
    /// </summary>
    private void AddWithDefaults(string name, string url, string controllerName, string[] namespaces)
    {
        var objDefaults = new { controller = controllerName };
        _mapRouteManager.MapHttpRoute(Mod2Sxc, name, url, objDefaults, namespaces);
    }

    /// <summary>
    /// Add WC - "With Constraints"
    /// </summary>
    private void AddWithConstraints(string name, string url, object defaults, object constraints, string[] namespaces)
        => _mapRouteManager.MapHttpRoute(Mod2Sxc, name, url, defaults, constraints, namespaces);

    #endregion

}
