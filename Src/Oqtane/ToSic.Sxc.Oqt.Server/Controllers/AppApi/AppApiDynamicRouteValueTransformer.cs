using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Oqtane.Infrastructure;
using Oqtane.Repository;
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
    public class AppApiDynamicRouteValueTransformer : DynamicRouteValueTransformer, IHasOqtaneDynamicCodeContext
    {
        public IServiceProvider ServiceProvider { get; private set; }
        private readonly IModuleRepository _moduleRepository;
        private readonly OqtTempInstanceContext _oqtTempInstanceContext;
        private readonly ITenantResolver _tenantResolver;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogManager _logger;
        private readonly ApplicationPartManager _partManager;
        public HttpRequest Request { get; private set; }

        public AppApiDynamicRouteValueTransformer(StatefulControllerDependencies dependencies, ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment, ILogManager logger, ApplicationPartManager partManager)
        {
            ServiceProvider = dependencies.ServiceProvider;
            _moduleRepository = dependencies.ModuleRepository;
            _oqtTempInstanceContext = dependencies.OqtTempInstanceContext;
            _tenantResolver = tenantResolver;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _partManager = partManager;
            Log = new Log(HistoryLogName, null);
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

            if (!values.ContainsKey("alias") || !values.ContainsKey("appFolder") || !values.ContainsKey("controller") || !values.ContainsKey("action")) return values;

            var aliasId = int.Parse((string)values["alias"]);
            var appFolder = (string)values["appFolder"];
            var controller = (string)values["controller"];
            var action = (string)values["action"];

            // Log this lookup and add to history for insights
            var log = new Log("Sxc.TransformAsync", null, $"alias:{aliasId},app:{appFolder},ctrl:{controller},act:{action}");
            var wrapLog = log.Call<RouteValueDictionary>();

            //try
            //{
            var controllerTypeName = $"{controller}Controller";

            // 1. Figure out the Path, or show error for that

            //// only check for app folder if we don't have a context
            //if (appFolder == null)
            //{
            //    log.Add("no folder found in url, will auto-detect");
            //    var block = Eav.Factory.StaticBuild<DnnGetBlock>().GetCmsBlock(Request, log);
            //    appFolder = block?.App?.Folder;
            //}

            log.Add($"App Folder: {appFolder}");

            var controllerPath = "";

            // new for 2sxc 9.34 #1651
            var edition = "";
            if (values.ContainsKey("edition"))
                edition = values["edition"].ToString();
            if (!string.IsNullOrEmpty(edition))
                edition += "/";

            log.Add($"Edition: {edition}");


            var alias = _tenantResolver.GetAlias();
            var aliasPart = $@"Content\Tenants\{alias.TenantId}\Sites\{alias.SiteId}\2sxc";

            var controllerFolder = Path.Combine(aliasPart, appFolder, edition + @"api");

            //controllerFolder = controllerFolder.Replace("\\", @"/");
            log.Add($"Controller Folder: {controllerFolder}");

            controllerPath = Path.Combine(controllerFolder, controllerTypeName + ".cs");
            log.Add($"Controller Path: {controllerPath}");

            // note: this may look like something you could optimize/cache the result, but that's a bad idea
            // because when the file changes, the type-object will be different, so please don't optimize :)
            var apiFile = Path.Combine(_hostingEnvironment.ContentRootPath, controllerPath);
            log.Add($"Absolute Path: {apiFile}");

            // Check for AppApi file
            if (!System.IO.File.Exists(apiFile)) return wrapLog("Error, AppApi file is missing.", values);

            // Check for AppApi source code
            var apiCode = System.IO.File.ReadAllText(apiFile);
            if (string.IsNullOrWhiteSpace(apiCode)) return wrapLog("Error, AppApi code is missing.", values);

            var className = $"DynCode_{controllerFolder.Replace(@"\","_")}_{System.IO.Path.GetFileNameWithoutExtension(apiFile)}";
            log.Add($"Class Name: {className}");

            // Remove older version of AppApi Controller
            var dllName = $"{className}.dll";
            foreach (var applicationPart in _partManager.ApplicationParts)
            {
                if (!applicationPart.Name.Equals(dllName)) continue;

                log.Add($"Remove ApplicationPart: {dllName}");
                _partManager.ApplicationParts.Remove(applicationPart);
            }

            // Build new AppApi Controller
            log.Add($"Compile assembly: {apiFile}, {className}");
            var compiledAssembly = new Compiler().Compile(apiFile, className);
            if (compiledAssembly == null) return wrapLog("Error, Can't compile AppApi code", values);

            var assembly = new Runner().Load(compiledAssembly);

            // Register new AppApi Controller
            log.Add($"Add ApplicationPart: {dllName}");
            _partManager.ApplicationParts.Add(new AssemblyPart(assembly));
            // Notify change
            AppApiActionDescriptorChangeProvider.Instance.HasChanged = true;
            AppApiActionDescriptorChangeProvider.Instance.TokenSource.Cancel();

            // help with path resolution for compilers running inside the created controller
            Request?.HttpContext.Items.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

            return wrapLog("ok", values);
        }

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
