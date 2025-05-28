using ToSic.Eav.Internal.Configuration;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using static System.IO.Path;

namespace ToSic.Eav.Internal.Environment;

public class GlobalPaths(LazySvc<IServerPaths> serverPaths, LazySvc<IGlobalConfiguration> config)
    : ServiceBase("Viw.Help", connect: [serverPaths, config])
{

    /// <summary>
    /// Returns the location where module global folder web assets are stored
    /// </summary>
    public string GlobalPathTo(string path, PathTypes pathType)
    {
        var l = Log.Fn<string>($"path:{path},pathType:{pathType}");
        var assetPath = Combine(config.Value.AssetsVirtualUrl().Backslash(), path);
        var assetLocation = pathType switch
        {
            PathTypes.Link => assetPath.ToAbsolutePathForwardSlash(),
            PathTypes.PhysRelative => assetPath.TrimStart('~').Backslash(),
            PathTypes.PhysFull => serverPaths.Value.FullAppPath(assetPath).Backslash(),
            _ => throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null)
        };
        return l.ReturnAsOk(assetLocation);
    }
}