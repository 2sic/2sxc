using ToSic.Sys.Caching;
using ToSic.Sys.Memory;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Internal configuration to know what we're varying by.
/// This is used to determine which parameters will be used to pre-check the cache.
/// </summary>
public record CacheSpecsVaryBy: ICanEstimateSize, ITimestamped
{
    public bool ByPage { get; init; }
    public bool ByModule { get; init; }
    public bool ByUser { get; init; }

    /// <summary>
    /// this must be tracked in addition to the list page parameters, because even if the parameters are empty, it must still be
    /// activated on re-checking the cache.
    /// </summary>
    public bool ByPageParameters { get; init; }

    public string? PageParameters { get; init; }

    public bool ByPageParametersCaseSensitive { get; init; }

    public CacheSpecsVaryBy Updated(string name, string? keys, bool caseSensitive) =>
        name switch
        {
            CacheSpecConstants.ByModule => this with { ByModule = true },
            CacheSpecConstants.ByPage => this with { ByPage = true },
            CacheSpecConstants.ByUser => this with { ByUser = true },
            CacheSpecConstants.ByPageParameters => this with { ByPageParameters = true, PageParameters = keys, ByPageParametersCaseSensitive = caseSensitive },
            _ => this
        };

    public SizeEstimate EstimateSize(ILog? log = default) => new(
        sizeof(bool) * 3 // ByPage, ByModule, ByUser
        + (PageParameters?.Length ?? 0) // ByParameters
    );

    long ITimestamped.CacheTimestamp { get; } = DateTime.Now.Ticks;

    /// <summary>
    /// Restore a cache specs to apply the same logic as was stored here.
    /// </summary>
    /// <param name="cacheSpecs"></param>
    /// <returns></returns>
    public ICacheSpecs Restore(ICacheSpecs cacheSpecs)
    {
        if (ByUser)
            cacheSpecs = cacheSpecs.VaryByUser();

        if (ByModule)
            cacheSpecs = cacheSpecs.VaryByModule();

        if (ByPage)
            cacheSpecs = cacheSpecs.VaryByPage();

        if (ByPageParameters)
            cacheSpecs = cacheSpecs.VaryByPageParameters(PageParameters, caseSensitive: ByPageParametersCaseSensitive);

        return cacheSpecs;
    }
}
