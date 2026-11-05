using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.ContentTypes.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Ancestors;

namespace ToSic.Sxc.Code.Generate.Data;

internal class CSharpModelSpecsContentTypeHelper
{
    internal static List<IContentType> GetSelectedContentTypes(IAppReadContentTypes appContentTypes,
        ICollection<string>? typeNames)
    {
        var selected = GetSelectedContentTypeNameIds(typeNames);

        var types = selected == null
            ? GetContentTypesInDefaultScope(appContentTypes)
            : GetSelectedContentTypes(appContentTypes, selected);
        return types;
    }

    private static List<IContentType> GetContentTypesInDefaultScope(IAppReadContentTypes appContentTypes)
    {
        // Prepare Content Types and add to Specs, so the generators know what is available
        // Generate classes for all types in scope Default
        var types = appContentTypes.ContentTypes
            .OfScope(ScopeConstants.Default)
            .Concat(new List<IContentType?>
                {
                    appContentTypes.TryGetContentType(AppLoadConstants.TypeAppResources),
                    appContentTypes.TryGetContentType(AppLoadConstants.TypeAppSettings),
                }
                .OfType<IContentType>()
            )
            .ToList();

        //appContentTypes.TryGetContentType(AppLoadConstants.TypeAppResources).DoIfNotNull(types.Add);
        //appContentTypes.TryGetContentType(AppLoadConstants.TypeAppSettings).DoIfNotNull(types.Add);

        // Fix Bug introduced in v19.03.01 or so
        // In that UI it accidentally created new content-types called AppResources or AppSettings
        // In the SystemConfiguration scope. These must really be ignored.
        // In addition, to avoid any other naming conflicts, we'll just skip all the names that were already used.
        var typeNames = types
            .Select(t => t.Name)
            .ToList();

        var appConfigTypes = appContentTypes.ContentTypes
            .OfScope(ScopeConstants.SystemConfiguration)
            .Where(ct => !ct.HasAncestor())
            .Where(ct => !typeNames.Contains(ct.Name))
            .ToList();

        //appConfigTypes = appConfigTypes
        //    .Where(ct => !typeNames.Contains(ct.Name))
        //    .ToList();

        return [.. types, .. appConfigTypes];
    }

    private static HashSet<string>? GetSelectedContentTypeNameIds(ICollection<string>? typeNames)
    {
        var selected = typeNames?
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return selected?.Any() == true
            ? selected
            : null;
    }

    private static List<IContentType> GetSelectedContentTypes(IAppReadContentTypes appContentTypes,
        HashSet<string> selected)
    {
        var filtered = appContentTypes.ContentTypes
            .Where(ct => selected.Contains(ct.NameId))
            .ToList();

        return filtered.Any()
            ? filtered
            : []; // No content types matched selection
    }
}
