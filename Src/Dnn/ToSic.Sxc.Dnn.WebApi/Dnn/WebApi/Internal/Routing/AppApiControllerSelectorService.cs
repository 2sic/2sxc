using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Caching;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http.Controllers;
using ToSic.Eav.Caching;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.WebApi.Routing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Integration;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

/// <summary>
/// Special service to handle the selection of the correct API controller.
///
/// It's in a separate class, so DI etc. works properly
/// </summary>
internal partial class AppApiControllerSelectorService(
    DnnAppFolderUtilities folderUtilities,
    ISite site,
    LazySvc<IRoslynBuildManager> roslynLazy,
    LazySvc<DnnGetBlock> getBlockLazy,
    LazySvc<SourceAnalyzer> analyzerLazy,
    LazySvc<CodeErrorHelpService> codeErrorSvc,
    LazySvc<AssemblyCacheManager> assemblyCacheManager,
    LazySvc<AppCodeLoader> appCodeLoader,
    LazySvc<ISxcContextResolver> sxcContextResolver,
    MemoryCacheService memoryCacheService)
    : ServiceBase("Dnn.ApiSSv", connect: [folderUtilities, site, roslynLazy, getBlockLazy, analyzerLazy, codeErrorSvc, assemblyCacheManager, appCodeLoader, sxcContextResolver, memoryCacheService])
{
    #region Setup / Init

    private void Setup(HttpConfiguration configuration, HttpRequestMessage request)
    {
        _configuration = configuration;
        _request = request;
    }

    private HttpRequestMessage Request => _request ?? throw new("Request not available - call Setup(...) first!");
    private HttpRequestMessage _request;

    private HttpConfiguration Configuration => _configuration ?? throw new("Configuration not available - call Setup(...) first!");
    private HttpConfiguration _configuration;

    #endregion


    public HttpControllerDescriptor SelectController(HttpConfiguration configuration, HttpRequestMessage request)
    {
        Setup(configuration, request);

        var l = Log.Fn<HttpControllerDescriptor>();

        var routeData = request.GetRouteData();

        var controllerTypeName = routeData.Values[VarNames.Controller] + "Controller";

        // Now Handle the 2sxc app-api queries

        try
        {
            // new for 2sxc 9.34 #1651
            var edition = VarNames.GetEdition(routeData.Values);
            l.A($"Edition: {edition}");

            // Specs for HotBuild - may not be available, but should be retrieved from the block or App-Path
            HotBuildSpec spec = null;

            // Try to get block - will only work, if the request has all headers
            var block = getBlockLazy.Value.GetCmsBlock(request);
            l.A($"has block: {block != null}");

            // Figure out the Path, or show error for that
            var appFolder = folderUtilities.Setup(request).GetAppFolder(true, block);
            l.A($"AppFolder: {appFolder}");

            if (block != null)
            {
                // TODO: HAD TO trim last slash, because it was added in the get-call, but causes trouble here
                // must ensure it's improved and done the same way in Oqtane!!!
                spec = new(block.AppId, edition: edition.TrimLastSlash(), block.App?.Name);
                l.A($"{nameof(spec)} from Block: {spec}");
            }
            else
            {
                // find AppId based on path - otherwise we don't have a proper spec, and things fail
                var app = sxcContextResolver.Value.SetAppOrNull(appFolder)?.AppState ?? throw new("App not found");
                spec = new(app.AppId, edition: edition.TrimLastSlash(), app.Name);
                l.A($"{nameof(spec)} from App based on path: {spec}");
            }

            // First check local app (in this site), then global
            var descriptor = Get(appFolder, edition, controllerTypeName, false, spec);
            if (descriptor != null) return l.ReturnAsOk(descriptor);

            l.A("path not found, will check on shared location");
            descriptor = Get(appFolder, edition, controllerTypeName, true, spec);
            if (descriptor != null) return l.ReturnAsOk(descriptor);
        }
        catch (Exception e)
        {
            throw l.Done(DnnHttpErrors.LogAndReturnException(request, HttpStatusCode.InternalServerError, e, DnnHttpErrors.ApiErrMessage, codeErrorSvc.Value));
        }

        // If we got to here we didn't find it.
        // But we want to throw the exception here, otherwise it's re-wrapped.
        l.A("Path / Controller not found in shared, error will be thrown in a moment");
        var msgFinal = $"2sxc Api Controller Finder: Controller {controllerTypeName} not found in app and paths.";
        throw l.Done(DnnHttpErrors.LogAndReturnException(request, HttpStatusCode.NotFound, new(), msgFinal, codeErrorSvc.Value));
    }

    private (HttpControllerDescriptor, CacheItemPolicy) BuildDescriptorIfExists(string appFolder, string edition, string controllerTypeName, bool shared, HotBuildSpec spec)
    {
        var l = Log.Fn<(HttpControllerDescriptor, CacheItemPolicy)>($"{nameof(appFolder)}:'{appFolder}'; {nameof(edition)}:'{edition}'; {nameof(controllerTypeName)}:'{controllerTypeName}'; {nameof(shared)}:{shared}; {spec}", timer: true);
        var expiration = new TimeSpan(1, 0, 0);
        var policy = new CacheItemPolicy { SlidingExpiration = expiration };

        // 1. Check AppCode Dll
        if (spec != null)
        {
            var appCodeAssemblyResult = appCodeLoader.Value.GetAppCode(spec).AssemblyResult;

            // If assembly found, check if this controller exists in that dll - if yes, return it
            var type = appCodeAssemblyResult?.Assembly?.FindControllerTypeByName(controllerTypeName);
            if (type != null)
            {
                policy.ChangeMonitors.Add(memoryCacheService.CreateCacheEntryChangeMonitor([appCodeAssemblyResult.CacheKey])); // cache dependency on existing cache item with AppCode assembly 
                return l.Return((new(Configuration, type.Name, type), policy), "Api controller from AppCode");
            }
        }

        // 2. Normal Api, compiled on the fly
        var controllerFolder = GetControllerFolder(appFolder, edition, shared);
        var controllerPath = GetControllerPath(appFolder, edition, shared, controllerTypeName);
        l.A($"Controller Folder: '{controllerFolder}' Path: '{controllerPath}'");

        // note: this may look like something you could optimize/cache the result, but that's a bad idea
        // because when the file changes, the type-object will be different, so please don't optimize :)
        var (descriptor, cacheKey) = BuildDescriptorOrThrow(controllerPath, controllerTypeName, spec);
        
        if (!string.IsNullOrEmpty(cacheKey)) 
            policy.ChangeMonitors.Add(memoryCacheService.CreateCacheEntryChangeMonitor([cacheKey])); // cache dependency on existing cache item;
        else
            policy.ChangeMonitors.Add(new HostFileChangeMonitor([HostingEnvironment.MapPath(controllerPath)])); // cache dependency on existing api file

        return l.Return((descriptor, policy), $"normal Api controller from '{controllerPath}'");
    }

    private string GetControllerFolder(string appFolder, string edition, bool shared)
        => Path.Combine(shared ? site.SharedAppsRootRelative() : site.AppsRootPhysical, appFolder, edition + "api/").ForwardSlash();

    private string GetControllerPath(string appFolder, string edition, bool shared, string controllerTypeName)
        => Path.Combine(GetControllerFolder(appFolder, edition, shared), $"{controllerTypeName}.cs");

    private (HttpControllerDescriptor HttpControllerDescriptor, string CacheKey) BuildDescriptorOrThrow(string fullPath, string typeName, HotBuildSpec spec)
    {
        var l = Log.Fn<(HttpControllerDescriptor, string)>($"{nameof(fullPath)}:'{fullPath}'; {nameof(typeName)}:'{typeName}'; {spec}", timer: true);
        AssemblyResult result = null;
        Assembly assembly = null;
        var codeFileInfo = analyzerLazy.Value.TypeOfVirtualPath(fullPath);
        if (codeFileInfo.AppCode)
        {
            l.A("AppCode - use Roslyn");
            result = roslynLazy.Value.GetCompiledAssembly(codeFileInfo, typeName, spec);
            assembly = result?.Assembly;
            l.A($"CacheKey: '{result?.CacheKey}'");
        }
        else
        {
            l.A("no AppCode - use BuildManager");
            assembly = BuildManager.GetCompiledAssembly(fullPath);
        }

        if (assembly == null) throw l.Ex(new Exception("Assembly not found or compiled to null (error)."));

        l.A($"FindControllerTypeByName: '{typeName}'");
        var type = assembly.FindControllerTypeByName(typeName)
            ?? throw l.Ex(new Exception($"Type '{typeName}' not found in assembly. Could be a compile error or name mismatch."));

        l.A($"Type found: '{type.Name}'");
        return l.Return((new (Configuration, type.Name, type), result?.CacheKey));
    }

    /// <summary>
    /// help with path resolution for compilers running inside the created controller
    /// </summary>
    private void HelperWithPathResolutionForCompilersInsideController(string appFolder, string edition, string controllerTypeName, bool shared)
    {
        var controllerPath = GetControllerPath(appFolder, edition, shared, controllerTypeName);
        if (!File.Exists(HostingEnvironment.MapPath(controllerPath))) return;

        Request?.Properties.Add(CodeCompiler.SharedCodeRootPathKeyInCache, GetControllerFolder(appFolder, edition, shared));
        Request?.Properties.Add(CodeCompiler.SharedCodeRootFullPathKeyInCache, controllerPath);
    }
}