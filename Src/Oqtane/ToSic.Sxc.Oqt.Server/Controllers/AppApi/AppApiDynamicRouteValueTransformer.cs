using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Builder;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    /// <summary>
    /// Enable dynamically manipulating of route value to select a 2sxc app api dynamic code controller action.
    /// </summary>
    public class AppApiDynamicRouteValueTransformer : DynamicRouteValueTransformer, IHasOqtaneDynamicCodeContext
    {
        private ConcurrentDictionary<string, bool> CompiledAppApiControllers { get; }
        public IServiceProvider ServiceProvider { get; private set; }
        private readonly IModuleRepository _moduleRepository;
        private readonly OqtTempInstanceContext _oqtTempInstanceContext;
        private readonly ITenantResolver _tenantResolver;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationPartManager _partManager;
        public HttpRequest Request { get; private set; }

        public AppApiDynamicRouteValueTransformer(StatefulControllerDependencies dependencies, ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment, ApplicationPartManager partManager, AppApiFileSystemWatcher appApiFileSystemWatcher)
        {
            CompiledAppApiControllers = appApiFileSystemWatcher.CompiledAppApiControllers;
            ServiceProvider = dependencies.ServiceProvider;
            _moduleRepository = dependencies.ModuleRepository;
            _oqtTempInstanceContext = dependencies.OqtTempInstanceContext;
            _tenantResolver = tenantResolver;
            _hostingEnvironment = hostingEnvironment;
            _partManager = partManager;
            Log = new Log(HistoryLogName, null, "AppApiDynamicRouteValueTransformer");
            History.Add(HistoryLogGroup, Log);
            dependencies.CtxResolver.AttachRealBlock(() => GetBlock());
            dependencies.CtxResolver.AttachBlockContext(GetContext);
        }

        public ILog Log { get; }

        protected string HistoryLogGroup { get; } = "app-api";

        protected string HistoryLogName => "App.api";

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            Request = httpContext.Request;

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

                var edition = GetEdition(values);
                Log.Add($"Edition: {edition}");

                var alias = _tenantResolver.GetAlias();
                var aliasPart = $@"Content\Tenants\{alias.TenantId}\Sites\{alias.SiteId}\2sxc";

                var controllerFolder = Path.Combine(aliasPart, appFolder, edition + @"api");
                //controllerFolder = controllerFolder.Replace("\\", @"/");
                Log.Add($"Controller Folder: {controllerFolder}");

                var controllerPath = Path.Combine(controllerFolder, controllerTypeName + ".cs");
                Log.Add($"Controller Path: {controllerPath}");

                // note: this may look like something you could optimize/cache the result, but that's a bad idea
                // because when the file changes, the type-object will be different, so please don't optimize :)
                var apiFile = Path.Combine(_hostingEnvironment.ContentRootPath, controllerPath);
                Log.Add($"Absolute Path: {apiFile}");

                var className = $"DynCode_{controllerFolder.Replace(@"\", "_")}_{System.IO.Path.GetFileNameWithoutExtension(apiFile)}";
                Log.Add($"Class Name: {className}");

                var dllName = $"{className}.dll";
                Log.Add($"Dll Name: {dllName}");

                // Remove older version of AppApi Controller
                if (CompiledAppApiControllers.TryGetValue(apiFile, out var updated))
                {
                    Log.Add($"CompiledAppApiControllers have value: {updated} for: {apiFile}.");
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
                Log.Add($"Compile assembly: {apiFile}, {className}");
                var compiledAssembly = new Compiler().Compile(apiFile, className);
                if (compiledAssembly == null) return wrapLog("Error, can't compile AppApi code.", values);

                var assembly = new Runner().Load(compiledAssembly);

                // Register new AppApi Controller
                AddController(dllName, assembly);

                // help with path resolution for compilers running inside the created controller
                Request?.HttpContext.Items.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

                return wrapLog(CompiledAppApiControllers.TryAdd(apiFile, false)
                    ? $"ok, Controller is compiled and added to ApplicationParts: {apiFile}."
                    : $"Error, while adding key {apiFile} to concurrent dictionary after AppApi Controller is compiled and added to ApplicationPart."
                    , values);
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
            _partManager.ApplicationParts.Add(new AssemblyPart(assembly));
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

                Log.Add(CompiledAppApiControllers.TryRemove(apiFile, out var removeValue)
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

        // TODO: wip, re-factor code bellow to avoid code duplication.

        public string CreateInstancePath { get; set; }

        public OqtaneDynamicCode DynCode => _dynCode ??= ServiceProvider.Build<OqtaneDynamicCode>().Init(GetBlock(), Log);
        private OqtaneDynamicCode _dynCode;

        protected dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            DynCode.CreateInstance(virtualPath, dontRelyOnParameterOrder, name,
                CreateInstancePath, throwOnError);

        protected IContextOfSite GetSiteContext()
        {
            return ServiceProvider.Build<IContextOfSite>();
        }


        protected IContextOfApp GetAppContext(int appId)
        {
            // First get a normal basic context which is initialized with site, etc.
            var appContext = ServiceProvider.Build<IContextOfApp>();
            appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }

        protected IContextOfBlock GetContext() => GetBlock()?.Context ?? ServiceProvider.Build<IContextOfBlock>().Init(Log) as IContextOfBlock;

        protected IBlock GetBlock(bool allowNoContextFound = true) => _block ??= InitializeBlock(allowNoContextFound);
        private IBlock _block;

        private IBlock InitializeBlock(bool allowNoContextFound)
        {
            var wrapLog = Log.Call<IBlock>($"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var moduleId = GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderInstanceId, -1);
            var contentBlockId =
                GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            var pageId = GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderPageId, -1);

            if (moduleId == -1 || pageId == -1)
            {
                if (allowNoContextFound) return wrapLog("not found", null);
                throw new Exception("No context found, cannot continue");
            }

            var module = _moduleRepository.GetModule(moduleId);
            var ctx = _oqtTempInstanceContext.CreateContext(pageId, module, Log);
            IBlock block = ServiceProvider.Build<BlockFromModule>().Init(ctx, Log);

            // only if it's negative, do we load the inner block
            if (contentBlockId > 0) return wrapLog("found", block);

            Log.Add($"Inner Content: {contentBlockId}");
            block = ServiceProvider.Build<BlockFromEntity>().Init(block, contentBlockId, Log);
            return wrapLog("found", block);
        }

        private T GetTypedHeader<T>(string headerName, T fallback)
        {
            var valueString = Request.Headers[headerName];
            if (valueString == StringValues.Empty) return fallback;

            try
            {
                return (T)Convert.ChangeType(valueString.ToString(), typeof(T));
            }
            catch
            {
                return fallback;
            }

        }
    }
}
