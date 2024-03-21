using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http.Controllers;
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
internal class AppApiControllerSelectorService(
    DnnAppFolderUtilities folderUtilities,
    ISite site,
    LazySvc<IRoslynBuildManager> roslynLazy,
    LazySvc<DnnGetBlock> getBlockLazy,
    LazySvc<SourceAnalyzer> analyzerLazy,
    LazySvc<CodeErrorHelpService> codeErrorSvc,
    LazySvc<AssemblyCacheManager> assemblyCacheManager,
    LazySvc<AppCodeLoader> appCodeLoader,
    LazySvc<ISxcContextResolver> sxcContextResolver)
    : ServiceBase("Dnn.ApiSSv", connect: [folderUtilities, site, roslynLazy, getBlockLazy, analyzerLazy, codeErrorSvc, assemblyCacheManager, appCodeLoader, sxcContextResolver])
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
            var appFolderUtilities = folderUtilities.Setup(request);
            var appFolder = appFolderUtilities.GetAppFolder(true, block);

            if (block != null)
            {
                spec = new(block.AppId, edition: edition, block.App?.Name);
                l.A($"{nameof(spec)} from Block: {spec}");
            }
            else
            {
                // find AppId based on path - otherwise we don't have a proper spec, and things fail
                var app = sxcContextResolver.Value.SetAppOrNull(appFolder)?.AppState ?? throw new("App not found");
                spec = new(app.AppId, edition: edition, app.Name);
                l.A($"{nameof(spec)} from App based on path: {spec}");
            }

            // First check local app (in this site), then global
            var descriptor = DescriptorIfExists(appFolder, edition, controllerTypeName, false, spec);
            if (descriptor != null) return l.ReturnAsOk(descriptor);

            l.A("path not found, will check on shared location");
            descriptor = DescriptorIfExists(appFolder, edition, controllerTypeName, true, spec);
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

    private HttpControllerDescriptor DescriptorIfExists(string appFolder, string edition, string controllerTypeName, bool shared, HotBuildSpec spec)
    {
        var l = Log.Fn<HttpControllerDescriptor>();

        // 1. Check AppCode Dll
        if (spec != null)
        {
            // First try cache, then try to compile
            var appCodeAssembly = assemblyCacheManager.Value.TryGetAppCode(spec).Result?.Assembly;
            appCodeAssembly ??= appCodeLoader.Value.GetAppCode(spec).Assembly;

            // If assembly found, check if this controller exists in that dll - if yes, return it
            if (appCodeAssembly != null)
            {
                var type = appCodeAssembly.GetType(controllerTypeName, false, true);
                if (type != null)
                    return l.Return(new(Configuration, type.Name, type), "Api controller from AppCode");
            }
        }

        // 2. Normal Api, compiled on the fly
        var controllerFolder = Path
            .Combine(shared ? site.SharedAppsRootRelative() : site.AppsRootPhysical, appFolder, edition + "api/")
            .ForwardSlash();
        var controllerPath = Path.Combine(controllerFolder, $"{controllerTypeName}.cs");
        l.A($"Controller Folder: '{controllerFolder}' Path: '{controllerPath}'");

        // note: this may look like something you could optimize/cache the result, but that's a bad idea
        // because when the file changes, the type-object will be different, so please don't optimize :)
        var exists = File.Exists(HostingEnvironment.MapPath(controllerPath));
        var descriptor = exists
            ? BuildDescriptor(controllerFolder, controllerPath, controllerTypeName, spec)
            : null;
        return l.Return(descriptor, $"{nameof(exists)}: {exists}");
    }


    private HttpControllerDescriptor BuildDescriptor(string folder, string fullPath, string typeName, HotBuildSpec spec)
    {
        var l = Log.Fn<HttpControllerDescriptor>();
        Assembly assembly;
        var codeFileInfo = analyzerLazy.Value.TypeOfVirtualPath(fullPath);
        if (codeFileInfo.AppCode)
        {
            l.A("AppCode - use Roslyn");
            assembly = roslynLazy.Value.GetCompiledAssembly(codeFileInfo, typeName, spec)?.Assembly;
        }
        else
        {
            l.A("no AppCode - use BuildManager");
            assembly = BuildManager.GetCompiledAssembly(fullPath);
        }

        if (assembly == null) throw l.Ex(new Exception("Assembly not found or compiled to null (error)."));

        // TODO: stv, implement more robust FindMainType
        var type = assembly.GetType(typeName, true, true)
                   ?? throw l.Ex(new Exception($"Type '{typeName}' not found in assembly. Could be a compile error or name mismatch."));

        // help with path resolution for compilers running inside the created controller
        Request?.Properties.Add(CodeCompiler.SharedCodeRootPathKeyInCache, folder);
        Request?.Properties.Add(CodeCompiler.SharedCodeRootFullPathKeyInCache, fullPath);

        var result = new HttpControllerDescriptor(Configuration, type.Name, type);
        return l.Return(result);
    }
}