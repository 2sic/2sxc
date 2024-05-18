using ToSic.Eav.Plumbing;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Services.Cache;

internal record CacheKeySpecs(string Main, string Segment = default, string VaryBy = default)
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
        return !string.IsNullOrWhiteSpace(keySpecs.VaryBy) && !mainKey.EndsWith(keySpecs.VaryBy)
            ? $"{mainKey}{keySpecs.VaryBy}"
            : mainKey;
    }
}
