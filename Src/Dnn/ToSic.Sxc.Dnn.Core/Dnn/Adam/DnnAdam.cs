using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Dnn.Adam;

/// <summary>
/// Casting helpers so DNN code can work with the file / folder in the DNN signature
/// </summary>
internal static class DnnAdam
{
    internal static File<int, int> AsDnn(this IFile file)
    {
        if (file == null) return null;
        if (file is not File<int, int> recast) throw new("Tried to cast IFile to internal type, failed");
        return recast;
    }

    internal static Folder<int, int> AsDnn(this IFolder folder)
    {
        if (folder == null) return null;
        if (folder is not Folder<int, int> recast)
            throw new("Tried to cast IFolder to internal type, failed");
        return recast;

    }
}