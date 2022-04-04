using System.Collections.Concurrent;

namespace ToSic.Sxc.Beta.LightSpeed
{
    public class AppResetMonitors
    {
        public AppResetMonitor GetOrCreate(int appId)
        {
            // Already has one
            if (Monitors.TryGetValue(appId, out var existing))
                return existing;

            // Try to create and add
            var created = new AppResetMonitor(appId);
            if (Monitors.TryAdd(appId, created))
                return created;

            // Add failed for unknown reasons, maybe it was created in the meantime?
            if (Monitors.TryGetValue(appId, out existing))
                return existing;

            // Still didn't work - unsure why, return the new one anyhow; better than to stop the application
            return created;
        }

        public ConcurrentDictionary<int, AppResetMonitor> Monitors { get; set; } = new ConcurrentDictionary<int, AppResetMonitor>();
    }
}
