using ToSic.Eav.Apps.Assets.Sys;
using ToSic.Eav.Sys;
using ToSic.Eav.Sys.Insights;
using ToSic.Eav.Sys.Insights.HtmlHelpers;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Utils;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Backend.Sys;

internal class InsightsLightSpeed(LightSpeedStats lightSpeedStats, IAppReaderFactory appReader)
    : InsightsProvider(new() { Name = Link, Teaser = "Show LightSpeed Caching Statistics", HelpCategory = "Performance" }, connect: [lightSpeedStats, appReader])
{
    public static string Link = "LightSpeedStats";

    public override string HtmlBody()
    {
        var msg = H1("LightSpeed Stats").ToString();
        try
        {
            var countStats = lightSpeedStats.AppsWithCount;
            //var sizeStats = lightSpeedStats.Size;
            msg += P($"Apps in Cache: {countStats.Count}");
            msg += "<table id='table'>"
                   + InsightsHtmlTable.HeadFields(["#", "ZoneId", "AppId", "Name", "Items in Cache", "Ca. Memory Use", "Uncompressed", "Compressed", "NameId"])
                   + "<tbody>";
            var count = 0;
            var totalItems = 0;
            var totalMemory = 0L;
            foreach (var cacheItem in countStats)
            {
                var appSpecs = (cacheItem.Key != 0)
                    ? appReader.Get(cacheItem.Key).Specs
                    : null;

                msg += InsightsHtmlTable.RowFields([
                    ++count,
                    // ZoneId, AppId, Name
                    SpecialField.Right(appSpecs?.ZoneId ?? 0),
                    SpecialField.Right(cacheItem.Key),
                    appSpecs?.Name ?? "unknown",

                    // Count, Size, Uncompressed, Compressed
                    SpecialField.Right(cacheItem.Value.Count),
                    SpecialField.Right(new SizeInfo(cacheItem.Value.Total).ToString()),
                    SpecialField.Right(new SizeInfo(cacheItem.Value.Uncompressed).ToString()),
                    SpecialField.Right(new SizeInfo(cacheItem.Value.Compressed).ToString()),

                    appSpecs?.NameId ?? "unknown"
                ]);
                totalItems += cacheItem.Value.Count;
                totalMemory += cacheItem.Value.Total;
            }
            msg += "</tbody>";
            msg += "<tfoot>";
            msg += InsightsHtmlTable.RowFields([
                B("Total:"),
                "",
                "",
                "",
                SpecialField.Right(B($"{totalItems}")),
                SpecialField.Right(B(ByteToKByte(totalMemory))),
                ""
            ]);

            msg += "</tfoot>";
            msg += "</table>";
            msg += "\n\n";
            msg += InsightsHtmlParts.JsTableSort();
        }
        catch
        {
            // ignored
        }
        return msg;
    }

    private string ByteToKByte(long bytes)
    {
        const int kb = 1024;
        if (bytes < kb) return bytes + " b";

        const int mb = kb * kb;
        if (bytes < 10 * mb)
            return ((double)bytes / kb).ToAposString() + " kb";

        return ((double)bytes / mb).ToAposString() + " mb";
    }

}