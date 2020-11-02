using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights: HasLog
    {
        private readonly IServiceProvider _serviceProvider;

        #region Constructor / DI

        public Insights(IServiceProvider serviceProvider): base("Api.SysIns")
        {
            _serviceProvider = serviceProvider;
        }


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

        private AppRuntime AppRt(int? appId) => _serviceProvider.GetRequiredService<AppRuntime>().Init(State.Identity(null, appId.Value), true, Log);

        private AppState AppState(int? appId) => State.Get(appId.Value);


    }
}
