using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Data;
using ToSic.Sxc.Run;
using EavApp = ToSic.Eav.Apps.App;

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

        public App(AppDependencies dependencies, ILinkPaths linkPaths) : base(dependencies, "App.SxcApp")
        {
            _linkPaths = linkPaths;
        }

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
            return this;
        }

        /// <summary>
        /// Quick init - won't provide data but can access properties, metadata etc.
        /// </summary>
        /// <param name="appIdentity"></param>
        /// <param name="parentLog"></param>
        /// <returns></returns>
        public App InitNoData(IAppIdentity appIdentity, ILog parentLog)
        {
            Init(appIdentity, null, parentLog);
            Log.Rename("App.SxcLgt");
            Log.Add("App only initialized for light use - data shouldn't be used");
            return this;
        }

        #endregion


        private readonly ILinkPaths _linkPaths;

        #region Dynamic Properties: Configuration, Settings, Resources
        /// <inheritdoc />
        public AppConfiguration Configuration => _appConfig
                                                 // Create config object. Note that AppConfiguration could be null, then it would use default values
                                                 ?? (_appConfig = new AppConfiguration(AppConfiguration, Log));

        private AppConfiguration _appConfig;

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

        private dynamic MakeDynProperty(IEntity contents) =>
            new DynamicEntity(contents, Site.SafeLanguagePriorityCodes(), 10, null, DataSourceFactory.ServiceProvider);

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
        public string Path => _path ?? (_path = Site.AppAssetsLinkTemplate
            .Replace(LinkPaths.AppFolderPlaceholder, Folder)
            .ToAbsolutePathForwardSlash()); // // .AppAsset(System.IO.Path.Combine(Site.AppsRootLink, Folder)));
        private string _path;

        /// <inheritdoc />
        public string Thumbnail => File.Exists(PhysicalPath + IconFile) ? Path + IconFile : null;

        #endregion


    }
}