#if NETCOREAPP

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context.Sys;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Backend.Context;

internal class NetCoreWebApiContextHelper: ServiceBase
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

    internal IExecutionContext? ExCtxOrNull { get; private set; }

    public ICodeLog CodeLog => field ??= new CodeLog(Log);

    private IContextCompleteService CtxService =>
        field ??= _helper.GetService<IWebApiContextBuilder>().PrepareContextResolverForApiRequest();
    private IWebApiAppContextBuilderService CtxBuilder => field ??= _helper.GetService<IWebApiAppContextBuilderService>();
    internal IBlock? BlockOptional => _blockOptional.Get(() => CtxService.BlockOrNull);
    private readonly LazyGet<IBlock?> _blockOptional = new();

    public void OnActionExecutingEnd(ActionExecutingContext context)
    {
        // Use the ServiceProvider of the current request to build DynamicCodeRoot
        // Note that BlockOptional was already retrieved in the base class
        var exCtx = context.HttpContext.GetSysRequestServices()
            .Build<IExecutionContextFactory>()
            .New(new()
            {
                OwnerOrNull = _owner,
                BlockOrNull = BlockOptional,
                ParentLog = Log,
                CompatibilityFallback = CompatibilityLevels.CompatibilityLevel12,
            });

        // In case SxcBlock was null, there is no instance, but we may still need the app
        if (exCtx.GetApp() == null! /* realistic case */)
        {
            Log.A("DynCode.App is null");
            var app = TryToAttachAppFromUrlParams(context);
            if (app != null)
                ((IExCtxAttachApp)exCtx).AttachApp(app);
        }

        ExCtxOrNull = exCtx;

        // Ensure the Api knows what path it's on, in case it will
        // create instances of .cs files
        if (context.HttpContext.Items.TryGetValue(SourceCodeConstants.SharedCodeRootPathKeyInCache, out var createInstancePath)
            && _owner is IGetCodePath withCodePath)
            withCodePath.CreateInstancePath = (createInstancePath as string)!;
    }


    private IAppForCode? TryToAttachAppFromUrlParams(ActionExecutingContext context)
    {
        var l = Log.Fn<IAppForCode?>();

        try
        {
            // Handed in from the App-API Transformer
            context.HttpContext.Items.TryGetValue(SxcWebApiConstants.HttpContextKeyForAppFolder, out var routeAppPathObj);
            if (routeAppPathObj == null)
                return l.ReturnNull("nothing to do");
            var routeAppPath = routeAppPathObj.ToString();

            var appId = CtxBuilder.TryFindAppFromPath(routeAppPath)
                            ?.AppReaderRequired.AppId
                        ?? Eav.Sys.EavConstants.NullId;

            if (appId == Eav.Sys.EavConstants.NullId)
                return l.ReturnNull("no app detected");

            // Since we only have the AppId, we need to load the app and attach it to the context
            l.A($"AppId: {appId}");
            var site = CtxService.Context.Site;
            var app = _helper.GetService<Generator<AppForCode, AppForCode.Options>>().New(new(site, new AppIdentityPure(site.ZoneId, appId), new()));
            l.Done("app initialized");
            return l.Return(app, "app initialized");
        }
        catch
        {
            return l.ReturnNull("error, ignore");
        }
    }

    #endregion
}

#endif
