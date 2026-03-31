using ToSic.Eav.Apps.Assets.Sys;
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
            var countStats = lightSpeedStats.GetStats();
            //var sizeStats = lightSpeedStats.Size;
            msg += P($"Apps in Cache: {countStats.Count}");
            msg += "<table id='table'>"
                   + InsightsHtmlTable.HeadFields(["#", "ZoneId", "AppId", "Name", "Items in Cache", "Ca. Memory Use", "Uncompressed", "Compressed", "Expanded", "Grand-Total", "Mem-Saved", "NameId"])
                   + "<tbody>";
            var count = 0;
            var totalItems = 0;
            var totalMemory = 0L;
            foreach (var cacheItem in countStats)
            {
                var appSpecs = (cacheItem.Key != 0)
                    ? appReader.Get(cacheItem.Key).Specs
                    : null;

                var stats = cacheItem.Value;
                msg += InsightsHtmlTable.RowFields([
                    ++count,
                    // ZoneId, AppId, Name
                    SpecialField.Right(appSpecs?.ZoneId ?? 0),
                    SpecialField.Right(cacheItem.Key),
                    appSpecs?.Name ?? "unknown",

                    // Count, Size, Uncompressed, Compressed
                    SpecialField.Right(stats.Count),
                    SpecialField.Right(new SizeInfo(stats.MemoryUse).ToString("N")),
                    SpecialField.Right(new SizeInfo(stats.Uncompressed).ToString("N")),
                    SpecialField.Right(new SizeInfo(stats.Compressed).ToString("N")),
                    SpecialField.Right(new SizeInfo(stats.Expanded).ToString("N")),
                    SpecialField.Right(new SizeInfo(stats.GrandTotal).ToString("N")),
                    SpecialField.Right(new SizeInfo(stats.GrandTotal - stats.Compressed).ToString("N")),

                    appSpecs?.NameId ?? "unknown"
                ]);
                totalItems += stats.Count;
                totalMemory += stats.MemoryUse;
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