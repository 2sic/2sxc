using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Apps.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class AppPathExtensions
{
    public static string PhysicalPathSwitch(this IAppPaths app, bool isShared) =>
        isShared
            ? app.PhysicalPathShared
            : app.PhysicalPath;

    public static string PathSwitch(this IAppPaths app, bool isShared, PathTypes type) =>
        type switch
        {
            PathTypes.PhysFull => isShared ? app.PhysicalPathShared : app.PhysicalPath,
            PathTypes.PhysRelative => isShared ? app.RelativePathShared : app.RelativePath,
            PathTypes.Link => isShared ? app.PathShared : app.Path,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

    // TODO: THIS SHOULD BE inlined in the 2 places it's used, so this function isn't necessary any more
    // Then these helpers should be moved to ToSxc.Eav.Apps.Paths

    public static string ViewPath(this IAppPaths app, IView view, PathTypes type) => Path.Combine(app.PathSwitch(view.IsShared, type), view.Path);
}