namespace ToSic.Sxc.Services.Cache.Sys.CacheKey;

/// <summary>
/// The parts which make up a cache key.
/// Can then be converted to a key using the GetKey() extension method.
/// </summary>
[PrivateApi]
public record CacheKeyParts
{
    /// <summary>
    /// Special marker to say that the cache should not vary by appId
    /// </summary>
    internal const int NoApp = -12345;

    public required int AppId { get; init; }

    public required string Main { get; init; }

    public string? RegionName { get; init; }

    public Dictionary<string, string>? VaryByDic { get; init; }

    /// <summary>
    /// Override the ToString method to return the key.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.GetKey();

    public CacheKeyParts WithUpdatedVaryBy(string name, string value, bool caseSensitive)
    {
        var varyByName = "VaryBy" + name;
        var varyByKey = caseSensitive ? varyByName : varyByName.ToLowerInvariant();
        var valueToUse = caseSensitive ? value : value.ToLowerInvariant();

        return this with
        {
            VaryByDic = new(VaryByDic ?? [], StringComparer.InvariantCultureIgnoreCase)
            {
                [varyByKey] = valueToUse
            }
        };
    }
}
