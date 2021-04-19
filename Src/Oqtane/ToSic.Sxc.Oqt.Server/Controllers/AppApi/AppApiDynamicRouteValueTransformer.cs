using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Oqtane.Repository;
using System;
using System.IO;
using System.Threading.Tasks;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Shared;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    /// <summary>
    /// Enable dynamically manipulating of route value to select a 2sxc app api dynamic code controller action.
    /// </summary>
    public class AppApiDynamicRouteValueTransformer : DynamicRouteValueTransformer
    {
        private readonly ITenantResolver _tenantResolver;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AppApiDynamicRouteValueTransformer(ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment)
        {
            _tenantResolver = tenantResolver;
            _hostingEnvironment = hostingEnvironment;
            Log = new Log(HistoryLogName, null, "AppApiDynamicRouteValueTransformer");
            History.Add(HistoryLogGroup, Log);
        }

        public ILog Log { get; }

        protected string HistoryLogGroup { get; } = "app-api";

        protected string HistoryLogName => "Route.Values";

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var wrapLog = Log.Call<RouteValueDictionary>();

            // Check required route values: alias, appFolder, controller, action.
            if (!values.ContainsKey("alias")) return wrapLog("Error: missing required 'alias' route value", values);
            var aliasId = int.Parse((string)values["alias"]);

            if (!values.ContainsKey("appFolder")) return wrapLog("Error: missing required 'appFolder' route value", values);
            var appFolder = (string)values["appFolder"];

            if (!values.ContainsKey("controller")) return wrapLog("Error: missing required 'controller' route value", values);
            var controller = (string)values["controller"];

            if (!values.ContainsKey("action")) return wrapLog("Error: missing required 'action' route value", values);
            var action = (string)values["action"];

            Log.Add($"TransformAsync route required values are present, alias:{aliasId}, app:{appFolder}, ctrl:{controller}, act:{action}.");

            try
            {
                var controllerTypeName = $"{controller}Controller";
                Log.Add($"Controller TypeName: {controllerTypeName}");
                values.Add("controllerTypeName", controllerTypeName);

                var edition = GetEdition(values);
                Log.Add($"Edition: {edition}");

                var alias = _tenantResolver.GetAlias();
                var aliasPart = $@"Content\Tenants\{alias.TenantId}\Sites\{alias.SiteId}\2sxc";

                var controllerFolder = Path.Combine(aliasPart, appFolder, edition.Backslash(), "api");
                Log.Add($"Controller Folder: {controllerFolder}");

                var area = $"{alias.SiteId}/{OqtConstants.ApiAppLinkPart}/{appFolder}/{edition}api";
                Log.Add($"Area: {area}");
                values.Add("area", area);

                var controllerPath = Path.Combine(controllerFolder, controllerTypeName + ".cs");
                Log.Add($"Controller Path: {controllerPath}");

                var apiFile = Path.Combine(_hostingEnvironment.ContentRootPath, controllerPath);
                Log.Add($"Absolute Path: {apiFile}");
                values.Add("apiFile", apiFile);

                var dllName = $"DynCode_{controllerFolder.Replace(@"\", "_")}_{System.IO.Path.GetFileNameWithoutExtension(apiFile)}";
                Log.Add($"Dll Name: {dllName}");
                values.Add("dllName", dllName);

                // help with path resolution for compilers running inside the created controller
                httpContext.Request?.HttpContext.Items.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

                return wrapLog($"ok, TransformAsync route required values are prepared", values);
            }
            catch (Exception e)
            {
                return wrapLog($"Error, unexpected error {e.Message} while preparing controller.", values);
            }
        }

        private static string GetEdition(RouteValueDictionary values)
        {
            // new for 2sxc 9.34 #1651
            var edition = "";
            if (values.ContainsKey("edition")) edition = values["edition"].ToString();
            if (!string.IsNullOrEmpty(edition)) edition += "/";
            return edition;
        }
    }
}
