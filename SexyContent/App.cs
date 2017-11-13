using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.AppEngine;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.DataSources.Pipeline;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;
using IApp = ToSic.SexyContent.Interfaces.IApp;

namespace ToSic.SexyContent
{
    /// <summary>
    /// The app class gives access to the App-object - for the data and things like the App:Path placeholder in a template
    /// </summary>
    public class App : IApp
    {
        #region Constants

        private const string ContentAppName = Eav.Constants.ContentAppName;
        #endregion

        #region Simple Properties
        public int AppId { get; internal set; }
        public int ZoneId { get; internal set; }
        public string Name { get; internal set; } 
        public string Folder { get; internal set; }
        public bool Hidden { get; internal set; }
        public dynamic Configuration { get; internal set; }
        public dynamic Settings { get; internal set; }
        public dynamic Resources { get; internal set; } 
        private IValueCollectionProvider ConfigurationProvider { get; set; }
        private bool ShowDraftsInData { get; set; }
        private bool VersioningEnabled { get; set; }
        #endregion

        #region App-Level TemplateManager, ContentGroupManager, EavContext
        private TemplateManager _templateManager;
        public TemplateManager TemplateManager => _templateManager 
            ?? (_templateManager = new TemplateManager(ZoneId, AppId, Log));

        private ContentGroupManager _contentGroupManager;
        public ContentGroupManager ContentGroupManager => _contentGroupManager 
            ?? (_contentGroupManager = new ContentGroupManager(ZoneId, AppId, ShowDraftsInData, VersioningEnabled, Log));

        #endregion

        #region Logging

        private Log Log;
        #endregion

        internal PortalSettings OwnerPortalSettings { get; set; }

        public string AppGuid { get; set; }

        private  IEnvironment _env;

        public App(PortalSettings ownerPortalSettings, int appId, Log parentLog = null) : this(-1, appId, ownerPortalSettings, parentLog: parentLog)
        {
        }

        // todo: I should change the order of the parameters app/zone because this is the only 
        // system which reverses it with app-first
        // better to create two overloads, but when I have two parameters, assume that zone is first
        // 2016-04-04 2dm: wait with refactoring/correcting this till 2tk checks in his code
        public App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool allowSideEffects = true, Log parentLog = null)
        {
            // init Log
            Log = new Log("App.2sxcAp", parentLog, $"prep App z#{zoneId}, a#{appId}, allowSE:{allowSideEffects}, P:{ownerPortalSettings?.PortalId}");
            _env = new Environment.DnnEnvironment(Log);

            // require valid ownerPS
            if (ownerPortalSettings == null)
                throw new Exception("no portal settings received");

            // if zone is missing, try to find it; if still missing, throw error
            if (zoneId == -1) zoneId = _env.ZoneMapper.GetZoneId(ownerPortalSettings.PortalId);
            if (zoneId == -1) throw new Exception("Cannot find zone-id for portal specified");

            // Save basic values
            AppId = appId;
            ZoneId = zoneId;
            OwnerPortalSettings = ownerPortalSettings;

            // 2016-03-27 2dm: move a bunch of stuff to here from AppManagement.GetApp
            // the code there was a slight bit different, could still cause errors

            // Look up name
            // Get appName from cache
            AppGuid = ((BaseCache)DataSource.GetCache(zoneId)).ZoneApps[zoneId].Apps[appId];

            if (AppGuid == Eav.Constants.DefaultAppName)
                Name = Folder = ContentAppName;
            else
                InitializeResourcesSettingsAndMetadata(allowSideEffects);
        }

        /// <summary>
        /// Assign all kinds of metadata / resources / settings (App-Mode only)
        /// </summary>
        private void InitializeResourcesSettingsAndMetadata(bool allowSideEffects)
        {
            Log.Add($"init app resources allowSE:{allowSideEffects}");
            _env = new Environment.DnnEnvironment(Log);

            if (allowSideEffects)
                // if it's a real App (not content/default), do more
                AppManagement.EnsureAppIsConfigured(ZoneId, AppId, parentLog: Log); // make sure additional settings etc. exist

            // Get app-describing entity
            var appAssignmentId = SystemRuntime.MetadataType(Eav.Constants.AppAssignmentName);
            var mds = DataSource.GetMetaDataSource(ZoneId, AppId);
            var appMetaData = mds
                    .GetMetadata(appAssignmentId, AppId,
                        SexyContent.Settings.AttributeSetStaticNameApps)
                    .FirstOrDefault();
            var appResources = mds
                    .GetMetadata(appAssignmentId, AppId,
                        SexyContent.Settings.AttributeSetStaticNameAppResources)
                    .FirstOrDefault();
            var appSettings = mds 
                    .GetMetadata(appAssignmentId, AppId,
                        SexyContent.Settings.AttributeSetStaticNameAppSettings)
                    .FirstOrDefault();

            dynamic appMetaDataDynamic = appMetaData != null
                ? new DynamicEntity(appMetaData, new[] {Thread.CurrentThread.CurrentCulture.Name}, null)
                : null;

            Name = appMetaDataDynamic?.DisplayName ?? "Error";
            Folder = appMetaDataDynamic?.Folder ?? "Error";
            Configuration = appMetaDataDynamic;
            Resources = appResources != null
                ? new DynamicEntity(appResources, new[] {Thread.CurrentThread.CurrentCulture.Name}, null)
                : null;
            Settings = appResources != null
                ? new DynamicEntity(appSettings, new[] {Thread.CurrentThread.CurrentCulture.Name}, null)
                : null;
            Hidden = appMetaDataDynamic?.Hidden ?? false;
        }

