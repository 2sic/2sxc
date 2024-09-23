using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http.Controllers;
using ToSic.Eav;
using ToSic.Eav.Apps.Internal;
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
using ToSic.Sxc.Dnn.Compile.Internal;
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
    MemoryCacheService memoryCacheService,
    LazySvc<IAppJsonService> appJson)
    : ServiceBase("Dnn.ApiSSv", connect: [folderUtilities, site, roslynLazy, getBlockLazy, analyzerLazy, codeErrorSvc, assemblyCacheManager, appCodeLoader, sxcContextResolver, memoryCacheService, appJson])
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

        var controllerTypeName = routeData.Values[VarNames.Controller] + Constants.ApiControllerSuffix;
        l.A($"Controller: {controllerTypeName}");

        // Now Handle the 2sxc app-api queries

        try
        {
            // new for 2sxc 9.34 #1651
            var edition = VarNames.GetEdition(routeData.Values);
            l.A($"Edition: {edition}");


            // Try to get block - will only work, if the request has all headers
            var block = getBlockLazy.Value.GetCmsBlock(request);
            l.A($"has block: {block != null}");

            // Figure out the Path, or show error for that
            var appFolder = folderUtilities.Setup(request).GetAppFolder(true, block);
            l.A($"AppFolder: {appFolder}");

            // Specs for HotBuild - may not be available, but should be retrieved from the block or App-Path
            HotBuildSpec spec;
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
                var appSpecs = sxcContextResolver.Value.SetAppOrNull(appFolder)?.AppReader.Specs ?? throw new("App not found");
                spec = new(appSpecs.AppId, edition: edition.TrimLastSlash(), appSpecs.Name);
                l.A($"{nameof(spec)} from App based on path: {spec}");
            }

            // First check local app (in this site), then global
            var descriptor = Get(appFolder, edition, controllerTypeName, false, spec);
            if (descriptor != null)
                return l.ReturnAsOk(descriptor);

            l.A("path not found, will check on shared location");
            descriptor = Get(appFolder, edition, controllerTypeName, true, spec);
            if (descriptor != null)
                return l.ReturnAsOk(descriptor);
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

    private (HttpControllerDescriptorWithPaths descriptor, IEnumerable<string> cacheKeys, IList<string> filePaths) BuildDescriptorIfExists(string appFolder, string edition, string controllerTypeName, bool shared, HotBuildSpec spec)
    {
        var l = Log.Fn<(HttpControllerDescriptorWithPaths descriptor, IEnumerable<string> cacheKeys, IList<string> filePaths)>(
            $"{nameof(appFolder)}:'{appFolder}'; {nameof(edition)}:'{edition}'; {nameof(controllerTypeName)}:'{controllerTypeName}'; {nameof(shared)}:{shared}; {spec}",
            timer: true
        );
        // 0. Prepare folders which will be used
        var controllerFolder = GetControllerFolder(appFolder, edition, shared);

        // 1. Check AppCode Dll
        if (spec != null)
        {
            var appCodeAssemblyResult = appCodeLoader.Value.GetAppCode(spec).AssemblyResult;

            // If assembly found, check if this controller exists in that dll - if yes, return it
            var type = appCodeAssemblyResult?.Assembly?.FindControllerTypeByName(controllerTypeName);
            if (type != null)
            {
                // cache dependency on existing cache item with AppCode assembly 
                var appCodeDescriptor = new HttpControllerDescriptor(Configuration, type.Name, type);
                var fakeFolder = controllerFolder.Replace("/api/", "/AppCode/");
                return l.Return((descriptor: new(appCodeDescriptor, fakeFolder, fakeFolder + "AppCode-auto-compiled.dll"),
                                cacheKeys: [appCodeAssemblyResult.CacheDependencyId],
                                filePaths: null), 
                        "Api controller from AppCode");
            }
        }

        // 2. Normal Api, compiled on the fly
        var controllerPath = GetControllerPath(appFolder, edition, shared, controllerTypeName);
        l.A($"Controller Folder: '{controllerFolder}' Path: '{controllerPath}'");

        // note: this may look like something you could optimize/cache the result, but that's a bad idea
        // because when the file changes, the type-object will be different, so please don't optimize :)
        var (descriptor, cacheDependencyKeys) = BuildDescriptorOrThrow(controllerPath, controllerTypeName, spec);
        var hasCacheKeys = cacheDependencyKeys?.Count > 0;
        return l.Return((descriptor: new(descriptor, controllerFolder, controllerPath), 
                        cacheKeys: hasCacheKeys ? cacheDependencyKeys : null, // cache dependency on existing cache item;
                        filePaths: hasCacheKeys ? null: [HostingEnvironment.MapPath(controllerPath)]), // cache dependency on existing api file
            $"normal Api controller from '{controllerPath}'"); 
    }

    private string GetControllerFolder(string appFolder, string edition, bool shared)
        => Path.Combine(shared ? site.SharedAppsRootRelative() : site.AppsRootPhysical, appFolder, edition + "api/").ForwardSlash();

    private string GetControllerPath(string appFolder, string edition, bool shared, string controllerTypeName)
        => Path.Combine(GetControllerFolder(appFolder, edition, shared), $"{controllerTypeName}.cs");

    private (HttpControllerDescriptor HttpControllerDescriptor, List<string> CacheDependecyKeys) BuildDescriptorOrThrow(string fullPath, string typeName, HotBuildSpec spec)
    {
        var l = Log.Fn<(HttpControllerDescriptor, List<string>)>($"{nameof(fullPath)}:'{fullPath}'; {nameof(typeName)}:'{typeName}'; {spec}", timer: true);
        Assembly assembly;
        List<string> cacheDependencyKeys = null;
        var codeFileInfo = analyzerLazy.Value.TypeOfVirtualPath(fullPath);
        var alwaysUseRoslyn = appJson.Value.DnnCompilerAlwaysUseRoslyn(spec.AppId);
        if (alwaysUseRoslyn || codeFileInfo.AppCode)
        {
            l.A("AppCode - use Roslyn");
            var result = roslynLazy.Value.GetCompiledAssembly(codeFileInfo, typeName, spec);
            assembly = result?.Assembly;

            // build list of cache dependencies keys
            if (!string.IsNullOrEmpty(result?.CacheDependencyId))
            {
                cacheDependencyKeys = [result.CacheDependencyId];
                if (alwaysUseRoslyn) 
                    cacheDependencyKeys.Add(appJson.Value.AppJsonCacheKey(spec.AppId));
            }
            l.A($"cache dependency keys: {cacheDependencyKeys?.Count ?? 0}");
        }
        else
        {
            l.A("no AppCode - use BuildManager");
            assembly = BuildManager.GetCompiledAssembly(fullPath);
        }

        if (assembly == null)
            throw l.Ex(new Exception("Assembly not found or compiled to null (error)."));

        l.A($"FindControllerTypeByName: '{typeName}'");
        var type = assembly.FindControllerTypeByName(typeName)
            ?? throw l.Ex(new Exception($"Type '{typeName}' not found in assembly. Could be a compile error or name mismatch."));

        l.A($"Type found: '{type.Name}'");
        return l.Return((new (Configuration, type.Name, type), cacheDependecyKeys: cacheDependencyKeys));
    }

    ///// <summary>
    ///// help with path resolution for compilers running inside the created controller
    ///// </summary>
    //private void SetPathForCompilersInsideController(string appFolder, string edition, string controllerTypeName, bool shared)
    //{
    //    var controllerPath = GetControllerPath(appFolder, edition, shared, controllerTypeName);
    //    if (!File.Exists(HostingEnvironment.MapPath(controllerPath))) return;
    //    var controllerFolder = GetControllerFolder(appFolder, edition, shared);
    //    PreservePathForGetCodeInController(controllerFolder, controllerPath);
    //}

    private void PreservePathForGetCodeInController(string controllerFolder, string controllerPath)
    {
        if (Request == null) return;
        Request.Properties.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);
        Request.Properties.Add(CodeCompiler.SharedCodeRootFullPathKeyInCache, controllerPath);
    }
}