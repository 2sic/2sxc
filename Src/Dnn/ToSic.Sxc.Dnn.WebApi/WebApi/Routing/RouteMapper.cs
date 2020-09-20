using System.Web.Http;
using System.Web.Http.Dispatcher;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Cms;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.Cms;
using ModuleController = ToSic.Sxc.WebApi.Cms.ModuleController;
using UiController = ToSic.Sxc.WebApi.Cms.UiController;

namespace ToSic.Sxc.WebApi
{
    // ReSharper disable once UnusedMember.Global
    public class RouteMapper : IServiceRouteMapper
    {

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            // Route Concept
            // starting with eav means it's a rather low-level admin function, always needs an AppId
            // eav
            // eav-???
            // starting with app means that it's a app-specific action, more for the JS developers working with content
            // app-content  will do basic content-actions like get one, edit, update, delete
            // app-query    will try to request a query
            // app-api      will call custom c# web-apis of a specific app

            // DNN Module Name
            const string mod2sxc = "2sxc";

            var stdNsWebApi = new[]
            {
                /*"ToSic.SexyContent.WebApi",*/
                typeof(TemplateController).Namespace, 
                typeof(AppContentController).Namespace
            };
            var stdNsApps = new[]
            {
                //"ToSic.SexyContent.Apps",
                typeof(AppContentController).Namespace
            };
            var stdNsAdam = new[]
            {
                typeof(AdamController).Namespace
            };
            var adamController = new {controller = "Adam"};
            #region EAV and View-routes
            // 2019-11-28 moved namespace for this stuff
            mapRouteManager.MapHttpRoute("2sxc", "EAV", "EAV/{controller}/{action}", new[]
            {
                //"ToSic.SexyContent.WebApi.EavApiProxies"
                typeof(UiController).Namespace
            });
            mapRouteManager.MapHttpRoute("2sxc", "View", "view/{controller}/{action}", new[]
            {
                //"ToSic.SexyContent.WebApi.View"
                typeof(ModuleController).Namespace
            });
            #endregion

            #region Constants for the Routes
            // Fragment Placeholders Constants - to ensure that all routes use the correct names
            const string frgQueryAppPath = "{apppath}";
            const string frgQueryName = "{name}";
            const string frgQueryStream = "{stream}";

            const string cntrAppQuery = "AppQuery";

            #endregion

            #region old API routes before 08.10
            // ADAM routes
            mapRouteManager.MapHttpRoute("2sxc", "adam-old-81", "app-content/{contenttype}/{guid}/{field}", adamController, stdNsAdam);
            mapRouteManager.MapHttpRoute("2sxc", "adam", "app-content/{contenttype}/{guid}/{field}/{action}", adamController, stdNsAdam);

