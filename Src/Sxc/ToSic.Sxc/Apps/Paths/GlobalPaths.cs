using System;
using ToSic.Eav.Configuration;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Engines;
using static System.IO.Path;

namespace ToSic.Sxc.Apps.Paths
{
    public class GlobalPaths: HasLog<GlobalPaths>
    {
        #region Constructor / DI

        public GlobalPaths(Lazy<IServerPaths> serverPaths, Lazy<IGlobalConfiguration> config): base("Viw.Help")
        {
            _serverPaths = serverPaths;
            _config = config;
        }
        private readonly Lazy<IServerPaths> _serverPaths;
        private readonly Lazy<IGlobalConfiguration> _config;

        #endregion
        

        /// <summary>
        /// Returns the location where module global folder web assets are stored
        /// </summary>
        public string SxcAssetsLocationWipMoveOut(string path, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"path:{path},pathType:{pathType}");
            var assetPath = Combine(_config.Value.AssetsVirtualUrl.Backslash(), path);
            string assetLocation;
            switch (pathType)
            {
                case PathTypes.Link:
                    assetLocation = assetPath.ToAbsolutePathForwardSlash();
                    break;
                case PathTypes.PhysRelative:
                    assetLocation = assetPath.TrimStart('~').Backslash();
                    break;
                case PathTypes.PhysFull:
                    assetLocation = _serverPaths.Value.FullAppPath(assetPath).Backslash();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }
            return wrapLog("ok", assetLocation);
        }
    }
}