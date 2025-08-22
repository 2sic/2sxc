using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys;
public static class CacheKeyConfigExtensions
{
    public static CacheKeyConfig Updated(this CacheKeyConfig keyConfig, string name, string? keys, bool caseSensitive) =>
        name switch
        {
            CacheSpecConstants.ByModule => keyConfig with { ByModule = true },
            CacheSpecConstants.ByPage => keyConfig with { ByPage = true },
            CacheSpecConstants.ByUser => keyConfig with { ByUser = true },
            CacheSpecConstants.ByPageParameters when !string.IsNullOrWhiteSpace(keys) => keyConfig with { ByPageParameters = Update(keyConfig.ByPageParameters, keys, caseSensitive) },
            CacheSpecConstants.ByModel when !string.IsNullOrWhiteSpace(keys) => keyConfig with { ByModel = Update(keyConfig.ByModel, keys, caseSensitive) },
            _ => keyConfig
        };

    private static CacheKeyConfigNamed Update(CacheKeyConfigNamed? original, string? newNames, bool caseSensitive)
    {
        // merge previous names with new names, ensure no duplicates, order alphabetically
        var names = (original?.Names + "," + newNames).ToLowerInvariant()
            .CsvToArrayWithoutEmpty()
            .OrderBy(s => s)
            .Distinct()
            .ToArray();
        
        return new() { Names = string.Join(",", names), CaseSensitive = caseSensitive };
    }


    public static ICacheSpecs RestoreAll(this ICacheSpecs cacheSpecs, CacheKeyConfig keyConfig, CacheWriteConfig writeConfig)
    {
        var specs = (CacheSpecs)cacheSpecs;
        return specs with
        {
            KeyConfiguration = keyConfig,
            WriteConfiguration = writeConfig,
            Key = null!,
            PolicyMaker = null!,
        };
        //cacheSpecs = cacheSpecs.RestoreConditionsAndWatch(keyConfig, writeConfig);
        //cacheSpecs = keyConfig.RestoreBy(cacheSpecs);
        //return cacheSpecs;
    }

    public static ICacheSpecs RestoreConditionsAndWatch(this ICacheSpecs cacheSpecs, CacheKeyConfig keyConfig, CacheWriteConfig writeConfig)
    {
        if (writeConfig.WatchAppData)
            cacheSpecs = cacheSpecs.WatchAppData();
        if (writeConfig.WatchAppFolder)
            cacheSpecs = cacheSpecs.WatchAppFolder();

        var slidingAny = keyConfig.GetSlidingAny();
        if (slidingAny != 0)
            cacheSpecs = cacheSpecs.SetSlidingExpiration(seconds: slidingAny);
        return cacheSpecs;
    }

    /// <summary>
    /// Restore a cache specs to apply the same logic as was stored here.
    /// </summary>
    /// <returns></returns>
    public static ICacheSpecs RestoreBy(this CacheKeyConfig keyConfig, ICacheSpecs cacheSpecs)
    {
        var specs = (CacheSpecs)cacheSpecs;
        return specs with
        {
            KeyConfiguration = keyConfig,
        };
        //if (keyConfig.ByUser)
        //    cacheSpecs = cacheSpecs.VaryByUser();

        //if (keyConfig.ByModule)
        //    cacheSpecs = cacheSpecs.VaryByModule();

        //if (keyConfig.ByPage)
        //    cacheSpecs = cacheSpecs.VaryByPage();

        //if (keyConfig.ByPageParameters != null)
        //    cacheSpecs = cacheSpecs.VaryByPageParameters(keyConfig.ByPageParameters.Names, caseSensitive: keyConfig.ByPageParameters.CaseSensitive);

        //if (keyConfig.ByModel != null)
        //    cacheSpecs = cacheSpecs.VaryByModel(keyConfig.ByModel.Names, caseSensitive: keyConfig.ByModel.CaseSensitive);

        //return cacheSpecs;
    }
}
