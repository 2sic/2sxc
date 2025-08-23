using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys.CacheKey;
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

    public static CacheKeyConfigNamed Update(CacheKeyConfigNamed? original, string? newNames, bool caseSensitive)
    {
        // merge previous names with new names, ensure no duplicates, order alphabetically
        var names = (original?.Names + "," + newNames).ToLowerInvariant()
            .CsvToArrayWithoutEmpty()
            .OrderBy(s => s)
            .Distinct()
            .ToArray();
        
        return new() { Names = string.Join(",", names), CaseSensitive = caseSensitive };
    }


    public static ICacheSpecs RestoreAll(this ICacheSpecs cacheSpecs, CacheKeyConfig keyConfig, CacheWriteConfig writeConfig) =>
        (CacheSpecs)cacheSpecs with
        {
            KeyConfig = keyConfig,
            WriteConfig = writeConfig,
            Key = null!,
            PolicyMaker = null!,
        };

    /// <summary>
    /// Restore a cache specs to apply the same logic as was stored here.
    /// </summary>
    /// <returns></returns>
    public static ICacheSpecs RestoreBy(this CacheKeyConfig keyConfig, ICacheSpecs cacheSpecs) =>
        (CacheSpecs)cacheSpecs with
        {
            KeyConfig = keyConfig,
        };
}
