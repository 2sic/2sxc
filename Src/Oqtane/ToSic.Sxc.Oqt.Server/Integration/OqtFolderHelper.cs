using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Integration;

internal static class OqtFolderHelper
{
    // Ensure forward slash on the end of the Oqtane folder path, but not on the start
    // except for edge case path = string.Empty
    public static string EnsureOqtaneFolderFormat(this string path) => string.IsNullOrEmpty(path) ? path : path.Trim().ForwardSlash().TrimPrefixSlash().TrimLastSlash() + '/';
}
