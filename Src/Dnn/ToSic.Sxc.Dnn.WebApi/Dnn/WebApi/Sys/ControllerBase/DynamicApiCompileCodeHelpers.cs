using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Code;

using ToSic.Sys.Code.InfoSystem;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

internal class DnnCustomWebApiHelper(DnnApiController ownerController, DnnWebApiHelper sysHlp)
    : ServiceBase("Dnn.CdHlp")
{

    #region Init

    public (ICodeContext RootExCtx, string Folder) InitExCtxWithBlockSpecOrApp(HttpControllerContext controllerContext)
    {
        var l = Log.Fn<(ICodeContext, string)>();

        // 1. Make sure we have the block (if it exists for this request)
        var request = controllerContext.Request;
        var currentCtxSvc = (sysHlp ?? throw new InvalidOperationException("System helper is not initialized"))
            .GetServiceNoExCtx<IContextCompleteService>();
        var block = currentCtxSvc.BlockOrNull;
        l.A($"HasBlock: {block != null}");

        // 2.1 Prepare Services for further initialization
        var services = sysHlp
            .GetServiceNoExCtx<ApiControllerDependencies>()
            .ConnectServices(Log);

        // 2.2 Check if we need to replace the App
        // In case SxcBlock was null, there is no instance, but we may still need the app
        // This is special in WebAPIs, so we must handle it here
        var appToAttach = block?.AppOrNull == null
            ? GetAppOrNullFromUrlParams(services, request, currentCtxSvc.Context, block)
            : null;

        // 2.3 Build Execution Context
        var exCtx = services.ExCtxFactory.New(new()
        {
            AppToAttach = appToAttach,
            OwnerOrNull = ownerController,
            BlockOrNull = block,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel10,
        });

        // 3.1 Add some DNN objects to the request, for later logging of exceptions
        request.Properties.Add(DnnConstants.DnnContextKey, (exCtx as IHasDnn)?.Dnn);

        // 3.2 Retrieve the underlying path of the current class from the properties
        // It was added by the AppAPiControllerSelectorService when this controller was selected.
        request.Properties.TryGetTyped(SourceCodeConstants.SharedCodeRootPathKeyInCache, out string path);

        // 3.3 v16.02 - try to log more details about the current API call
        var currentPath = request.Properties.TryGetTyped(SourceCodeConstants.SharedCodeRootFullPathKeyInCache, out string p2) ? p2.AfterLast("/") : null;
        sysHlp.WebApiLogging?.AddLogSpecs(block, exCtx.App, currentPath, sysHlp.GetServiceNoExCtx<CodeInfosInScope>());

        // 4. Finally return the execution context and the path of the current class
        return l.ReturnAsOk((exCtx, path));
    }


    private IAppForCode? GetAppOrNullFromUrlParams(ApiControllerDependencies services, HttpRequestMessage request,
        IContextOfSite siteCtx, IBlock blockOrNull)
    {
        var l = Log.Fn<IAppForCode?>();
        try
        {
            var routeAppPath = services.WebApiAppFolderUtilities.New(request)
                .GetAppFolder(errorIfNotFound: false, blockOrNull: blockOrNull);
            var appCtx = sysHlp.GetServiceNoExCtx<IWebApiAppContextBuilderService>().TryFindAppFromPath(routeAppPath);
            var appReader = appCtx?.AppReaderRequired;

            if (appReader == default)
                return l.ReturnNull("no app detected");
            
            // Look up if page publishing is enabled - if module context is not available, always false
            l.A($"AppId: {appReader.AppId}");
            var app = services.AppOverrideLazy.New(new(siteCtx.Site, appReader.PureIdentity(), new()));
            return l.Return(app, $"found #{app.AppId}");
        }
        catch
        {
            // ignore
            return l.ReturnNull("exception, ignore");
        }
    }

    #endregion
}
