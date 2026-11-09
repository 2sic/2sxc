#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Backend.Context;
internal class NetCoreWebApiContextHelper: ServiceWithContext
{
    private readonly ControllerBase _owner;
    private readonly ICanGetService _helper;

    public NetCoreWebApiContextHelper(ControllerBase owner, ICanGetService helper) : base("Oqt.ApiHlp")
    {
        if (owner is IHasLog ownerWithLog)
            this.LinkLog(ownerWithLog.Log);
        _owner = owner;
        _helper = helper;
    }

    #region Initialize

    internal new IExecutionContext? ExCtxOrNull => base.ExCtxOrNull;

    public ICodeLog CodeLog => field ??= new CodeLog(Log);

    /// <summary>
    /// This will make sure that any services requiring the context can get it.
    /// It must usually be called from the base class which expects to use this.
    /// </summary>
    public void InitializeBlockContext(ActionExecutingContext context)
    {
        if (_blockContextInitialized)
            return;
        _blockContextInitialized = true;
        var getBlock = _helper.GetService<IWebApiContextBuilder>();
        CtxService = getBlock.PrepareContextResolverForApiRequest();
        CtxBuilder = _helper.GetService<IContextBuilderService>();
        BlockOptional = CtxService.BlockOrNull;
    }

    private bool _blockContextInitialized;

    private IContextCompleteService CtxService { get; set; } = null!;
    private IContextBuilderService CtxBuilder { get; set; } = null!;
    internal IBlock? BlockOptional { get; private set; }

    public void OnActionExecutingEnd(ActionExecutingContext context)
    {
        // base.OnActionExecuting(context);
        InitializeBlockContext(context);

        // Use the ServiceProvider of the current request to build DynamicCodeRoot
        // Note that BlockOptional was already retrieved in the base class
        var exCtx = context.HttpContext.RequestServices
            .Build<IExecutionContextFactory>()
            .New(new()
            {
                OwnerOrNull = _owner,
                BlockOrNull = BlockOptional,
                ParentLog = Log,
                CompatibilityFallback = CompatibilityLevels.CompatibilityLevel12,
            });
        ConnectToRoot(exCtx);

        AdamCode = exCtx.GetService<AdamCode>();

        // In case SxcBlock was null, there is no instance, but we may still need the app
        if (ExCtx.GetApp() == null! /* realistic case */)
        {
            Log.A("DynCode.App is null");
            TryToAttachAppFromUrlParams(context);
        }

        // Ensure the Api knows what path it's on, in case it will
        // create instances of .cs files
        if (context.HttpContext.Items.TryGetValue(SourceCodeConstants.SharedCodeRootPathKeyInCache, out var createInstancePath)
            && _owner is IGetCodePath withCodePath)
            withCodePath.CreateInstancePath = (createInstancePath as string)!;
    }


    private void TryToAttachAppFromUrlParams(ActionExecutingContext context)
    {
        var l = Log.Fn();

        try
        {
            // Handed in from the App-API Transformer
            context.HttpContext.Items.TryGetValue(SxcWebApiConstants.HttpContextKeyForAppFolder, out var routeAppPathObj);
            if (routeAppPathObj == null)
            {
                l.Done("nothing to do");
                return;
            }
            var routeAppPath = routeAppPathObj.ToString();

            var appId = CtxBuilder.TryFindAppFromPath(routeAppPath)
                            ?.AppReaderRequired.AppId
                        ?? Eav.Sys.EavConstants.NullId;

            if (appId == Eav.Sys.EavConstants.NullId)
            {
                l.Done("no app detected");
                return;
            }

            // Since we only have the AppId, we need to load the app and attach it to the context
            l.A($"AppId: {appId}");
            var site = CtxService.Context.Site;
            var app = _helper.GetService<Generator<AppForCodeShared, AppForCodeShared.Options>>().New(new(site, new AppIdentityPure(site.ZoneId, appId), new()));
            ((IExCtxAttachApp)ExCtx).AttachApp(app);
            l.Done("app initialized");
            return;
        }
        catch
        {
            l.Done("error, ignore");
        }
    }

    #endregion

    #region Context Maker

    public void SetupResponseMaker() => _helper.GetService<IResponseMaker>().Init(_owner);

    #endregion

    #region Adam

    public AdamCode AdamCode { get; private set; } = null!;

    public Sxc.Adam.IFile SaveInAdam(NoParamOrder npo = default,
        Stream? stream = null,
        string? fileName = null,
        string? contentType = null,
        Guid? guid = null,
        string? field = null,
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
#endif