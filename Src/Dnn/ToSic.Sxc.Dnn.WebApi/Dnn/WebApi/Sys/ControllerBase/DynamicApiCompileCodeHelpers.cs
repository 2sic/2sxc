using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Sys;
using ToSic.Razor.Blade;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.InfoSystem;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

internal class DynamicApiCompileCodeHelpers: CompileCodeHelper
{

    public DynamicApiCompileCodeHelpers(DnnApiController owner, DnnWebApiHelper sysHlp)
    {
        _owner = owner;
        this.LinkLog((owner as IHasLog)?.Log);
        _sysHlp = sysHlp;
    }

    private readonly DnnApiController _owner;
    private readonly DnnWebApiHelper _sysHlp;

    #region Init

    /// <summary>
    /// This will make sure that any services requiring the context can get it.
    /// It must usually be called from the base class which expects to use this.
    /// </summary>
    /// <param name="request"></param>
    public void InitializeBlockContext(HttpRequestMessage request)
    {
        if (_blockContextInitialized)
            return;
        _blockContextInitialized = true;
        SharedCurrentContextService = _sysHlp.GetService<ISxcCurrentContextService>();
        SharedCurrentContextService.AttachBlock(_sysHlp.GetBlockAndContext(request));
    }

    private bool _blockContextInitialized;


    public (IExecutionContext Root, string Folder) Initialize(HttpControllerContext controllerContext)
    {
        var request = controllerContext.Request;
        InitializeBlockContext(request);

        // Note that the CmsBlock is created by the BaseClass, if it's detectable. Otherwise, it's null
        var block = _sysHlp.GetBlockAndContext(request);
        Log.A($"HasBlock: {block != null}");

        var services = _sysHlp.GetService<ApiControllerDependencies>().ConnectServices(Log);
        var codeRoot = services.ExCtxFactory
            .New(new()
            {
                OwnerOrNull = _owner,
                BlockOrNull = block,
                ParentLog = Log,
                CompatibilityFallback = CompatibilityLevels.CompatibilityLevel10,
            });
            //.New(_owner, block, Log, compatibilityFallback: CompatibilityLevels.CompatibilityLevel10);

        _sysHlp.ConnectToRoot(codeRoot);

        AdamCode = codeRoot.GetService<AdamCode>();

        // In case SxcBlock was null, there is no instance, but we may still need the app
        var app = codeRoot.GetApp();
        if (app == null! /* Special: there are cases where it's null, even though the API doesn't show it */)
        {
            Log.A("DynCode.App is null");
            app = GetAppOrNullFromUrlParams(services, request);
            if (app != null)
                ((IExCtxAttachApp)codeRoot).AttachApp(app);
        }

        var reqProperties = request.Properties;

        // must run this after creating AppAndDataHelpers
        reqProperties.Add(DnnConstants.DnnContextKey, (codeRoot as IHasDnn)?.Dnn);

        /*if (*/
        reqProperties.TryGetTyped(SourceCodeConstants.SharedCodeRootPathKeyInCache, out string path);
        /*) CreateInstancePath = path; */

        // 16.02 - try to log more details about the current API call
        var currentPath = reqProperties.TryGetTyped(SourceCodeConstants.SharedCodeRootFullPathKeyInCache, out string p2) ? p2.AfterLast("/") : null;
        _sysHlp.WebApiLogging?.AddLogSpecs(block, app, currentPath, _sysHlp.GetService<CodeInfosInScope>());


        return (codeRoot, path);
    }


    private IApp GetAppOrNullFromUrlParams(ApiControllerDependencies services, HttpRequestMessage request)
    {
        var l = Log.Fn<IApp>();
        try
        {
            var routeAppPath = services.AppFolderUtilities.Setup(request).GetAppFolder(false);
            var appReader = SharedCurrentContextService.SetAppOrNull(routeAppPath)?.AppReaderRequired;

            if (appReader == default)
                return l.ReturnNull("no app detected");
            
            var siteCtx = SharedCurrentContextService.Site();
            // Look up if page publishing is enabled - if module context is not available, always false
            l.A($"AppId: {appReader.AppId}");
            var app = services.AppOverrideLazy.Value;
            app.Init(siteCtx.Site, appReader.PureIdentity(), new());
            return l.Return(app, $"found #{app.AppId}");
        }
        catch
        {
            // ignore
            return l.ReturnNull("exception, ignore");
        }
    }

    #endregion

    public ISxcCurrentContextService SharedCurrentContextService;

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