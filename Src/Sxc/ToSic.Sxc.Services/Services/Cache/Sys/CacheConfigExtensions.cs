using ToSic.Sxc.Cms.Users;

namespace ToSic.Sxc.Services.Cache.Sys;
public static class CacheConfigExtensions
{
    public static CacheConfig Updated(this CacheConfig config, string name, string? keys, bool caseSensitive) =>
        name switch
        {
            CacheSpecConstants.ByModule => config with { ByModule = true },
            CacheSpecConstants.ByPage => config with { ByPage = true },
            CacheSpecConstants.ByUser => config with { ByUser = true },
            CacheSpecConstants.ByPageParameters => config with { ByPageParameters = new() { Names = keys, CaseSensitive = caseSensitive } },
            CacheSpecConstants.ByModel => config with { ByModel = new() { Names = keys, CaseSensitive = caseSensitive } }, // Model is like parameters
            _ => config
        };

    public static ICacheSpecs RestoreAll(this CacheConfig config, ICacheSpecs cacheSpecs)
    {
        cacheSpecs = config.RestoreConditionsAndWatch(cacheSpecs);
        cacheSpecs = config.RestoreBy(cacheSpecs);
        return cacheSpecs;
    }

    public static ICacheSpecs RestoreConditionsAndWatch(this CacheConfig config, ICacheSpecs cacheSpecs)
    {
        if (config.WatchAppData)
            cacheSpecs = cacheSpecs.WatchAppData();
        if (config.WatchAppFolder)
            cacheSpecs = cacheSpecs.WatchAppFolder();
        if (config.Sliding != 0)
            cacheSpecs = cacheSpecs.SetSlidingExpiration(seconds: config.Sliding);
        return cacheSpecs;
    }

    /// <summary>
    /// Restore a cache specs to apply the same logic as was stored here.
    /// </summary>
    /// <returns></returns>
    public static ICacheSpecs RestoreBy(this CacheConfig config, ICacheSpecs cacheSpecs)
    {
        if (config.ByUser)
            cacheSpecs = cacheSpecs.VaryByUser();

        if (config.ByModule)
            cacheSpecs = cacheSpecs.VaryByModule();

        if (config.ByPage)
            cacheSpecs = cacheSpecs.VaryByPage();

        if (config.ByPageParameters != null)
            cacheSpecs = cacheSpecs.VaryByPageParameters(config.ByPageParameters.Names, caseSensitive: config.ByPageParameters.CaseSensitive);

        if (config.ByModel != null)
            cacheSpecs = cacheSpecs.VaryByModel(config.ByModel.Names, caseSensitive: config.ByModel.CaseSensitive);

        return cacheSpecs;
    }

    public static bool IsForAllOrInRangeOfConfig(this UserElevation user, CacheConfig config)
        => user.IsForAllOrInRange(config.MinDisabledElevation, config.MaxDisabledElevation);
}