        /// <summary>
        /// needed to initialize data - must always happen a bit later because the show-draft info isn't available when creating the first App-object.
        /// todo: later this should be moved to initialization of this object
        /// </summary>
        /// <param name="showDrafts"></param>
        /// <param name="versioningEnabled"></param>
        /// <param name="configurationValues">this is needed for providing parameters to the data-query-system</param>
        internal void InitData(bool showDrafts, bool versioningEnabled, IValueCollectionProvider configurationValues)
        {
            Log.Add($"init data drafts:{showDrafts}, vers:{versioningEnabled}, hasConf:{configurationValues != null}");
            ConfigurationProvider = configurationValues;
            ShowDraftsInData = showDrafts;
            VersioningEnabled = versioningEnabled;
        }

        private void ConfigureDataOnDemand()
        {
            Log.Add("configure on demand start");
            if(ConfigurationProvider == null)
                throw new Exception("Cannot provide Data for the object App as crucial information is missing. Please call InitData first to provide this data.");
            // ToDo: Remove this as soon as App.Data getter on App class is fixed #1 and #2
            if (_data == null)
            {
                // ModulePermissionController does not work when indexing, return false for search
                var initialSource = DataSource.GetInitialDataSource(ZoneId, AppId, ShowDraftsInData, configProvider: ConfigurationProvider as ValueCollectionProvider, parentLog: Log);

                // todo: probably use th efull configuration provider from function params, not from initial source?
                _data = DataSource.GetDataSource<DataSources.App>(initialSource.ZoneId,
                    initialSource.AppId, initialSource, initialSource.ConfigurationProvider, Log);
                


                var defaultLanguage = "";
                var languagesActive = _env.ZoneMapper.CulturesWithState(OwnerPortalSettings.PortalId, ZoneId)
                    /* 2017-04-01 2dm: from this: ZoneHelpers.CulturesWithState(OwnerPortalSettings.PortalId, ZoneId)*/
                    .Any(c => c.Active);
                if (languagesActive)
                    defaultLanguage = OwnerPortalSettings.DefaultLanguage;
                var xData = (DataSources.App) Data;
                xData.DefaultLanguage = defaultLanguage;
                xData.CurrentUserName = _env.User.CurrentUserIdentityToken;// Environment.Dnn7.UserIdentity.CurrentUserIdentityToken /*OwnerPS.UserInfo.Username*/;
            }
            Log.Add("configure on demand completed");
        }

        #region Paths
        public string Path
        {
            get
            {
                var appPath = System.IO.Path.Combine(AppHelpers.AppBasePath(OwnerPortalSettings), Folder);
                return VirtualPathUtility.ToAbsolute(appPath);
            }
        }
        public string PhysicalPath
        {
            get
            {
                var appPath = System.IO.Path.Combine(AppHelpers.AppBasePath(OwnerPortalSettings), Folder);
                return HostingEnvironment.MapPath(appPath);
            }
        }
        #endregion

        #region Data / Query
        private IAppData _data;
        public IAppData Data
        {
            get
            {
                if (_data == null)
                        ConfigureDataOnDemand();
                return _data;
            }
        }

        /// <summary>
        /// Cached list of queries
        /// </summary>
        private IDictionary<string, IDataSource> _queries;


        /// <summary>
        /// Accessor to queries. Use like:
        /// - App.Query.Count
        /// - App.Query.ContainsKey(...)
        /// - App.Query["One Event"].List
        /// </summary>
        public IDictionary<string, IDataSource> Query
        {
            get
            {
                if (_queries != null) return _queries;

                if(ConfigurationProvider == null)
                    throw new Exception("Can't use app-queries, because the necessary configuration provider hasn't been initialized. Call InitData first.");
                _queries = DataPipeline.AllPipelines(ZoneId, AppId, ConfigurationProvider, Log);
                return _queries;
            }
        }

        public string Thumbnail
        {
            get
            {
                const string iconFile = "/" + AppConstants.AppIconFile;
                return System.IO.File.Exists(PhysicalPath + iconFile) ? Path + iconFile : null;
            }
        }

        #endregion
    }
}