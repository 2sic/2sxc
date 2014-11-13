using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Web.Api;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using ToSic.SexyContent.WebApi;


namespace ToSic.SexyContent
{
    public class RouteMapper : IServiceRouteMapper
    {

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("2sxc", "EAV", "EAV/{controller}/{action}", new[] { "ToSic.SexyContent.EAVExtensions.EavApiProxies" });
            mapRouteManager.MapHttpRoute("2sxc", "View", "View/{controller}/{action}", new[] { "ToSic.SexyContent.ViewAPI" });
            mapRouteManager.MapHttpRoute("2sxc", "App", "App/{appFolder}/{controller}/{action}", new string[] { "ToSic.SexyContent.Apps" });
            mapRouteManager.MapHttpRoute("2sxc", "default", "{controller}/{action}", new[] { "ToSic.SexyContent.GettingStarted" });

            var config = GlobalConfiguration.Configuration;
            var previousSelector = config.Services.GetService(typeof(IHttpControllerSelector)) as IHttpControllerSelector;
            config.Services.Replace(typeof(IHttpControllerSelector), new AppApiControllerSelector(config) { PreviousSelector = previousSelector });
        }

    }
}