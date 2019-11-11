using System;
using System.Threading;
using System.Web;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.SexyContent
{
    /// <summary>
    /// The app class gives access to the App-object - for the data and things like the App:Path placeholder in a template
    /// </summary>
    public class App : Eav.Apps.App, Sxc.Apps.IApp
    {
        #region Dynamic Properties: Configuration, Settings, Resources
        public dynamic Configuration
        {
            get
            {
                if(!_configLoaded && AppMetadata != null)
                    _config = new DynamicEntity(AppMetadata, new[] {Thread.CurrentThread.CurrentCulture.Name}, null);
                _configLoaded = true;
                return _config;
            }
        }
        private bool _configLoaded;
        private dynamic _config;

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

        #region App-Level ViewManager, BlocksManager, EavContext --> must move to EAV some time


        private BlocksManager _blocksManager;
        public BlocksManager BlocksManager => _blocksManager 
            ?? (_blocksManager = new BlocksManager(ZoneId, AppId, ShowDraftsInData, VersioningEnabled, Log));

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