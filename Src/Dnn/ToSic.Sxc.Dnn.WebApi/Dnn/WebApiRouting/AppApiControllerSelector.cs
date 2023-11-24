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
using ToSic.Lib.DI;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.WebApi.Sys;

namespace ToSic.Sxc.Dnn.WebApiRouting;

/// <inheritdoc />
/// <summary>
/// This controller will check if it's responsible (based on url)
/// ...and if yes, compile / run the app-specific api controllers
/// ...otherwise hand processing back to next api controller up-stream
/// </summary>
internal class AppApiControllerSelector : IHttpControllerSelector
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
        var simpleMatch = AllowedRoutes.Any(a => routeData.Route.RouteTemplate.ToLowerInvariant().Contains(a));
        if (simpleMatch)
            return true;

        var rexMatch = RegExRoutes.Any(
            a => new Regex(a, RegexOptions.None).IsMatch(routeData.Route.RouteTemplate.ToLowerInvariant()) );
        return rexMatch;

    }

    public HttpControllerDescriptor SelectController(HttpRequestMessage request)
    {
        // Do this once and early, to be really sure we always use the same one
        var sp = DnnStaticDi.GetPageScopedServiceProvider();

        // Log this lookup and add to history for insights
        var log = new Log("Sxc.Http", null, request?.RequestUri?.AbsoluteUri);
        AddToInsightsHistory(sp, request?.RequestUri?.AbsoluteUri, log);

        var l = log.Fn<HttpControllerDescriptor>();

        if (!HandleRequestWithThisController(request))
            return l.Return(PreviousSelector.SelectController(request), "upstream");

        var routeData = request.GetRouteData();

        var controllerTypeName = routeData.Values[VarNames.Controller] + "Controller";

        // Now Handle the 2sxc app-api queries
            
        // Figure out the Path, or show error for that
        var appFolder = sp.Build<DnnAppFolderUtilities>(log).GetAppFolder(request, true);

        try
        {
            // new for 2sxc 9.34 #1651
            var edition = GetEdition(routeData);
            l.A($"Edition: {edition}");

            var site = sp.Build<ISite>(log);

            // First check local app (in this site), then global
            var descriptor = DescriptorIfExists(log, request, site, appFolder, edition, controllerTypeName, false);
            if (descriptor != null) return l.ReturnAsOk(descriptor);

            l.A("path not found, will check on shared location");
            descriptor = DescriptorIfExists(log, request, site, appFolder, edition, controllerTypeName, true);
            if (descriptor != null) return l.ReturnAsOk(descriptor);
        }
        catch (Exception e)
        {
            throw l.Done(DnnHttpErrors.LogAndReturnException(request, HttpStatusCode.InternalServerError, e, DnnHttpErrors.ApiErrMessage, sp.Build<CodeErrorHelpService>()));
        }

        // If we got to here we didn't find it.
        // But we want to throw the exception here, otherwise it's re-wrapped.
        l.A("Path / Controller not found in shared, error will be thrown in a moment");
        var msgFinal = $"2sxc Api Controller Finder: Controller {controllerTypeName} not found in app and paths.";
        throw l.Done(DnnHttpErrors.LogAndReturnException(request, HttpStatusCode.NotFound, new Exception(), msgFinal, sp.Build<CodeErrorHelpService>()));

    }

    private HttpControllerDescriptor DescriptorIfExists(ILog log, HttpRequestMessage request, ISite site, string appFolder, string edition, string controllerTypeName, bool shared)
    {
        var l = log.Fn<HttpControllerDescriptor>();
        var controllerFolder = Path
            .Combine(shared ? site.SharedAppsRootRelative() : site.AppsRootPhysical, appFolder, edition + "api/")
            .ForwardSlash();
        var controllerPath = Path.Combine(controllerFolder, $"{controllerTypeName}.cs");
        l.A($"Controller Folder: '{controllerFolder}' Path: '{controllerPath}'");

        // note: this may look like something you could optimize/cache the result, but that's a bad idea
        // because when the file changes, the type-object will be different, so please don't optimize :)
        var exists = File.Exists(HostingEnvironment.MapPath(controllerPath));
        var descriptor = exists ? BuildDescriptor(request, controllerFolder, controllerPath, controllerTypeName) : null;
        return l.Return(descriptor, $"{nameof(exists)}: {exists}");
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

    private HttpControllerDescriptor BuildDescriptor(HttpRequestMessage request, string folder, string fullPath, string typeName)
    {
        var assembly = BuildManager.GetCompiledAssembly(fullPath)
                       ?? throw new Exception("Assembly not found or compiled to null (error).");
        var type = assembly.GetType(typeName, true, true)
                   ?? throw new Exception($"Type '{typeName}' not found in assembly. Could be a compile error or name mismatch.");

        // help with path resolution for compilers running inside the created controller
        request?.Properties.Add(CodeCompiler.SharedCodeRootPathKeyInCache, folder);
        request?.Properties.Add(CodeCompiler.SharedCodeRootFullPathKeyInCache, fullPath);

        return new HttpControllerDescriptor(_config, type.Name, type);
    }

    private static void AddToInsightsHistory(IServiceProvider sp, string url, ILog log)
    {
        // 2022-12-21 ATM we seem to have an error adding this - must review later
        // TODO:
        try
        {
            var addToHistory = true;
#pragma warning disable CS0162
            if (InsightsController.InsightsLoggingEnabled)
                addToHistory = (url?.Contains(InsightsController.InsightsUrlFragment) ?? false);
#pragma warning restore CS0162
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (addToHistory) sp.Build<ILogStore>().Add("http-request", log);
        }
        catch { /* ignore */ }
    }
}