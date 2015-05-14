using System.Web.Http;
using System.Web.Http.Dispatcher;
using DotNetNuke.Web.Api;
using Microsoft.Practices.Unity;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent
{
    public class RouteMapper : IServiceRouteMapper
    {

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("2sxc", "EAV", "EAV/{controller}/{action}", new[] { "ToSic.SexyContent.EAVExtensions.EavApiProxies" });

            mapRouteManager.MapHttpRoute("2sxc", "app-data", "app-data/{contenttype}/{id}", defaults: new { controller = "NewEntities", id = RouteParameter.Optional }, namespaces: new[] { "ToSic.SexyContent.EAVExtensions.EavApiProxies" });

            mapRouteManager.MapHttpRoute("2sxc", "View", "View/{controller}/{action}", new[] { "ToSic.SexyContent.ViewAPI" });
            mapRouteManager.MapHttpRoute("2sxc", "app-api", "app-api/{appFolder}/{controller}/{action}", new[] { "ToSic.SexyContent.Apps" });
            mapRouteManager.MapHttpRoute("2sxc", "app-query", "app-query/{appFolder}/{name}", new { controller = "Query"}, new[] { "ToSic.SexyContent.ViewAPI" });
            mapRouteManager.MapHttpRoute("2sxc", "default", "{controller}/{action}", new[] { "ToSic.SexyContent.GettingStarted" });

            var config = GlobalConfiguration.Configuration;
            var previousSelector = config.Services.GetService(typeof(IHttpControllerSelector)) as IHttpControllerSelector;
            config.Services.Replace(typeof(IHttpControllerSelector), new AppApiControllerSelector(config) { PreviousSelector = previousSelector });

            // Todo: also register Unity Dependency-Injection here, since this will certainly run once early during bootup
            var cont = Eav.Factory.Container; 
            // cont.RegisterType(typeof(SexyContent), typeof(SexyContent), null, new )
            cont.RegisterType(typeof(Eav.Serializers.Serializer), typeof(ToSic.SexyContent.Serializers.Serializer), new InjectionConstructor());//, null, null, null);
        }

    }
}