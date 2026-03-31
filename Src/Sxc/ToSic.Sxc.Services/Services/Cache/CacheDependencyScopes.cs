namespace ToSic.Sxc.Services.Cache;

/// <summary>
/// Internal scopes for named cache dependencies.
/// These keep output-cache markers isolated from future cache consumers which may reuse the same service.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class CacheDependencyScopes
{
    public const string OutputCache = "output-cache";
}
