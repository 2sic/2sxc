using System;
using System.Runtime.Caching;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.Web.LightSpeed
{
    /// <summary>
    /// Experimental way to signal that all the items in an app cache should be flushed
    /// Idea from here: https://stackoverflow.com/questions/25269338/is-this-a-good-solution-to-clear-a-c-sharp-memorycache
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class AppResetMonitor: ChangeMonitor
    {

        public int AppId => _appState.AppId;
        
        internal AppResetMonitor(AppState appState)
        {
            _appState = appState;
            _appState.AppStateChanged += HandleAppStateChanged;
            // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching.changemonitor?view=dotnet-plat-ext-6.0
            InitializationComplete(); // necessary for ChangeMonitors
        }
        private AppState _appState;

        ~AppResetMonitor() => _appState.AppStateChanged -= HandleAppStateChanged;


        protected override void Dispose(bool disposing)
        {
            _appState.AppStateChanged -= HandleAppStateChanged;
            _appState = null;
            if (disposing) Dispose();
        }

        public override string UniqueId { get; } = Guid.NewGuid().ToString();

        public void HandleAppStateChanged(object sender, EventArgs e)
        {
            if (_removed) return;
            _removed = true;
            // flush a cache and dispose ChangeMonitor
            this.OnChanged(null);
        }

        private bool _removed;
    }
}
