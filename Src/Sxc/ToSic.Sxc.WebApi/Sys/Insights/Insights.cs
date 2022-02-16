using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Caching;
using ToSic.Eav.Configuration.Licenses;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.WebApi.Sys
{
    public partial class Insights: HasLog
    {

        #region Constructor / DI

        public Insights(IServiceProvider serviceProvider, IAppStates appStates, SystemManager systemManager, IAppsCache appsCache, LogHistory logHistory, Lazy<ILicenseService> licenseServiceLazy, IUser user)
            : base("Api.SysIns")
        {
            _serviceProvider = serviceProvider;
            _appStates = appStates;
            _appsCache = appsCache;
            _logHistory = logHistory;
            _licenseServiceLazy = licenseServiceLazy;
            _user = user;
            SystemManager = systemManager.Init(Log);
        }
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppStates _appStates;
        private readonly IAppsCache _appsCache;
        private readonly LogHistory _logHistory;
        private readonly Lazy<ILicenseService> _licenseServiceLazy;
        private readonly IUser _user;
        protected readonly SystemManager SystemManager;


        public Insights Init(ILog parentLog) 
        {
            Log.LinkTo(parentLog);
            return this;
        }

        //private Action ThrowIfNotSuperUser;
        private Exception CreateBadRequest(string msg) => HttpException.BadRequest(msg);

        private void ThrowIfNotSuperUser()
        {
            if(!_user.IsSuperUser) throw HttpException.PermissionDenied("requires Superuser permissions");
        }

        #endregion

        private AppRuntime AppRt(int? appId) => _serviceProvider.Build<AppRuntime>().Init(appId.Value, true, Log);

        private AppState AppState(int? appId) => _appStates.Get(appId.Value);


    }
}
