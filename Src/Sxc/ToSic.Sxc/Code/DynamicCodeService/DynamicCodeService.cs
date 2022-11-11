using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
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
    public partial class DynamicCodeService: HasLog, IDynamicCodeService
    {
        #region Constructor and Init

        public class Dependencies
        {
            public Dependencies(IServiceProvider serviceProvider, 
                Lazy<LogHistory> history, 
                Lazy<IUser> user, 
                // Dependencies to get primary app
                Lazy<ISite> site,
                Lazy<IZoneMapper> zoneMapper, 
                Lazy<IAppStates> appStates)
            {
                ServiceProvider = serviceProvider;
                History = history;
                User = user;
                Site = site;
                ZoneMapper = zoneMapper;
                AppStates = appStates;
            }
            internal IServiceProvider ServiceProvider { get; }
            public Lazy<LogHistory> History { get; }
            public Lazy<IUser> User { get; }
            public Lazy<ISite> Site { get; }
            public Lazy<IZoneMapper> ZoneMapper { get; }
            public Lazy<IAppStates> AppStates { get; }
        }

        public DynamicCodeService(Dependencies dependencies): base($"{Constants.SxcLogName}.DCS")
        {
            _dependencies = dependencies;
            ScopedServiceProvider = dependencies.ServiceProvider.CreateScope().ServiceProvider;
            // Important: These generators must be built inside the scope, so they must be made here
            // and not come from the constructor injection
            CodeRootGenerator = ScopedServiceProvider.Build<Generator<DynamicCodeRoot>>();
            AppGenerator = ScopedServiceProvider.Build<Generator<App>>();
            AppConfigDelegateGenerator = ScopedServiceProvider.Build<GeneratorLog<AppConfigDelegate>>().SetLog(Log);
            ModuleAndBlockBuilder = ScopedServiceProvider.Build<LazyInitLog<IModuleAndBlockBuilder>>().SetLog(Log);
        }
        /// <summary>
        /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects
        /// </summary>
        protected IServiceProvider ScopedServiceProvider { get; }
        private readonly Dependencies _dependencies;
        protected readonly Generator<DynamicCodeRoot> CodeRootGenerator;
        protected readonly Generator<App> AppGenerator;
        protected readonly GeneratorLog<AppConfigDelegate> AppConfigDelegateGenerator;
        protected readonly LazyInitLog<IModuleAndBlockBuilder> ModuleAndBlockBuilder;

        public IDynamicCodeService Init(ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _logInitDone = true; // if we link it to a parent, we don't need to add own entry in log history
            return this;
        }
        private bool _logInitDone;

        protected void MakeSureLogIsInHistory()
        {
            if (_logInitDone) return;
            _logInitDone = true;
            _dependencies.History.Value.Add("dynamic-code-service", base.Log);
        }

        protected void ActivateEditUi()
        {
            EditUiRequired = true;
        }

        protected bool EditUiRequired = false;

        #endregion

    }
}
