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
using ToSic.SexyContent.Internal;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This controller will check if it's responsible (based on url)
    /// ...and if yes, compile / run the app-specific api controllers
    /// ...otherwise hand processing back to next api controller up-stream
    /// </summary>
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

        private static readonly string[] AllowedRoutes = {"desktopmodules/2sxc/api/app-api/", "api/2sxc/app-api/"}; // old routes, dnn 7/8 & dnn 9 


        private static readonly string[] RegExRoutes = { @"desktopmodules\/2sxc\/api\/app\/[^/]+\/api", @"api\/2sxc\/app\/[^/]+\/api" }; // todo!!!
        private bool HandleRequestWithThisController(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();
            var simpleMatch = AllowedRoutes.Any(a => routeData.Route.RouteTemplate.ToLower().Contains(a));
            if (simpleMatch)
                return true;

            var rexMatch = RegExRoutes.Any(
                a => new Regex(a, RegexOptions.None).IsMatch(routeData.Route.RouteTemplate.ToLower()) );
            return rexMatch;

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
                var appFolder = routeData.Values["apppath"]?.ToString();

                if(appFolder == null)
                {
                    var sexy = Helpers.GetSxcOfApiRequest(request);
                    appFolder = sexy.App.Folder;
                }

                var portalSettings = PortalSettings.Current;
                var controllerPath = Path.Combine(AppHelpers.AppBasePath(portalSettings), appFolder,
                    "api/" + controllerTypeName + ".cs");

                if (File.Exists(HostingEnvironment.MapPath(controllerPath)))
                {
                    var assembly = BuildManager.GetCompiledAssembly(controllerPath);
                    var type = assembly.GetType(controllerTypeName);
                    return new HttpControllerDescriptor(_config, controllerTypeName, type);
                }
            }
            catch (Exception e)
            {
                var exception = new Exception("2sxc Api Controller Finder: Error while selecting / compiling a controller for the request. Pls check the event-log and the code. See the inner exception for more details.", e);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception.Message, e));
            }

            throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "2sxc Api Controller Finder: Controller " + controllerTypeName + " not found in app."));
        }
    }
}