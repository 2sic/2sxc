using ToSic.Eav;
using ToSic.Eav.Apps.Internal.Insights;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Sys.Insights;
using ToSic.Sxc.Web.Internal.LightSpeed;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Backend.Sys;

internal class InsightsLightSpeed(LightSpeedStats lightSpeedStats, IAppReaderFactory appReader) : InsightsProvider(Link, teaser: "Show LightSpeed Caching Statistics", helpCategory: "Performance")
{
    public static string Link = "LightSpeedStats";

    public override string HtmlBody()
    {
        var msg = H1("LightSpeed Stats").ToString();
        try
        {
            var countStats = lightSpeedStats.ItemsCount;
            var sizeStats = lightSpeedStats.Size;
            msg += P($"Apps in Cache: {countStats.Count}");
            msg += "<table id='table'>"
                   + InsightsHtmlTable.HeadFields("#", "ZoneId", "AppId", "Name", "Items in Cache", "Ca. Memory Use", "NameId")
                   + "<tbody>";
            var count = 0;
            var totalItems = 0;
            var totalMemory = 0L;
            foreach (var cacheItem in countStats)
            {
                var appSpecs = appReader.Get(cacheItem.Key).Specs;
                msg += InsightsHtmlTable.RowFields(
                    ++count,
                    SpecialField.Right(appSpecs.ZoneId),
                    SpecialField.Right(cacheItem.Key),
                    appSpecs.Name,
                    SpecialField.Right(cacheItem.Value),
                    SpecialField.Right(sizeStats.TryGetValue(cacheItem.Key, out var size) ? ByteToKByte(size) : Constants.NullNameId),
                    appSpecs.NameId
                );
                totalItems += cacheItem.Value;
                totalMemory += size;
            }
            msg += "</tbody>";
            msg += "<tfoot>";
            msg += InsightsHtmlTable.RowFields(
                B("Total:"),
                "",
                "",
                "",
                SpecialField.Right(B($"{totalItems}")),
                SpecialField.Right(B(ByteToKByte(totalMemory))),
                ""
            );

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
        if (bytes < kb) return bytes + "b";

        const int mb = kb * kb;
        if (bytes < 10 * mb)
            return ((double)bytes / kb).ToAposString() + "kb";

        return ((double)bytes / mb).ToAposString() + "mb";
    }

}