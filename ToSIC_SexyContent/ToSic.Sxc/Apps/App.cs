using System;
using System.Threading;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web;
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
                return c?.Entity != null ? new DynamicEntity(c.Entity, 
                    new[] {Thread.CurrentThread.CurrentCulture.Name}, 
                    10, 
                    null) : null;
            }
        }

        /// <inheritdoc />
        public dynamic Settings
        {
            get
            {
                if(!_settingsLoaded && AppSettings != null)
                    _settings = new DynamicEntity(AppSettings, new[] {Thread.CurrentThread.CurrentCulture.Name}, 10, null);
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
                if(!_resLoaded && AppResources!= null)
                    _res = new DynamicEntity(AppResources, new[] {Thread.CurrentThread.CurrentCulture.Name}, 10, null);
                _resLoaded = true;
                return _res;
            }
        }
        private bool _resLoaded;
        private dynamic _res;

        #endregion


        #region DI Constructors

        public App(IAppEnvironment appEnvironment, ITenant tenant) : base(appEnvironment, tenant)
        {

        }

        public App PreInit(ITenant tenant)
        {
            Tenant = tenant;
            return this;
        }

        /// <summary>
        /// New constructor which auto-configures the app-data
        /// </summary>
        [PrivateApi]
        public App Init(IAppIdentity appId, Func<EavApp, IAppDataConfiguration> buildConfig, bool allowSideEffects, ILog parentLog)
        {
            Init(appId, allowSideEffects, buildConfig, "Sxc.App", parentLog, "Create");
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
            Init(appIdentity, false, null, "Sxc.AppLgt", parentLog, "light use only");
            return this;
        }

        #endregion


        #region Paths
        /// <inheritdoc />
        public string Path => _path ?? (_path = Factory.Resolve<IHttp>().ToAbsolute(GetRootPath()));
        private string _path;

        /// <inheritdoc />
        public string Thumbnail => System.IO.File.Exists(PhysicalPath + IconFile) ? Path + IconFile : null;

        #endregion

        
    }
}