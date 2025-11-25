using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Services.Sys.CodeApiServiceHelpers;
public abstract class CodeApiServiceBase(CodeApiServiceBase.Dependencies services, string logName)
    : ServiceBase<CodeApiServiceBase.Dependencies>(services, logName),
        ILogWasConnected
{
    public class Dependencies(
        IServiceProvider serviceProvider,
        LazySvc<ILogStore> logStore,
        LazySvc<IUser> user,
        // Dependencies to get primary app
        LazySvc<ISite> site,
        LazySvc<IZoneMapper> zoneMapper,
        LazySvc<IAppsCatalog> appsCatalog)
        : DependenciesBase(connect: [/* never! serviceProvider */ logStore, user, site, zoneMapper, appsCatalog])
    {
        internal IServiceProvider ServiceProvider { get; } = serviceProvider;
        public LazySvc<ILogStore> LogStore { get; } = logStore;
        public LazySvc<IUser> User { get; } = user;
        public LazySvc<ISite> Site { get; } = site;
        public LazySvc<IZoneMapper> ZoneMapper { get; } = zoneMapper;
        public LazySvc<IAppsCatalog> AppsCatalog { get; } = appsCatalog;
    }

    public class ScopedDependencies(
        Generator<IExecutionContextFactory> codeRootGenerator,
        Generator<App> appGenerator,
        LazySvc<IModuleAndBlockBuilder> modAndBlockBuilder)
        : DependenciesBase(connect: [codeRootGenerator, appGenerator, modAndBlockBuilder])
    {
        public Generator<App> AppGenerator { get; } = appGenerator;
        public Generator<IExecutionContextFactory> CodeRootGenerator { get; } = codeRootGenerator;
        public LazySvc<IModuleAndBlockBuilder> ModAndBlockBuilder { get; } = modAndBlockBuilder;
    }

    protected IApp GetApp(Generator<App> appGenerator, NoParamOrder npo = default, int? zoneId = null, int? appId = null, ISite? site = null, bool? withUnpublished = null)
    {
        MakeSureLogIsInHistory();

        // Ensure AppId is provided
        var realAppId = appId ?? throw new ArgumentException($@"At least the {nameof(appId)} is required and must be a valid AppId", nameof(appId));

        // lookup zoneId if not provided
        var realZoneId = zoneId ?? Services.AppsCatalog.Value.AppIdentity(realAppId).ZoneId;
        return GetAndInitApp(appGenerator.New(), new AppIdentityPure(realZoneId, realAppId), site, showDrafts: withUnpublished);
    }


    protected IAppIdentityPure GetPrimaryAppIdentity(int? siteId, ISite? site = default)
    {
        siteId ??= site?.Id ?? Services.Site.Value.Id;
        var zoneId = Services.ZoneMapper.Value.GetZoneId(siteId.Value);
        var primaryApp = Services.AppsCatalog.Value.PrimaryAppIdentity(zoneId);
        return primaryApp;
    }

    protected IApp GetAndInitApp(App app, IAppIdentityPure appIdentity, ISite? overrideSite, bool? showDrafts = null)
    {
        var l = Log.Fn<IApp>($"{appIdentity.Show()}, site:{overrideSite != null}, showDrafts: {showDrafts}");
        app.Init(overrideSite, appIdentity, new() { ShowDrafts = showDrafts });
        return l.Return(app);
    }


    public void LogWasConnected() => _logInitDone = true; // if we link it to a parent, we don't need to add own entry in log history
    private bool _logInitDone;

    protected void MakeSureLogIsInHistory()
    {
        if (_logInitDone) return;
        _logInitDone = true;
        Services.LogStore.Value.Add("dynamic-code-service", Log);
    }

}
