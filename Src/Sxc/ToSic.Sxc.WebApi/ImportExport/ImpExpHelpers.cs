using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Sxc.LookUp;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ImpExpHelpers: HasLog<ImpExpHelpers>
    {
        private readonly Apps.App _unInitializedApp;
        private readonly AppConfigDelegate _configProvider;

        #region Constructor / DI

        public ImpExpHelpers(Apps.App unInitializedApp, AppConfigDelegate configProvider) : base("Sxc.ImExHl")
        {
            _unInitializedApp = unInitializedApp;
            _configProvider = configProvider.Init(Log);
        }

        #endregion
        /// <summary>
        /// Get an app - but only allow zone change if super-user
        /// </summary>
        /// <returns></returns>
        internal IApp GetAppAndCheckZoneSwitchPermissions(int zoneId, int appId, IUser user, int contextZoneId)
        {
            var wrapLog = Log.Fn<IApp>($"superuser: {user.IsSystemAdmin}");
            if (!user.IsSystemAdmin && zoneId != contextZoneId)
            {
                wrapLog.ReturnNull("error");
                throw Eav.WebApi.Errors.HttpException.PermissionDenied("Tried to access app from another zone. Requires SuperUser permissions.");
            }

            var app = _unInitializedApp.Init(new AppIdentity(zoneId, appId),
                _configProvider.Build(true), Log);
            return wrapLog.Return(app);
        }

    }
}