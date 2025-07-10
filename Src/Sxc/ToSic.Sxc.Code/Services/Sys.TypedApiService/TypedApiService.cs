using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys.AppTyped;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApiService;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services.Sys.CodeApiServiceHelpers;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.Sys.TypedApiService;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class TypedApiService(CodeApiServiceBase.Dependencies services, string? logName = null /* must be nullable for DI */)
    : CodeApiServiceBase(services, logName ?? $"{SxcLogName}.TypeCS"),
        ITypedApiService
{
    #region Constructor and Init

    public class ScopedDependencies(
        Generator<IExecutionContextFactory> codeRootGenerator,
        Generator<App> appGenerator,
        Generator<IAppTyped> appTypedGenerator,
        LazySvc<IModuleAndBlockBuilder> modAndBlockBuilder,
        Generator<ICodeDataFactory> cdfGenerator)
        : DependenciesBase(connect: [codeRootGenerator, appGenerator, modAndBlockBuilder, appTypedGenerator, cdfGenerator])
    {
        public Generator<App> AppGenerator { get; } = appGenerator;
        public Generator<IAppTyped> AppTypedGenerator { get; } = appTypedGenerator;

        public Generator<IExecutionContextFactory> CodeRootGenerator { get; } = codeRootGenerator;
        //public LazySvc<IModuleAndBlockBuilder> ModAndBlockBuilder { get; } = modAndBlockBuilder;
        public Generator<ICodeDataFactory> CdfGenerator { get; } = cdfGenerator;
    }

    /// <summary>
    /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects.
    /// It's important to understand that everything in here will use the scoped service provider.
    /// </summary>
    [field: AllowNull, MaybeNull]
    protected IServiceProvider ScopedServiceProvider => field ??= Services.ServiceProvider.CreateScope().ServiceProvider;

    [field: AllowNull, MaybeNull]
    private ScopedDependencies ServicesScoped => field ??= ScopedServiceProvider.Build<ScopedDependencies>().ConnectServices(Log);


    protected void ActivateEditUi() => EditUiRequired = true;

    protected bool EditUiRequired;

    #endregion

    #region App

    public IAppTyped App(NoParamOrder noParamOrder = default, int? zoneId = null, int? appId = null, ISite? site = null, bool? withUnpublished = null)
    {
        var l = Log.Fn<IAppTyped>();
        MakeSureLogIsInHistory();

        //var codeRoot = ServicesScoped.CodeRootGenerator.New()
        //    .New(parentClassOrNull: null, blockOrNull: null, Log, CompatibilityLevels.CompatibilityLevel16);
        var app = GetApp(ServicesScoped.AppGenerator, zoneId: zoneId, appId: appId, site: site, withUnpublished: withUnpublished);
        var codeRoot = GetNewCodeRoot();
        ((IExCtxAttachApp)codeRoot).AttachApp(app);

        //var appTyped = ServicesScoped.AppTypedGenerator.New();
        //((AppTyped)appTyped).SetupForStandaloneUse((App)app, ServicesScoped.CdfGenerator.New());
        var appTyped = codeRoot.GetState<IAppTyped>();
        return l.ReturnAsOk(appTyped);
    }

    /// <inheritdoc />
    public IAppTyped AppOfSite()
    {
        var l = Log.Fn<IAppTyped>();

        MakeSureLogIsInHistory();
        var app = GetAndInitApp(ServicesScoped.AppGenerator.New(), GetPrimaryAppIdentity(null), null);
        var codeRoot = GetNewCodeRoot();
        ((IExCtxAttachApp)codeRoot).AttachApp(app);
        return l.ReturnAsOk(codeRoot.GetState<IAppTyped>());
        //var appTyped = ServicesScoped.AppTypedGenerator.New();
        //((AppTyped)appTyped).SetupForStandaloneUse((App)app, ServicesScoped.CdfGenerator.New());
        //return l.ReturnAsOk(appTyped);
    }

    #endregion


    #region Of App / Site / Module etc.

    /// <inheritdoc />
    public ITypedApi ApiOfApp(int appId) => OfAppInternal(appId: appId);

    ///// <inheritdoc />
    //public IDynamicCode12 OfApp(int zoneId, int appId) => OfAppInternal(zoneId: zoneId, appId: appId);

    ///// <inheritdoc />
    //public IDynamicCode12 OfApp(IAppIdentity appIdentity) => OfAppInternal(zoneId: appIdentity.ZoneId, appId: appIdentity.AppId);

    ///// <inheritdoc />
    //public IDynamicCode12 OfModule(int pageId, int moduleId)
    //{
    //    var l = Log.Fn<IDynamicCode12>($"{pageId}, {moduleId}");
    //    MakeSureLogIsInHistory();
    //    ActivateEditUi();
    //    var cmsBlock = ServicesScoped.ModAndBlockBuilder.Value.BuildBlock(pageId, moduleId);
    //    var codeRoot = ServicesScoped.CodeRootGenerator.New()
    //        .New(parentClassOrNull: null, cmsBlock, Log, CompatibilityLevels.CompatibilityLevel12);

    //    var code12 = new DynamicCode12Proxy(codeRoot, ((ExecutionContext)codeRoot).DynamicApi);
    //    return l.ReturnAsOk(code12);
    //}

    ///// <inheritdoc />
    //public IDynamicCode12 OfSite() => OfApp(GetPrimaryAppIdentity(null));

    ///// <inheritdoc />
    //public IDynamicCode12 OfSite(int siteId) => OfApp(GetPrimaryAppIdentity(siteId));

    private ITypedApi OfAppInternal(int? zoneId = null, int? appId = null)
    {
        var l = Log.Fn<ITypedApi>();
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var codeRoot = GetNewCodeRoot();
        var app = GetApp(ServicesScoped.AppGenerator, zoneId: zoneId, appId: appId);
        ((IExCtxAttachApp)codeRoot).AttachApp(app);
        var code12 = new TypedApiProxy(codeRoot, ((ExecutionContext)codeRoot).TypedApi);
        return l.ReturnAsOk(code12);
    }

    private IExecutionContext GetNewCodeRoot() =>
        ServicesScoped.CodeRootGenerator.New()
            .New(parentClassOrNull: null, null, Log, CompatibilityLevels.CompatibilityLevel16);

    #endregion

}