namespace ToSic.Sxc.Services.Cache.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class CacheSpecsExtensions
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static ICacheSpecs SwapKeyInternal(this ICacheSpecs specs, Func<CacheKeySpecs, CacheKeySpecs> modifier)
    {
        var typed = (CacheSpecs)specs;

        return typed with { KeySpecs = modifier(typed.KeySpecs) };
    }

    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static ICacheSpecs MergePolicy(this ICacheSpecs specs, ICacheSpecs specsWithPolicy)
    {
        var typed = (CacheSpecs)specs;

        return typed with { PolicyMaker = specs.PolicyMaker };
    }
    /// <summary>
    /// Give access to internal VaryByList.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static CacheSpecsVaryBy GetVaryByList(this ICacheSpecs specs)
        => ((CacheSpecs)specs).VaryByList;


    public static ICacheSpecs AttachModel(this ICacheSpecs specs, IDictionary<string, object?>? model)
        => ((CacheSpecs)specs) with { Model = model };

}
