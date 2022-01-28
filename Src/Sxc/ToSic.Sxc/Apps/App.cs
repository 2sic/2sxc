using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;
using EavApp = ToSic.Eav.Apps.App;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    /// <summary>
    /// A <em>single-use</em> app-object providing quick simple api to access
    /// name, folder, data, metadata etc.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class App : EavApp, IApp
    {
        #region DI Constructors

        public App(AppDependencies dependencies, Lazy<GlobalPaths> globalPaths, Lazy<AppPaths> appPathsLazy) : base(dependencies, "App.SxcApp")
        {
            _globalPaths = globalPaths;
            _appPathsLazy = appPathsLazy;
        }

        private readonly Lazy<GlobalPaths> _globalPaths;
        private readonly Lazy<AppPaths> _appPathsLazy;
        private AppPaths _appPaths;
        private AppPaths AppPaths => _appPaths ?? (_appPaths = _appPathsLazy.Value.Init(Site, AppState, Log));

        public App PreInit(ISite site)
        {
            Site = site;
            return this;
        }

        /// <summary>
        /// Main constructor which auto-configures the app-data
        /// </summary>
        [PrivateApi]
        public new App Init(IAppIdentity appId, Func<EavApp, IAppDataConfiguration> buildConfig, ILog parentLog)
        {
            base.Init(appId, buildConfig, parentLog);
            if (buildConfig != null) return this;
            Log.Rename("App.SxcLgt");
            Log.Add("App only initialized for light use - .Data shouldn't be used");
            return this;
        }

        #endregion


        #region Dynamic Properties: Configuration, Settings, Resources
        /// <inheritdoc />
        public AppConfiguration Configuration
            // Create config object. Note that AppConfiguration could be null, then it would use default values
            => _appConfig ?? (_appConfig = new AppConfiguration(AppConfiguration, Log));

        private AppConfiguration _appConfig;

#if NETFRAMEWORK
        [PrivateApi("obsolete, use the typed accessor instead, only included for old-compatibility")]
        [Obsolete("use the new, typed accessor instead")]
        dynamic SexyContent.Interfaces.IApp.Configuration
        {
            get
            {
                var c = Configuration;
                return c?.Entity != null ? MakeDynProperty(c.Entity) : null;
            }
        }
#endif
        private dynamic MakeDynProperty(IEntity contents) => new DynamicEntity(contents, DynamicEntityDependencies);

        // TODO: THIS CAN PROBABLY BE IMPROVED
        // TO GET THE DynamicEntityDependencies from the DynamicCodeRoot which creates the App...? 
        // ATM it's a bit limited, for example it probably cannot resolve links
        private DynamicEntityDependencies DynamicEntityDependencies
            => _dynamicEntityDependencies ?? (_dynamicEntityDependencies = DataSourceFactory.ServiceProvider
                .Build<DynamicEntityDependencies>().Init(null, Site.SafeLanguagePriorityCodes(), Log));
        private DynamicEntityDependencies _dynamicEntityDependencies;

        /// <inheritdoc />
        public dynamic Settings
        {
            get
            {
                if (!_settingsLoaded && AppSettings != null) _settings = MakeDynProperty(AppSettings);
                _settingsLoaded = true;
                return _settings;
            }
        }
        private bool _settingsLoaded;
        private dynamic _settings;

        /// <inheritdoc />
        public dynamic Resources
        {
            get
            {
                if (!_resLoaded && AppResources != null) _res = MakeDynProperty(AppResources);
                _resLoaded = true;
                return _res;
            }
        }
        private bool _resLoaded;
        private dynamic _res;

        #endregion


        #region Paths

        /// <inheritdoc />
        public string Path => _path ?? (_path = AppPaths.Path);
        private string _path;

        /// <inheritdoc />
        public string Thumbnail
        {
            get
            {
                if (_thumbnail != null) return _thumbnail;

                // Primary app - we only PiggyBack cache the icon in this case
                // Because otherwise the icon could get moved, and people would have a hard time seeing the effect
                if (NameId == Eav.Constants.PrimaryAppGuid)
                    return _thumbnail = AppState.GetPiggyBack(nameof(Thumbnail), () => _globalPaths.Value.SxcAssetsLocationWipMoveOut(AppConstants.AppPrimaryIconFile, PathTypes.Link));

                // standard app (not global) try to find app-icon in its (portal) app folder
                if (!AppState.IsGlobal())
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
        public string PathShared => _pathShared ?? (_pathShared = AppPaths.PathShared);
        private string _pathShared;

        /// <inheritdoc />
        public string PhysicalPathShared => _physicalPathGlobal ?? (_physicalPathGlobal = AppPaths.PhysicalPathShared);
        private string _physicalPathGlobal;

        [PrivateApi("not public, not sure if we should surface this")]
        public string RelativePath => _relativePath ?? (_relativePath = AppPaths.RelativePath);
        private string _relativePath;

        [PrivateApi("not public, not sure if we should surface this")]
        public string RelativePathShared => _relativePathShared ?? (_relativePathShared = AppPaths.RelativePathShared);
        private string _relativePathShared;


        #endregion


    }
}