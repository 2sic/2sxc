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
using ToSic.SexyContent.Internal;
using System;

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

            // Handle the app-api queries
            if (routeData.Route.RouteTemplate.Contains("DesktopModules/2sxc/API/app-api/") && module.DesktopModule.ModuleName == "2sxc-app")
            {
                var controllerTypeName = routeData.Values["controller"] + "Controller";

                try
                {
                    var portalSettings = PortalSettings.Current;
                    var sexy = request.GetSxcOfModuleContext();

                    // previously we assumed that there is a sub-folder with a future-app-id, but 2015-05-15 decided it's probably not worth trying, because each request currently needs tokens anyhow
                    // if ((string) routeData.Values["appFolder"] != "auto-detect-app" && (string) routeData.Values["appFolder"] != sexy.App.Folder)
                    //    throw new HttpException("AppFolder was not correct - was " + routeData.Values["appFolder"] + " but should be " + sexy.App.Folder);

                    var controllerPath = Path.Combine(AppHelpers.AppBasePath(portalSettings), sexy.App.Folder,
                        "Api/" + controllerTypeName + ".cs");

                    if (File.Exists(HostingEnvironment.MapPath(controllerPath)))
                    {
                        var assembly = BuildManager.GetCompiledAssembly(controllerPath);
                        var type = assembly.GetType(controllerTypeName);
                        return new HttpControllerDescriptor(_config, controllerTypeName, type);
                    }
                }
                catch (Exception e)
                {
                    var exception = new Exception("Error while selecting / compiling a controller for the request. See the inner exception for more details.", e);
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
                }

                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "Controller " + controllerTypeName + " not found in app."));
            }
            return PreviousSelector.SelectController(request);

        }
    }
}