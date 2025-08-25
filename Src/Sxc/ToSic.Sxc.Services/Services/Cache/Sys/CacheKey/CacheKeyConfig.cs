using ToSic.Sxc.Cms.Users;
using ToSic.Sys.Memory;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys.CacheKey;

/// <summary>
/// Internal configuration for the cache which will be relevant to the final key.
/// This is used to determine which parameters will be used to pre-check the cache.
/// Because of this, it must be a pure, simple record (to allow for easy comparison)
/// and never contain any objects which are specific to the current request.
/// </summary>
public record CacheKeyConfig(): ICanEstimateSize
{
    public const int Disabled = -1;
    public const int EnabledWithoutTime = 0;

    public CacheKeyConfig(NoParamOrder protector = default, int? seconds = null, string? varyBy = null, string? url = null, string? model = null): this()
    {
        if (seconds != null)
            ForElevation = new() { [UserElevation.All] = seconds.Value };

        foreach (var varyPart in (varyBy?.ToLowerInvariant()).CsvToArrayWithoutEmpty())
            switch (varyPart)
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
            ByPageParameters = new()
            {
                CaseSensitive = false,
                Names = url
            };

        if (!string.IsNullOrWhiteSpace(model))
            ByModel = new()
            {
                CaseSensitive = false,
                Names = model
            };
    }


    public bool ByPage { get; init; }
    public bool ByModule { get; init; }
    public bool ByUser { get; init; }

    /// <summary>
    /// this must be tracked in addition to the list page parameters, because even if the parameters are empty, it must still be
    /// activated on re-checking the cache.
    /// </summary>
    public CacheKeyConfigNamed? ByPageParameters { get; init; }

    public CacheKeyConfigNamed? ByModel { get; init; }

    /// <summary>
    /// Rules per elevation; the value is the sliding expiration in seconds.
    /// </summary>
    public Dictionary<UserElevation, int> ForElevation { get; init; } = [];

    SizeEstimate ICanEstimateSize.EstimateSize(ILog? log) => new(
        sizeof(bool) * 3 // ByPage, ByModule, ByUser
        + (ByPageParameters?.Names?.Length ?? 0) // ByParameters
        + (ByModel?.Names?.Length ?? 0) // ByModel
    );
}