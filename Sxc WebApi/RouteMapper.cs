using System.Web.Http;
using System.Web.Http.Dispatcher;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi
{
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

            var stdNsWebApi = new[] {"ToSic.SexyContent.WebApi"};
            var stdNsApps = new[] {"ToSic.SexyContent.Apps"};
            var stdNsAdam = new[] {"ToSic.Sxc.Adam.WebApi"};

            #region EAV and View-routes
            mapRouteManager.MapHttpRoute("2sxc", "EAV", "EAV/{controller}/{action}", new[] { "ToSic.SexyContent.WebApi.EavApiProxies" });
            mapRouteManager.MapHttpRoute("2sxc", "View", "view/{controller}/{action}", new[] { "ToSic.SexyContent.WebApi.View" });
            #endregion

            #region old API routes before 08.10
            // ADAM routes
            mapRouteManager.MapHttpRoute("2sxc", "adam-old-81", "app-content/{contenttype}/{guid}/{field}", new { controller = "Adam" }, stdNsAdam);
            mapRouteManager.MapHttpRoute("2sxc", "adam", "app-content/{contenttype}/{guid}/{field}/{action}", new { controller = "Adam" }, stdNsAdam);

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
            mapRouteManager.MapHttpRoute("2sxc", "app-query-old-81", "app-query/{name}", new { controller = "AppQuery" }, stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-query-nomod-old-81", "app-query/{appname}/{name}", new { controller = "AppQuery" }, stdNsWebApi); // keep for backward compatibility...
            #endregion

            #region new API routes after 08.10

            const string appAuto = "app/auto/";
            const string appPath = "app/{apppath}/";
            const string appAutoEdition = appAuto + RouteParts.EditionToken + "/";
            const string appPathWithEdition = appPath + RouteParts.EditionToken + "/";

            // ADAM routes
            mapRouteManager.MapHttpRoute("2sxc", "adam-auto", appAuto + "content/{contenttype}/{guid}/{field}", new { controller = "Adam" }, stdNsAdam);
            mapRouteManager.MapHttpRoute("2sxc", "adam2-auto", appAuto + "content/{contenttype}/{guid}/{field}/{action}", new { controller = "Adam" }, stdNsAdam);

            // App Content routes - for GET/DELETE/PUT entities using REST
            // 1. Type and null or int-id
            // 2. Type and guid-id
            mapRouteManager.MapHttpRoute("2sxc", "app-content-auto", appAuto + "content/{contenttype}/{id}", new { controller = "AppContent", id = RouteParameter.Optional },
                new { id = @"^\d*$" },   // Only matches if "id" is null, or built only with digits.
                stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-content-new-guid", appAuto + "content/{contenttype}/{guid}", new { controller = "AppContent" }, stdNsWebApi);

            mapRouteManager.MapHttpRoute("2sxc", "app-content-new-named", appPath + "content/{contenttype}/{id}", new { controller = "AppContent", id = RouteParameter.Optional },
                new { id = @"^\d*$" },   // Only matches if "id" is null, or built only with digits.
                stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-content-new-guid-named", appPath + "content/{contenttype}/{guid}", new { controller = "AppContent" }, stdNsWebApi);

            // App-API routes - for the custom code API calls of an app
            mapRouteManager.MapHttpRoute("2sxc", "app-api-auto", appAuto + RouteParts.PathApiContAct, stdNsApps);  // new, v08.10+
            mapRouteManager.MapHttpRoute("2sxc", "app-api-public", appPath + RouteParts.PathApiContAct, stdNsApps); // new public v08.10+

            // New for 2sxc 9.34 #1651 Open-Heart-Surgery
            mapRouteManager.MapHttpRoute("2sxc", "app-api-edition-auto", appAutoEdition + RouteParts.PathApiContAct, stdNsApps);  // new, v09. 34
            mapRouteManager.MapHttpRoute("2sxc", "app-api-edition-public", appPathWithEdition + RouteParts.PathApiContAct, stdNsApps); // new public v09. 34


            // App-Query routes - to access designed queries
            // new routes, v08.10+
            mapRouteManager.MapHttpRoute("2sxc", "app-query-auto", appAuto + "query/{name}", new { controller = "AppQuery" }, stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-query-auto-slash", appAuto + "query/{name}/", new { controller = "AppQuery" }, stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-query-auto-stream", appAuto + "query/{name}/{stream}", new { controller = "AppQuery" }, stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-query-public", appPath + "query/{name}", new { controller = "AppQuery" }, stdNsWebApi);
            #endregion


            #region API calls for app-sys, dnn, default
            // System calls to app, dnn, default
            // 2017-02-17 2dm: disabled, as the "app" route will be used for apps now: 
            // mapRouteManager.MapHttpRoute("2sxc", "app", "app/{controller}/{action}", stdNsWebApi);
            // 2017-02-17 2dm: new alternate route to replace the "app" route, because I want app to be reserved!
            mapRouteManager.MapHttpRoute("2sxc", "app-sys", "app-sys/{controller}/{action}", stdNsWebApi);  
			mapRouteManager.MapHttpRoute("2sxc", "dnn", "dnn/{controller}/{action}", new[] { "ToSic.SexyContent.WebApi.Dnn" });
            mapRouteManager.MapHttpRoute("2sxc", "default", "{controller}/{action}", stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "insights", "sys/insights/{action}", new { controller = "Insights", action = "help"}, new[] {"ToSic.Sxc.WebApi.System"});
            #endregion

            // Add custom service locator into the chain of service-locators
            // this is needed to enable custom API controller lookup for the app-api
            var config = GlobalConfiguration.Configuration;
            var previousSelector = config.Services.GetService(typeof(IHttpControllerSelector)) as IHttpControllerSelector;
            config.Services.Replace(typeof(IHttpControllerSelector), new AppApiControllerSelector(config) { PreviousSelector = previousSelector });

            // Also register Unity Dependency-Injection here, since this will certainly run once early during bootup
            // do this by accessing a setting, which registers everything
            Settings.EnsureSystemIsInitialized();
        }

    }
}