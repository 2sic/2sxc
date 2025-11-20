using System.Security.Cryptography;
using System.Text;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Internal helper for computing normalized cache key path segments and hashes.
/// Extracted from <see cref="CacheKey"/> to keep the class small and SRP-focused.
/// </summary>
internal static class CacheKeyPathUtils
{
    internal const string RootEdition = "root";

    internal static string NormalizePath(string templatePath, string edition, string appPath = null)
    {
        if (!templatePath.HasValue())
            throw new ArgumentNullException(nameof(templatePath));

        var normalizedTemplatePath = NormalizePathInt(templatePath); // lower-case, forward slashes, trimmed
        var normalizedEdition = NormalizeEdition(edition);
        var normalizedAppPath = NormalizePathInt(appPath);
        var trimmedForAppScope = TrimToAppScope(normalizedTemplatePath, normalizedEdition, normalizedAppPath);
        var sanitizedFullPath = SanitizeRelativePath(trimmedForAppScope);
        var hash = ComputePathHash(normalizedTemplatePath);

        return $"{sanitizedFullPath}-{hash}";
    }

    internal static string GetAppFolder(int appId, string edition)
        => $"{appId:0000}-{NormalizeEdition(edition)}";

    internal static string NormalizeEdition(string edition)
        => edition.HasValue()
            ? SanitizeSegment(edition, RootEdition)
            : RootEdition;

    private static string SanitizeSegment(string value, string fallback)
    {
        if (!value.HasValue())
            return fallback;
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(value.Select(ch => invalidChars.Contains(ch) ? '-' : ch).ToArray());
        return cleaned.HasValue()
            ? cleaned :
            fallback;
    }

    private static string SanitizeRelativePath(string normalizedPath)
    {
        var sanitized = normalizedPath
            .Replace('/', '-')
            .Replace('\\', '-')
            .Replace(".cshtml", "-cshtml")
            .Replace('.', '-')
            .Replace(' ', '-')
            .Trim('-');

        return SanitizeSegment(sanitized, "template");
    }

    private static string ComputePathHash(string normalizedPath)
    {
        var bytes = Encoding.UTF8.GetBytes(normalizedPath);
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(bytes);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hash.Substring(0, 8);
    }

    private static string NormalizePathInt(string path)
    {
        if (!path.HasValue())
            return null;

        return path
            .ForwardSlash()
            .Trim('/')
            .ToLowerInvariant();
    }

    private static string TrimToAppScope(string templatePath, string edition, string appPath)
    {
        var pathInAppScope = TrimAppRoot(templatePath, appPath);

        if (edition.HasValue() && !string.Equals(edition, RootEdition, StringComparison.Ordinal))
        {
            var editionPrefix = $"{edition}/";
            if (pathInAppScope.StartsWith(editionPrefix, StringComparison.Ordinal))
                return pathInAppScope.Substring(editionPrefix.Length);

            var editionSegment = $"/{edition}/";
            var editionIndex = pathInAppScope.IndexOf(editionSegment, StringComparison.Ordinal);
            if (editionIndex >= 0)
                return pathInAppScope.Substring(editionIndex + editionSegment.Length);

            var editionSuffix = $"/{edition}";
            if (string.Equals(pathInAppScope, edition, StringComparison.Ordinal) || pathInAppScope.EndsWith(editionSuffix, StringComparison.Ordinal))
                return string.Empty;
        }

        return pathInAppScope;
    }

    private static string TrimAppRoot(string templatePath, string appPath)
    {
        if (appPath.HasValue())
        {
            if (templatePath.StartsWith(appPath + "/", StringComparison.Ordinal))
                return templatePath.Substring(appPath.Length + 1);

            if (string.Equals(templatePath, appPath, StringComparison.Ordinal))
                return string.Empty;
        }

        // fallback
        const string marker = "/2sxc/";
        var markerIndex = templatePath.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0)
            return templatePath;

        var appFolderStart = markerIndex + marker.Length;
        var appFolderEnd = templatePath.IndexOf('/', appFolderStart);
        if (appFolderEnd < 0)
            return templatePath;

        // Remove everything up to and including the app folder so the remaining path starts at the edition or deeper folders.
        return templatePath.Substring(appFolderEnd + 1);
    }
}
