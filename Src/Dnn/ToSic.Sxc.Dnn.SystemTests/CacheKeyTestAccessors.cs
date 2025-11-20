using ToSic.Sxc.Dnn.Razor.Sys;

namespace ToSic.Sxc.Dnn;

/// <summary>
/// Logic-free test accessor wrappers for <see cref="CacheKey"/> to keep Find-All-References of production methods clean.
/// </summary>
internal static class CacheKeyTestAccessors
{
    // Constructor forwarder (factory)
    public static CacheKey NewCacheKeyTac(int appId, string? edition, string templatePath, string contentHash, string appCodeHash, string? appPath = null)
        => new CacheKey(appId: appId, edition: edition, templatePath: templatePath, contentHash: contentHash, appCodeHash: appCodeHash, appPath: appPath);

    // Static method forwarders
    public static string NormalizePathTac(string templatePath, string edition, string? appPath = null)
        => CacheKey.NormalizePath(templatePath: templatePath, edition: edition, appPath: appPath);

    public static string GetAppFolderTac(int appId, string edition)
        => CacheKey.GetAppFolder(appId: appId, edition: edition);

    public static string ToStringTac(this CacheKey cacheKey)
        => cacheKey.ToString();

    public static string GetFilePathTac(this CacheKey cacheKey, string cacheDirectory)
        => cacheKey.GetFilePath(cacheDirectory: cacheDirectory);

    public static bool EqualsTac(this CacheKey cacheKey, CacheKey other)
        => cacheKey.Equals(other: other);

    public static int GetHashCodeTac(this CacheKey cacheKey)
        => cacheKey.GetHashCode();
}
