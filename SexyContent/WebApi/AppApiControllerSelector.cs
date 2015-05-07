using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Api;

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
            var routeData = request.GetRouteData();
            var module = request.FindModuleInfo();
            if (routeData.Route.RouteTemplate.Contains("DesktopModules/2sxc/API/app-api/") && module.DesktopModule.ModuleName == "2sxc-app")
            {
                var portalSettings = PortalSettings.Current;
                var sexy = request.GetSxcOfModuleContext();

                if ((string) routeData.Values["appFolder"] != "auto-detect-app" && (string) routeData.Values["appFolder"] != sexy.App.Folder)
                    throw new HttpException("AppFolder was not correct - was " + routeData.Values["appFolder"] + " but should be " + sexy.App.Folder);

                var controllerTypeName = routeData.Values["controller"] + "Controller";

                var controllerPath = Path.Combine(SexyContent.AppBasePath(portalSettings), sexy.App.Folder,
                    "Api/" + controllerTypeName + ".cs");

                if (File.Exists(HostingEnvironment.MapPath(controllerPath)))
                {
                    var assembly = BuildManager.GetCompiledAssembly(controllerPath);
                    var type = assembly.GetType(controllerTypeName);
                    return new HttpControllerDescriptor(_config, controllerTypeName, type);
                }
	            throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "Controller " + controllerTypeName + " not found in app."));
            }
	        return PreviousSelector.SelectController(request);
        }
    }
}