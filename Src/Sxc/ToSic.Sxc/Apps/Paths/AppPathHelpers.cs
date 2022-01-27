using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Apps.Paths
{
    public class AppPathHelpers: HasLog<AppPathHelpers>
    {
        #region Constructor / DI

        public AppPathHelpers(Lazy<IServerPaths> serverPaths, Lazy<IGlobalConfiguration> config): base("Viw.Help")
        {
            _serverPaths = serverPaths;
            _config = config;
        }
        private readonly Lazy<IServerPaths> _serverPaths;
        private readonly Lazy<IGlobalConfiguration> _config;

        #endregion


        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public string AppPathRoot(ISite site, AppState appState, bool shared, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"{shared}, {pathType}");
            string path;
            switch (pathType)
            {
                case PathTypes.Link:
                    path = shared ? LinkShared(appState) : Link(site, appState);
                    break;
                case PathTypes.PhysRelative:
                    path = shared ? PathRelativeShared(appState) : PathRelative(site, appState);
                    break;
                case PathTypes.PhysFull:
                    path = shared ? PathFullShared(appState) : PathFull(site, appState);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }

            return wrapLog(path, path);
        }

        public string Link(ISite site, AppState appState) =>
            site.AppAssetsLinkTemplate.Replace(AppConstants.AppFolderPlaceholder, appState.Folder)
                .ToAbsolutePathForwardSlash();

        public string LinkShared(AppState appState) =>
            Path.Combine(_config.Value.GlobalSiteFolder, Settings.AppsRootFolder, appState.Folder)
                .ToAbsolutePathForwardSlash();

        public string PathRelativeShared(AppState appState) 
            => Path.Combine(_config.Value.GlobalSiteFolder, Settings.AppsRootFolder, appState.Folder).ToAbsolutePathForwardSlash();

        public string PathRelative(ISite site, AppState appState) 
            => Path.Combine(site.AppsRootPhysical, appState.Folder);

        public string PathFullShared(AppState appState) 
            => _serverPaths.Value.FullAppPath(Path.Combine(_config.Value.GlobalSiteFolder, Settings.AppsRootFolder, appState.Folder));

        public string PathFull(ISite site, AppState appState) 
            => Path.Combine(site.AppsRootPhysicalFull, appState.Folder);


        /// <summary>
        /// Returns the location where module global folder web assets are stored
        /// </summary>
        public string AssetsLocation(string path, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"path:{path},pathType:{pathType}");
            var assetPath = Path.Combine(_config.Value.AssetsVirtualUrl.Backslash(), path);
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