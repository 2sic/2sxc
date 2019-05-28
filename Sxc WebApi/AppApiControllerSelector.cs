using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.AutoDetectContext;
using ToSic.Sxc.Compiler;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.WebApi
{
    /// <inheritdoc />
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

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping() => PreviousSelector.GetControllerMapping();

        private static readonly string[] AllowedRoutes = {"desktopmodules/2sxc/api/app-api/", "api/2sxc/app-api/"}; // old routes, dnn 7/8 & dnn 9 


        // before 2sxc 9.34 
        // private static readonly string[] RegExRoutes = { @"desktopmodules\/2sxc\/api\/app\/[^/]+\/api", @"api\/2sxc\/app\/[^/]+\/api" };
        // new in 2sxc 9.34 #1651 - added "([^/]+\/)?" to allow an optional edition parameter
        private static readonly string[] RegExRoutes =
        {
            @"desktopmodules\/2sxc\/api\/app\/[^/]+\/([^/]+\/)?api",
            @"api\/2sxc\/app\/[^/]+\/([^/]+\/)?api"
        };

        /// <summary>
        /// Verify if this request is one which should be handled by this system
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if we want to handle it</returns>
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
            var controllerTypeName = routeData.Values[RouteParts.ControllerKey] + "Controller";
            // Handle the app-api queries
            try
            {
                var appFolder = Route.AppPathOrNull(routeData);

                if(appFolder == null)
                {
                    var sexy = Helpers.GetSxcOfApiRequest(request);
                    appFolder = sexy.App.Folder;
                }

                // new for 2sxc 9.34 #1651
                var edition = "";
                if (routeData.Values.ContainsKey(RouteParts.EditionKey))
                    edition = routeData.Values[RouteParts.EditionKey].ToString();
                if (!string.IsNullOrEmpty(edition))
                    edition += "/";

                var controllerFolder = Path.Combine(DnnMapAppToInstance.AppBasePath(), appFolder,
                    edition + "api/");
                var controllerPath = Path.Combine(controllerFolder + controllerTypeName + ".cs");

                if (File.Exists(HostingEnvironment.MapPath(controllerPath)))
                {
                    var assembly = BuildManager.GetCompiledAssembly(controllerPath);
                    var type = assembly.GetType(controllerTypeName, true, true);

                    // todo!!
                    //if (type is ISharedCodeBuilder codeBuilder) codeBuilder.SharedCodePath = controllerFolder;
                    _config.Properties.TryAdd(CsCompiler.SharedCodeRootPathKeyInCache, controllerFolder);
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