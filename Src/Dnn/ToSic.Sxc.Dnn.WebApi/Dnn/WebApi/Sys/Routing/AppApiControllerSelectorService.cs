using System.Net;
using System.Web.Http.Controllers;
using ToSic.Eav.Context;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.Routing;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

/// <summary>
/// Special service to handle the selection of the correct API controller.
///
/// It's in a separate class, so DI etc. works properly
/// </summary>
internal partial class AppApiControllerSelectorService(
    Generator<DnnWebApiAppFolderUtilities, HttpRequestMessage> folderUtilities,
    ISite site,
    LazySvc<AppApiControllerCompiler> apiCompilerLazy,
    LazySvc<DnnGetBlock> getBlockLazy,
    LazySvc<CodeErrorHelpService> codeErrorSvc,
    LazySvc<ISxcCurrentContextService> sxcContextResolver,
    MemoryCacheService memoryCacheService)
    : ServiceBase("Dnn.ApiSSv", connect: [folderUtilities, apiCompilerLazy, getBlockLazy, codeErrorSvc, sxcContextResolver, memoryCacheService])
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

        // 1. Figure out the exact C# Controller Type Name
        var controllerTypeName = routeData.Values[VarNames.Controller] + EavConstants.ApiControllerSuffix;
        l.A($"Controller: {controllerTypeName}");

        // 2. Now Handle the 2sxc app-api queries
        try
        {
            // 2.1. Get edition from URL (if provided), as we'll need it for various operations
            var edition = VarNames.GetEdition(routeData.Values);
            l.A($"Edition: {edition}");


            // 2.2. Try to get block - will only work, if the request has all headers
            var blockOrNull = getBlockLazy.Value.TryGetBlockSpecsUsingModuleAndContentBlockHeaders(request);
            l.A($"has block: {blockOrNull != null}, if not null, will broadcast");
            if (blockOrNull != null)
                sxcContextResolver.Value.ShareBlockSpecs(blockOrNull);

            // 3. Figure out the Path, or show error for that
            var appFolder = folderUtilities.New(request).GetAppFolder(errorIfNotFound: true, blockOrNull: blockOrNull);
            l.A($"AppFolder: {appFolder}");

            // 4. Specs for HotBuild - may not be available, but should be retrieved from the block or App-Path
            HotBuildSpec spec;
            if (blockOrNull != null)
            {
                // TODO: HAD TO trim last slash, because it was added in the get-call, but causes trouble here
                // must ensure it's improved and done the same way in Oqtane!!!
                spec = new(blockOrNull.AppId, edition: edition.TrimLastSlash(), blockOrNull.AppOrNull?.Name);
                l.A($"{nameof(spec)} from Block: {spec}");
            }
            else
            {
                // find AppId based on path - otherwise we don't have a proper spec, and things fail
                var appCtx = sxcContextResolver.Value.TryFindAppFromPath(appFolder);
                var appSpecs = appCtx?.AppReaderRequired.Specs ?? throw new("App not found");
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

}