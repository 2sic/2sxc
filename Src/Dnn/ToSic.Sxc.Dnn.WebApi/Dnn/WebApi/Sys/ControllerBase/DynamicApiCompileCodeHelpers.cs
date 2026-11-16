using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.InfoSystem;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

internal class DnnCustomWebApiHelper(DnnApiController ownerController, DnnWebApiHelper sysHlp)
    : ServiceWithContext("Dnn.CdHlp")
{

    #region Init


    public (IExecutionContext RootExCtx, string Folder) InitExCtxWithBlockSpecOrApp(HttpControllerContext controllerContext)
    {
        var l = Log.Fn<(IExecutionContext, string)>();
        //if (_blockContextInitialized)
        //    throw new("Web API Block Context Was already initialized. This is not expected.");

        // 1. Make sure we have the block (if it exists for this request)
        var request = controllerContext.Request;
        var currentCtxSvc = (sysHlp ?? throw new InvalidOperationException("System helper is not initialized"))
            .GetService<ISxcCurrentContextService>();
        var block = currentCtxSvc.BlockOrNull; // InitBlockSpecs(request);
        l.A($"HasBlock: {block != null}");

        // 2.1 Prepare Services for further initialization
        var services = sysHlp
            .GetService<ApiControllerDependencies>()
            .ConnectServices(Log);

        // 2.2 Build Execution Context
        var exCtx = services.ExCtxFactory.New(new()
        {
            OwnerOrNull = ownerController,
            BlockOrNull = block,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel10,
        });

        // 2.3 Connect the system helper to the new execution context.
        sysHlp.ConnectToRoot(exCtx);

        // 2.4 Connect Adam so we can provide SaveInAdam() APIs below.
        AdamCode = exCtx.GetService<AdamCode>();

        // 2.5 In case SxcBlock was null, there is no instance, but we may still need the app
        // This is special in WebAPIs, so we must handle it here
        var app = exCtx.GetApp();
        if (app == null! /* Special: there are cases where it's null, even though the nullable API doesn't show it */)
        {
            l.A("DynCode.App is null");
            app = GetAppOrNullFromUrlParams(services, request, currentCtxSvc.Context, block);
            if (app != null)
                ((IExCtxAttachApp)exCtx).AttachApp(app);
        }

        // 3.1 Add some DNN objects to the request, for later logging of exceptions
        request.Properties.Add(DnnConstants.DnnContextKey, (exCtx as IHasDnn)?.Dnn);

        // 3.2 Retrieve the underlying path of the current class from the properties
        // It was added by the AppAPiControllerSelectorService when this controller was selected.
        request.Properties.TryGetTyped(SourceCodeConstants.SharedCodeRootPathKeyInCache, out string path);

        // 3.3 v16.02 - try to log more details about the current API call
        var currentPath = request.Properties.TryGetTyped(SourceCodeConstants.SharedCodeRootFullPathKeyInCache, out string p2) ? p2.AfterLast("/") : null;
        sysHlp.WebApiLogging?.AddLogSpecs(block, app, currentPath, sysHlp.GetService<CodeInfosInScope>());

        // 4. Finally return the execution context and the path of the current class
        return l.ReturnAsOk((exCtx, path));
    }


    private IAppForCode GetAppOrNullFromUrlParams(ApiControllerDependencies services, HttpRequestMessage request,
        IContextOfSite siteCtx, IBlock blockOrNull)
    {
        var l = Log.Fn<IAppForCode>();
        try
        {
            var routeAppPath = services.WebApiAppFolderUtilities.New(request)
                .GetAppFolder(errorIfNotFound: false, blockOrNull: blockOrNull);
            var appCtx = sysHlp.GetService<IWebApiAppContextBuilderService>().TryFindAppFromPath(routeAppPath);
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


    #region Adam

    public AdamCode AdamCode { get; private set; }

    public Sxc.Adam.IFile SaveInAdam(NoParamOrder npo = default,
        Stream stream = null,
        string fileName = null,
        string contentType = null,
        Guid? guid = null,
        string field = null,
        string subFolder = "") =>
        AdamCode.SaveInAdam(
            stream: stream,
            fileName: fileName,
            contentType: contentType,
            guid: guid,
            field: field,
            subFolder: subFolder);

    #endregion

}
