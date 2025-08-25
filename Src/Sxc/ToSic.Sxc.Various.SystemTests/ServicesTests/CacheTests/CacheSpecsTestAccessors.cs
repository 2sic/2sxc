using ToSic.Sxc.Context;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.Cache.Sys;
using ToSic.Sxc.Services.Cache.Sys.CacheKey;

namespace ToSic.Sxc.ServicesTests.CacheTests;

internal static class CacheSpecsTestAccessors
{
    public static ICacheSpecs CreateSpecsTac(this ICacheService cacheSpecs, string key, NoParamOrder protector = default,
        string? regionName = default, bool? shared = default)
    {
        var almost = (CacheSpecs)cacheSpecs.CreateSpecs(
            key,
            protector,
            regionName,
            shared: true /* pretend that shared is true, so it doesn't try to access the AppId */
        );

        // Simulate situation as if shared was specified originally, so it doesn't try to access the AppId
        if (shared == null)
            almost = almost with
            {
                CacheSpecsContextAndTools = almost.CacheSpecsContextAndTools with
                {
                    BaseKeyParts = almost.CacheSpecsContextAndTools.BaseKeyParts with
                    {
                        AppId = shared == null ? -1 : CacheKeyParts.NoApp,
                    }
                }
            };
        return almost;
    }

    public static ICacheSpecs VaryByParametersTac(this ICacheSpecs cacheSpecs, IParameters parameters, NoParamOrder protector = default, string? names = default, bool caseSensitive = false)
        => cacheSpecs.VaryByParameters(parameters, protector, names, caseSensitive);

    public static ICacheSpecs VaryByTac(this ICacheSpecs cacheSpecs, string name, string value, NoParamOrder protector = default, bool caseSensitive = false)
        => cacheSpecs.VaryBy(name, value, protector, caseSensitive);
}