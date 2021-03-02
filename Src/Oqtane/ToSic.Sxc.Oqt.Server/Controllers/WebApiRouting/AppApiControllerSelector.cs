//using System;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Routing;
//using Oqtane.Infrastructure;
//using Oqtane.Repository;
//using System.IO;
//using Microsoft.AspNetCore.Http.Extensions;
//using Microsoft.AspNetCore.Mvc.Controllers;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Primitives;
//using ToSic.Eav.Helpers;
//using ToSic.Eav.Logging;
//using ToSic.Eav.Logging.Simple;
//using ToSic.Eav.Plumbing;
//using ToSic.Oqt.Helpers;
//using ToSic.Sxc.Blocks;
//using ToSic.Sxc.Code;
//using ToSic.Sxc.Context;
//using ToSic.Sxc.Oqt.Server.Code;
//using DynamicCode = ToSic.Sxc.Code.DynamicCode;

//namespace ToSic.Sxc.Oqt.Server.Controllers.WebApiRouting
//{
//    /// <inheritdoc />
//    /// <summary>
//    /// This controller will check if it's responsible (based on url)
//    /// ...and if yes, compile / run the app-specific api controllers
//    /// ...otherwise hand processing back to next api controller up-stream
//    /// </summary>
//    [Route("{alias}/api/2sxc/app")]
//    [Route("{alias}/api/sxc/app")]
//    public class AppApiControllerSelector : DynamicCode
//    {
//        public IServiceProvider ServiceProvider { get; }
//        public virtual string Route => "default";
//        private readonly ITenantResolver _tenantResolver;
//        private readonly IWebHostEnvironment _hostingEnvironment;
//        private readonly ILogManager _logger;
//        private readonly IControllerFactory _defaultControllerFactory;


//        public AppApiControllerSelector(StatefulControllerDependencies dependencies, ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment, ILogManager logger, IControllerFactory defaultControllerFactory, IServiceProvider serviceProvider) : base(dependencies)
//        {
//            ServiceProvider = serviceProvider;
//            _tenantResolver = tenantResolver;
//            _hostingEnvironment = hostingEnvironment;
//            _logger = logger;
//            _defaultControllerFactory = defaultControllerFactory;


//            Log = new Log(HistoryLogName, null);
//            History.Add(HistoryLogGroup, Log);
//        }

//        public ILog Log { get; }

//        protected string HistoryLogGroup { get; } = "app-api";

//        protected string HistoryLogName => "App.api";

//        public string SelectController(string appFolder, string ctrl)
//        {
//            // Log this lookup and add to history for insights
//            var log = new Log("Sxc.SelCtrl", null, $"app:{appFolder},ctrl:{ctrl}");
//            var wrapLog = log.Call<AppApiControllerSelector>();

//            try
//            {
//                var routeData = Request.RouteValues;
//                var controllerTypeName = $"{ctrl}Controller";

//                // 1. Figure out the Path, or show error for that

//                //// only check for app folder if we don't have a context
//                //if (appFolder == null)
//                //{
//                //    log.Add("no folder found in url, will auto-detect");
//                //    var block = Eav.Factory.StaticBuild<DnnGetBlock>().GetCmsBlock(Request, log);
//                //    appFolder = block?.App?.Folder;
//                //}

//                log.Add($"App Folder: {appFolder}");

//                var controllerPath = "";

//                // new for 2sxc 9.34 #1651
//                var edition = "";
//                if (routeData.ContainsKey("edition"))
//                    edition = routeData["edition"].ToString();
//                if (!string.IsNullOrEmpty(edition))
//                    edition += "/";

//                log.Add($"Edition: {edition}");


//                var alias = _tenantResolver.GetAlias();
//                var aliasPart = $@"Content\Tenants\{alias.TenantId}\Sites\{alias.SiteId}\2sxc";

//                var controllerFolder = Path.Combine(aliasPart, appFolder, edition + "api/");

//                //controllerFolder = controllerFolder.Replace("\\", @"/");
//                log.Add($"Controller Folder: {controllerFolder}");

//                controllerPath = Path.Combine(controllerFolder + controllerTypeName + ".cs");
//                log.Add($"Controller Path: {controllerPath}");

//                // note: this may look like something you could optimize/cache the result, but that's a bad idea
//                // because when the file changes, the type-object will be different, so please don't optimize :)
//                var absolutePath = Path.Combine(_hostingEnvironment.ContentRootPath, controllerPath);
//                log.Add($"Absolute Path: {absolutePath}");
//                if (System.IO.File.Exists(absolutePath))
//                {
//                    var instance = DynCode.CreateInstance(virtualPath: $"/{controllerPath.Forwardslash()}");
//                    //_defaultControllerFactory.CreateController()
//                    //var assembly = BuildManager.GetCompiledAssembly(controllerPath);
//                    //var type = assembly.GetType(controllerTypeName, true, true);

//                    // help with path resolution for compilers running inside the created controller
//                    Request?.HttpContext.Items.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

//                    //var descriptor = new HttpControllerDescriptor(_config, type.Name, type);
//                    //return wrapLog("ok", descriptor);
//                }

//                log.Add("path not found, error will be thrown in a moment");
//                return NotFound();

//                // ***********************************************************
//                //var fullFilePath = ContentFileHelper.GetCodePath(_hostingEnvironment.ContentRootPath,
//                //    alias, Route, $"{appFolder}/api/", $"{controllerTypeName}.cs");
//                //if (string.IsNullOrEmpty(fullFilePath)) return NotFound();

//                //var fileBytes = System.IO.File.ReadAllBytes(fullFilePath);
//                //var mimeType = ContentFileHelper.GetMimeType(fullFilePath);

//                //return mimeType.StartsWith("image") ? File(fileBytes, mimeType) :
//                //    new FileContentResult(fileBytes, mimeType) { FileDownloadName = Path.GetFileName(fullFilePath) };
//            }
//            catch
//            {
//                return NotFound();
//            }
//        }

//        private static void AddToInsightsHistory(string url, ILog log)
//        {
//            var addToHistory = true;
//            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
//            if (!InsightsController.InsightsLoggingEnabled)
//            {
//                //if (url?.Contains(InsightsController.InsightsUrlFragment) ?? false)
//                addToHistory = false;
//            }
//            if (addToHistory) History.Add("http-request", log);
//        }
//    }
//}
