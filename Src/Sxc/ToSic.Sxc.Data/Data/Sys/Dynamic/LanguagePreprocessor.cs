using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys.Dynamic;
internal class LanguagePreprocessor
{
    internal static string?[] GetLookupLanguages(string? language, ICodeDataFactory cdf)
    {
        // use the standard dimensions or overload
        var languages = language == null
            ? cdf.Dimensions
            : GetFinalLanguagesList(language, cdf.SiteCultures, cdf.Dimensions);
        return languages;
    }

    /// <summary>
    /// Full logic, as static, testable method
    /// </summary>
    /// <param name="language"></param>
    /// <param name="possibleDims"></param>
    /// <param name="defaultDims"></param>
    /// <returns></returns>
    internal static string?[] GetFinalLanguagesList(string language, List<string> possibleDims, string?[] defaultDims)
    {
        // if nothing specified, use default
        if (language == null! /* paranoid */)
            return defaultDims;

        var languages = language.ToLowerInvariant()
            .Split(',')
            .Select(s => s.Trim())
            .ToArray();

        // expand language codes, e.g.
        // - "en" should become "en-us" if available
        // - "" should become null to signal fallback to default
        var final = languages
            .Select(l =>
            {
                if (l == "")
                    return null;
                // note: availableDims usually has a null-entry at the end
                // note: both l and availableDims are lowerInvariant
                var found = possibleDims.FirstOrDefault(ad => ad?.StartsWith(l) == true);
                return found ?? "not-found";
            })
            .Where(s => s != "not-found")
            .ToArray();

        return final;
    }
}
