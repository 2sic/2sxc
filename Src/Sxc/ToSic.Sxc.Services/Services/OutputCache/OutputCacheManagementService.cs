using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Services.OutputCache;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class OutputCacheManagementService(INamedCacheDependencyService namedDependencies)
    : ServiceBase("Sxc.OutCacMng", connect: [namedDependencies]), IOutputCacheManagementService
{
    public int Flush(int appId, IEnumerable<string>? dependencies = null)
    {
        if (appId <= 0)
            throw new ArgumentOutOfRangeException(nameof(appId), appId, "App id must be greater than zero.");

        var normalized = namedDependencies.NormalizeNames(dependencies);
        if (normalized.Count == 0)
        {
            namedDependencies.TouchApp(CacheDependencyScopes.OutputCache, appId);
            return 0;
        }

        return namedDependencies.Touch(CacheDependencyScopes.OutputCache, appId, normalized);
    }
}
