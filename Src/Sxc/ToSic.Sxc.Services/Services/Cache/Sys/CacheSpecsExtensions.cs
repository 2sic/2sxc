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
    public static CacheConfig GetConfig(this ICacheSpecs specs)
        => ((CacheSpecs)specs).Configuration;


    public static ICacheSpecs AttachModel(this ICacheSpecs specs, IDictionary<string, object?>? model)
    {
        var typed = (CacheSpecs)specs;
        var l = typed.Log.Fn<ICacheSpecs>($"hasModel: {model != null}; count: {model?.Count}");
        return l.ReturnAsOk(typed with { Model = model });
    }
}
