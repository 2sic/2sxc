using ToSic.Sxc.Context;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Cache configuration information which is only relevant when writing to the cache.
/// This information is either
/// - not relevant for retrieving from the cache
/// - too complex / changing to be serialized
/// - would cause trouble if also cached, since it might change fairly randomly
/// </summary>
public record CacheWriteConfig
{
    public CacheWriteConfig(NoParamOrder protector = default, string? watch = null)
    {
        foreach (var part in watch.CsvToArrayWithoutEmpty())
            switch (part.ToLowerInvariant())
            {
                case "data":
                    WatchAppData = true;
                    break;
                // ReSharper disable once StringLiteralTypo
                case "folder":
                    WatchAppFolder = true;
                    break;
                default:
                    throw new ArgumentException($@"Unknown {nameof(watch)} part '{part}'", nameof(watch));
            }
    }

    public bool WatchAppData { get; init; }
    public bool WatchAppFolder { get; init; }
    public bool WatchAppSubfolders { get; init; }

    public DateTimeOffset AbsoluteExpiration { get; init; }

    public int SlidingExpirationSeconds { get; init; }

    public List<(IParameters Parameters, string Names, bool CaseSensitive)> AdditionalParameters = [];

    public List<(string Name, string Value, bool CaseSensitive)> AdditionalValues = [];
}
