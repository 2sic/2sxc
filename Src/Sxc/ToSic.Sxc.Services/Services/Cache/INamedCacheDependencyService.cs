using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Services.Cache;

/// <summary>
/// Internal marker service for app-scoped named cache dependencies.
/// It currently backs <see cref="IModuleOutputCacheService"/> and <see cref="IOutputCacheManagementService"/>,
/// and is structured so future <c>Kit.Cache</c> APIs can reuse the same invalidation mechanism.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface INamedCacheDependencyService
{
    IReadOnlyList<string> GetOrEnsureKeys(string scope, int appId, IEnumerable<string>? names);

    string GetOrEnsureAppKey(string scope, int appId);

    int Touch(string scope, int appId, IEnumerable<string>? names);

    void TouchApp(string scope, int appId);

    IReadOnlyList<string> NormalizeNames(IEnumerable<string>? names);
}
