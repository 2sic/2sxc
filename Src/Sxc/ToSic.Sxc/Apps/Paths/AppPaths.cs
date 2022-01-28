using System;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using static System.IO.Path;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps.Paths
{
    public class AppPaths: HasLog, IAppPaths
    {
        public AppPaths(Lazy<IServerPaths> serverPaths, Lazy<IGlobalConfiguration> config) : base($"{LogNames.Eav}.AppPth")
        {
            _serverPaths = serverPaths;
            _config = config;
        }

        private readonly Lazy<IServerPaths> _serverPaths;
        private readonly Lazy<IGlobalConfiguration> _config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="site">The site - in some cases the site of the App can be different from the context-site, so it must be passed in</param>
        /// <param name="appState"></param>
        /// <param name="parentLog"></param>
        /// <returns></returns>
        public AppPaths Init(ISite site, AppState appState, ILog parentLog)
        {
            this._site = site;
            this._appState = appState;
            Log.LinkTo(parentLog);
            InitDone = true;
            return this;
        }
        private ISite _site;
        private AppState _appState;
        public bool InitDone;

        public string Path => _appState.GetPiggyBack(nameof(Path), 
            () => _site.AppAssetsLinkTemplate.Replace(AppConstants.AppFolderPlaceholder, _appState.Folder).ToAbsolutePathForwardSlash());

        public string PathShared => _appState.GetPiggyBack(nameof(PathShared), 
            () => Combine(_config.Value.SharedAppsFolder, AppConstants.AppsRootFolder, _appState.Folder)
                .ToAbsolutePathForwardSlash());

        public string PhysicalPath => _appState.GetPiggyBack(nameof(PhysicalPath), 
            () => Combine(_site.AppsRootPhysicalFull, _appState.Folder));

        public string PhysicalPathShared => _appState.GetPiggyBack(nameof(PhysicalPathShared), 
            () => _serverPaths.Value.FullAppPath(Combine(_config.Value.SharedAppsFolder, AppConstants.AppsRootFolder, _appState.Folder)));

        public string RelativePath => _appState.GetPiggyBack(nameof(RelativePath), 
            () => Combine(_site.AppsRootPhysical, _appState.Folder));
        
        public string RelativePathShared => _appState.GetPiggyBack(nameof(RelativePathShared), 
            () => Combine(_config.Value.SharedAppsFolder, AppConstants.AppsRootFolder, _appState.Folder).ToAbsolutePathForwardSlash());
    }
}
