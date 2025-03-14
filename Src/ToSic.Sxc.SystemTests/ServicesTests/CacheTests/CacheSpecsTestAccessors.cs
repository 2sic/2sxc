using ToSic.Lib.Coding;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.ServicesTests.CacheTests;

internal static class CacheSpecsTestAccessors
{
    public static ICacheSpecs CreateSpecsTac(this ICacheService cacheSpecs, string key, NoParamOrder protector = default,
        string regionName = default, bool? shared = default)
        => cacheSpecs.CreateSpecs(key, protector, regionName, shared);

    public static ICacheSpecs VaryByParametersTac(this ICacheSpecs cacheSpecs, IParameters parameters, NoParamOrder protector = default, string names = default, bool caseSensitive = false)
        => cacheSpecs.VaryByParameters(parameters, protector, names, caseSensitive);

    public static ICacheSpecs VaryByTac(this ICacheSpecs cacheSpecs, string name, string value, NoParamOrder protector = default, bool caseSensitive = false)
        => cacheSpecs.VaryBy(name, value, protector, caseSensitive);
}