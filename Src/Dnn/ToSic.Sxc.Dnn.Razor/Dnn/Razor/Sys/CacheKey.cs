namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Value object representing a disk cache key for compiled Razor assemblies.
/// Immutable structure containing all components needed for cache lookup and invalidation.
/// </summary>
/// <remarks>
/// Thread-safe by design (immutable).
/// </remarks>
public sealed class CacheKey : IEquatable<CacheKey>
{
    /// <summary>
    /// Application identifier
    /// </summary>
    public int AppId { get; }

    /// <summary>
    /// App edition (e.g., "live", "staging"). Never null; defaults to "root" if not provided.
    /// </summary>
    public string Edition { get; }

    /// <summary>
    /// Normalized template path (lowercase, slash-normalized)
    /// </summary>
    public string NormalizedPath { get; }

    /// <summary>
    /// SHA256 hash of template source code content
    /// </summary>
    public string ContentHash { get; }

    /// <summary>
    /// SHA256 hash of AppCode assembly (includes version info)
    /// </summary>
    public string AppCodeHash { get; }

    /// <summary>
    /// Creates a new cache key with the specified components.
    /// </summary>
    public CacheKey(int appId, string edition, string normalizedPath, string contentHash, string appCodeHash)
    {
        if (appId <= 0)
            throw new ArgumentException("AppId must be positive", nameof(appId));
        if (string.IsNullOrWhiteSpace(normalizedPath))
            throw new ArgumentNullException(nameof(normalizedPath));
        if (string.IsNullOrWhiteSpace(contentHash))
            throw new ArgumentNullException(nameof(contentHash));
        if (string.IsNullOrWhiteSpace(appCodeHash))
            throw new ArgumentNullException(nameof(appCodeHash));

        AppId = appId;
        Edition = NormalizeEdition(edition);
        NormalizedPath = normalizedPath;
        ContentHash = contentHash;
        AppCodeHash = appCodeHash;
    }

    /// <summary>
    /// Generates the cache key file name in the format:
    /// {normalizedPath}-{contentHash}-{appCodeHash}.dll
    /// </summary>
    /// <example>
    /// views-default-cshtml-a1b2c3-x9y8z7.dll
    /// </example>
    public override string ToString()
    {
        // Truncate hashes to first 6 characters for readability
        var contentHashShort = ContentHash.Length > 6 ? ContentHash.Substring(0, 6) : ContentHash;
        var appCodeHashShort = AppCodeHash.Length > 6 ? AppCodeHash.Substring(0, 6) : AppCodeHash;
        
        return $"{NormalizedPath}-{contentHashShort}-{appCodeHashShort}.dll";
    }

    /// <summary>
    /// Gets the full file system path for this cache key in the specified directory.
    /// </summary>
    /// <param name="cacheDirectory">Root cache directory path</param>
    /// <returns>Full path to the cached DLL file</returns>
    public string GetFilePath(string cacheDirectory)
    {
        if (string.IsNullOrWhiteSpace(cacheDirectory))
            throw new ArgumentNullException(nameof(cacheDirectory));

        var directory = Path.Combine(cacheDirectory, GetAppFolder(AppId), GetEditionFolder(Edition));
        return Path.Combine(directory, ToString());
    }

    /// <summary>
    /// Normalizes a template path for use in cache keys.
    /// </summary>
    /// <param name="templatePath">Original template path (may have mixed case, forward/back slashes)</param>
    /// <returns>Normalized path (lowercase, hyphens instead of slashes)</returns>
    /// <example>
    /// Input: "Views/Default.cshtml" → Output: "views-default-cshtml"
    /// </example>
    public static string NormalizePath(string templatePath)
    {
        if (string.IsNullOrWhiteSpace(templatePath))
            throw new ArgumentNullException(nameof(templatePath));

        return Path.GetFileName(templatePath)
            .ToLowerInvariant()
            .Replace('/', '-')
            .Replace('\\', '-')
            .Replace(".cshtml", "-cshtml")
            .Replace('.', '-')
            .Replace(' ', '-')
            .Trim('-');
    }

    internal static string NormalizeEdition(string edition)
    {
        const string root = "root";
        return edition.HasValue()
            ? SanitizeSegment(edition, root)
            : root;
    }

    internal static string GetAppFolder(int appId)
        => $"{appId}";

    internal static string GetEditionFolder(string edition)
        => $"{edition}";

    private static string SanitizeSegment(string value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(value.Select(ch => invalidChars.Contains(ch) ? '-' : ch).ToArray());
        return string.IsNullOrWhiteSpace(cleaned)
            ? fallback
            : cleaned;
    }

    #region Equality Members

    public bool Equals(CacheKey other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return AppId == other.AppId 
            && Edition == other.Edition 
            && NormalizedPath == other.NormalizedPath 
            && ContentHash == other.ContentHash 
            && AppCodeHash == other.AppCodeHash;
    }

    public override bool Equals(object obj) 
        => ReferenceEquals(this, obj) || obj is CacheKey other
            && Equals(other);

    /// <summary>
    /// Provide a fast, well-distributed hash for CacheKey so it can be used in hash-based collections (Dictionary, HashSet) consistent with Equals.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        // How it works:
        // - Seeds with AppId(an int), then mixes in all other fields with a prime multiplier 397 and XOR.
        // - unchecked avoids overflow exceptions during arithmetic(overflow is fine for hash codes).
        // - Uses all identity fields: AppId, Edition, NormalizedPath, ContentHash, AppCodeHash.
        //   This matches the Equals implementation, preserving the contract: equal objects must produce equal hash codes.
        unchecked
        {
            var hashCode = AppId;
            hashCode = (hashCode * 397) ^ Edition.GetHashCode();
            hashCode = (hashCode * 397) ^ NormalizedPath.GetHashCode();
            hashCode = (hashCode * 397) ^ ContentHash.GetHashCode();
            hashCode = (hashCode * 397) ^ AppCodeHash.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(CacheKey left, CacheKey right)
        => Equals(left, right);

    public static bool operator !=(CacheKey left, CacheKey right)
        => !Equals(left, right);

    #endregion
}
