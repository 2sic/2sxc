using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Oqtane.Repository;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Builder;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;
using File = System.IO.File;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    /// <summary>
    /// Enable dynamically manipulating of route value to select a 2sxc app api dynamic code controller action.
    /// </summary>
    public class AppApiDynamicRouteValueTransformer : DynamicRouteValueTransformer
    {
        private readonly ConcurrentDictionary<string, bool> _compiledAppApiControllers;
        private readonly ITenantResolver _tenantResolver;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationPartManager _partManager;

        public AppApiDynamicRouteValueTransformer(ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment, ApplicationPartManager partManager, AppApiFileSystemWatcher appApiFileSystemWatcher)
        {
            _tenantResolver = tenantResolver;
            _hostingEnvironment = hostingEnvironment;
            _partManager = partManager;
            _compiledAppApiControllers = appApiFileSystemWatcher.CompiledAppApiControllers;
            Log = new Log(HistoryLogName, null, "AppApiDynamicRouteValueTransformer");
            History.Add(HistoryLogGroup, Log);
        }

        public ILog Log { get; }

        protected string HistoryLogGroup { get; } = "app-api";

        protected string HistoryLogName => "App.api";

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var request = httpContext.Request;

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
                //controllerFolder = controllerFolder.Replace("\\", @"/");
                Log.Add($"Controller Folder: {controllerFolder}");

                var area = $"{alias.SiteId}/{OqtConstants.ApiAppLinkPart}/{appFolder}/{edition}api";
                Log.Add($"Area: {area}");
                values.Add("area", area);

                var controllerPath = Path.Combine(controllerFolder, controllerTypeName + ".cs");
                Log.Add($"Controller Path: {controllerPath}");

                // note: this may look like something you could optimize/cache the result, but that's a bad idea
                // because when the file changes, the type-object will be different, so please don't optimize :)
                var apiFile = Path.Combine(_hostingEnvironment.ContentRootPath, controllerPath);
                Log.Add($"Absolute Path: {apiFile}");

                var dllName = $"DynCode_{controllerFolder.Replace(@"\", "_")}_{System.IO.Path.GetFileNameWithoutExtension(apiFile)}";
                Log.Add($"Dll Name: {dllName}");
                values.Add("dllName", dllName);

                // help with path resolution for compilers running inside the created controller
                request?.HttpContext.Items.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

                // If we have a key (that controller is compiled and registered, but not updated) controller was prepared before, so just return values.
                // Alternatively remove older version of AppApi controller (if we got updated flag from file system watcher).
                if (_compiledAppApiControllers.TryGetValue(apiFile, out var updated))
                {
                    Log.Add($"_compiledAppApiControllers have value: {updated} for: {apiFile}.");
                    if (updated)
                        RemoveController(dllName, apiFile);
                    else
                        return wrapLog(
                            $"ok, nothing to do, AppApi Controller is already compiled and added to ApplicationPart: {apiFile}.",
                            values);
                }
                else
                    Log.Add($"We need to prepare controller for: {apiFile}.");


                // Check for AppApi file
                if (!File.Exists(apiFile)) return wrapLog($"Error, missing AppApi file {apiFile}.", values);

                // Check for AppApi source code
                var apiCode = await File.ReadAllTextAsync(apiFile);
                if (string.IsNullOrWhiteSpace(apiCode)) return wrapLog($"Error, missing AppApi code in file {apiFile}.", values);

                // Build new AppApi Controller
                Log.Add($"Compile assembly: {apiFile}, {dllName}");
                //var assembly = new Compiler().CompileApiCode(apiFile, dllName, alias.SiteId, appFolder, edition);
                var assembly = new Compiler().Compile(apiFile, dllName);

                // Add new key to concurrent dictionary, before registering new AppAPi controller.
                if (!_compiledAppApiControllers.TryAdd(apiFile, false))
                    return wrapLog($"Error, while adding key {apiFile} to concurrent dictionary, so will not register AppApi Controller to avoid duplicate controller routes.", values);

                //// Register new AppApi Controller.
                //AddController(dllName, assembly);

                return wrapLog($"ok, Controller is compiled and added to ApplicationParts: {apiFile}.", values);
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

        private void AddController(string dllName, Assembly assembly)
        {
            Log.Add($"Add ApplicationPart: {dllName}");
            _partManager.ApplicationParts.Add(new CompilationReferencesProvider(assembly));
            // Notify change
            NotifyChange();
        }

        private void RemoveController(string dllName, string apiFile)
        {
            Log.Add($"In ApplicationParts, find AppApi controller: {dllName}.");
            var applicationPart = _partManager.ApplicationParts.FirstOrDefault(a => a.Name.Equals(dllName));
            if (applicationPart != null)
            {
                Log.Add($"From ApplicationParts, remove AppApi controller: {dllName}.");
                _partManager.ApplicationParts.Remove(applicationPart);
                NotifyChange();

                Log.Add(_compiledAppApiControllers.TryRemove(apiFile, out var removeValue)
                    ? $"Value removed: {removeValue} for {apiFile}."
                    : $"Error, can't remove value for {apiFile}.");
            }
            else
            {
                Log.Add($"In ApplicationParts, can't find AppApi controller: {dllName}");
            }
        }

        private static void NotifyChange()
        {
            // Notify change
            AppApiActionDescriptorChangeProvider.Instance.HasChanged = true;
            AppApiActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}
