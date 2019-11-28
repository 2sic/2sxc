using System;
using System.Threading;
using System.Web;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;
using ToSic.Eav.Logging;
using ToSic.SexyContent;

namespace ToSic.Sxc.Apps
{
    /// <summary>
    /// The app class gives access to the App-object - for the data and things like the App:Path placeholder in a template
    /// </summary>
    public class App : Eav.Apps.App, ToSic.Sxc.Apps.IApp
    {
        #region Dynamic Properties: Configuration, Settings, Resources
        public AppConfiguration Configuration => _appConfig
                                                 // Create config object. Note that AppConfiguration could be null, then it would use default values
                                                 ?? (_appConfig = new AppConfiguration(AppConfiguration, Log));

        //private bool _configLoaded;
        //private dynamic _config;
        private AppConfiguration _appConfig;

        [PrivateApi("obsolete, use the typed accessor instead, only included for old-compatibility")]
        [Obsolete("use the new, typed accessor instead")]
        dynamic SexyContent.Interfaces.IApp.Configuration
        {
            get
            {
                var c = Configuration;
                if (c?.Entity != null) return new DynamicEntity(c.Entity, new[] {Thread.CurrentThread.CurrentCulture.Name}, null);
                return null;
            }
        }

        public dynamic Settings
        {
            get
            {
                if(!_settingsLoaded && AppSettings != null)
                    _settings = new DynamicEntity(AppSettings, new[] {Thread.CurrentThread.CurrentCulture.Name}, null);
                _settingsLoaded = true;
                return _settings;
            }
        }
        private bool _settingsLoaded;
        private dynamic _settings;

        public dynamic Resources
        {
            get
            {
                if(!_resLoaded && AppResources!= null)
                    _res = new DynamicEntity(AppResources, new[] {Thread.CurrentThread.CurrentCulture.Name}, null);
                _resLoaded = true;
                return _res;
            }
        }
        private bool _resLoaded;
        private dynamic _res;

        #endregion


        /// <summary>
        /// New constructor which auto-configures the app-data
        /// </summary>
        public App(ITenant tenant, 
            int zoneId, 
            int appId, 
            Func<Eav.Apps.App, IAppDataConfiguration> buildConfig, 
            bool allowSideEffects, 
            ILog parentLog = null)
            : base(tenant, zoneId, appId, allowSideEffects, buildConfig, parentLog) { }

        #region Paths
        public string Path => VirtualPathUtility.ToAbsolute(GetRootPath());
        public string Thumbnail => System.IO.File.Exists(PhysicalPath + IconFile) ? Path + IconFile : null;

        #endregion

        
    }
}