using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Oqt.Server.Adam;

/// <summary>
/// Casting helpers so DNN code can work with the file / folder in the DNN signature
/// </summary>
internal static class OqtAdam
{
    internal static File<int, int> AsOqt(this IFile file)
    {
        if (file == null) return null;
        if (file is not File<int, int> recast) throw new("Tried to cast IFile to internal type, failed");
        return recast;
    }

    internal static Folder<int, int> AsOqt(this IFolder folder)
    {
        if (folder == null) return null;
        if (folder is not Folder<int, int> recast) throw new("Tried to cast IFolder to internal type, failed");
        return recast;
    }
}