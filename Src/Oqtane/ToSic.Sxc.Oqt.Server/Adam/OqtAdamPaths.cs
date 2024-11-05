using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Adam;

/// <summary>
/// Basic AdamPaths resolver, assumes that files are in Content/[tenant]/site/[site]/adam for now
/// </summary>
internal class OqtAdamPaths(IServerPaths serverPaths, AliasResolver aliasResolver)
    : AdamPathsBase(serverPaths, OqtConstants.OqtLogPrefix)
{
    public string Path(string path)
    {
        var original = base.Url(path);
        return original.PrefixSlash();
    }

    public override string Url(string path)
    {
        // Convert path to url.
        string url = path.ForwardSlash();
        var parts = url.Split("/").ToList();
        if (parts[0] == "adam")
        {
            // Swap 'adam' and appName.
            parts[0] = parts[1];
            parts[1] = "adam";

            // Insert alias path.
            var alias = aliasResolver.Alias;
            var aliasPath = alias.Path;
            parts.Insert(0, $"{aliasPath}/app");

            // Build url string.
            url = string.Join('/', parts.ToArray());
        }

        return url.PrefixSlash();
    }
}