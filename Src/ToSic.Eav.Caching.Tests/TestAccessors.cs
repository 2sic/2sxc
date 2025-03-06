using System.Runtime.Caching;

namespace ToSic.Eav.Caching.Tests;

internal static class TestAccessors
{
    internal static CacheItemPolicy CreateResultTac(this IPolicyMaker policyMaker)
        => policyMaker.CreateResult();
}