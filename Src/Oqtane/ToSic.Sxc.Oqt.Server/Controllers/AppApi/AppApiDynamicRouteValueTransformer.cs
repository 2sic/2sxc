using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Oqtane.Models;
using Oqtane.Repository;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ToSic.Eav.Helpers;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Routing;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Backend;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Shared;
using Log = ToSic.Lib.Logging.Log;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

/// <summary>
/// Enable dynamically manipulating of route value to select a 2sxc app api dynamic code controller action.
/// </summary>
internal class AppApiDynamicRouteValueTransformer : DynamicRouteValueTransformer, IHasLog
{
    private readonly ITenantResolver _tenantResolver;
    private readonly IWebHostEnvironment _hostingEnvironment;


    public AppApiDynamicRouteValueTransformer(
        ITenantResolver tenantResolver,
        IWebHostEnvironment hostingEnvironment,
        ILogStore logStore)
    {
        Log = new Log(HistoryLogName, null, nameof(AppApiDynamicRouteValueTransformer));
        logStore.Add(HistoryLogGroup, Log);
        this.ConnectLogs([
            _tenantResolver = tenantResolver,
            _hostingEnvironment = hostingEnvironment
        ]);
    }

    public ILog Log { get; }

    protected string HistoryLogGroup { get; } = "app-api";

    protected string HistoryLogName => "Route.Values";

    public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        return await Task.Run(() =>
        {
            var l = Log.Fn<RouteValueDictionary>();

            #region Ensure required alias

            Alias alias;
            if (values.ContainsKey("alias"))
            {
                alias = _tenantResolver.GetAlias();
            }
            else
            {
                var serviceProvider = httpContext.RequestServices;
                // this transient dependency is not provided as usual constructor provided lazy/generator dependency (from root service provider)
                // but as a transient dependency from the request service provider
                var aliasResolver = serviceProvider.Build<AliasResolver>();
                alias = aliasResolver.Alias
                        ?? throw new HttpExceptionAbstraction(HttpStatusCode.NotFound,
                            $"Error: missing required 'alias' route value.", "Not Found");
            }

            var aliasPart = OqtServerPaths.GetAppRootWithSiteId(alias.SiteId);

            #endregion

            // Ensure required route values: alias, appFolder, controller, action.
            if (!values.ContainsKey("appFolder"))
                throw new HttpExceptionAbstraction(HttpStatusCode.NotFound,
                    $"Error: missing required 'appFolder' route value.", "Not Found");
            var appFolder = (string) values["appFolder"];
            if (appFolder == OqtWebApiConstants.Auto)
            {
                // before trying to get the AppFolder, we must init the ICmsContext as this will
                // this transient dependencies are not provided as usual constructor provided lazy/generator dependencies (from root service provider)
                // but as a transient dependencies from the request service provider
                var blockInitializer = httpContext.RequestServices.Build<IWebApiContextBuilder>();
                blockInitializer.PrepareContextResolverForApiRequest();
                appFolder = httpContext.RequestServices.Build<AppFolder>().GetAppFolder();
            }

            if (!values.ContainsKey("controller"))
                throw new HttpExceptionAbstraction(HttpStatusCode.NotFound,
                    $"Error: missing required 'controller' route value.", "Not Found");
            var controller = (string) values["controller"];

            if (!values.ContainsKey("action"))
                throw new HttpExceptionAbstraction(HttpStatusCode.NotFound,
                    $"Error: missing required 'action' route value.", "Not Found");
            var action = (string) values["action"];

            l.A(
                $"TransformAsync route required values are present, alias:{alias.AliasId}, app:{appFolder}, ctrl:{controller}, act:{action}.");

            var controllerTypeName = $"{controller}Controller";
            l.A($"Controller TypeName: {controllerTypeName}");
            values.Add("controllerTypeName", controllerTypeName);

            var edition = GetEdition(values);
            l.A($"Edition: {edition}");


            var controllerFolder = Path.Combine(aliasPart, appFolder, edition.Backslash(), "api");
            l.A($"Controller Folder: {controllerFolder}");

            var area = $"{alias.SiteId}/{OqtConstants.ApiAppLinkPart}/{appFolder}/{edition}api";
            l.A($"Area: {area}");
            values.Add("area", area);

            var controllerPath = Path.Combine(controllerFolder, controllerTypeName + ".cs");
            l.A($"Controller Path: {controllerPath}");

            var apiFile = Path.Combine(_hostingEnvironment.ContentRootPath, controllerPath);
            l.A($"Absolute Path: {apiFile}");
            values.Add("apiFile", apiFile);

            var dllName = GetDllName(controllerFolder, apiFile);
            l.A($"Dll Name: {dllName}");
            values.Add("dllName", dllName);

            // help with path resolution for compilers running inside the created controller
            httpContext./*Request?.HttpContext.*/Items.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

            httpContext./*Request?.HttpContext.*/Items.Add(SxcWebApiConstants.HttpContextKeyForAppFolder, appFolder);

            return l.Return(values, $"ok, TransformAsync route required values are prepared");
        });
    }

    public static string GetDllName(string controllerFolder, string apiFile)
    {
        return
            $"DynCode_{controllerFolder.Replace(@"\", "_")}_{System.IO.Path.GetFileNameWithoutExtension(apiFile)}";
    }

    private static string GetEdition(RouteValueDictionary values)
    {
        return VarNames.GetEdition(values);
        //// new for 2sxc 9.34 #1651
        //var edition = "";
        //if (values.TryGetValue(VarNames.Edition, out var value)) edition = value?.ToString();
        //return edition + (string.IsNullOrEmpty(edition) ? "" : "/");
    }
}