using ToSic.Eav.Sys;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Shared helpers for resolving extension-related paths and edition segments.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class AppExtensionPathHelpers
{
    
    internal static string GetExtensionRoot(IAppPaths appPaths, string extensionName, string edition)
        => GetExtensionRoot(appPaths.PhysicalPath, extensionName, edition);

    internal static string GetExtensionRoot(string appRoot, string extensionName, string edition)
        => Path.Combine(AppEditionPathsHelpers.GetEditionRoot(appRoot, edition), FolderConstants.AppExtensionsFolder, extensionName);

    internal static string GetExtensionAppCodePath(string appRoot, string extensionName, string edition)
    {
        var editionAppCode = Path.Combine(appRoot, edition, FolderConstants.AppCodeFolder);
        return Directory.Exists(editionAppCode)
            ? Path.Combine(editionAppCode, FolderConstants.AppExtensionsFolder, extensionName)
            : Path.Combine(appRoot, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, extensionName);
    }

    /// <summary>
    /// Check which editions contain a specific extension
    /// </summary>
    /// <param name="appRoot"></param>
    /// <param name="knownEditions"></param>
    /// <param name="extensionName"></param>
    /// <returns></returns>
    internal static List<string> EditionsContainingExtension(string appRoot, List<string> knownEditions, string extensionName)
    {
        // If the root contains the extension, start with a list containing the root (empty) folder
        var extensionInRootPath = Path.Combine(appRoot, FolderConstants.AppExtensionsFolder, extensionName);
        List<string> list = Directory.Exists(extensionInRootPath)
            ? [""]
            : [];

        var editionsContainingExtension = Directory
            .GetDirectories(appRoot)
            .Select(fullPath => new { FullPath = fullPath, FolderName = Path.GetFileName(fullPath) })
            // Keep only folders which are listed in the known editions list
            .Where(pair => knownEditions.Contains(pair.FolderName, StringComparer.OrdinalIgnoreCase))
            // Keep only these which have the /extensions/extensionName folder
            .Where(pair => Directory.Exists(Path.Combine(pair.FullPath, FolderConstants.AppExtensionsFolder, extensionName)))
            .Select(t => t.FolderName);

        return list
            .Concat(editionsContainingExtension)
            .ToList();
    }
}
