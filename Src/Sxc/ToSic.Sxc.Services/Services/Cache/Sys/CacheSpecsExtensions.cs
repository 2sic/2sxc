using ToSic.Sxc.Services.Cache.Sys.CacheKey;

namespace ToSic.Sxc.Services.Cache.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class CacheSpecsExtensions
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static ICacheSpecs WithPolicyOf(this ICacheSpecs specs, ICacheSpecs specsWithPolicy) => 
        (CacheSpecs)specs with { PolicyMaker = specsWithPolicy.PolicyMaker };

    /// <summary>
    /// Give access to internal VaryByList.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static CacheKeyConfig GetConfig(this ICacheSpecs specs)
        => ((CacheSpecs)specs).KeyConfig;


    public static ICacheSpecs AttachModel(this ICacheSpecs specs, IDictionary<string, object?>? model)
    {
        var typed = (CacheSpecs)specs;
        var l = typed.Log.Fn<ICacheSpecs>($"hasModel: {model != null}; count: {model?.Count}");
        return l.ReturnAsOk(typed with
        {
            CacheSpecsContextAndTools = typed.CacheSpecsContextAndTools with
            {
                Model = model
            }
        });
    }
}
