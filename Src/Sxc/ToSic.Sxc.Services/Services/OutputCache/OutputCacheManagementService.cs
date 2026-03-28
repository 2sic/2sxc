using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Services.OutputCache;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class OutputCacheManagementService(INamedCacheDependencyService cacheDependenciesSvc)
    : ServiceBase("Sxc.OutCacMng", connect: [cacheDependenciesSvc]), IOutputCacheManagementService
{
    public int Flush(int appId, NoParamOrder npo = default, IEnumerable<string>? dependencies = null)
    {
        var l = Log.Fn<int>($"Flush appId: {appId}, dependencies: {string.Join(", ", dependencies ?? [])}");

        if (appId <= 0)
            throw l.Ex(new ArgumentOutOfRangeException(nameof(appId), appId, "App id must be greater than zero."));

        var normalized = cacheDependenciesSvc.NormalizeNames(dependencies);
        if (normalized.Count == 0)
        {
            cacheDependenciesSvc.TouchApp(CacheDependencyScopes.OutputCache, appId);
            return l.Return(0, "flushed, no dependencies");
        }

        var count = cacheDependenciesSvc.Touch(CacheDependencyScopes.OutputCache, appId, normalized);
        return l.Return(count, "flushed, with dependencies");
    }
}
