using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Eav.Internal.Environment;

namespace ToSic.Sxc.Adam.Paths.Internal;

/// <summary>
/// Basic AdamPaths resolver, assumes that files are in wwwroot/adam for now.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamPathsWwwroot(IServerPaths serverPaths) : AdamPathsBase(serverPaths, LogScopes.Base)
{
    /// <summary>
    /// This will just assume that the path - containing 'wwwroot' will not have the 'wwwroot' in the link from outside
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public override string Url(string path)
    {
        var original = base.Url(path);
        var url = "/" + original.Replace("wwwroot/", "");
        return url;
    }
}