using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.LookUp;
using EavApp = ToSic.Eav.Apps.App;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    /// <summary>
    /// A <em>single-use</em> app-object providing quick simple api to access
    /// name, folder, data, metadata etc.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public partial class App : EavApp, IApp
    {
        #region DI Constructors
        [PrivateApi]
        public App(MyServices services, 
            LazySvc<GlobalPaths> globalPaths, 
            LazySvc<AppPaths> appPathsLazy, 
            LazySvc<DynamicEntity.MyServices> dynamicEntityDependenciesLazy,
            Generator<IAppStates> appStates,
            Generator<AppConfigDelegate> appConfigDelegate) 
            : base(services, "App.SxcApp")
        {
            ConnectServices(
                _globalPaths = globalPaths,
                _appPathsLazy = appPathsLazy,
                _dynEntSvcLazy = dynamicEntityDependenciesLazy,
                _appStates = appStates,
                _appConfigDelegate = appConfigDelegate
            );
        }

        private readonly LazySvc<GlobalPaths> _globalPaths;
        private readonly LazySvc<AppPaths> _appPathsLazy;
        private readonly LazySvc<DynamicEntity.MyServices> _dynEntSvcLazy;
        private readonly Generator<IAppStates> _appStates;
        private readonly Generator<AppConfigDelegate> _appConfigDelegate;

        private AppPaths AppPaths => _appPaths.Get(() => _appPathsLazy.Value.Init(Site, AppState));
        private readonly GetOnce<AppPaths> _appPaths = new GetOnce<AppPaths>();

        #endregion


        #region Paths

        /// <inheritdoc />
        public string Path => _path.Get(() => AppPaths.Path);
        private readonly GetOnce<string> _path = new GetOnce<string>();

        /// <inheritdoc />
        public string Thumbnail
        {
            get
            {
                if (_thumbnail != null) return _thumbnail;

                // Primary app - we only PiggyBack cache the icon in this case
                // Because otherwise the icon could get moved, and people would have a hard time seeing the effect
                if (NameId == Eav.Constants.PrimaryAppGuid)
                    return _thumbnail = AppState.GetPiggyBack(nameof(Thumbnail), 
                        () => _globalPaths.Value.GlobalPathTo(AppConstants.AppPrimaryIconFile, PathTypes.Link));

                // standard app (not global) try to find app-icon in its (portal) app folder
                if (!AppState.IsShared())
                    if (File.Exists(PhysicalPath + "/" + AppConstants.AppIconFile))
                        return _thumbnail = Path + "/" + AppConstants.AppIconFile;

                // global app (and standard app without app-icon in its portal folder) looks for app-icon in global shared location 
                if (File.Exists(PhysicalPathShared + "/" + AppConstants.AppIconFile))
                    return _thumbnail = PathShared + "/" + AppConstants.AppIconFile;

                return null;
            }
        }
        private string _thumbnail;

        /// <inheritdoc />
        public string PathShared => _pathShared.Get(() => AppPaths.PathShared);
        private readonly GetOnce<string> _pathShared = new GetOnce<string>();

        /// <inheritdoc />
        public string PhysicalPathShared => _physicalPathGlobal.Get(() => AppPaths.PhysicalPathShared);
        private readonly GetOnce<string> _physicalPathGlobal = new GetOnce<string>();

        [PrivateApi("not public, not sure if we should surface this")]
        public string RelativePath => _relativePath.Get(() => AppPaths.RelativePath);
        private readonly GetOnce<string> _relativePath = new GetOnce<string>();


        [PrivateApi("not public, not sure if we should surface this")]
        public string RelativePathShared => _relativePathShared.Get(() => AppPaths.RelativePathShared);
        private readonly GetOnce<string> _relativePathShared = new GetOnce<string>();


        #endregion


    }
}