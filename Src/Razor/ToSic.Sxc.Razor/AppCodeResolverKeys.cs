using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Razor;

// Keeps AppCode registration and Razor metadata lookup in sync when Oqtane exposes
// the same app through view paths, app paths, or edition paths.
internal static class AppCodeResolverKeys
{
    private const string AppRootSegment = "2sxc";
    private const string TenantsSegment = "Tenants";
    private const string SitesSegment = "Sites";
    private static readonly char[] Separators = ['\\', '/'];

    /// <summary>
    /// Builds all resolver keys that may identify the AppCode assembly for a view.
    /// </summary>
    public static IReadOnlyList<string> Build(string viewPath, IEnumerable<string?> appPathSeeds)
        => (appPathSeeds ?? [])
            .Concat(AppPathSeedsFromViewPath(viewPath))
            .OfType<string>()
            .Select(seed => Normalize(seed).TrimStart(Path.DirectorySeparatorChar))
            .Where(seed => seed.HasValue())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>
    /// Formats resolver keys for diagnostics with consistent path separators.
    /// </summary>
    public static string Describe(IEnumerable<string> keys)
        => string.Join(";", keys.Select(key => key.ToSystemPath()));

    /// <summary>
    /// Resolves each key through the singleton assembly resolver and records if the physical DLL still exists.
    /// </summary>
    public static IReadOnlyList<AppCodeResolverLookupResult> Resolve(AssemblyResolver assemblyResolver, IEnumerable<string> keys)
        => keys
            .Select(key =>
            {
                var location = assemblyResolver.GetAssemblyLocation(key);
                return new AppCodeResolverLookupResult(key, location, location.HasValue() && File.Exists(location));
            })
            .ToList();

    /// <summary>
    /// Picks the best resolver hit, preferring a location that exists on disk because Roslyn needs a file reference.
    /// </summary>
    public static AppCodeResolverLookupResult? PickBest(IReadOnlyList<AppCodeResolverLookupResult> results)
        => results.FirstOrDefault(result => result.Exists)
            ?? results.FirstOrDefault(result => result.Location.HasValue());

    /// <summary>
    /// Formats lookup results so logs show which keys were tried and why one did or did not match.
    /// </summary>
    public static string DescribeResults(IEnumerable<AppCodeResolverLookupResult> results)
        => string.Join(";", results.Select(result => $"{result.Key.ToSystemPath()}=>{result.Location}|exists:{result.Exists}"));

    /// <summary>
    /// Infers app and edition resolver seeds from a physical or virtual view path.
    /// </summary>
    private static IEnumerable<string> AppPathSeedsFromViewPath(string viewPath)
    {
        // Fallback for cases where the current block context points to one app,
        // but the compiled view path belongs to a shared app.
        var parts = Normalize(viewPath)
            .Trim(Path.DirectorySeparatorChar)
            .Split(Separators, StringSplitOptions.RemoveEmptyEntries);

        var appRootIndex = Array.FindIndex(parts, part => part.Equals(AppRootSegment, StringComparison.OrdinalIgnoreCase));
        if (appRootIndex < 0)
            yield break;

        if (IsOqtaneTenantSitePath(parts, appRootIndex))
        {
            foreach (var seed in AppAndEditionSeeds(parts, appRootIndex + 5))
                yield return seed;
            yield break;
        }

        // Non-Oqtane hosts keep the app folder directly below 2sxc.
        foreach (var seed in AppAndEditionSeeds(parts, appRootIndex + 1))
            yield return seed;
    }

    /// <summary>
    /// Detects the current Oqtane app path shape: 2sxc\Tenants\{tenant}\Sites\{site}\{app}.
    /// </summary>
    private static bool IsOqtaneTenantSitePath(IReadOnlyList<string> parts, int appRootIndex)
        => parts.Count > appRootIndex + 5
            && parts[appRootIndex + 1].Equals(TenantsSegment, StringComparison.OrdinalIgnoreCase)
            && parts[appRootIndex + 3].Equals(SitesSegment, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Produces the app root key and, when present, the edition-specific key.
    /// </summary>
    private static IEnumerable<string> AppAndEditionSeeds(IReadOnlyList<string> parts, int appIndex)
    {
        if (parts.Count <= appIndex)
            yield break;

        yield return JoinThrough(parts, appIndex);

        var editionIndex = appIndex + 1;
        if (parts.Count > editionIndex && !Path.HasExtension(parts[editionIndex]))
            yield return JoinThrough(parts, editionIndex);
    }

    /// <summary>
    /// Joins path parts from the beginning through the requested segment index.
    /// </summary>
    private static string JoinThrough(IEnumerable<string> parts, int index)
        => string.Join(Path.DirectorySeparatorChar.ToString(), parts.Take(index + 1));

    /// <summary>
    /// Normalizes path separators and trims whitespace before key comparison or lookup.
    /// </summary>
    private static string Normalize(string key)
        => key.ToSystemPath().Trim();
}

internal sealed record AppCodeResolverLookupResult(string Key, string? Location, bool Exists);
