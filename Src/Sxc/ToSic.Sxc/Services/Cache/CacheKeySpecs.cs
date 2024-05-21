using System.Text;
using ToSic.Eav.Plumbing;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Services.Cache;

internal record CacheKeySpecs(int AppId, string Main, string RegionName = default, Dictionary<string, string> VaryByDic = default)
{
    /// <summary>
    /// Special marker to say that the cache should not vary by appId
    /// </summary>
    internal const int NoApp = -9876;

    public string Key => _key ??= GetKey(this);
    private string _key;

    public override string ToString() => Key;

    private static string GetKey(CacheKeySpecs keySpecs)
    {
        if (string.IsNullOrWhiteSpace(keySpecs.Main))
            throw new ArgumentException("Key must not be empty", nameof(keySpecs.Main));

        // Prevent accidental adding of the prefix/segment multiple times
        var mainKey = keySpecs.Main.StartsWith(DefaultPrefix)
            ? keySpecs.Main
            : $"{DefaultPrefix}{(keySpecs.AppId == NoApp ? "" : Sep + "App:" + keySpecs.AppId)}{Sep}{SegmentPrefix}{keySpecs.RegionName.NullIfNoValue() ?? DefaultSegment}{Sep}{keySpecs.Main}";

        if (keySpecs.VaryByDic == null || keySpecs.VaryByDic.Count == 0)
            return mainKey;

        var varyBy = GetVaryByOfDic(keySpecs.VaryByDic);
        if (string.IsNullOrWhiteSpace(varyBy) || mainKey.EndsWith(varyBy))
            return mainKey;

        return $"{mainKey}{varyBy}";
    }

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
