using System;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps.Paths
{
    public class AppPaths: HasLog, IAppPaths
    {
        public AppPaths(Lazy<AppPathHelpers> pathHelpers) : base($"{LogNames.Eav}.AppPth") => _helpersLazy = pathHelpers;
        private readonly Lazy<AppPathHelpers> _helpersLazy;
        private AppPathHelpers _helpers;
        private AppPathHelpers Helpers => _helpers ?? (_helpers = _helpersLazy.Value.Init(Log));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="site">The site - in some cases the site of the App can be different from the context-site, so it must be passed in</param>
        /// <param name="appState"></param>
        /// <param name="parentLog"></param>
        /// <returns></returns>
        public AppPaths Init(ISite site, AppState appState, ILog parentLog)
        {
            _site = site;
            _appState = appState;
            Log.LinkTo(parentLog);
            InitDone = true;
            return this;
        }
        private ISite _site;
        private AppState _appState;
        public bool InitDone;

        public string Path => _appState.GetPiggyBack(nameof(Path), () => Helpers.Link(_site, _appState));

        public string PathShared => _appState.GetPiggyBack(nameof(PathShared), () => Helpers.LinkShared(_appState));

        public string PhysicalPath => _appState.GetPiggyBack(nameof(PhysicalPath), () => Helpers.PathFull(_site, _appState));

        public string PhysicalPathShared => _appState.GetPiggyBack(nameof(PhysicalPathShared), () => Helpers.PathFullShared(_appState));

        public string RelativePath => _appState.GetPiggyBack(nameof(RelativePath), () => Helpers.PathRelative(_site, _appState));
        
        public string RelativePathShared => _appState.GetPiggyBack(nameof(RelativePathShared), () => Helpers.PathRelativeShared(_appState));
    }
}
