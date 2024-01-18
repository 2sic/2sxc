using System.Web.Http.Dispatcher;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Dnn.Backend;
using ToSic.Sxc.Dnn.Backend.Admin;
using ToSic.Sxc.Dnn.Backend.App;
using ToSic.Sxc.Dnn.Backend.Cms;
using ToSic.Sxc.Dnn.Backend.Module;
using ToSic.Sxc.Dnn.Backend.Sys;
using ToSic.Sxc.Dnn.Providers;

namespace ToSic.Sxc.Dnn.WebApi;

// ReSharper disable once UnusedMember.Global
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RouteMapper : IServiceRouteMapper
{
    // DNN Module Name used in the route
    const string Mod2Sxc = "2sxc";

    // Route Concept
    // starting with eav means it's a rather low-level admin function, always needs an AppId
    // eav
    // eav-???
    // starting with app means that it's a app-specific action, more for the JS developers working with content
    // app-content  will do basic content-actions like get one, edit, update, delete
    // app-query    will try to request a query
    // app-api      will call custom c# web-apis of a specific app

    private static readonly string[] StdNsWebApi = [typeof(AppDataController).Namespace];
    private static readonly string[] AdamNamespace = [typeof(AdamController).Namespace];
    private IMapRoute _mapRouteManager;
    private static readonly object AppContentDefs = new {controller = ControllerNames.AppContent, id = RouteParameter.Optional };


    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void RegisterRoutes(IMapRoute mapRouteManager)
    {
        _mapRouteManager = mapRouteManager;

        // old API routes before 08.10
        RegisterOldRoutesBefore0810();


        #region new API routes after 08.10

        // ADAM routes
        AddWD("adam-auto", AppRoots.AppAutoContent + "/" + ValueTokens.SetTypeGuidField, ControllerNames.Adam, AdamNamespace);
        AddWD("adam2-auto", AppRoots.AppAutoContent + "/" + ValueTokens.SetTypeGuidFieldAction, ControllerNames.Adam, AdamNamespace);
        AddWD("adam3-auto", AppRoots.AppAutoData + "/" + ValueTokens.SetTypeGuidField, ControllerNames.Adam, AdamNamespace); // new, v13
        AddWD("adam4-auto", AppRoots.AppAutoData + "/" + ValueTokens.SetTypeGuidFieldAction, ControllerNames.Adam, AdamNamespace); // new, v13

        // App Content routes - for GET/DELETE/PUT entities using REST
        // 1. Type and null or int-id
        // 2. Type and guid-id
        var idNullOrNumber = ConstraintForIdEmptyOrNumber();
        foreach (var part in Roots.Content)
        {
            AddWC("2sxc-" + part.Name,      $"{part.Path}/{ValueTokens.SetTypeAndId}", AppContentDefs, idNullOrNumber, StdNsWebApi);
            AddWD("2sxc-guid-" + part.Name, $"{part.Path}/{ValueTokens.SetTypeAndGuid}",  ControllerNames.AppContent, StdNsWebApi);
        }

        // App-API routes - for the custom code API calls of an app
        foreach (var part in Roots.AppAutoNamedInclEditions)
            AddNs("app-api" + part.Name, part.Path + "/" + RouteParts.RouteApiControllerAction, StdNsWebApi); // new, v08.10+


        // App-Query routes - to access designed queries
        // new routes, v08.10+
        foreach (var part in Roots.QueryRoots)
        {
            AddWD("2sxc-auto-" + part.Name,        $"{part.Path}/{ValueTokens.Name}", ControllerNames.AppQuery, StdNsWebApi);
            AddWD("2sxc-auto-slash" + part.Name,   $"{part.Path}/{ValueTokens.Name}/", ControllerNames.AppQuery, StdNsWebApi);
            AddWD("2sxc-auto-stream" + part.Name,  $"{part.Path}/{ValueTokens.Name}/{ValueTokens.Stream}", ControllerNames.AppQuery, StdNsWebApi);
        }
        #endregion


        #region New routes in 2sxc 11.06+ which should replace most previous internal routes

        // /Sys/ Part 1: Special update v13 - all the insights-commands go through "Details?view=xyz
        // It's important that this comes first, otherwise the second /sys/ will capture this as well
        _mapRouteManager.MapHttpRoute(Mod2Sxc, "2sxc-sys-new", $"{Areas.Sys}/Insights/{{View}}",
            new { controller = "Insights", action = nameof(InsightsController.Details) }, [typeof(InsightsController).Namespace]);

        // /Sys/ Part 2: All others
        AddTy("2sxc-sys",     Areas.Sys + "/" + ValueTokens.SetControllerAction,           typeof(InstallController));
        AddTy("2sxc-cms",     Areas.Cms + "/" + ValueTokens.SetControllerAction,           typeof(BlockController));
        AddTy("2sic-admin",   Areas.Admin + "/" + ValueTokens.SetControllerAction,         typeof(MetadataController));

        #endregion

        // DNN: System calls to dnn - this is just for module delete
        AddTy("dnn", "dnn/" + ValueTokens.SetControllerAction, typeof(ModuleController));


        // Add custom service locator into the chain of service-locators
        // this is needed to enable custom API controller lookup for the app-api
        var config = GlobalConfiguration.Configuration;
        var previousSelector = config.Services.GetService(typeof(IHttpControllerSelector)) as IHttpControllerSelector;
        config.Services.Replace(typeof(IHttpControllerSelector), new AppApiControllerSelector(config) { PreviousSelector = previousSelector });

        // Attempt to add another Module Resolver to the list which will work with the header PageId instead of TabId
        GlobalConfiguration.Configuration.AddTabAndModuleInfoProvider(new ModifiedTabAndModuleInfoProvider());
    }

    private void RegisterOldRoutesBefore0810()
    {
        // ADAM routes
        var oldContentRoot = "app-content";
        AddWD("adam-old-81", oldContentRoot + "/" + ValueTokens.SetTypeGuidField, ControllerNames.Adam, AdamNamespace);
        AddWD("adam", oldContentRoot + "/" + ValueTokens.SetTypeGuidFieldAction, ControllerNames.Adam, AdamNamespace);

        // App Content routes - for GET/DELETE/PUT entities using REST
        // 1. Type and null or int-id
        // 2. Type and guid-id
        var idNullOrNumber = ConstraintForIdEmptyOrNumber();
        AddWC("app-content", $"{oldContentRoot}/{ValueTokens.SetTypeAndId}", AppContentDefs, idNullOrNumber, StdNsWebApi);
        AddWD("app-content-guid", $"{oldContentRoot}/{ValueTokens.SetTypeAndGuid}", ControllerNames.AppContent, StdNsWebApi);

        // App-API routes - for the custom code API calls of an app
        // these are the old routes, before 2sxc v08.10
        AddWD(Mod2Sxc, "app-api-old-81", "app-api/" + ValueTokens.SetControllerAction, StdNsWebApi);

        // App-Query routes - to access designed queries
        // these are the old routes, before 2sxc v08.10
        const string rootQueryPre0810 = "app-query";
        AddWD("app-query-old-81", $"{rootQueryPre0810}/{ValueTokens.Name}", ControllerNames.AppQuery, StdNsWebApi);
        // Note 2020-04-09 - this had "appname" instead of "apppath" in it - probably for 2 years! only 1 app (Manor) now had trouble, so I think this is not in use elsewhere
        // 2020-11-12 will turn off for now - leave comment in till 2021-03
        //AddWD("app-query-nomod-old-81", $"{rootQueryPre0810}/{Token.AppPath}/{Token.Name}", ControllerNames.AppQuery, StdNsWebApi); // keep for backward compatibility...
    }

    /// <summary>
    /// Generate a constraint which only matches an ID parameter which is either empty or contains only digits.
    /// </summary>
    /// <returns></returns>
    private static object ConstraintForIdEmptyOrNumber() => new { id = @"^\d*$" };

    #region "Add" shorthands



    /// <summary>
    /// Add WD - "With Defaults"
    /// </summary>
    void AddWD(string name, string url, object defaults, string[] namespaces)
    {
        defaults = StringToControllerDefaults(defaults);
        _mapRouteManager.MapHttpRoute(Mod2Sxc, name, url, defaults, namespaces);
    }

    /// <summary>
    /// Add WC - "With Constraints"
    /// </summary>
    void AddWC(string name, string url, object defaults, object constraints, string[] namespaces)
    {
        defaults = StringToControllerDefaults(defaults);
        _mapRouteManager.MapHttpRoute(Mod2Sxc, name, url, defaults, constraints, namespaces);
    }

    private static object StringToControllerDefaults(object defaults)
        => defaults is string controllerName ? new { controller = controllerName } : defaults;

    /// <summary>
    /// Add just with namespaces
    /// </summary>
    void AddNs(string name, string url, string[] namespaces)
        => _mapRouteManager.MapHttpRoute(Mod2Sxc, name, url, namespaces);

    /// <summary>
    /// Add with type
    /// </summary>
    void AddTy(string name, string url, Type nsType)
        => AddNs(name, url, [nsType.Namespace]);


    #endregion

}