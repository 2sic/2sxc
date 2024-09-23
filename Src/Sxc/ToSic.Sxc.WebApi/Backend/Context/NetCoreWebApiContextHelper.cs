#if NETCOREAPP
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Code;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Lib.Coding;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Internal;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Backend.Context;
internal class NetCoreWebApiContextHelper: CodeHelperBase
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

    /// <summary>
    /// This will make sure that any services requiring the context can get it.
    /// It must usually be called from the base class which expects to use this.
    /// </summary>
    public void InitializeBlockContext(ActionExecutingContext context)
    {
        if (_blockContextInitialized) return;
        _blockContextInitialized = true;
        var getBlock = _helper.GetService<IWebApiContextBuilder>();
        CtxResolver = getBlock.PrepareContextResolverForApiRequest();
        BlockOptional = CtxResolver.BlockOrNull();
    }

    private bool _blockContextInitialized;

    private ISxcContextResolver CtxResolver { get; set; }
    internal IBlock BlockOptional { get; private set; }

    public void OnActionExecutingEnd(ActionExecutingContext context)
    {
        // base.OnActionExecuting(context);
        InitializeBlockContext(context);

        // Use the ServiceProvider of the current request to build DynamicCodeRoot
        // Note that BlockOptional was already retrieved in the base class
        var codeRoot = context.HttpContext.RequestServices
            .Build<CodeApiServiceFactory>()
            .BuildCodeRoot(_owner, BlockOptional, Log, compatibilityFallback: CompatibilityLevels.CompatibilityLevel12);
        ConnectToRoot(codeRoot);

        AdamCode = codeRoot.GetService<AdamCode>();

        // In case SxcBlock was null, there is no instance, but we may still need the app
        if (_CodeApiSvc.App == null)
        {
            Log.A("DynCode.App is null");
            TryToAttachAppFromUrlParams(context);
        }

        // Ensure the Api knows what path it's on, in case it will
        // create instances of .cs files
        if (context.HttpContext.Items.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var createInstancePath))
            if (_owner is IGetCodePath withCodePath)
                withCodePath.CreateInstancePath = createInstancePath as string;
    }


    private void TryToAttachAppFromUrlParams(ActionExecutingContext context) => base.Log.Do(() =>
    {
        var found = false;
        try
        {
            // Handed in from the App-API Transformer
            context.HttpContext.Items.TryGetValue(SxcWebApiConstants.HttpContextKeyForAppFolder,
                out var routeAppPathObj);
            if (routeAppPathObj == null) return "";
            var routeAppPath = routeAppPathObj.ToString();

            var appId = CtxResolver.SetAppOrNull(routeAppPath)?.AppReader.AppId ?? Eav.Constants.NullId;

            if (appId != Eav.Constants.NullId)
            {
                // Look up if page publishing is enabled - if module context is not available, always false
                base.Log.A($"AppId: {appId}");
                var app = LoadAppOnly(appId, CtxResolver.Site().Site);
                ((ICodeApiServiceInternal)_CodeApiSvc).AttachApp(app);
                found = true;
            }
        }
        catch
        {
            /* ignore */
        }

        return found.ToString();
    });

    /// <summary>
    /// Only load the app in case we don't have a module context
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="site"></param>
    /// <returns></returns>
    private IApp LoadAppOnly(int appId, ISite site)
    {
        var l = Log.Fn<IApp>($"{appId}");
        var app = _helper.GetService<Apps.App>();
        app.Init(site, new AppIdentityPure(site.ZoneId, appId), new());
        return l.Return(app);
    }


    #endregion

    #region Context Maker

    public void SetupResponseMaker() => _helper.GetService<IResponseMaker>().Init(_owner);

    #endregion

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
#endif