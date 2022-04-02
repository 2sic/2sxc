using System;
using System.Runtime.Caching;

namespace ToSic.Sxc.Beta.LightSpeed
{
    /// <summary>
    /// Experimental way to signal that all the items in an app cache should be flushed
    /// Idea from here: https://stackoverflow.com/questions/25269338/is-this-a-good-solution-to-clear-a-c-sharp-memorycache
    /// </summary>
    internal class AppResetMonitor: ChangeMonitor
    {
        public int AppId { get; }

        public AppResetMonitor(int appId)
        {
            AppId = appId;
            // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching.changemonitor?view=dotnet-plat-ext-6.0
            InitializationComplete(); // necessary for ChangeMonitors
        }


        protected override void Dispose(bool disposing)  { }

        public override string UniqueId { get; } = Guid.NewGuid().ToString();

        public void Flush()
        {
            this.OnChanged(null);
        }
    }
}
