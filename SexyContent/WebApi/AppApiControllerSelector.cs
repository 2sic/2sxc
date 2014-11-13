using System;
using System.Web;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace ToSic.SexyContent.WebApi
{
    public class AppApiControllerSelector : IHttpControllerSelector
    {
        private readonly HttpConfiguration _config;
        public IHttpControllerSelector PreviousSelector { get; set; }

        public AppApiControllerSelector(HttpConfiguration configuration)
        {
            _config = configuration;
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return PreviousSelector.GetControllerMapping();
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            IHttpRouteData routeData = request.GetRouteData();
            var module = request.FindModuleInfo();
            if (routeData.Route.RouteTemplate.Contains("/DesktopModules/2sxc/API/App/") && module.DesktopModule.ModuleName == "2sxc-app")
            {
                var portalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current;
                var sexy = request.GetSxcOfModuleContext();

                if ((string) routeData.Values["appFolder"] != "auto-detect-app" && (string) routeData.Values["appFolder"] != sexy.App.Folder)
                    throw new HttpException("AppFolder was not correct - was " + routeData.Values["appFolder"] + " but should be " + sexy.App.Folder);

                var controllerTypeName = routeData.Values["controller"] + "Controller";

                var controllerPath = Path.Combine(SexyContent.AppBasePath(portalSettings), sexy.App.Folder,
                    "Api/" + controllerTypeName + ".cs");

                if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(controllerPath)))
                {
                    var assembly = BuildManager.GetCompiledAssembly(controllerPath);
                    var type = assembly.GetType(controllerTypeName);
                    return new HttpControllerDescriptor(_config, controllerTypeName, type);
                }
                else
                {
                    throw new HttpException(404, "Controller " + controllerTypeName + " not found in app.");
                }
            }
            else
                return PreviousSelector.SelectController(request);
        }
    }
}