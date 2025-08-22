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
    public string FinalKey
    {
        get => field ??= CacheKeyGenerator.GetKey(this);
        init;
    }

    /// <summary>
    /// Override the ToString method to return the key.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => FinalKey;

    public CacheKeySpecs WithUpdatedVaryBy(string name, string value, bool caseSensitive)
    {
        var varyByName = "VaryBy" + name;
        var varyByKey = caseSensitive ? varyByName : varyByName.ToLowerInvariant();
        var valueToUse = caseSensitive ? value : value.ToLowerInvariant();

        var newDic = new Dictionary<string, string>(VaryByDic ?? [], StringComparer.InvariantCultureIgnoreCase)
        {
            [varyByKey] = valueToUse
        };

        return this with
        {
            FinalKey = null! /* requires reset */,
            VaryByDic = newDic
        };
    }
}
