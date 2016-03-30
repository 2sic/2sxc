using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    /// <summary>
    /// The app class gives access to the App-object - for the data and things like the App:Path placeholder in a template
    /// </summary>
    public class App : IApp
    {
        #region Constants
        private const string ContentAppName = "Content";
        #endregion
        public int AppId { get; internal set; }
        public int ZoneId { get; internal set; }
        public string Name { get; internal set; } 
        public string Folder { get; internal set; }
        public bool Hidden { get; internal set; }
        public dynamic Configuration { get; internal set; }
        public dynamic Settings { get; internal set; }
        public dynamic Resources { get; internal set; } 
        private IValueCollectionProvider ConfigurationProvider { get; set; }
        private bool showDraftsInData { get; set; }

        #region App-Level TemplateManager, ContentGroupManager, EavContext
        private TemplateManager _templateManager;
        public TemplateManager TemplateManager => _templateManager 
            ?? (_templateManager = new TemplateManager(ZoneId, AppId));

        private ContentGroupManager _contentGroupManager;
        public ContentGroupManager ContentGroupManager => _contentGroupManager 
            ?? (_contentGroupManager = new ContentGroupManager(ZoneId, AppId));

        private EavDataController _eavContext;

        public EavDataController EavContext
        {
            get
            {
                if (_eavContext == null)
                {
                    _eavContext = EavDataController.Instance(ZoneId, AppId);
                    _eavContext.UserName = Environment.Dnn7.UserIdentity.CurrentUserIdentityToken;
                }
                return _eavContext;
            }
        }

        #endregion

        internal PortalSettings OwnerPortalSettings { get; set; }

        public string AppGuid { get; set; } 


        public App(PortalSettings ownerPortalSettings, int appId, int zoneId = -1)
        {
            // require valid ownerPS - note: not sure what this is actually used for
            if (ownerPortalSettings == null)
                throw new Exception("no portal settings received");

            // if zone is missing, try to find it; if still missing, throw error
            if (zoneId == -1) zoneId = ZoneHelpers.GetZoneID(ownerPortalSettings.PortalId) ?? -1;
            if (zoneId == -1) throw new Exception("Cannot find zone-id for portal specified");

            // Save basic values
            AppId = appId;
            ZoneId = zoneId;
            OwnerPortalSettings = ownerPortalSettings;

            // 2016-03-27 2dm: move a bunch of stuff to here from AppManagement.GetApp
            // the code there was a slight bit different, could still cause errors

            // Look up name
            // Get appName from cache
            AppGuid = ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps[appId];

            if (AppGuid == Constants.DefaultAppName)
                Name = Folder = ContentAppName;
            else
                InitializeResourcesSettingsAndMetadata();
        }

        /// <summary>
        /// Assign all kinds of metadata / resources / settings (App-Mode only)
        /// </summary>
        private void InitializeResourcesSettingsAndMetadata()
        {
            // if it's a real App (not content/default), do more
            AppManagement.EnsureAppIsConfigured(ZoneId, AppId); // make sure additional settings etc. exist

            // Get app-describing entity
            var appMetaData =
                DataSource.GetMetaDataSource(ZoneId, AppId)
                    .GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, AppId,
                        SexyContent.Settings.AttributeSetStaticNameApps)
                    .FirstOrDefault();
            var appResources =
                DataSource.GetMetaDataSource(ZoneId, AppId)
                    .GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, AppId,
                        SexyContent.Settings.AttributeSetStaticNameAppResources)
                    .FirstOrDefault();
            var appSettings =
                DataSource.GetMetaDataSource(ZoneId, AppId)
                    .GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, AppId,
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
        internal void InitData(bool showDrafts, IValueCollectionProvider configurationValues)
        {
            ConfigurationProvider = configurationValues;
            showDraftsInData = showDrafts;
        }

        private void ConfigureDataOnDemand()
        {
            if(ConfigurationProvider == null)
                throw new Exception("Cannot provide Data for the object App as crucial information is missing. Please call InitData first to provide this data.");
            // ToDo: Remove this as soon as App.Data getter on App class is fixed #1 and #2
            if (_data == null)
            {
                // ModulePermissionController does not work when indexing, return false for search
                var initialSource = DataSource.GetInitialDataSource(ZoneId, AppId, showDraftsInData);

                // todo: probably use th efull configuration provider from function params, not from initial source?
                _data = DataSource.GetDataSource<DataSources.App>(initialSource.ZoneId,
                    initialSource.AppId, initialSource, initialSource.ConfigurationProvider);
                var defaultLanguage = "";
                var languagesActive =
                    ZoneHelpers.GetCulturesWithActiveState(OwnerPortalSettings.PortalId, ZoneId).Any(c => c.Active);
                if (languagesActive)
                    defaultLanguage = OwnerPortalSettings.DefaultLanguage;
                var xData = Data as DataSources.App;
                xData.DefaultLanguage = defaultLanguage;
                xData.CurrentUserName = Environment.Dnn7.UserIdentity.CurrentUserIdentityToken /*OwnerPS.UserInfo.Username*/;
            }
        }

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
                if (_queries == null)
                {
                    if(ConfigurationProvider == null)
                        throw new Exception("Can't use app-queries, because the necessary configuration provider hasn't been initialized. Call EnableQuery first.");
                    _queries = DataPipeline.AllPipelines(ZoneId, AppId, ConfigurationProvider);
                }
                return _queries;
            }
        }
        #endregion
    }
}