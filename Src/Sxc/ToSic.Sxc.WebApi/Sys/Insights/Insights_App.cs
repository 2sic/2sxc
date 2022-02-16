using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Debug;
using ToSic.Eav.Caching;
using ToSic.Razor.Blade;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.WebApi.Sys
{
    public partial class Insights
    {
        private string LoadLog(int? appId)
        {
            if (UrlParamsIncomplete(appId, out var message))
                return message;

            Log.Add($"debug app-load {appId}");
            return PageStyles() + DumpTree($"2sxc load log for app {appId}", AppRt(appId).AppState.Log);
        }

        private string Cache()
        {
            var msg = H1("Apps In Cache").ToString();

            var zones = _appStates.Zones.OrderBy(z => z.Key);

            msg += "<table id='table'>"
                + HeadFields("Zone ↕", "App ↕", Eav.Data.Attributes.GuidNiceName, "InCache", "Name ↕", "Folder ↕", "Details", "Actions", "Hash", "Timestamp", "List-Timestamp")
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
                            Id = a.Key,
                            Guid = a.Value,
                            InCache = inCache,
                            Name = inCache
                                ? appState?.Name ?? "unknown, app-infos not json"
                                : "not-loaded",
                            Folder = inCache
                                ? appState?.Folder ?? "unknown, app-infos not json"
                                : "not-loaded",
                            Hash = appState?.GetHashCode(),
                            TS = appState?.CacheTimestamp,
                            ListTs = appState?.ListCache()?.CacheTimestamp,
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
                        $"{LinkTo("stats", nameof(Stats), app.Id)} | {LinkTo("load log", nameof(LoadLog), app.Id)} | {LinkTo("types", nameof(Types), app.Id)}",
                        Tag.Details(
                            Summary("show actions"),
                            app.Id != Eav.Constants.PresetAppId ? LinkTo("purge", nameof(Purge), app.Id) : null
                        ),
                        app.Hash?.ToString() ?? "-",
                        app.TS,
                        app.ListTs
                    );
                }
            }
            msg += "</tbody>"
                   + "</table>"
                   + JsTableSort();
            return msg;
        }

        private string Stats(int? appId)
        {
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
                        .Select(timestamp => Li($"Resets: {timestamp.ResetCount}; Items: {timestamp.ItemCount}; TS: {timestamp.Timestamp} = {timestamp.Timestamp.ToReadable()}"))
                        .ToArray<object>()
                );
            }
            catch { /* ignore */ }

            return msg;
        }

    }
}
