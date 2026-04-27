using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Razor;

internal static class AppCodeResolverKeys
{
    private const char Separator = '\\';
    private const string AppRootSegment = "2sxc";
    private const string TenantsSegment = "Tenants";
    private const string SitesSegment = "Sites";
    private const string SharedSegment = "Shared";
    private static readonly char[] Separators = ['\\', '/'];

    public static IReadOnlyList<string> Build(string viewPath, IEnumerable<string?> appPathSeeds)
        => Seeds(viewPath, appPathSeeds)
            .Where(seed => seed.HasValue())
            .Select(seed => Normalize(seed!))
            .SelectMany(Variants)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    public static string Describe(IEnumerable<string> keys)
        => string.Join(";", keys.Select(key => key.Backslash()));

    public static IReadOnlyList<AppCodeResolverLookupResult> Resolve(AssemblyResolver assemblyResolver, IEnumerable<string> keys)
        => keys
            .Select(key =>
            {
                var location = assemblyResolver.GetAssemblyLocation(key);
                return new AppCodeResolverLookupResult(key, location, location.HasValue() && File.Exists(location));
            })
            .ToList();

    public static AppCodeResolverLookupResult? PickBest(IReadOnlyList<AppCodeResolverLookupResult> results)
        => results.FirstOrDefault(result => result.Exists)
            ?? results.FirstOrDefault(result => result.Location.HasValue());

    public static string DescribeResults(IEnumerable<AppCodeResolverLookupResult> results)
        => string.Join(";", results.Select(result => $"{result.Key.Backslash()}=>{result.Location}|exists:{result.Exists}"));

    private static IEnumerable<string?> Seeds(string viewPath, IEnumerable<string?> appPathSeeds)
    {
        foreach (var seed in appPathSeeds ?? [])
            yield return seed;

        foreach (var seed in AppPathSeedsFromViewPath(viewPath))
            yield return seed;
    }

    private static IEnumerable<string> AppPathSeedsFromViewPath(string viewPath)
    {
        var parts = Normalize(viewPath)
            .Trim(Separator)
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

        var appIndex = IsOqtaneLegacySitePath(parts, appRootIndex)
            ? appRootIndex + 2
            : appRootIndex + 1;
        foreach (var seed in AppAndEditionSeeds(parts, appIndex))
            yield return seed;
    }

    private static bool IsOqtaneTenantSitePath(IReadOnlyList<string> parts, int appRootIndex)
        => parts.Count > appRootIndex + 5
            && parts[appRootIndex + 1].Equals(TenantsSegment, StringComparison.OrdinalIgnoreCase)
            && parts[appRootIndex + 3].Equals(SitesSegment, StringComparison.OrdinalIgnoreCase);

    private static bool IsOqtaneLegacySitePath(IReadOnlyList<string> parts, int appRootIndex)
        => parts.Count > appRootIndex + 2
            && (int.TryParse(parts[appRootIndex + 1], out _)
                || parts[appRootIndex + 1].Equals(SharedSegment, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<string> AppAndEditionSeeds(IReadOnlyList<string> parts, int appIndex)
    {
        if (parts.Count <= appIndex)
            yield break;

        yield return JoinThrough(parts, appIndex);

        var editionIndex = appIndex + 1;
        if (parts.Count > editionIndex && !Path.HasExtension(parts[editionIndex]))
            yield return JoinThrough(parts, editionIndex);
    }

    private static string JoinThrough(IEnumerable<string> parts, int index)
        => string.Join(Separator.ToString(), parts.Take(index + 1));

    private static IEnumerable<string> Variants(string key)
    {
        var normalized = Normalize(key).TrimStart(Separator);
        if (!normalized.HasValue())
            yield break;

        yield return normalized;
        yield return $"{Separator}{normalized}";
    }

    private static string Normalize(string key)
        => key.Backslash().Trim();
}

internal sealed record AppCodeResolverLookupResult(string Key, string? Location, bool Exists);
