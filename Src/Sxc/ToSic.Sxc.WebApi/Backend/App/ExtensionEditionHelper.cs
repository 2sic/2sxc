using ToSic.Eav.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Shared helpers for resolving extension-related paths and edition segments.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class ExtensionEditionHelper
{
    internal static List<string> NormalizeEditions(string[]? editions)
    {
        var segments = (editions ?? Array.Empty<string>())
            .SelectMany(raw =>
            {
                // Split comma-delimited entries so DNN/webforms can send editions=staging,live
                if (raw == null)
                    return new[] { string.Empty };
                return raw.Split(new[] { ',' }, StringSplitOptions.None);
            });

        var normalized = segments
            .Select(NormalizeEdition)
            .ToList();

        if (normalized.Count == 0)
            normalized.Add(string.Empty);

        return normalized.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>
    /// Normalize an edition segment and guard against path traversal.
    /// </summary>
    internal static string NormalizeEdition(string? edition)
    {
        if (edition.IsEmpty())
            return string.Empty;

        var normalized = edition.Trim().TrimPrefixSlash().TrimLastSlash();
        return normalized.ContainsPathTraversal()
            ? throw new ArgumentException("edition contains invalid path traversal", nameof(edition))
            : normalized;
    }

    internal static List<string> MergeEditions(List<string> requested, List<string> installed)
        => requested
            .Concat(installed)
            .Where(e => e != null)
            .Select(e => e!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    internal static string GetEditionRoot(IAppPaths appPaths, string edition)
        => GetEditionRoot(appPaths.PhysicalPath, edition);

    internal static string GetEditionRoot(string appRoot, string edition)
        => edition.HasValue()
            ? Path.Combine(appRoot, edition)
            : appRoot;

    internal static string GetExtensionRoot(IAppPaths appPaths, string extensionName, string edition)
        => GetExtensionRoot(appPaths.PhysicalPath, extensionName, edition);

    internal static string GetExtensionRoot(string appRoot, string extensionName, string edition)
        => Path.Combine(GetEditionRoot(appRoot, edition), FolderConstants.AppExtensionsFolder, extensionName);

    internal static string GetExtensionAppCodePath(string appRoot, string extensionName, string edition)
    {
        var editionAppCode = Path.Combine(appRoot, edition, FolderConstants.AppCodeFolder);
        return Directory.Exists(editionAppCode)
            ? Path.Combine(editionAppCode, FolderConstants.AppExtensionsFolder, extensionName)
            : Path.Combine(appRoot, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, extensionName);
    }

    internal static List<string> DetectInstalledEditions(string appRoot, List<string> availableEditions, string extensionName)
    {
        var list = new List<string>();
        var rootPath = Path.Combine(appRoot, FolderConstants.AppExtensionsFolder, extensionName);
        if (Directory.Exists(rootPath))
            list.Add(string.Empty);

        foreach (var dir in Directory.GetDirectories(appRoot))
        {
            var name = Path.GetFileName(dir);
            if (!availableEditions.Contains(name, StringComparer.OrdinalIgnoreCase))
                continue;
            var editionExtPath = Path.Combine(dir, FolderConstants.AppExtensionsFolder, extensionName);
            if (Directory.Exists(editionExtPath))
                list.Add(name);
        }

        return list;
    }
}
