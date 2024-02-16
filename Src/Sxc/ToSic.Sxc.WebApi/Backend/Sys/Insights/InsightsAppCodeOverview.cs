using ToSic.Eav.Apps.Internal.Insights;
using ToSic.Eav.Apps.State;
using ToSic.Eav.WebApi.Sys.Insights;

namespace ToSic.Sxc.Backend.Sys;

internal class InsightsAppCodeOverview(IAppStates appStates) : InsightsProvider(Link, helpCategory: "AppCode")
{
    public static string Link => "AppCodeOverview";

    public override string HtmlBody()
    {
        var msg = "";
        msg += "<table id='table'>"
               + InsightsHtmlTable.HeadFields("Zone ↕", "App ↕", "Name", "Is Loaded", "Build App Code")
               + "<tbody>";

        var zones = appStates.Zones.OrderBy(z => z.Key);


        foreach (var zone in zones)
        {
            var apps = zone.Value.Apps
                .Select(a =>
                {
                    var appIdentity = new AppIdentity(zone.Value.ZoneId, a.Key);
                    var inCache = appStates.IsCached(appIdentity);
                    var appState = inCache ? appStates.GetReader(appIdentity) : null;
                    return new
                    {
                        Id = a.Key,
                        Guid = a.Value,
                        InCache = inCache,
                        Name = inCache
                            ? appState?.Name ?? "unknown, app-infos not json"
                            : "not-loaded",
                        Reader = appState
                    };
                })
                .OrderBy(a => a.Id);


            msg = apps.Aggregate(msg, (current, app)
                => current
                   + InsightsHtmlTable.RowFields(
                       zone.Key.ToString(),
                       app.Id.ToString(),
                       app.Name,
                       app.InCache ? "yes" : "no",
                       AppStateExtensions.AppGuidIsAPreset(app.Guid)
                           ? ""
                           : Linker.LinkTo(view: InsightsAppCodeBuild.Link, label: "Build",
                               appId: app.Id))
            );
        }
        msg += "</tbody>"
               + "</table>"
               + InsightsHtmlParts.JsTableSort();
        return msg;
    }
}