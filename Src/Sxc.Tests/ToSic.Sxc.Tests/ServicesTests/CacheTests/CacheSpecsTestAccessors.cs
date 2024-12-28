using ToSic.Lib.Coding;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Tests.ServicesTests.CacheTests;

internal static class CacheSpecsTestAccessors
{
    public static ICacheSpecs VaryByParametersTac(this ICacheSpecs cacheSpecs, IParameters parameters, NoParamOrder protector = default, string names = default, bool caseSensitive = false)
        => cacheSpecs.VaryByParameters(parameters, protector, names, caseSensitive);

    public static ICacheSpecs VaryByTac(this ICacheSpecs cacheSpecs, string name, string value, NoParamOrder protector = default, bool caseSensitive = false)
        => cacheSpecs.VaryBy(name, value, protector, caseSensitive);
}