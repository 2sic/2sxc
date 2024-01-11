using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Integration;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Internal;
using ToSic.Sxc.LookUp;
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

    public class MyServices: MyServicesBase
    {
        public MyServices(
            IServiceProvider serviceProvider,
            LazySvc<ILogStore> logStore,
            LazySvc<IUser> user,
            // Dependencies to get primary app
            LazySvc<ISite> site,
            LazySvc<IZoneMapper> zoneMapper,
            LazySvc<IAppStates> appStates
        ) => ConnectServices(
            ServiceProvider = serviceProvider,
            LogStore = logStore,
            User = user,
            Site = site,
            ZoneMapper = zoneMapper,
            AppStates = appStates
        );

        internal IServiceProvider ServiceProvider { get; }
        public LazySvc<ILogStore> LogStore { get; }
        public LazySvc<IUser> User { get; }
        public LazySvc<ISite> Site { get; }
        public LazySvc<IZoneMapper> ZoneMapper { get; }
        public LazySvc<IAppStates> AppStates { get; }
    }

    public class MyScopedServices: MyServicesBase
    {
        public Generator<App> AppGenerator { get; }
        public Generator<CodeApiServiceFactory> CodeRootGenerator { get; }
        public Generator<AppConfigDelegate> AppConfigDelegateGenerator { get; }
        public LazySvc<IModuleAndBlockBuilder> ModAndBlockBuilder { get; }

        public MyScopedServices(
            Generator<CodeApiServiceFactory> codeRootGenerator,
            Generator<App> appGenerator,
            Generator<AppConfigDelegate> appConfigDelegateGenerator,
            LazySvc<IModuleAndBlockBuilder> modAndBlockBuilder)
        {
            ConnectServices(
                CodeRootGenerator = codeRootGenerator,
                AppGenerator = appGenerator,
                AppConfigDelegateGenerator = appConfigDelegateGenerator,
                ModAndBlockBuilder = modAndBlockBuilder
            );
        }
    }

    public DynamicCodeService(MyServices services): this(services, $"{SxcLogging.SxcLogName}.DCS") { }
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