using System.Text;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Services.Cache.Sys.CacheServiceConstants;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Note: Internal
/// </summary>
[PrivateApi]
public record CacheKeySpecs
{
    public required int AppId { get; init; }

    public required string Main { get; init; }

    public string? RegionName { get; init; }

    public Dictionary<string, string>? VaryByDic { get; init; }

    /// <summary>
    /// Special marker to say that the cache should not vary by appId
    /// </summary>
    internal const int NoApp = -12345;

    /// <summary>
    /// Generate the final key for these specs.
    /// </summary>
    [field: AllowNull, MaybeNull]
    public string Key
    {
        get => field ??= GetKey(this);
        init;
    }

    /// <summary>
    /// Override the ToString method to return the key.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Key;

    /// <summary>
    /// Generate the key according to specs.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private static string GetKey(CacheKeySpecs keySpecs)
    {
        // Make sure the Main key (prefix) is not empty
        if (string.IsNullOrWhiteSpace(keySpecs.Main))
            throw new ArgumentException(@"Key must not be empty", nameof(keySpecs.Main));

        // Prevent accidental adding of the prefix/segment multiple times
        var mainKey = GetBestKeyBase(keySpecs);

        // If no additional keys are specified, exit early.
        if (keySpecs.VaryByDic == null || keySpecs.VaryByDic.Count == 0)
            return mainKey;

        // If there are no new keys, or they are already in the main key, exit early.
        var varyBy = GetVaryByOfDic(keySpecs.VaryByDic);
        if (string.IsNullOrWhiteSpace(varyBy) || mainKey.EndsWith(varyBy))
            return mainKey;

        // Combine and return.
        return $"{mainKey}{varyBy}";
    }

    private static string GetBestKeyBase(CacheKeySpecs keySpecs)
    {
        // Prevent accidental adding of the prefix/segment multiple times
        if (keySpecs.Main.StartsWith(DefaultPrefix))
            return keySpecs.Main;

        var isMagicOverride = keySpecs.Main.StartsWith(CacheSpecConstants.PrefixForDontPrefix);
        var prefix = isMagicOverride
            ? keySpecs.Main.TrimStart('*')
            : $"{DefaultPrefix}{(keySpecs.AppId == NoApp ? "" : Sep + "App:" + keySpecs.AppId)}{Sep}{SegmentPrefix}{keySpecs.RegionName.NullIfNoValue() ?? DefaultSegment}";

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
