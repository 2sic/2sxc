using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent
{
    public class RouteMapper : IServiceRouteMapper
    {

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("ToSIC_SexyContent", "default", "{controller}/{action}", new[] { "ToSic.SexyContent.GettingStarted" });
        }

    }
}