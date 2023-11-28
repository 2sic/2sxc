using ToSic.Eav.Helpers;

namespace ToSic.Sxc.Oqt.Server.Integration;

internal static class OqtFolderHelper
{
    // ensure backslash on the end of path, but not on the start
    // except for edge case path = string.Empty
    public static string EnsureOqtaneFolderFormat(this string path) => string.IsNullOrEmpty(path) ? path : path.Trim().ForwardSlash().TrimPrefixSlash().TrimLastSlash() + '/';
}