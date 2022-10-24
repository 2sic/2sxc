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
using System.Web.Http.Routing;
using ToSic.Eav.Context;
using ToSic.Eav.DI;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Routing;
using ToSic.Lib.Logging.Simple;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.WebApi.Sys;


namespace ToSic.Sxc.Dnn.WebApiRouting
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

        private const string ApiErrPrefix = "2sxc Api Controller Finder Error: ";
        private const string ApiErrGeneral = "Error selecting / compiling an API controller. ";
        private const string ApiErrSuffix = "Check event-log, code and inner exception. ";


        /// <summary>
        /// Verify if this request is one which should be handled by this system
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if we want to handle it</returns>
        private bool HandleRequestWithThisController(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();
            var simpleMatch = AllowedRoutes.Any(a => routeData.Route.RouteTemplate.ToLowerInvariant().Contains(a));
            if (simpleMatch)
                return true;

            var rexMatch = RegExRoutes.Any(
                a => new Regex(a, RegexOptions.None).IsMatch(routeData.Route.RouteTemplate.ToLowerInvariant()) );
            return rexMatch;

        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            // Log this lookup and add to history for insights
            var log = new Log("Sxc.Http", null, request?.RequestUri?.AbsoluteUri);
            AddToInsightsHistory(request?.RequestUri?.AbsoluteUri, log);

            var wrapLog = log.Fn<HttpControllerDescriptor>();

            if (!HandleRequestWithThisController(request))
                return wrapLog.Return(PreviousSelector.SelectController(request), "upstream");

            // Do this once and early, to be really sure we always use the same one
            var sp = DnnStaticDi.GetPageScopedServiceProvider();

            var routeData = request.GetRouteData();

            var controllerTypeName = routeData.Values[VarNames.Controller] + "Controller";

            // Now Handle the 2sxc app-api queries
            
            // Figure out the Path, or show error for that
            var appFolder = AppFolderUtilities.GetAppFolder(sp, request, log, wrapLog);

            var controllerPath = "";
            try
            {
                // new for 2sxc 9.34 #1651
                var edition = GetEdition(routeData);
                log.A($"Edition: {edition}");

                var site = (DnnSite)sp.Build<ISite>();

                var controllerFolder = GetControllerFolder(site, appFolder, edition, shared: false);
                log.A($"Controller Folder: {controllerFolder}");

                controllerPath = GetControllerPath(controllerFolder, controllerTypeName);
                log.A($"Controller Path: {controllerPath}");

                // note: this may look like something you could optimize/cache the result, but that's a bad idea
                // because when the file changes, the type-object will be different, so please don't optimize :)
                if (File.Exists(HostingEnvironment.MapPath(controllerPath)))
                    return HttpControllerDescriptor(request, controllerFolder, controllerPath, controllerTypeName, wrapLog);

                log.A("path not found, will check on shared location");

                var sharedControllerFolder = GetControllerFolder(site, appFolder, edition, shared: true);
                log.A($"Shared Controller Folder: {sharedControllerFolder}");

                var sharedControllerPath = GetControllerPath(sharedControllerFolder, controllerTypeName);
                log.A($"Shared Controller Path: {sharedControllerPath}");

                if (File.Exists(HostingEnvironment.MapPath(sharedControllerPath)))
                    return HttpControllerDescriptor(request, sharedControllerFolder, sharedControllerPath, controllerTypeName, wrapLog);

                log.A("path not found in shared, error will be thrown in a moment");
            }
            catch (Exception e)
            {
                var msg = ApiErrPrefix + ApiErrGeneral + ApiErrSuffix;
                throw AppFolderUtilities.ReportToLogAndThrow(request, HttpStatusCode.InternalServerError, e, msg, wrapLog);
            }

            var msgFinal = $"2sxc Api Controller Finder: Controller {controllerTypeName} not found in app. " +
                           $"We checked the virtual path '{controllerPath}'";
            throw AppFolderUtilities.ReportToLogAndThrow(request, HttpStatusCode.NotFound, new Exception(), msgFinal, wrapLog);
        }

        private static string GetEdition(IHttpRouteData routeData)
        {
            var edition = "";
            if (routeData.Values.ContainsKey(VarNames.Edition))
                edition = routeData.Values[VarNames.Edition].ToString();
            if (!string.IsNullOrEmpty(edition))
                edition += "/";
            return edition;
        }

        private static string GetControllerFolder(DnnSite site, string appFolder, string edition, bool shared = false) 
            => Path.Combine(shared ? site.SharedAppsRootRelative: site.AppsRootRelative, appFolder, edition + "api/")
                .ForwardSlash();

        private static string GetControllerPath(string controllerFolder, string controllerTypeName)
            => Path.Combine(controllerFolder + controllerTypeName + ".cs");

        private HttpControllerDescriptor HttpControllerDescriptor(HttpRequestMessage request, 
            string controllerFolder, string controllerPath, string controllerTypeName,
            LogCall<HttpControllerDescriptor> wrapLog)
        {
            var assembly = BuildManager.GetCompiledAssembly(controllerPath);
            var type = assembly.GetType(controllerTypeName, true, true);

            // help with path resolution for compilers running inside the created controller
            request?.Properties.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

            var descriptor = new HttpControllerDescriptor(_config, type.Name, type);
            return wrapLog.ReturnAsOk(descriptor);
        }

        private static void AddToInsightsHistory(string url, ILog log)
        {
            var addToHistory = true;
#pragma warning disable CS0162
            if (InsightsController.InsightsLoggingEnabled)
                addToHistory = (url?.Contains(InsightsController.InsightsUrlFragment) ?? false);
#pragma warning restore CS0162
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (addToHistory) new LogHistory().Add("http-request", log);
        }
    }
}