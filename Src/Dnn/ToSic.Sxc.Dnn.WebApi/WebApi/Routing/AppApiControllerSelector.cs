using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Run;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Sys;

namespace ToSic.Sxc.WebApi
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
            // Log this lookup and add to history for insights
            var log = new Log("Sxc.Http", null, request?.RequestUri?.AbsoluteUri);
            AddToInsightsHistory(request?.RequestUri?.AbsoluteUri, log);

            var wrapLog = log.Call<HttpControllerDescriptor>();

            if (!HandleRequestWithThisController(request))
                return wrapLog("upstream", PreviousSelector.SelectController(request));

            var routeData = request.GetRouteData();
            var controllerTypeName = routeData.Values[RouteParts.ControllerKey] + "Controller";
            // Handle the app-api queries
            try
            {
                var appFolder = Route.AppPathOrNull(routeData);

                if(appFolder == null)
                {
                    log.Add("no folder found in url, will auto-detect");
                    var block = DnnGetBlock.GetCmsBlock(request, false, log);
                    appFolder = block.App.Folder;
                }

                log.Add($"App Folder: {appFolder}");

                // new for 2sxc 9.34 #1651
                var edition = "";
                if (routeData.Values.ContainsKey(RouteParts.EditionKey))
                    edition = routeData.Values[RouteParts.EditionKey].ToString();
                if (!string.IsNullOrEmpty(edition))
                    edition += "/";

                log.Add($"Edition: {edition}");

                var tenant = Factory.Resolve<ITenant>();
                var controllerFolder = Path.Combine(tenant.AppsRoot, appFolder, edition + "api/");

                controllerFolder = controllerFolder.Replace("\\", @"/");
                log.Add($"Controller Folder: {controllerFolder}");

                var controllerPath = Path.Combine(controllerFolder + controllerTypeName + ".cs");
                log.Add($"Controller Path: {controllerPath}");

                // note: this may look like something you could optimize/cache the result, but that's a bad idea
                // because when the file changes, the type-object will be different, so please don't optimize :)
                if (File.Exists(HostingEnvironment.MapPath(controllerPath)))
                {
                    var assembly = BuildManager.GetCompiledAssembly(controllerPath);
                    var type = assembly.GetType(controllerTypeName, true, true);

                    // help with path resolution for compilers running inside the created controller
                    request?.Properties.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

                    var descriptor = new HttpControllerDescriptor(_config, type.Name, type);
                    return wrapLog("ok", descriptor);
                }

                log.Add("path not found");
            }
            catch (Exception e)
            {
                var apiErrPrefix = "2sxc Api Controller Finder: " +
                                   "Error selecting / compiling an API controller. " +
                                   "Check event-log, code and inner exception. ";
                var helpText = ErrorHelp.HelpText(e);
                var exception = new Exception(apiErrPrefix + helpText, e);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
                wrapLog("error", null);
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception.Message, e));
            }

            wrapLog("error", null);
            throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "2sxc Api Controller Finder: Controller " + controllerTypeName + " not found in app."));
        }

        private static void AddToInsightsHistory(string url, ILog log)
        {
            var addToHistory = true;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!InsightsController.InsightsLoggingEnabled)
                if (url?.Contains(InsightsController.InsightsUrlFragment) ?? false)
                    addToHistory = false;
            if (addToHistory) History.Add("http-request", log);
        }
    }
}