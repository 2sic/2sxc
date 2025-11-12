using ToSic.Eav.Security.Files;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class ExtensionFolderNameValidator
{
    public static bool IsValid(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (name.Contains(Path.DirectorySeparatorChar) || name.Contains(Path.AltDirectorySeparatorChar)) return false;
        if (name.Equals(".") || name.Equals("..") || name.Contains("..")) return false;
        if (name != Path.GetFileName(name)) return false;

        var sanitized = FileNames.SanitizeFileName(name);
        if (string.IsNullOrWhiteSpace(sanitized) || sanitized == FileNames.SafeChar) return false;
        if (!string.Equals(sanitized, name, StringComparison.Ordinal)) return false;
        if (FileNames.IsKnownRiskyExtension(name)) return false;

        return true;
    }
}
