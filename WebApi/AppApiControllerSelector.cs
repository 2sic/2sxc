using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.Internal;
using System;
using System.Linq;

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

        private bool HandleRequestWithThisController(HttpRequestMessage request)
        {
            var allowedRoutes = new[] { "DesktopModules/2sxc/API/app-api/", "API/2sxc/app-api/" };
            var routeData = request.GetRouteData();

            if (!(allowedRoutes.Any(a => routeData.Route.RouteTemplate.Contains(a))))
                return false;
            
            //if (request.FindModuleInfo().DesktopModule.ModuleName != "2sxc-app")
            //    return false;

            return true;
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            if(!HandleRequestWithThisController(request))
                return PreviousSelector.SelectController(request);

            var routeData = request.GetRouteData();
            var controllerTypeName = routeData.Values["controller"] + "Controller";
            // Handle the app-api queries
            try
            {
                var appFolder = routeData.Values["appname"]?.ToString();

                if(appFolder == null || !(appFolder is String))
                {
                    var sexy = Helpers.GetSxcOfApiRequest(request);// request.GetSxcOfModuleContext();
                    appFolder = sexy.App.Folder;
                }

                var portalSettings = PortalSettings.Current;
                var controllerPath = Path.Combine(AppHelpers.AppBasePath(portalSettings), appFolder,
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
                var exception = new Exception("Error while selecting / compiling a controller for the request. Pls check the event-log and the code. See the inner exception for more details.", e);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception.Message, e));
            }

            throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "Controller " + controllerTypeName + " not found in app."));
        }
    }
}