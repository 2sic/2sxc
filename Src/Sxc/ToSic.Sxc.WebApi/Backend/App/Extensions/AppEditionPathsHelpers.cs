using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

public static class AppEditionPathsHelpers
{
    internal static string GetEditionRoot(this IAppPaths appPaths, string edition)
        => GetEditionRoot(appPaths.PhysicalPath, edition);

    internal static string GetEditionRoot(string appRoot, string edition)
        => edition.HasValue()
            ? Path.Combine(appRoot, edition)
            : appRoot;

    internal static List<string> NormalizeEditionsCsvOrThrow(string? editions)
    {
        if (editions.IsEmptyOrWs())
            return [""];

        var segments = editions.CsvToArrayPreserveEmpty();

        if (segments.Length == 0)
            return [""];

        var normalized = segments
            .Select(NormalizeEditionNameOrThrow)
            .ToList();

        return normalized
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Normalize an edition segment and guard against path traversal.
    /// </summary>
    internal static string NormalizeEditionNameOrThrow(string? edition)
    {
        if (edition.IsEmpty())
            return "";

        var normalized = edition.Trim().TrimPrefixSlash().TrimLastSlash();
        return normalized.ContainsPathTraversal()
            ? throw new ArgumentException(@"edition contains invalid path traversal", nameof(edition))
            : normalized;
    }
}