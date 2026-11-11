using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Context;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Services;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class TypedApiService(IServiceProvider serviceProvider, LazySvc<ILogStore> logStore, ISite site, LazySvc<IZoneFinder> zoneFinder, LazySvc<IAppsCatalog> appsCatalog)
    : ServiceBase($"{SxcLogName}.TypeCS", connect: [/* never! serviceProvider */ logStore, zoneFinder, appsCatalog]),
        ITypedApiService,
        ILogWasConnected
{
    #region Dependencies

    public record Dependencies(
        IServiceProvider ServiceProvider,
        LazySvc<ILogStore> LogStore,
        IUser User,
        ISite Site,
        LazySvc<IZoneMapper> ZoneMapper,
        LazySvc<IAppsCatalog> AppsCatalog)
        : DependenciesBase(connect: [/* never! serviceProvider */ LogStore, ZoneMapper, AppsCatalog]);


    public record ScopedDependencies(
        Generator<IExecutionContextFactory> ExCtxGenerator,
        Generator<AppForCode, AppForCode.Options> AppGenerator,
        LazySvc<IBlockBuilder> ModAndBlockBuilder,
        LazySvc<IEnvModuleBuilder> EnvModAndBlockBuilder)
        : DependenciesBase(connect: [ExCtxGenerator, AppGenerator, ModAndBlockBuilder, EnvModAndBlockBuilder]);

    #endregion

    #region Get App

    private IAppForCode GetApp(NoParamOrder npo = default, int? zoneId = null, int? appId = null, ISite? site = null, bool? withUnpublished = null)
    {
        MakeSureLogIsInHistory();

        // Ensure AppId is provided
        var realAppId = appId ?? throw new ArgumentException($@"At least the {nameof(appId)} is required and must be a valid AppId", nameof(appId));

        // lookup zoneId if not provided
        var realZoneId = zoneId ?? appsCatalog.Value.AppIdentity(realAppId).ZoneId;
        return GetAndInitApp(new AppIdentityPure(realZoneId, realAppId), site, showDrafts: withUnpublished);
    }


    private IAppIdentityPure GetPrimaryAppIdentity(int? siteId, ISite? preferredSite = default)
    {
        siteId ??= preferredSite?.Id ?? site.Id;
        var zoneId = zoneFinder.Value.GetZoneId(siteId.Value);
        var primaryApp = appsCatalog.Value.PrimaryAppIdentity(zoneId);
        return primaryApp;
    }

    private IAppForCode GetAndInitApp(IAppIdentityPure appIdentity, ISite? overrideSite, bool? showDrafts = null)
    {
        var appGen = ServicesScoped.AppGenerator;
        var l = Log.Fn<IAppForCode>($"{appIdentity.Show()}, site:{overrideSite != null}, showDrafts: {showDrafts}");
        var app = appGen.New(new(overrideSite, appIdentity, new() { ShowDrafts = showDrafts }));
        return l.Return(app);
    }




    #endregion

    /// <summary>
    /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects.
    /// It's important to understand that everything in here will use the scoped service provider.
    /// </summary>
    [field: AllowNull, MaybeNull]
    protected IServiceProvider ScopedServiceProvider => field
        ??= serviceProvider.CreateScope().ServiceProvider;

    [field: AllowNull, MaybeNull]
    private ScopedDependencies ServicesScoped => field
        ??= ScopedServiceProvider.Build<ScopedDependencies>().ConnectServices(Log);


    protected void ActivateEditUi() => EditUiRequired = true;

    protected bool EditUiRequired;

    #region App

    public IAppTyped App(NoParamOrder npo = default, int? zoneId = null, int? appId = null, ISite? site = null, bool? withUnpublished = null)
    {
        var l = Log.Fn<IAppTyped>();
        MakeSureLogIsInHistory();

        var app = GetApp(zoneId: zoneId, appId: appId, site: site, withUnpublished: withUnpublished);
        return l.ReturnAsOk(GetNewCodeRoot(app).GetTypedApi().AppTyped);
    }

    /// <inheritdoc />
    public IAppTyped AppOfSite()
    {
        var l = Log.Fn<IAppTyped>();

        MakeSureLogIsInHistory();
        var app = GetAndInitApp(GetPrimaryAppIdentity(null), null);
        return l.ReturnAsOk(GetNewCodeRoot(app).GetTypedApi().AppTyped);
    }

    #endregion


    #region Of App / Site / Module etc.

    /// <inheritdoc />
    public ITypedApi ApiOfApp(int appId) => OfAppOrSiteInternal(appId: appId);

    /// <inheritdoc />
    public ITypedApi ApiOfApp(int zoneId, int appId) => OfAppOrSiteInternal(zoneId: zoneId, appId: appId);

    /// <inheritdoc />
    public ITypedApi ApiOfModule(int pageId, int moduleId)
    {
        var l = Log.Fn<ITypedApi>($"{pageId}, {moduleId}");
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var cmsBlock = ServicesScoped.ModAndBlockBuilder.Value.BuildBlock(pageId, moduleId);

        var exCtx = ServicesScoped.ExCtxGenerator.New().New(new()
        {
            OwnerOrNull = null,
            BlockOrNull = cmsBlock,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel16,
        });

        var code12 = new TypedApiStandalone(exCtx, exCtx.GetTypedApi());
        return l.ReturnAsOk(code12);
    }

    /// <inheritdoc />
    public ITypedApi ApiOfSite() =>
        ApiOfAppOrSite(GetPrimaryAppIdentity(null));

    /// <inheritdoc />
    public ITypedApi ApiOfSite(int siteId) =>
        ApiOfAppOrSite(GetPrimaryAppIdentity(siteId));


    public ITypedApi ApiOfSite(int siteId, int pageId, int moduleId)
    {
        var moduleFallback = ServicesScoped.EnvModAndBlockBuilder.Value.GetModule(pageId, moduleId);
        var appIdentity = GetPrimaryAppIdentity(siteId);
        return OfAppOrSiteInternal(zoneId: appIdentity.ZoneId, appId: appIdentity.AppId, moduleFallback);
    }

    private ITypedApi ApiOfAppOrSite(IAppIdentity appIdentity) =>
        OfAppOrSiteInternal(zoneId: appIdentity.ZoneId, appId: appIdentity.AppId);


    private ITypedApi OfAppOrSiteInternal(int? zoneId = null, int? appId = null, IModule? moduleIfBlockUnknown = default)
    {
        var l = Log.Fn<ITypedApi>();
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var app = GetApp(zoneId: zoneId, appId: appId);
        var exCtx = GetNewCodeRoot(app, moduleIfBlockUnknown);
        var code12 = new TypedApiStandalone(exCtx, exCtx.GetTypedApi());
        return l.ReturnAsOk(code12);
    }

    private IExecutionContext GetNewCodeRoot(IAppForCode? appToAttach = default, IModule? moduleIfBlockUnknown = default)
    {
        var exCtx = ServicesScoped.ExCtxGenerator.New().New(new()
        {
            OwnerOrNull = null,
            BlockOrNull = null,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel16,
            ModuleIfBlockUnknown = moduleIfBlockUnknown,
        });
        if (appToAttach != null)
            ((IExCtxAttachApp)exCtx).AttachApp(appToAttach);
        return exCtx;
    }

    #endregion


    public void LogWasConnected() => _logInitDone = true; // if we link it to a parent, we don't need to add own entry in log history
    private bool _logInitDone;

    private void MakeSureLogIsInHistory()
    {
        if (_logInitDone) return;
        _logInitDone = true;
        logStore.Value.Add("code-api-service", Log);
    }

}