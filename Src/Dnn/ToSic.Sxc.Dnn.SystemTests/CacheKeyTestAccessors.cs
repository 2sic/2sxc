using ToSic.Sxc.Dnn.Razor.Sys;

namespace ToSic.Sxc.Dnn;

/// <summary>
/// Logic-free test accessor wrappers for <see cref="CacheKey"/> to keep Find-All-References of production methods clean.
/// </summary>
internal static class CacheKeyTestAccessors
{
    // Constructor forwarder (factory)
    public static CacheKey NewCacheKeyTac(int appId, string? edition, string templatePath, string contentHash, string appCodeHash)
        => new CacheKey(appId: appId, edition: edition, templatePath: templatePath, contentHash: contentHash, appCodeHash: appCodeHash);

    // Static method forwarders
    public static string NormalizePathTac(string templatePath)
        => CacheKey.NormalizePath(templatePath: templatePath);

    public static string NormalizeEditionTac(string edition)
        => CacheKey.NormalizeEdition(edition: edition);

    public static string GetAppFolderTac(int appId, string edition)
        => CacheKey.GetAppFolder(appId: appId, edition: edition);

    // Instance method forwarders (extension style)
    public static string ToStringTac(this CacheKey cacheKey)
        => cacheKey.ToString();

    public static string GetFilePathTac(this CacheKey cacheKey, string cacheDirectory)
        => cacheKey.GetFilePath(cacheDirectory: cacheDirectory);

    public static bool EqualsTac(this CacheKey cacheKey, CacheKey other)
        => cacheKey.Equals(other: other);

    public static int GetHashCodeTac(this CacheKey cacheKey)
        => cacheKey.GetHashCode();
}
