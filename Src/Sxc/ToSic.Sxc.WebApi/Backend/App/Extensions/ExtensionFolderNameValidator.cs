using ToSic.Eav.Security.Files;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class ExtensionFolderNameValidator
{
    public static bool IsValid(string name)
    {
        // Not empty
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // No path characters or structures or traversal
        if (name.Contains(Path.DirectorySeparatorChar) || name.Contains(Path.AltDirectorySeparatorChar))
            return false;
        if (name.Equals(".") || name.Contains(".."))
            return false;
        if (name != Path.GetFileName(name))
            return false;

        // No spaces anywhere (not expected in extension-names)
        if (name.Contains(' '))
            return false;

        // Sanitization should not change the name or return empty
        var sanitized = FileNames.SanitizeFileName(name);
        if (string.IsNullOrWhiteSpace(sanitized) || sanitized == FileNames.SafeChar)
            return false;
        if (!string.Equals(sanitized, name, StringComparison.Ordinal))
            return false;

        // No risky extensions
        if (FileNames.IsKnownRiskyExtension(name))
            return false;

        return true;
    }
}
