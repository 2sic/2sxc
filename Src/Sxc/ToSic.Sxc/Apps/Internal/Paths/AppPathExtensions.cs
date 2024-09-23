using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Apps.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class AppPathExtensions
{
    public static string PhysicalPathSwitch(this IAppPaths app, bool isShared) => isShared ? app.PhysicalPathShared : app.PhysicalPath;

    public static string PathSwitch(this IAppPaths app, bool isShared, PathTypes type)
    {
        switch (type)
        {
            case PathTypes.PhysFull:
                return isShared ? app.PhysicalPathShared : app.PhysicalPath;
            case PathTypes.PhysRelative:
                return isShared ? app.RelativePathShared : app.RelativePath;
            case PathTypes.Link:
                return isShared ? app.PathShared : app.Path;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    // TODO: THIS SHOULD BE inlined in the 2 places it's used, so this function isn't necessary any more
    // Then these helpers should be moved to ToSxc.Eav.Apps.Paths

    public static string ViewPath(this IAppPaths app, IView view, PathTypes type) => Path.Combine(app.PathSwitch(view.IsShared, type), view.Path);
}