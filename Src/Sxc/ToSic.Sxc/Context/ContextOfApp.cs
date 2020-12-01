using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
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

        public void InitApp(IAppIdentity appIdentity, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AppIdentity = appIdentity;
            _showDrafts = null;
            //LoadDataIfPossible();
        }
        internal IAppIdentity AppIdentity;

        public bool EditAllowed
        {
            get
            {
                if (_showDrafts.HasValue) return _showDrafts.Value;
                var wrapLog = Log.Call<bool>();
                if (AppState == null)
                {
                    Log.Add("App is null. Will return false, but not cache so in future it may change.");
                    return wrapLog("missing", false);
                }
                _showDrafts = ServiceProvider.Build<AppPermissionCheck>()
                    .ForAppInInstance(this, AppState, Log)
                    .UserMay(GrantSets.WriteSomething);
                return wrapLog($"{_showDrafts.Value}", _showDrafts.Value);
            }
        }
        private bool? _showDrafts;

        public AppState AppState => _appState ?? (_appState = AppIdentity == null ? null : State.Get(AppIdentity));
        private AppState _appState;

        public int ZoneId => AppIdentity?.ZoneId ?? throw new Exception("App Identity not set yet");
        public int AppId => AppIdentity?.AppId ?? throw new Exception("App Identity not set yet");


        //public bool DataIsMissing { get; }

        //private void LoadDataIfPossible()
        //{
        //    var wrapLog = Log.Call();
        //    var appId = AppIdentity?.AppId;

        //    wrapLog("ok");
        //}
    }
}
