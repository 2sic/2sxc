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


            mapRouteManager.MapHttpRoute("2sxc", "EAV", "EAV/{controller}/{action}", new[] { "ToSic.SexyContent.EAVExtensions.EavApiProxies" });
            mapRouteManager.MapHttpRoute("2sxc", "View", "view/{controller}/{action}", new[] { "ToSic.SexyContent.WebApi.View" });

            // ADAM routes
            var stdNsAdam = new[] {"ToSic.SexyContent.Adam"};
            mapRouteManager.MapHttpRoute("2sxc", "adam", "app-content/{contenttype}/{guid}/{field}", new { controller = "Adam" }, stdNsAdam);
            mapRouteManager.MapHttpRoute("2sxc", "adam2", "app-content/{contenttype}/{guid}/{field}/{action}", new { controller = "Adam" }, stdNsAdam);

            // App Content routes - for GET/DELETE/PUT entities using REST
            // 1. Type and null or int-id
            // 2. Type and guid-id
            mapRouteManager.MapHttpRoute("2sxc", "app-content", "app-content/{contenttype}/{id}", new { controller = "AppContent", id = RouteParameter.Optional }, 
                new { id = @"^\d*$" },   // Only matches if "id" is null, or built only with digits.
                stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-content-guid", "app-content/{contenttype}/{guid}", new { controller = "AppContent" }, stdNsWebApi);

            // App-API routes - for the custom code API calls of an app
            mapRouteManager.MapHttpRoute("2sxc", "app-api", "app-api/{controller}/{action}", stdNsApps);
            mapRouteManager.MapHttpRoute("2sxc", "app-api-nomod", "app-api/{appfolder}/{controller}/{action}", stdNsApps);

            // App-Query routes - to access designed queries
            mapRouteManager.MapHttpRoute("2sxc", "app-query", "app-query/{name}", new { controller = "AppQuery"}, stdNsWebApi);
            mapRouteManager.MapHttpRoute("2sxc", "app-query-nomod", "app-query/{appname}/{name}", new { controller = "AppQuery" }, stdNsWebApi);

            // System calls to app, dnn, default
			mapRouteManager.MapHttpRoute("2sxc", "app", "app/{controller}/{action}", stdNsWebApi);
			mapRouteManager.MapHttpRoute("2sxc", "dnn", "dnn/{controller}/{action}", new[] { "ToSic.SexyContent.WebApi.Dnn" });
            mapRouteManager.MapHttpRoute("2sxc", "default", "{controller}/{action}", stdNsWebApi);

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