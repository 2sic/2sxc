using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Apps.Sys.Paths;

public class GlobalPaths(LazySvc<IServerPaths> serverPaths, LazySvc<IGlobalConfiguration> config)
    : ServiceBase("Viw.Help", connect: [serverPaths, config])
{

    /// <summary>
    /// Returns the location where module global folder web assets are stored
    /// </summary>
    public string GlobalPathTo(string path, PathTypes pathType)
    {
        var l = Log.Fn<string>($"path:{path},pathType:{pathType}");
        var assetPath = $"{config.Value.AssetsVirtualUrl().TrimLastSlash()}/{path.TrimPrefixSlash()}";
        var assetLocation = pathType switch
        {
            PathTypes.Link => assetPath.ToAbsolutePathForwardSlash(),
            PathTypes.PhysRelative => assetPath.TrimStart('~').ToSystemPath(),
            PathTypes.PhysFull => serverPaths.Value.FullAppPath(assetPath).ToSystemPath(),
            _ => throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null)
        };
        return l.ReturnAsOk(assetLocation);
    }
}
