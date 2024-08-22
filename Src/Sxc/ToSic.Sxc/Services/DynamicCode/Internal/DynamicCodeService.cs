using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Integration;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Services.Internal;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class DynamicCodeService: ServiceBase<DynamicCodeService.MyServices>, IDynamicCodeService, ILogWasConnected
{
    #region Constructor and Init

    public class MyServices(
        IServiceProvider serviceProvider,
        LazySvc<ILogStore> logStore,
        LazySvc<IUser> user,
        // Dependencies to get primary app
        LazySvc<ISite> site,
        LazySvc<IZoneMapper> zoneMapper,
        LazySvc<IAppsCatalog> appsCatalog)
        : MyServicesBase(connect: [/* never! serviceProvider */ logStore, user, site, zoneMapper, appsCatalog])
    {
        internal IServiceProvider ServiceProvider { get; } = serviceProvider;
        public LazySvc<ILogStore> LogStore { get; } = logStore;
        public LazySvc<IUser> User { get; } = user;
        public LazySvc<ISite> Site { get; } = site;
        public LazySvc<IZoneMapper> ZoneMapper { get; } = zoneMapper;
        public LazySvc<IAppsCatalog> AppsCatalog { get; } = appsCatalog;
    }

    public class MyScopedServices(
        Generator<CodeApiServiceFactory> codeRootGenerator,
        Generator<App> appGenerator,
        LazySvc<IModuleAndBlockBuilder> modAndBlockBuilder)
        : MyServicesBase(connect: [codeRootGenerator, appGenerator, modAndBlockBuilder])
    {
        public Generator<App> AppGenerator { get; } = appGenerator;
        public Generator<CodeApiServiceFactory> CodeRootGenerator { get; } = codeRootGenerator;
        public LazySvc<IModuleAndBlockBuilder> ModAndBlockBuilder { get; } = modAndBlockBuilder;
    }

    public DynamicCodeService(MyServices services): this(services, $"{SxcLogName}.DCS") { }
    protected DynamicCodeService(MyServices services, string logName): base(services, logName)
    {
        ScopedServiceProvider = services.ServiceProvider.CreateScope().ServiceProvider;
        // Important: These generators must be built inside the scope, so they must be made here
        // and NOT come from the constructor injection
        // TODO: @2DM - put log in Build call?
        _myScopedServices = ScopedServiceProvider.Build<MyScopedServices>().ConnectServices(Log);
    }

    /// <summary>
    /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects
    /// </summary>
    protected IServiceProvider ScopedServiceProvider { get; }
    private readonly MyScopedServices _myScopedServices;

    public void LogWasConnected() => _logInitDone = true; // if we link it to a parent, we don't need to add own entry in log history
    private bool _logInitDone;

    protected void MakeSureLogIsInHistory()
    {
        if (_logInitDone) return;
        _logInitDone = true;
        Services.LogStore.Value.Add("dynamic-code-service", Log);
    }

    protected void ActivateEditUi() => EditUiRequired = true;

    protected bool EditUiRequired;

    #endregion

}