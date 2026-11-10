using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Services.Sys.DynamicCodeService;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract partial class DynamicCodeService(DynamicCodeService.Dependencies services, string? logName = null /* must be nullable for DI */)
    : ServiceBase<DynamicCodeService.Dependencies>(services, logName ?? $"Dnn.DCS"),
#pragma warning disable CS0618 // Type or member is obsolete
        IDynamicCodeService,
        ILogWasConnected
#pragma warning restore CS0618 // Type or member is obsolete
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
        Generator<AppForCodeShared, AppForCodeShared.Options> AppGenerator,
        Generator<IAppDynamic, IAppForCode> AppDynGenerator,
        LazySvc<IModuleAndBlockBuilder> ModAndBlockBuilder)
        : DependenciesBase(connect: [ExCtxGenerator, AppGenerator, AppDynGenerator, ModAndBlockBuilder]);


    #endregion

    #region Get App

    /// <summary>
    /// Get proper app Identity from nullable AppId and Zone; AppId is actually required.
    /// </summary>
    /// <param name="zoneId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private AppIdentityPure ToAppIdentityOrThrow(int? zoneId, int? appId)
    {
        // Ensure AppId is provided
        var realAppId = appId ?? throw new ArgumentException($@"At least the {nameof(appId)} is required and must be a valid AppId", nameof(appId));

        // lookup zoneId if not provided
        var realZoneId = zoneId ?? Services.AppsCatalog.Value.AppIdentity(realAppId).ZoneId;
        var appIdentity = new AppIdentityPure(realZoneId, realAppId);
        return appIdentity;
    }


    private IAppIdentityPure GetPrimaryAppIdentity(int? siteId, ISite? site = default)
    {
        siteId ??= site?.Id ?? Services.Site.Id;
        var zoneId = Services.ZoneMapper.Value.GetZoneId(siteId.Value);
        var primaryApp = Services.AppsCatalog.Value.PrimaryAppIdentity(zoneId);
        return primaryApp;
    }

    private IAppDynamic GetAndInitApp(IAppIdentityPure appIdentity, ISite? overrideSite, bool? showDrafts = null)
    {
        var appGen = ServicesScoped.AppGenerator;

        var l = Log.Fn<IAppDynamic>($"{appIdentity.Show()}, site:{overrideSite != null}, showDrafts: {showDrafts}");
        var app = appGen.New(new(overrideSite, appIdentity, new() { ShowDrafts = showDrafts }));
        var dynApp = ServicesScoped.AppDynGenerator.New(app);
        return l.Return(dynApp);
    }

    public void LogWasConnected() => _logInitDone = true; // if we link it to a parent, we don't need to add own entry in log history
    private bool _logInitDone;

    protected void MakeSureLogIsInHistory()
    {
        if (_logInitDone) return;
        _logInitDone = true;
        Services.LogStore.Value.Add("code-api-service", Log);
    }

    #endregion

    #region Constructor and Init

    /// <summary>
    /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects.
    /// It's important to understand that everything in here will use the scoped service provider.
    /// </summary>
    //[field: AllowNull, MaybeNull]
    protected IServiceProvider ScopedServiceProvider => field
        ??= Services.ServiceProvider.CreateScope().ServiceProvider;

    //[field: AllowNull, MaybeNull]
    private ScopedDependencies ServicesScoped => field
        ??= ScopedServiceProvider.Build<ScopedDependencies>().ConnectServices(Log);

    // #ActivateEditUi seems unused, it's never read; keep till ca. mid 2027
    //private void ActivateEditUi() => EditUiRequired = true;
    //private bool EditUiRequired;

    #endregion

    #region App

    /// <inheritdoc />
    public IAppDynamic App(NoParamOrder npo = default, int? zoneId = null, int? appId = null, ISite? site = null, bool? withUnpublished = null)
    {
        MakeSureLogIsInHistory();
        return GetAndInitApp(ToAppIdentityOrThrow(zoneId, appId), site, showDrafts: withUnpublished);
    }

    /// <inheritdoc />
    public IAppDynamic AppOfSite()
        => GetAndInitApp(GetPrimaryAppIdentity(null), null);

    /// <inheritdoc />
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public IAppDynamic AppOfSite(NoParamOrder npo = default, int? siteId = null, ISite? overrideSite = null, bool? withUnpublished = null)
        => GetAndInitApp(GetPrimaryAppIdentity(siteId, overrideSite), overrideSite, withUnpublished);

    #endregion
}