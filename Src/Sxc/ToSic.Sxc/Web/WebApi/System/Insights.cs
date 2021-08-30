using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights: HasLog
    {

        #region Constructor / DI

        public Insights(IServiceProvider serviceProvider, IAppStates appStates, SystemManager systemManager) : base("Api.SysIns")
        {
            _serviceProvider = serviceProvider;
            _appStates = appStates;
            SystemManager = systemManager.Init(Log);
        }
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppStates _appStates;
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