            // App Content routes - for GET/DELETE/PUT entities using REST
            // 1. Type and null or int-id
            // 2. Type and guid-id
            mapRouteManager.MapHttpRoute("2sxc", "app-content", "app-content/{contenttype}/{id}", new { controller = "AppContent", id = RouteParameter.Optional }, 
                new { id = @"^\d*$" },   // Only matches if "id" is null, or built only with digits.
                stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-content-guid", "app-content/{contenttype}/{guid}", new { controller = "AppContent" }, stdNsWebApi);

            // App-API routes - for the custom code API calls of an app
            // these are the old routes, before 2sxc v08.10
            mapRouteManager.MapHttpRoute("2sxc", "app-api-old-81", "app-api/{controller}/{action}", stdNsApps);    

            // App-Query routes - to access designed queries
            // these are the old routes, before 2sxc v08.10
            const string rootQueryPre0810 = "app-query";
            mapRouteManager.MapHttpRoute("2sxc", "app-query-old-81", $"{rootQueryPre0810}/{frgQueryName}", new { controller = cntrAppQuery }, stdNsWebApi);
            // Note 2020-04-09 - this had "appname" instead of "apppath" in it - probably for 2 years! only 1 app (Manor) now had trouble, so I think this is not in use elsewhere
            mapRouteManager.MapHttpRoute("2sxc", "app-query-nomod-old-81", $"{rootQueryPre0810}/{frgQueryAppPath}/{frgQueryName}", new { controller = cntrAppQuery }, stdNsWebApi); // keep for backward compatibility...
            #endregion

            #region new API routes after 08.10
            const string appAuto = "app/auto/";
            const string appPath = "app/" + frgQueryAppPath + "/";
            const string appAutoEdition = appAuto + RouteParts.EditionToken + "/";
            const string appPathWithEdition = appPath + RouteParts.EditionToken + "/";

            // ADAM routes
            mapRouteManager.MapHttpRoute(mod2sxc, "adam-auto", appAuto + "content/{contenttype}/{guid}/{field}", adamController, stdNsAdam);
            mapRouteManager.MapHttpRoute(mod2sxc, "adam2-auto", appAuto + "content/{contenttype}/{guid}/{field}/{action}", adamController, stdNsAdam);

            // App Content routes - for GET/DELETE/PUT entities using REST
            // 1. Type and null or int-id
            // 2. Type and guid-id
            mapRouteManager.MapHttpRoute(mod2sxc, "app-content-auto", appAuto + "content/{contenttype}/{id}", new { controller = "AppContent", id = RouteParameter.Optional },
                new { id = @"^\d*$" },   // Only matches if "id" is null, or built only with digits.
                stdNsWebApi);
            mapRouteManager.MapHttpRoute(mod2sxc, "app-content-new-guid", appAuto + "content/{contenttype}/{guid}", new { controller = "AppContent" }, stdNsWebApi);

            mapRouteManager.MapHttpRoute(mod2sxc, "app-content-new-named", appPath + "content/{contenttype}/{id}", new { controller = "AppContent", id = RouteParameter.Optional },
                new { id = @"^\d*$" },   // Only matches if "id" is null, or built only with digits.
                stdNsWebApi);
            mapRouteManager.MapHttpRoute(mod2sxc, "app-content-new-guid-named", appPath + "content/{contenttype}/{guid}", new { controller = "AppContent" }, stdNsWebApi);

            // App-API routes - for the custom code API calls of an app
            mapRouteManager.MapHttpRoute(mod2sxc, "app-api-auto", appAuto + RouteParts.PathApiContAct, stdNsApps);  // new, v08.10+
            mapRouteManager.MapHttpRoute(mod2sxc, "app-api-public", appPath + RouteParts.PathApiContAct, stdNsApps); // new public v08.10+

            // New for 2sxc 9.34 #1651 Open-Heart-Surgery
            mapRouteManager.MapHttpRoute(mod2sxc, "app-api-edition-auto", appAutoEdition + RouteParts.PathApiContAct, stdNsApps);  // new, v09. 34
            mapRouteManager.MapHttpRoute(mod2sxc, "app-api-edition-public", appPathWithEdition + RouteParts.PathApiContAct, stdNsApps); // new public v09. 34


            // App-Query routes - to access designed queries
            // new routes, v08.10+
            const string rootQueryAuto = appAuto + "query";
            const string rootQueryNamed = appPath + "query";
            mapRouteManager.MapHttpRoute(mod2sxc, "app-query-auto", $"{rootQueryAuto}/{frgQueryName}", new { controller = cntrAppQuery }, stdNsWebApi);
            mapRouteManager.MapHttpRoute(mod2sxc, "app-query-auto-slash", $"{rootQueryAuto}/{frgQueryName}/", new { controller = cntrAppQuery }, stdNsWebApi);
            mapRouteManager.MapHttpRoute(mod2sxc, "app-query-auto-stream", $"{rootQueryAuto}/{frgQueryName}/{frgQueryStream}", new { controller = cntrAppQuery }, stdNsWebApi);
            mapRouteManager.MapHttpRoute(mod2sxc, "app-query-public", $"{rootQueryNamed}/{frgQueryName}", new { controller = cntrAppQuery }, stdNsWebApi);
            #endregion


            #region API calls for app-sys, dnn, default
            // System calls to app, dnn, default
            // 2017-02-17 2dm: disabled, as the "app" route will be used for apps now: 
            // mapRouteManager.MapHttpRoute(mod2sxc, "app", "app/{controller}/{action}", stdNsWebApi);
            // 2017-02-17 2dm: new alternate route to replace the "app" route, because I want app to be reserved!
            mapRouteManager.MapHttpRoute(mod2sxc, "app-sys", "app-sys/{controller}/{action}", stdNsWebApi);
            mapRouteManager.MapHttpRoute(mod2sxc, "dnn", "dnn/{controller}/{action}",
                new[]
                {
                    //"ToSic.SexyContent.WebApi.Dnn",
                    typeof(HyperlinkController).Namespace
                });
            mapRouteManager.MapHttpRoute(mod2sxc, "default", "{controller}/{action}", stdNsWebApi);
            mapRouteManager.MapHttpRoute(mod2sxc, "2sxc-sys", "sys/{controller}/{action}",
                new[] {typeof(InsightsController).Namespace});
            #endregion

            #region New routes in 2sxc 11.06+ which should replace most previous internal routes

            mapRouteManager.MapHttpRoute(mod2sxc, "2sxc-cms", RouteParts.RootCms + "/{controller}/{action}",
                new[] {typeof(BlockController).Namespace});

            mapRouteManager.MapHttpRoute(mod2sxc, "2sic-admin", RouteParts.RootAdmin + "/{controller}/{action}",
                new[] {typeof(MetadataController).Namespace});

            #endregion

            // Add custom service locator into the chain of service-locators
            // this is needed to enable custom API controller lookup for the app-api
            var config = GlobalConfiguration.Configuration;
            var previousSelector = config.Services.GetService(typeof(IHttpControllerSelector)) as IHttpControllerSelector;
            config.Services.Replace(typeof(IHttpControllerSelector), new AppApiControllerSelector(config) { PreviousSelector = previousSelector });
            
            // Also register Dependency-Injection here, since this will certainly run once early during boot
            // do this by accessing a setting, which registers everything
            Settings.EnsureSystemIsInitialized();
        }

    }
}