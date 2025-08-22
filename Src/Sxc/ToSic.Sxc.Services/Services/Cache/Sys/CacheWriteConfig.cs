using ToSic.Sxc.Context;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys;
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
