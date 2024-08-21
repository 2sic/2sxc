using System.IO;
using System.Web.Http.Controllers;
using ToSic.Eav.Apps;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Generics;
using ToSic.Lib.Coding;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Internal;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

internal class DynamicApiCodeHelpers: CodeHelper
{

    public DynamicApiCodeHelpers(DnnApiController owner, DnnWebApiHelper sysHlp)
    {
        _owner = owner;
        this.LinkLog((owner as IHasLog)?.Log);
        SysHlp = sysHlp;
    }

    private readonly DnnApiController _owner;
    private readonly DnnWebApiHelper SysHlp;

    #region Init

    /// <summary>
    /// This will make sure that any services requiring the context can get it.
    /// It must usually be called from the base class which expects to use this.
    /// </summary>
    /// <param name="request"></param>
    public void InitializeBlockContext(HttpRequestMessage request)
    {
        if (_blockContextInitialized) return;
        _blockContextInitialized = true;
        SharedContextResolver = SysHlp.GetService<ISxcContextResolver>();
        SharedContextResolver.AttachBlock(SysHlp.GetBlockAndContext(request));
    }

    private bool _blockContextInitialized;


    public (ICodeApiService Root, string Folder) Initialize(HttpControllerContext controllerContext)
    {
        var request = controllerContext.Request;
        InitializeBlockContext(request);

        // Note that the CmsBlock is created by the BaseClass, if it's detectable. Otherwise it's null
        var block = SysHlp.GetBlockAndContext(request);
        Log.A($"HasBlock: {block != null}");

        var services = SysHlp.GetService<ApiControllerMyServices>().ConnectServices(Log);
        var codeRoot = services.CodeApiServiceFactory
            .BuildCodeRoot(_owner, block, Log, compatibilityFallback: CompatibilityLevels.CompatibilityLevel10);

        SysHlp.ConnectToRoot(codeRoot);

        AdamCode = codeRoot.GetService<AdamCode>();

        // In case SxcBlock was null, there is no instance, but we may still need the app
        if (codeRoot.App == null)
        {
            Log.A("DynCode.App is null");
            var app = GetAppOrNullFromUrlParams(services, request);
            if (app != null)
                ((ICodeApiServiceInternal)codeRoot).AttachApp(app);
        }

        var reqProperties = request.Properties;

        // must run this after creating AppAndDataHelpers
        reqProperties.Add(DnnConstants.DnnContextKey, (codeRoot as IHasDnn)?.Dnn);

        /*if (*/
        reqProperties.TryGetTyped(CodeCompiler.SharedCodeRootPathKeyInCache, out string path);
        /*) CreateInstancePath = path; */

        // 16.02 - try to log more details about the current API call
        var currentPath = reqProperties.TryGetTyped(CodeCompiler.SharedCodeRootFullPathKeyInCache, out string p2) ? p2.AfterLast("/") : null;
        SysHlp.WebApiLogging?.AddLogSpecs(block, codeRoot.App, currentPath, SysHlp.GetService<CodeInfosInScope>());


        return (codeRoot, path);
    }


    private IApp GetAppOrNullFromUrlParams(ApiControllerMyServices services, HttpRequestMessage request)
    {
        var l = Log.Fn<IApp>();
        try
        {
            var routeAppPath = services.AppFolderUtilities.Setup(request).GetAppFolder(false);
            var appReader = SharedContextResolver.SetAppOrNull(routeAppPath)?.AppReader;

            if (appReader == default)
                return l.ReturnNull("no app detected");
            
            var siteCtx = SharedContextResolver.Site();
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

    public ISxcContextResolver SharedContextResolver;

    #region Adam

    public AdamCode AdamCode { get; private set; }

    public Sxc.Adam.IFile SaveInAdam(NoParamOrder noParamOrder = default,
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