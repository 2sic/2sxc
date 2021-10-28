using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Caching;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Types;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights: HasLog
    {

        #region Constructor / DI

        public Insights(IServiceProvider serviceProvider, IAppStates appStates, SystemManager systemManager, IAppsCache appsCache, LogHistory logHistory, 
            GlobalTypes globalTypes)
            : base("Api.SysIns")
        {
            _serviceProvider = serviceProvider;
            _appStates = appStates;
            _appsCache = appsCache;
            _logHistory = logHistory;
            _globalTypes = globalTypes;
            SystemManager = systemManager.Init(Log);
        }
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppStates _appStates;
        private readonly IAppsCache _appsCache;
        private readonly LogHistory _logHistory;
        private readonly GlobalTypes _globalTypes;
        protected readonly SystemManager SystemManager;


        public Insights Init(ILog parentLog, Action throwIfNotSuperUser, Func<string, Exception> createBadRequest) 
        {
            Log.LinkTo(parentLog);
            ThrowIfNotSuperUser = throwIfNotSuperUser;
            CreateBadRequest = createBadRequest;
            return this;
        }

        private Action ThrowIfNotSuperUser;
        private Func<string, Exception> CreateBadRequest;

        #endregion

        private AppRuntime AppRt(int? appId) => _serviceProvider.Build<AppRuntime>().Init(appId.Value, true, Log);

        private AppState AppState(int? appId) => _appStates.Get(appId.Value);


    }
}
