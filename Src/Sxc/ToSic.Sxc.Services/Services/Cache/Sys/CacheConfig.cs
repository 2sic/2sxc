using ToSic.Sxc.Cms.Users;
using ToSic.Sys.Caching;
using ToSic.Sys.Memory;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Internal configuration to know what we're varying by.
/// This is used to determine which parameters will be used to pre-check the cache.
/// </summary>
public record CacheConfig(): ICanEstimateSize, ITimestamped
{
    public CacheConfig(NoParamOrder protector = default, int? sliding = null, string? watch = null, string? varyBy = null, string? url = null, string? model = null): this()
    {
        if (sliding != null)
            Sliding = sliding.Value;

        foreach (var part in watch.CsvToArrayWithoutEmpty())
            switch (part.ToLowerInvariant())
            {
                case "data" or "appdata":
                    WatchAppData = true;
                    break;
                // ReSharper disable once StringLiteralTypo
                case "folder" or "appfolder":
                    WatchAppFolder = true;
                    break;
                default:
                    throw new ArgumentException($@"Unknown {nameof(watch)} part '{part}'", nameof(watch));
            }

        foreach (var varyPart in varyBy.CsvToArrayWithoutEmpty())
            switch (varyPart.ToLowerInvariant())
            {
                case "page":
                    ByPage = true;
                    break;
                case "user":
                    ByUser = true;
                    break;
                case "module":
                    ByModule = true;
                    break;
                default:
                    throw new ArgumentException($@"Unknown {nameof(varyBy)} part '{varyPart}'", nameof(varyBy));
            }

        if (!string.IsNullOrWhiteSpace(url))
            ByPageParameters = new() { CaseSensitive = false, Names = url};

        if (!string.IsNullOrWhiteSpace(model))
            ByModel = new() { CaseSensitive = false, Names = model };
    }

    public bool WatchAppData { get; init; }
    public bool WatchAppFolder { get; init; }
    public int Sliding { get; init; }

    public bool ByPage { get; init; }
    public bool ByModule { get; init; }
    public bool ByUser { get; init; }

    /// <summary>
    /// this must be tracked in addition to the list page parameters, because even if the parameters are empty, it must still be
    /// activated on re-checking the cache.
    /// </summary>
    public CacheConfigByNamed? ByPageParameters { get; init; }

    public CacheConfigByNamed? ByModel { get; init; }

    public UserElevation MinDisabledElevation { get; init; }
    public UserElevation MaxDisabledElevation { get; init; }

    SizeEstimate ICanEstimateSize.EstimateSize(ILog? log) => new(
        sizeof(bool) * 3 // ByPage, ByModule, ByUser
        + (ByPageParameters?.Names?.Length ?? 0) // ByParameters
        + (ByModel?.Names?.Length ?? 0) // ByModel
    );

    long ITimestamped.CacheTimestamp { get; } = DateTime.Now.Ticks;

}