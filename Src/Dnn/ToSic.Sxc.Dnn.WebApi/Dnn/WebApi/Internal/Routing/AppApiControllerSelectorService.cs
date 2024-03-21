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
    LazySvc<CodeErrorHelpService> codeErrorSvc)
    : ServiceBase("Dnn.ApiSSv", connect: [roslynLazy, getBlockLazy])
{
    #region Setup / Init

    public AppApiControllerSelectorService Setup(HttpConfiguration configuration, HttpRequestMessage request)
    {
        _configuration = configuration;
        _request = request;
        return this;
    }

    public HttpControllerDescriptor SelectController(HttpConfiguration configuration, HttpRequestMessage request)
    {
        Setup(configuration, request);

        var l = Log.Fn<HttpControllerDescriptor>();

        var routeData = request.GetRouteData();

        var controllerTypeName = routeData.Values[VarNames.Controller] + "Controller";

        // Now Handle the 2sxc app-api queries

        // Figure out the Path, or show error for that
        var appFolderUtilities = folderUtilities.Setup(request);
        var appFolder = appFolderUtilities.GetAppFolder(true);

        try
        {
            // new for 2sxc 9.34 #1651
            var edition = VarNames.GetEdition(routeData.Values);
            l.A($"Edition: {edition}");

            // First check local app (in this site), then global
            var descriptor = DescriptorIfExists(site, appFolder, edition, controllerTypeName, false);
            if (descriptor != null) return l.ReturnAsOk(descriptor);

            l.A("path not found, will check on shared location");
            descriptor = DescriptorIfExists(site, appFolder, edition, controllerTypeName, true);
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

    private HttpRequestMessage Request  => _request ?? throw new("Request not available - call Setup(...) first!");
    private HttpRequestMessage _request;

    private HttpConfiguration Configuration => _configuration ?? throw new("Configuration not available - call Setup(...) first!");
    private HttpConfiguration _configuration;

    #endregion

    private HttpControllerDescriptor DescriptorIfExists(ISite site, string appFolder, string edition, string controllerTypeName, bool shared)
    {
        var l = Log.Fn<HttpControllerDescriptor>();
        var controllerFolder = Path
            .Combine(shared ? site.SharedAppsRootRelative() : site.AppsRootPhysical, appFolder, edition + "api/")
            .ForwardSlash();
        var controllerPath = Path.Combine(controllerFolder, $"{controllerTypeName}.cs");
        l.A($"Controller Folder: '{controllerFolder}' Path: '{controllerPath}'");

        // note: this may look like something you could optimize/cache the result, but that's a bad idea
        // because when the file changes, the type-object will be different, so please don't optimize :)
        var exists = File.Exists(HostingEnvironment.MapPath(controllerPath));
        var descriptor = exists
            ? BuildDescriptor(controllerFolder, controllerPath, controllerTypeName, edition.TrimLastSlash())
            : null;
        return l.Return(descriptor, $"{nameof(exists)}: {exists}");
    }


    private HttpControllerDescriptor BuildDescriptor(string folder, string fullPath, string typeName, string edition)
    {
        var l = Log.Fn<HttpControllerDescriptor>();
        Assembly assembly;
        var codeFileInfo = analyzerLazy.Value.TypeOfVirtualPath(fullPath);
        if (codeFileInfo.AppCode)
        {
            l.A("AppCode - use Roslyn");
            // Figure edition
            HotBuildSpec spec = null;
            var block = getBlockLazy.Value.GetCmsBlock(Request);
            l.A($"has block: {block != null}");
            if (block != null)
            {
                spec = new(block.AppId, edition: edition, block.App?.Name);
                l.A($"{nameof(spec)}: {spec}");
            }
            // TODO: Otherwise try to find AppId

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