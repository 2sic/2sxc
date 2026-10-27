using System.Net;
using System.Web.Http.Controllers;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.Routing;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Dnn.WebApi.Custom.Sys;

/// <summary>
/// Special service to handle finding and compiling the correct API controller.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("only made public for docs")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class DnnAppApiControllerService(
    Generator<DnnWebApiAppFolderUtilities, HttpRequestMessage> dnnApiFolderUtils,
    LazySvc<DnnAppApiCompiler> apiCompilerLazy,
    LazySvc<DnnGetBlock> getBlock,
    LazySvc<CodeErrorHelpService> codeErrorSvc,
    LazySvc<ISxcCurrentContextService> currentCtxSvc,
    LazySvc<IContextBuilderService> ctxBuilder,
    MemoryCacheService cacheSvc)
    : ServiceBase("Dnn.ApiSSv", connect: [dnnApiFolderUtils, apiCompilerLazy, getBlock, codeErrorSvc, currentCtxSvc, ctxBuilder, cacheSvc])
{
    /// <summary>
    /// Find and build the controller descriptor referencing the correct WebApi controller class.
    /// ASP.net will pick this up and use it from there.
    /// </summary>
    /// <remarks>
    /// This is only called if the <see cref="DnnAppApiControllerSelector"/> believes it must be used.
    /// So if it can't find a controller, it will throw an exception.
    /// </remarks>
    public HttpControllerDescriptor FindAndBuild(HttpConfiguration configuration, HttpRequestMessage request)
    {
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
            var blockOrNull = getBlock.Value.TryGetBlockSpecsUsingModuleAndContentBlockHeaders(request);
            l.A($"has block: {blockOrNull != null}, if not null, will broadcast");
            if (blockOrNull != null)
                currentCtxSvc.Value.ShareBlockSpecs(blockOrNull);

            // 3. Figure out the Path, or show error for that
            var appFolder = dnnApiFolderUtils.New(request).GetAppFolder(errorIfNotFound: true, blockOrNull: blockOrNull);
            l.A($"AppFolder: {appFolder}");

            // 4. Specs for HotBuild - may not be available, but should be retrieved from the block or App-Path
            HotBuildSpec spec;
            // 4.1. If we have a block, we can get the AppId and name from it
            if (blockOrNull != null)
            {
                // Note: Had to trim last slash, because it was added in the get-call, but causes trouble here
                // must ensure it's improved and done the same way in Oqtane!!!
                spec = new(blockOrNull.AppId, edition: edition.TrimLastSlash(), blockOrNull.AppOrNull?.Name);
                l.A($"{nameof(spec)} from Block: {spec}");
            }
            // 4.2. If we don't have a block, we can try to get the AppId and name from the path; will throw if not found
            else
            {
                // find AppId based on path - otherwise we don't have a proper spec, and things fail
                var appCtx = ctxBuilder.Value.TryFindAppFromPath(appFolder);
                var appSpecs = appCtx?.AppReaderRequired.Specs
                               ?? throw new("App not found; nothing in the request suffices to find it.");
                spec = new(appSpecs.AppId, edition: edition.TrimLastSlash(), appSpecs.Name);
                l.A($"{nameof(spec)} from App based on path: {spec}");
            }

            // 5.1. First check local app (in this site), then global
            var descriptor = GetControllerDescriptor(configuration, request, appFolder, edition, false, controllerTypeName, spec);
            if (descriptor != null)
                return l.Return(descriptor, "from local app");

            // 5.2. If not found, check shared location
            l.A("path not found, will check on shared location");
            descriptor = GetControllerDescriptor(configuration, request, appFolder, edition, true, controllerTypeName, spec);
            if (descriptor != null)
                return l.Return(descriptor, "from shared location");
        }
        catch (Exception e)
        {
            throw l.Done(DnnHttpErrors.LogAndReturnException(request, HttpStatusCode.InternalServerError, e, DnnHttpErrors.ApiErrMessage, codeErrorSvc.Value));
        }

        // 6. If we got to here we didn't find it. So throw (outside try-cache)
        l.A("Path / Controller not found in shared, error will be thrown in a moment");
        var msgFinal = $"2sxc {nameof(DnnAppApiControllerService)}: Controller '{controllerTypeName}' not found in app and paths.";
        throw l.Done(DnnHttpErrors.LogAndReturnException(request, HttpStatusCode.NotFound, new(), msgFinal, codeErrorSvc.Value));
    }

}