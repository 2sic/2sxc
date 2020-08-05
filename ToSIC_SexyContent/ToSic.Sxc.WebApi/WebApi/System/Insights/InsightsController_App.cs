using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController 
    {

        [HttpGet]
        public string LoadLog(int? appId = null)
        {
            ThrowIfNotSuperuser();

            if (UrlParamsIncomplete(appId, out var message))
                return message;

            Log.Add($"debug app-load {appId}");
            return FormatLog($"2sxc load log for app {appId}", AppRt(appId).AppState.Log);
        }

        [HttpGet]
        public string Cache()
        {
            ThrowIfNotSuperuser();

            var msg = h1("Apps In Cache");
            var cache = State.Cache;

            var zones = cache.Zones.OrderBy(z => z.Key);

            msg += "<table id='table'><thead>"
                + tr(new []{"Zone", "App", "Guid", "InCache", "Details", "Actions"}, true)
                + "</thead>"
                + "<tbody>";
            foreach (var zone in zones)
            {
                var apps = zone.Value.Apps
                    .Select(a => new { Id = a.Key, Guid = a.Value, InCache = cache.Has(new AppIdentity(zone.Value.ZoneId, a.Key))})
                    .OrderBy(a => a.Id);
                foreach (var app in apps)
                {
                    msg += tr(new[]
                    {
                        zone.Key.ToString(),
                        app.Id.ToString(),
                        $"{app.Guid}",
                        app.InCache ? "yes" : "no",
                        $"{a("stats", $"stats?appid={app.Id}")} | {a("load log", $"loadlog?appid={app.Id}")} | {a("types", $"types?appid={app.Id}")}",
                        summary("show actions",
                            $"{a("purge", $"purge?appid={app.Id}")}"
                        )
                    });
                }
            }
            msg += "</tbody>"
                   + "</table>"
                   + JsTableSort();
            return msg;
        }

        [HttpGet]
        public string Stats(int? appId = null)
        {
            ThrowIfNotSuperuser();

            if (UrlParamsIncomplete(appId, out var message))
                return message;

            Log.Add($"debug app-internals for {appId}");
            var appRead = AppRt(appId);// new AppRuntime(appId.Value, true, Log);
            var pkg = appRead.AppState;

            var msg = h1($"App internals for {appId}");
            try
            {
                Log.Add("general stats");
                msg += p(
                    ToBr($"AppId: {pkg.AppId}\n"
                         + $"Timestamp: {pkg.CacheTimestamp}\n"
                         + $"Update Count: {pkg.CacheUpdateCount}\n"
                         + $"Dyn Update Count: {pkg.DynamicUpdatesCount}\n"
                         + "\n")
                );
            }
            catch { /* ignore */ }

            return msg;
        }

    }
}