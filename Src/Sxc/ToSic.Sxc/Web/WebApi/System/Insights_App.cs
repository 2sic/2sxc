using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Caching;
using ToSic.Razor.Blade;
using static ToSic.Razor.Blade.Tag;

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

            var msg = H1("Apps In Cache").ToString();

            var zones = _appsCache.Zones.OrderBy(z => z.Key);

            msg += "<table id='table'>"
                + HeadFields("Zone ↕", "App ↕", Eav.Data.Attributes.GuidNiceName, "InCache", "Name ↕", "Folder ↕", "Details", "Actions")
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
                    msg += RowFields(
                        zone.Key.ToString(),
                        app.Id.ToString(),
                        $"{app.Guid}",
                        app.InCache ? "yes" : "no",
                        app.Name,
                        app.Folder,
                        $"{A("stats").Href($"stats?appid={app.Id}")} | {A("load log").Href($"loadlog?appid={app.Id}")} | {A("types").Href($"types?appid={app.Id}")}",
                        Details(
                            Summary("show actions"),
                            A("purge").Href($"purge?appid={app.Id}").ToString()
                        )
                    );
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

            var msg = H1($"App internals for {appId}").ToString();
            try
            {
                Log.Add("general stats");
                msg += P(
                     Tags.Nl2Br($"AppId: {pkg.AppId}\n"
                         + $"Timestamp First : {pkg.CacheStatistics.FirstTimestamp} = {pkg.CacheStatistics.FirstTimestamp.ToReadable()}\n"
                         + $"Timestamp Current: {pkg.CacheStatistics.CacheTimestamp} = {pkg.CacheStatistics.CacheTimestamp.ToReadable()}\n"
                         + $"Update Count: {pkg.CacheStatistics.ResetCount}\n"
                         + $"Dyn Update Count: {pkg.DynamicUpdatesCount}\n"
                         + "\n")
                );

                msg += H2("History");

                msg += Ol(
                    pkg.CacheStatistics.History
                        .Reverse()
                        .Select(timestamp => Li(timestamp + " = " + timestamp.ToReadable()))
                        .ToArray<object>()
                );
            }
            catch { /* ignore */ }

            return msg;
        }

    }
}
