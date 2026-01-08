using System.Text;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Services.Cache.Sys.CacheServiceConstants;

namespace ToSic.Sxc.Services.Cache.Sys.CacheKey;

/// <summary>
/// Functions to generate the final cache key based on the specifications.
/// </summary>
[PrivateApi]
public static class CacheKeyPartsExtensions
{
    /// <summary>
    /// Generate the key according to specs.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    internal static string GetKey(this CacheKeyParts keyParts)
    {
        // Make sure the Main key (prefix) is not empty
        if (string.IsNullOrWhiteSpace(keyParts.Main))
            throw new ArgumentException(@"Key must not be empty", nameof(keyParts.Main));

        // Prevent accidental adding of the prefix/segment multiple times
        var mainKey = GetBestKeyBase(keyParts);

        // If no additional keys are specified, exit early.
        if (keyParts.VaryByDic == null || keyParts.VaryByDic.Count == 0)
            return mainKey;

        // If there are no new keys, or they are already in the main key, exit early.
        var varyBy = GetVaryByOfDic(keyParts.VaryByDic);
        if (string.IsNullOrWhiteSpace(varyBy) || mainKey.EndsWith(varyBy))
            return mainKey;

        // Combine and return.
        return $"{mainKey}{varyBy}";
    }

    private static string GetBestKeyBase(CacheKeyParts keyParts)
    {
        // Prevent accidental adding of the prefix/segment multiple times
        if (keyParts.Main.StartsWith(DefaultPrefix))
            return keyParts.Main;

        var appKey = keyParts.RuntimeKey.HasValue()
            ? keyParts.RuntimeKey
            : keyParts.AppId.ToString();
        var isMagicOverride = keyParts.Main.StartsWith(CacheSpecConstants.PrefixForDontPrefix);
        var prefix = isMagicOverride
            ? keyParts.Main.TrimStart('*')
            : DefaultPrefix +
              (keyParts.AppId == CacheKeyParts.NoApp ? "" : Sep + "App:" + appKey) +
              $"{Sep}{SegmentPrefix}{keyParts.RegionName.NullIfNoValue() ?? DefaultSegment}{Sep}{keyParts.Main}";

        return prefix;
    }

    /// <summary>
    /// Generate string containing all parameters which should be included in the cache key.
    /// </summary>
    internal static string GetVaryByOfDic(Dictionary<string, string> dic)
    {
        // Keys must be ordered A-Z so that they are the same, no mater the order of adding
        var ordered = dic
            .OrderBy(p => p.Key, comparer: StringComparer.InvariantCultureIgnoreCase)
            .ThenBy(p => p.Value, comparer: StringComparer.InvariantCultureIgnoreCase)
            .ToList();

        var sb = new StringBuilder();
        foreach (var pair in ordered)
            sb.Append($"{Sep}{pair.Key}={pair.Value}");
        return sb.ToString();
    }
}
