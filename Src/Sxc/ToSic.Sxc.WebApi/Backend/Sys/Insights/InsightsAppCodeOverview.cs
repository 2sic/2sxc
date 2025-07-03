using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Sys;
using ToSic.Eav.Sys.Insights;
using ToSic.Eav.Sys.Insights.HtmlHelpers;

namespace ToSic.Sxc.Backend.Sys;

internal class InsightsAppCodeOverview(IAppReaderFactory appReaders, IAppStateCacheService appStates, IAppsCatalog appsCatalog)
    : InsightsProvider(new() { Name = Link, HelpCategory = FolderConstants.AppCodeFolder }, connect: [appReaders, appStates, appsCatalog])
{
    public static string Link => "AppCodeOverview";

    public override string HtmlBody()
    {
        var msg = "";
        msg += "<table id='table'>"
               + InsightsHtmlTable.HeadFields(["Zone ↕", "App ↕", "Name", "Is Loaded", "Build App Code"])
               + "<tbody>";

        var zones = appsCatalog.Zones
            .OrderBy(z => z.Key);


        foreach (var zone in zones)
        {
            var apps = zone.Value
                .Apps
                .Select(a =>
                {
                    var appIdentity = new AppIdentity(zone.Value.ZoneId, a.Key);
                    var inCache = appStates.IsCached(appIdentity);
                    var appState = inCache ? appReaders.Get(appIdentity) : null;
                    return new
                    {
                        Id = a.Key,
                        Guid = a.Value,
                        InCache = inCache,
                        Name = inCache
                            ? appState?.Specs.Name ?? "unknown, app-infos not json"
                            : "not-loaded",
                        Reader = appState
                    };
                })
                .OrderBy(a => a.Id);


            msg = apps.Aggregate(msg, (current, app)
                => current
                   + InsightsHtmlTable.RowFields([
                       zone.Key.ToString(),
                       app.Id.ToString(),
                       app.Name,
                       app.InCache ? "yes" : "no",
                       AppStateExtensions.AppGuidIsAPreset(app.Guid)
                           ? ""
                           : Linker.LinkTo(view: InsightsAppCodeBuild.Link, label: "Build",
                               appId: app.Id)
                   ])
            );
        }
        msg += "</tbody>"
               + "</table>"
               + InsightsHtmlParts.JsTableSort();
        return msg;
    }
}