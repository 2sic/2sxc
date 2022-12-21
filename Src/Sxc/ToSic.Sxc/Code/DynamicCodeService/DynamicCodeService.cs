using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Services;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
    /// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
    /// </summary>
    public partial class DynamicCodeService: HasLog, IDynamicCodeService, ILogWasConnected
    {
        #region Constructor and Init

        public class Dependencies: ServiceDependencies
        {
            public Dependencies(
                IServiceProvider serviceProvider,
                Lazy<ILogStore> logStore,
                Lazy<IUser> user,
                // Dependencies to get primary app
                Lazy<ISite> site,
                Lazy<IZoneMapper> zoneMapper,
                Lazy<IAppStates> appStates
            ) => AddToLogQueue(
                ServiceProvider = serviceProvider,
                LogStore = logStore,
                User = user,
                Site = site,
                ZoneMapper = zoneMapper,
                AppStates = appStates
            );

            internal IServiceProvider ServiceProvider { get; }
            public Lazy<ILogStore> LogStore { get; }
            public Lazy<IUser> User { get; }
            public Lazy<ISite> Site { get; }
            public Lazy<IZoneMapper> ZoneMapper { get; }
            public Lazy<IAppStates> AppStates { get; }
        }

        public class ScopedDependencies: ServiceDependencies
        {
            public Generator<App> AppGenerator { get; }
            public Generator<DynamicCodeRoot> CodeRootGenerator { get; }
            public Generator<AppConfigDelegate> AppConfigDelegateGenerator { get; }
            public LazyInitLog<IModuleAndBlockBuilder> ModAndBlockBuilder { get; }

            public ScopedDependencies(
                Generator<DynamicCodeRoot> codeRootGenerator,
                Generator<App> appGenerator,
                Generator<AppConfigDelegate> appConfigDelegateGenerator,
                LazyInitLog<IModuleAndBlockBuilder> modAndBlockBuilder
            ) => AddToLogQueue(
                CodeRootGenerator = codeRootGenerator,
                AppGenerator = appGenerator,
                AppConfigDelegateGenerator = appConfigDelegateGenerator,
                ModAndBlockBuilder = modAndBlockBuilder
            );
        }

        public DynamicCodeService(Dependencies dependencies): base($"{Constants.SxcLogName}.DCS")
        {
            _dependencies = dependencies.SetLog(Log);
            ScopedServiceProvider = dependencies.ServiceProvider.CreateScope().ServiceProvider;
            // Important: These generators must be built inside the scope, so they must be made here
            // and NOT come from the constructor injection
            _scopedDeps = ScopedServiceProvider.Build<ScopedDependencies>().SetLog(Log);
        }
        /// <summary>
        /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects
        /// </summary>
        protected IServiceProvider ScopedServiceProvider { get; }
        private readonly Dependencies _dependencies;
        private readonly ScopedDependencies _scopedDeps;

        public void LogWasConnected() => _logInitDone = true; // if we link it to a parent, we don't need to add own entry in log history
        private bool _logInitDone;

        protected void MakeSureLogIsInHistory()
        {
            if (_logInitDone) return;
            _logInitDone = true;
            _dependencies.LogStore.Value.Add("dynamic-code-service", Log);
        }

        protected void ActivateEditUi() => EditUiRequired = true;

        protected bool EditUiRequired;

        #endregion

    }
}
