using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Engines
{
    public enum PathTypes
    {
        PhysFull,
        PhysRelative,
        Link
    }

    public class AppPathHelpers: HasLog<AppPathHelpers>
    {
        #region Constructor / DI

        public AppPathHelpers(IServerPaths serverPaths, IGlobalConfiguration globalConfiguration): base("Viw.Help")
        {
            ServerPaths = serverPaths;
            _globalConfiguration = globalConfiguration;
        }
        private IServerPaths ServerPaths { get; }
        private readonly IGlobalConfiguration _globalConfiguration;

        #endregion


        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public string AppPathRoot(ISite site, AppState appState, bool global, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"{global}, {pathType}");
            string basePath;
            switch (pathType)
            {
                case PathTypes.Link:
                    basePath = (global
                        ? Path.Combine(_globalConfiguration.GlobalSiteFolder, Settings.AppsRootFolder, appState.Folder)
                        : site.AppAssetsLinkTemplate.Replace(LinkPaths.AppFolderPlaceholder, appState.Folder))
                        .ToAbsolutePathForwardSlash();
                    break;
                case PathTypes.PhysRelative:
                    basePath = global
                        ? Path.Combine(_globalConfiguration.GlobalSiteFolder, Settings.AppsRootFolder, appState.Folder).ToAbsolutePathForwardSlash()
                        : Path.Combine(site.AppsRootPhysical, appState.Folder);
                    break;
                case PathTypes.PhysFull:
                    basePath = global
                        ? ServerPaths.FullAppPath(Path.Combine(_globalConfiguration.GlobalSiteFolder, Settings.AppsRootFolder, appState.Folder))
                        : Path.Combine(site.AppsRootPhysicalFull, appState.Folder);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }

            return wrapLog(basePath, basePath);
        }
        

        /// <summary>
        /// Returns the location where module global folder web assets are stored
        /// </summary>
        public string AssetsLocation(string path, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"path:{path},pathType:{pathType}");
            var assetPath = Path.Combine(_globalConfiguration.AssetsVirtualUrl.Backslash(), path);
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
                    assetLocation = ServerPaths.FullAppPath(assetPath).Backslash();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }
            return wrapLog("ok", assetLocation);
        }
    }
}