using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Caching;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        public string LoadLog(int? appId = null)
        {
            ThrowIfNotSuperUser();

            if (UrlParamsIncomplete(appId, out var message))
                return message;

            Log.Add($"debug app-load {appId}");
            return FormatLog($"2sxc load log for app {appId}", AppRt(appId).AppState.Log);
        }

        public string Cache()
        {
            ThrowIfNotSuperUser();

            var msg = h1("Apps In Cache");
            //var cache = State.Cache;

            var zones = _appsCache.Zones.OrderBy(z => z.Key);

            msg += "<table id='table'><thead>"
                + tr(new[] { "Zone", "App", Eav.Data.Attributes.GuidNiceName, "InCache", "Name", "Folder", "Details", "Actions" }, true)
                + "</thead>"
                + "<tbody>";
            foreach (var zone in zones)
            {
                var apps = zone.Value.Apps
                    .Select(a =>
                    {
                        var appIdentity = new AppIdentity(zone.Value.ZoneId, a.Key);
                        var inCache = _appsCache.Has(appIdentity);
                        var appState = inCache
                            ? _appStates.Get(appIdentity)
                            : null;
                        return new
                        {
                            Id = a.Key, Guid = a.Value,
                            InCache = inCache,
                            Name = inCache
                                ? appState?.Name ?? "unknown, app-infos not json"
                                : "not-loaded",
                            Folder = inCache
                                ? appState?.Folder ?? "unknown, app-infos not json"
                                : "not-loaded",
                        };
                    })
                    .OrderBy(a => a.Id);


                foreach (var app in apps)
                {
                    msg += tr(new[]
                    {
                        zone.Key.ToString(),
                        app.Id.ToString(),
                        $"{app.Guid}",
                        app.InCache ? "yes" : "no",
                        app.Name,
                        app.Folder,
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

        public string Stats(int? appId = null)
        {
            ThrowIfNotSuperUser();

            if (UrlParamsIncomplete(appId, out var message))
                return message;

            Log.Add($"debug app-internals for {appId}");
            var appRead = AppRt(appId);
            var pkg = appRead.AppState;

            var msg = h1($"App internals for {appId}");
            try
            {
                Log.Add("general stats");
                msg += p(
                    ToBr($"AppId: {pkg.AppId}\n"
                         + $"Timestamp First : {pkg.CacheStatistics.FirstTimestamp} = {pkg.CacheStatistics.FirstTimestamp.ToReadable()}\n"
                         + $"Timestamp Current: {pkg.CacheStatistics.CacheTimestamp} = {pkg.CacheStatistics.CacheTimestamp.ToReadable()}\n"
                         + $"Update Count: {pkg.CacheStatistics.ResetCount}\n"
                         + $"Dyn Update Count: {pkg.DynamicUpdatesCount}\n"
                         + "\n")
                );

                msg += h2("History");

                var list = pkg.CacheStatistics.History.Reverse().Aggregate("", (current, timestamp) 
                    => current + li(timestamp + " = " + timestamp.ToReadable()));

                msg += ol(list);
            }
            catch { /* ignore */ }

            return msg;
        }

    }
}
