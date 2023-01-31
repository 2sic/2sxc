using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Lib.Logging;
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
        public App(AppDependencies dependencies, 
            LazySvc<GlobalPaths> globalPaths, 
            LazySvc<AppPaths> appPathsLazy, 
            LazySvc<DynamicEntityDependencies> dynamicEntityDependenciesLazy,
            Generator<IAppStates> appStates,
            Generator<AppConfigDelegate> appConfigDelegate) 
            : base(dependencies, "App.SxcApp")
        {
            this.ConnectServices(
                _globalPaths = globalPaths,
                _appPathsLazy = appPathsLazy,
                _dynamicEntityDependenciesLazy = dynamicEntityDependenciesLazy,
                _appStates = appStates,
                _appConfigDelegate = appConfigDelegate
            );
        }

        private readonly LazySvc<GlobalPaths> _globalPaths;
        private readonly LazySvc<AppPaths> _appPathsLazy;
        private readonly LazySvc<DynamicEntityDependencies> _dynamicEntityDependenciesLazy;
        private readonly Generator<IAppStates> _appStates;
        private readonly Generator<AppConfigDelegate> _appConfigDelegate;

        private AppPaths AppPaths => _appPaths.Get(() => _appPathsLazy.Value.Init(Site, AppState));
        private readonly GetOnce<AppPaths> _appPaths = new GetOnce<AppPaths>();

        [PrivateApi]
        public App PreInit(ISite site)
        {
            Site = site;
            return this;
        }

        /// <summary>
        /// Main constructor which auto-configures the app-data
        /// </summary>
        [PrivateApi]
        public new App Init(IAppIdentity appIdentity, Func<EavApp, IAppDataConfiguration> buildConfig)
        {
            base.Init(appIdentity, buildConfig);
            if (buildConfig != null) return this;
            Log.A("App only initialized for light use - .Data shouldn't be used");
            return this;
        }

        #endregion


        #region Dynamic Properties: Configuration, Settings, Resources
        /// <inheritdoc />
        public AppConfiguration Configuration
            // Create config object. Note that AppConfiguration could be null, then it would use default values
            => _appConfig.Get(() => new AppConfiguration(AppConfiguration, Log));
        private readonly GetOnce<AppConfiguration> _appConfig = new GetOnce<AppConfiguration>();

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
            => _dynamicEntityDependencies.Get(() =>
                _dynamicEntityDependenciesLazy.Value.Init(null, Site.SafeLanguagePriorityCodes(), Log));
        private readonly GetOnce<DynamicEntityDependencies> _dynamicEntityDependencies = new GetOnce<DynamicEntityDependencies>();

        /// <inheritdoc />
        public dynamic Settings => AppSettings != null ? _settings.Get(() => MakeDynProperty(AppSettings)) : null;
        private readonly GetOnce<dynamic> _settings = new GetOnce<dynamic>();

        /// <inheritdoc />
        public dynamic Resources => AppResources != null ? _res.Get(() => MakeDynProperty(AppResources)) : null;
        private readonly GetOnce<dynamic> _res = new GetOnce<dynamic>();

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