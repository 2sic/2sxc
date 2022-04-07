//using System.Collections.Concurrent;
//using ToSic.Eav.Apps;

//namespace ToSic.Sxc.Beta.LightSpeed
//{
//    public class AppResetMonitors
//    {
//        public AppResetMonitor GetOrCreate(AppState appState)
//        {
//            // Already has one
//            if (Monitors.TryGetValue(appState.AppId, out var existing))
//                return existing;

//            // Try to create and add
//            var created = new AppResetMonitor(appState);
//            if (Monitors.TryAdd(appState.AppId, created))
//                return created;

//            // Add failed for unknown reasons, maybe it was created in the meantime?
//            if (Monitors.TryGetValue(appState.AppId, out existing))
//                return existing;

//            // Still didn't work - unsure why, return the new one anyhow; better than to stop the application
//            return created;
//        }

//        //private void Remove(int appId)
//        //{
//        //    Monitors.TryRemove(appId, out var existing);
//        //}

//        public ConcurrentDictionary<int, AppResetMonitor> Monitors { get; set; } = new ConcurrentDictionary<int, AppResetMonitor>();
//    }
//}
