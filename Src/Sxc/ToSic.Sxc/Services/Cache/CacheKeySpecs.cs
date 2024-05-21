using System.Text;
using ToSic.Eav.Plumbing;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Services.Cache;

internal record CacheKeySpecs(string Main, string Segment = default, string VaryBy = default, Dictionary<string, string> VaryByDic = default)
{
    public string Key => _key ??= GetKey(this);
    private string _key;

    private static string GetKey(CacheKeySpecs keySpecs)
    {
        if (string.IsNullOrWhiteSpace(keySpecs.Main))
            throw new ArgumentException("Key must not be empty", nameof(keySpecs.Main));

        // Prevent accidental adding of the prefix/segment multiple times
        var mainKey = keySpecs.Main.StartsWith(DefaultPrefix)
            ? keySpecs.Main
            : $"{DefaultPrefix}{Sep}{SegmentPrefix}{keySpecs.Segment.NullIfNoValue() ?? DefaultSegment}{Sep}{keySpecs.Main}";

        // Prevent accidental adding of the varyBy multiple times
        var withVaryByString = mainKey;
        //!string.IsNullOrWhiteSpace(keySpecs.VaryBy) && !mainKey.EndsWith(keySpecs.VaryBy)
        //    ? $"{mainKey}{keySpecs.VaryBy}"
        //    : mainKey;

        var withVaryByDic = keySpecs.VaryByDic != null
            ? $"{withVaryByString}{GetVaryByOfDic(keySpecs.VaryByDic)}"
            : withVaryByString;

        return withVaryByDic;
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
