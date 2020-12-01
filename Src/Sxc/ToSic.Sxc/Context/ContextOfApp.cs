using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.Context
{
    public class ContextOfApp: ContextOfSite, IContextOfApp
    {
        public ContextOfApp(IServiceProvider serviceProvider, ISite site, IUser user) : base(serviceProvider, site, user)
        {
            Log.Rename("Sxc.CtxApp");
        }

        public void ResetApp(IAppIdentity appIdentity)
        {
            if (AppIdentity == null || AppIdentity.AppId != appIdentity.AppId) 
                AppIdentity = appIdentity;

        }

        protected virtual IAppIdentity AppIdentity
        {
            get => _appIdentity;
            set
            {
                _appIdentity = value;
                _appState = null;
                _userMayEdit = null;
            }
        }

        private IAppIdentity _appIdentity;

        public override bool UserMayEdit
        {
            get
            {
                if (_userMayEdit.HasValue) return _userMayEdit.Value;
                var wrapLog = Log.Call<bool>();
                if (AppState == null)
                {
                    Log.Add("App is null. Will return false, but not cache so in future it may change.");
                    return wrapLog("missing", false);
                }
                _userMayEdit = ServiceProvider.Build<AppPermissionCheck>()
                    .ForAppInInstance(this, AppState, Log)
                    .UserMay(GrantSets.WriteSomething);
                return wrapLog($"{_userMayEdit.Value}", _userMayEdit.Value);
            }
        }
        private bool? _userMayEdit;

        public AppState AppState => _appState ?? (_appState = AppIdentity == null ? null : State.Get(AppIdentity));
        private AppState _appState;

    }
}
