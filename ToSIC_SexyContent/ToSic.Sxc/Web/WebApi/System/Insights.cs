using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Ins: HasLog
    {
        public Ins(ILog parentLog, Action throwIfNotSuperUser, Func<string, Exception> createBadRequest) : base("Api.SysIns", parentLog)
        {
            ThrowIfNotSuperUser = throwIfNotSuperUser;
            CreateBadRequest = createBadRequest;
        }

        private readonly Action ThrowIfNotSuperUser;
        private readonly Func<string, Exception> CreateBadRequest;


        private AppRuntime AppRt(int? appId) => new AppRuntime(appId.Value, true, Log);

        private AppState AppState(int? appId) => State.Get(appId.Value);


    }
}
