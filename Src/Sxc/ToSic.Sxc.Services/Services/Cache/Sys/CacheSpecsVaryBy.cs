namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Internal configuration to know what we're varying by.
/// This is used to determine which parameters will be used to pre-check the cache.
/// </summary>
public record CacheSpecsVaryBy
{
    public bool ByPage { get; init; }
    public bool ByModule { get; init; }
    public bool ByUser { get; init; }

    public CacheSpecsVaryBy Updated(string key) =>
        key switch
        {
            CacheSpecConstants.ByModule => this with { ByModule = true },
            CacheSpecConstants.ByPage => this with { ByPage = true },
            CacheSpecConstants.ByUser => this with { ByUser = true },
            _ => this
        };
}
