using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Search.Entities;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Engines.TokenEngine;
using ToSic.SexyContent.Search;
using Assembly = System.Reflection.Assembly;
using FileInfo = System.IO.FileInfo;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Business Layer
    /// Centralizes all global constants and business logic
    /// </summary>
    public class SexyContent : ModuleSearchBase, IUpgradeable
    {
        #region Constants

        public const string ModuleVersion = "08.00.10"; // always the newest version
        public const string TemplateID = "TemplateID";
		public const string ContentGroupGuidString = "ToSIC_SexyContent_ContentGroupGuid";
        public const string AppIDString = "AppId";
	    public const string AppNameString = "ToSIC_SexyContent_AppName";
        public const string SettingsShowTemplateChooser = "ToSIC_SexyContent_ShowTemplateChooser";
        public const string ContentGroupItemIDString = "ContentGroupItemID";
        public const string SortOrderString = "SortOrder";
        public const string EntityID = "EntityID";
        public const string AttributeSetIDString = "AttributeSetID";
        public const string RazorC = "C# Razor";
        public const string RazorVB = "VB Razor";
        public const string TokenReplace = "Token";
	    public const string InternalUserName = "Internal";

        public static class ControlKeys
        {
            public const string View = "";
            public const string Settings = "settings";
            public const string SettingsWrapper = "settingswrapper";
            public const string EavManagement = "eavmanagement";
            public const string EditContentGroup = "editcontentgroup";
            public const string EditTemplate = "edittemplate";
            public const string ManageTemplates = "managetemplates";
            public const string EditList = "editlist";
            public const string TemplateHelp = "templatehelp";
            public const string Export = "export";
            public const string Import = "import";
            public const string DataExport = "dataexport";
            public const string DataImport = "dataimport";
            public const string EditTemplateFile = "edittemplatefile";
            public const string EditTemplateDefaults = "edittemplatedefaults";
            public const string GettingStarted = "gettingstarted";
            public const string PortalConfiguration = "portalconfiguration";
            public const string EditDataSource = "editdatasource";
            public const string AppExport = "appexport";
            public const string AppImport = "appimport";
            public const string AppConfig = "appconfig";
			public const string PipelineManagement = "pipelinemanagement";
			public const string PipelineDesigner = "pipelinedesigner";
            public const string WebApiHelp = "WebApiHelp";
            public const string Permissions = "Permissions";
        }

        public const string PortalHostDirectory = "~/Portals/_default/";
        public const string LocationIDCurrentPortal = "Portal File System";
        public const string TemplateFolder = "2sxc";
        public const string PortalSettingsPrefix = "ToSIC_SexyContent_";

        /// <summary>
        /// Collection of Template Locations
        /// </summary>
        public class TemplateLocations
        {
            public const string PortalFileSystem = "Portal File System";
            public const string HostFileSystem = "Host File System";
        }

        public const string WebConfigTemplatePath = "~/DesktopModules/ToSIC_SexyContent/WebConfigTemplate.config";
        public const string WebConfigFileName = "web.config";
        public const string SexyContentGroupName = "2sxc designers";
        public const string AttributeSetScope = "2SexyContent";
        public const string AttributeSetScopeApps = "2SexyContent-App";
        public const string AttributeSetStaticNameTemplateMetaData = "2SexyContent-Template-Metadata";
        public const string AttributeSetStaticNameApps = "2SexyContent-App";
        public const string AttributeSetStaticNameAppResources = "App-Resources";
        public const string AttributeSetStaticNameAppSettings = "App-Settings";
        public const string TemporaryDirectory = "~/DesktopModules/ToSIC_SexyContent/Temporary";

        #endregion

        #region Properties

        // todo: 2dm - try to refactor the context out of this
        /// <summary>
        /// The Content Data Context
        /// </summary>
        public EavDataController ContentContext;

        private int? ZoneId { get; set; }

        public int? AppId { get; private set; }

        public Templates Templates { get; internal set; }
		public ContentGroups ContentGroups { get; internal set; }

        // must cache App, it gets re-created on each single call - about 10x per request!
        private App _app;
        public App App {
            get
            {
                if(_app == null)
                    _app = GetApp(ZoneId.Value, AppId.Value, OwnerPS);
                return _app; // GetApp(ZoneId.Value, AppId.Value, OwnerPS);
        }
        }

        public PortalSettings OwnerPS { get; set; }
        public PortalSettings PS { get; set; }

        #endregion

        #region AssignmentObjectType Lookups
        /// <summary>
        /// Returns the Default AssignmentObjectTypeID (no assignment / default)
        /// </summary>
        public static int AssignmentObjectTypeIDDefault
        {
            get
            {
                return Eav.Configuration.AssignmentObjectTypeIdDefault;
            }
        }

        /// <summary>
        /// Returns the AssignmentObjectTypeID for 2sxc Templates
        /// </summary>        
        [Obsolete("Do not use this anymore")]    
        public static int AssignmentObjectTypeIDSexyContentTemplate
        {
            get
            {
                return DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId).GetAssignmentObjectTypeId("2SexyContent-Template");
            }
        }

        /// <summary>
        /// Returns the AssignmentObjectTypeID for 2sxc Apps
        /// </summary>        
        public static int AssignmentObjectTypeIDSexyContentApp
        {
            get
            {
                return DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId).GetAssignmentObjectTypeId("App");
            }
        }

        #endregion

        #region SexyContent Constructor

        /// <summary>
        /// Constructor overload for DotNetNuke
        /// (BusinessControllerClass needs parameterless constructor)
        /// </summary>
        public SexyContent()
            : this(0, 0, true)
        {

        }

        static SexyContent()
        {
            new UnityConfig().Configure();
            SetEAVConnectionString();
        }

        /// <summary>
        /// Instanciates Content and Template-Contexts
        /// </summary>
        public SexyContent(int zoneId, int appId, bool enableCaching = true, int? ownerPortalId = null)
        {
            OwnerPS = ownerPortalId.HasValue ? new PortalSettings(ownerPortalId.Value) : PortalSettings.Current;
            PS = PortalSettings.Current;

            if (zoneId == 0)
                if (OwnerPS == null || !GetZoneID(OwnerPS.PortalId).HasValue)
                    zoneId = Constants.DefaultZoneId;
                else
                    zoneId = GetZoneID(OwnerPS.PortalId).Value;

            if (appId == 0)
                appId = GetDefaultAppId(zoneId);

            // Only disable caching of templates and contentgroupitems
            // if AppSetting "ToSIC_SexyContent_EnableCaching" is disabled
            if(enableCaching)
            {
                var cachingSetting = ConfigurationManager.AppSettings["ToSIC_SexyContent_EnableCaching"];
                if (!String.IsNullOrEmpty(cachingSetting) && cachingSetting.ToLower() == "false")
                    enableCaching = false;
            }

            Templates = new Templates(zoneId, appId);
			ContentGroups = new ContentGroups(zoneId, appId);

            // Set Properties on ContentContext
            ContentContext = EavDataController.Instance(zoneId, appId); // EavContext.Instance(zoneId, appId);
            ContentContext.UserName = (HttpContext.Current == null || HttpContext.Current.User == null) ? InternalUserName : HttpContext.Current.User.Identity.Name;

            ZoneId = zoneId;
            AppId = appId;

        }

        /// <summary>
        /// Set EAV's connection string to DNN's
        /// </summary>
        public static void SetEAVConnectionString()
        {
            Eav.Configuration.SetConnectionString("SiteSqlServer");
        }

        #endregion

        #region Template File Handling
        /// <summary>
        /// Returns all template files in the template folder.
        /// </summary>
        public IEnumerable<string> GetTemplateFiles(HttpServerUtility Server, string TemplateType, string TemplateLocation)
        {
            var TemplatePathRootMapPath = Server.MapPath(GetTemplatePathRoot(TemplateLocation, App));
            var Directory = new DirectoryInfo(TemplatePathRootMapPath);

            EnsureTemplateFolderExists(Server, TemplateLocation);

            // Filter the files according to type
            var FileFilter = "*.html";
            switch (TemplateType)
            {
                case RazorC:
                    FileFilter = "*.cshtml";
                    break;
                case RazorVB:
                    FileFilter = "*.vbhtml";
                    break;
                case TokenReplace:
                    FileFilter = "*.html";
                    break;
            }

            var Files = Directory.GetFiles(FileFilter, SearchOption.AllDirectories);
            return (from d in Files where d.Name != WebConfigFileName select (d.FullName).Replace(TemplatePathRootMapPath + "\\", "").Replace('\\','/'));
        }

        /// <summary>
        /// Creates a template file if it does not already exists, and uses a default text to insert. Returns the new path
        /// </summary>
        public string CreateTemplateFileIfNotExists(string name, string type, string location, HttpServerUtility server, string contents = "")
        {
            if (type == RazorC)
            {
                if (!name.StartsWith("_"))
                    name = "_" + name;
                if (Path.GetExtension(name) != ".cshtml")
                    name += ".cshtml";
            }
            else if (type == RazorVB)
            {
                if (!name.StartsWith("_"))
                    name = "_" + name;
                if (Path.GetExtension(name) != ".vbhtml")
                    name += ".vbhtml";
            }
            else if (type == TokenReplace)
            {
                if (Path.GetExtension(name) != ".html")
                    name += ".html";
            }

			var templatePath = Regex.Replace(name, @"[?:\/*""<>|]", "");
            var absolutePath = server.MapPath(Path.Combine(GetTemplatePathRoot(location, App), templatePath));

            if (!File.Exists(absolutePath))
            {
                var stream = new StreamWriter(File.Create(absolutePath));
                stream.Write(contents);
                stream.Flush();
                stream.Close();
            }

	        return templatePath;
        }

        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="sexyFolder"></param>
        /// <param name="ControlPath"></param>
        private void EnsureTemplateFolderExists(HttpServerUtility server, string templateLocation)
        {
            var portalPath = templateLocation == TemplateLocations.HostFileSystem ? server.MapPath(PortalHostDirectory) : OwnerPS.HomeDirectoryMapPath;
            var sexyFolderPath = Path.Combine(portalPath, TemplateFolder);

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            if (!sexyFolder.Exists)
                sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles("web.config").Any())
                File.Copy(server.MapPath(WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, WebConfigFileName));

            // Create a Content folder (or App Folder)
            if (!String.IsNullOrEmpty(App.Folder))
            {
                var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, App.Folder));
                if (!contentFolder.Exists)
                    contentFolder.Create();
            }

            
        }

        /// <summary>
        /// Configures a Portal (Creates a 2sxc Folder containing a web.config File)
        /// </summary>
        public void ConfigurePortal(HttpServerUtility server)
        {
            EnsureTemplateFolderExists(server, TemplateLocations.PortalFileSystem);
        }

        public static string AppBasePath(PortalSettings ownerPS)
        {
            return Path.Combine(ownerPS.HomeDirectory, TemplateFolder);
        }

        /// <summary>
        /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        /// </summary>
        public bool PortalIsConfigured(HttpServerUtility server, string controlPath)
        {
            var sexyFolder = new DirectoryInfo(server.MapPath(Path.Combine(OwnerPS.HomeDirectory, TemplateFolder)));
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, "Content"));
            var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, WebConfigFileName));
            return sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists;
        }

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public static string GetTemplatePathRoot(string locationID, App app)
        {
            var rootFolder = (locationID == LocationIDCurrentPortal ? app.OwnerPS.HomeDirectory : PortalHostDirectory);
            rootFolder += TemplateFolder + "/" + app.Folder;
            return rootFolder;
        }

        #endregion

        #region Template Selector

        /// <summary>
        /// Returns all templates that should be available in the template selector
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetAvailableTemplatesForSelector(ModuleInfo module)
        {
            IEnumerable<Template> availableTemplates;
	        var contentGroup = ContentGroups.GetContentGroupForModule(module.ModuleID);
			var items = contentGroup.Content;
            
            if (items.Any(e => e != null))
				availableTemplates = GetCompatibleTemplates(contentGroup).Where(p => !p.IsHidden);
            else if (items.Count <= 1)
                availableTemplates = Templates.GetVisibleTemplates();
            else
                availableTemplates = Templates.GetVisibleTemplates().Where(p => p.UseForList);
            
            return availableTemplates;
        } 

		private IEnumerable<Template> GetCompatibleTemplates(ContentGroup contentGroup)
        {
			var isList = contentGroup.Content.Count > 1;

			var compatibleTemplates = Templates.GetAllTemplates().Where(t => t.UseForList || !isList);
			compatibleTemplates = compatibleTemplates
				.Where(t => contentGroup.Content.All(c => c == null) || contentGroup.Content.First(e => e != null).Type.StaticName == t.ContentTypeStaticName)
				.Where(t => contentGroup.Presentation.All(c => c == null) || contentGroup.Presentation.First(e => e != null).Type.StaticName == t.PresentationTypeStaticName)
				.Where(t => contentGroup.ListContent.All(c => c == null) || contentGroup.ListContent.First(e => e != null).Type.StaticName == t.ListContentTypeStaticName)
				.Where(t => contentGroup.ListPresentation.All(c => c == null) || contentGroup.ListPresentation.First(e => e != null).Type.StaticName == t.ListPresentationTypeStaticName);

			return compatibleTemplates;
        }

        #endregion

        #region Security / User Management
        /// <summary>
        /// Returns true if a DotNetNuke User Group "SexyContent Designers" exists and contains at minumum one user
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        public static bool SexyContentDesignersGroupConfigured(int portalId)
        {
            var roleControl = new RoleController();
            var role = roleControl.GetRoleByName(portalId, SexyContentGroupName);
            return role != null;
        }

        /// <summary>
        /// Returns true if a user is in the SexyContent Designers group
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsInSexyContentDesignersGroup(UserInfo user)
        {
            return user.IsInRole(SexyContentGroupName);
        }

        /// <summary>
        /// Returns true if the user is able to edit this module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static bool HasEditPermission(ModuleInfo module)
        {
            // Make sure that HasEditPermission still works while search indexing
            if (PortalSettings.Current == null)
                return false;
            return ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "CONTENT", module);
        }


        #endregion

        #region Get DataSources

        /// <summary>
        /// Gets the initial DataSource
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static IDataSource GetInitialDataSource(int zoneId, int appId, bool showDrafts = false)
        {
            return DataSource.GetInitialDataSource(zoneId, appId, showDrafts);
        }

        // 2015-04-30 2dm must cache this, shouldn't get re-created on a single call, it's always the same
        // todo: must ask 2rm if it's ok to cache - can this SexyContent be re-used for other modules?
        private ValueCollectionProvider _valueCollectionProvider;
	    public ValueCollectionProvider GetConfigurationProvider(int moduleId)
        {
	        if (_valueCollectionProvider == null)
	        {
                var provider = new ValueCollectionProvider();

                // only add these in running inside an http-context. Otherwise leave them away!
                if (HttpContext.Current != null)
                {
                    var request = HttpContext.Current.Request;
                    provider.Sources.Add("querystring", new FilteredNameValueCollectionPropertyAccess("querystring", request.QueryString));
                    provider.Sources.Add("server", new FilteredNameValueCollectionPropertyAccess("server", request.ServerVariables));
                    provider.Sources.Add("form", new FilteredNameValueCollectionPropertyAccess("form", request.Form));
                }

				// Add the standard DNN property sources if PortalSettings object is available
		        if (PS != null)
        {
			        var dnnUsr = PS.UserInfo;
			        var dnnCult = Thread.CurrentThread.CurrentCulture;
			        var dnn = new TokenReplaceDnn(App, moduleId, PS, dnnUsr);
			        var stdSources = dnn.PropertySources;
			        foreach (var propertyAccess in stdSources)
				        provider.Sources.Add(propertyAccess.Key,
					        new ValueProviderWrapperForPropertyAccess(propertyAccess.Key, propertyAccess.Value, dnnUsr, dnnCult));
		        }

		    provider.Sources.Add("app", new AppPropertyAccess("app", App));

                // add module if it was not already added previously
	            if (!provider.Sources.ContainsKey("module"))
	            {
	                var modulePropertyAccess = new StaticValueProvider("module");
		    modulePropertyAccess.Properties.Add("ModuleID", moduleId.ToString(CultureInfo.InvariantCulture));
		    provider.Sources.Add(modulePropertyAccess.Name, modulePropertyAccess);
                }
	            _valueCollectionProvider = provider;
            }
	        return _valueCollectionProvider;
        }

        /// <summary>
        /// The EAV DataSource
        /// </summary>
        private IDataSource _viewDataSource;// { get; set; }
		public IDataSource GetViewDataSource(int moduleId, bool showDrafts, Template template)
        {
            if (_viewDataSource == null)
            {
	            var configurationProvider = GetConfigurationProvider(moduleId);

				// Get ModuleDataSource
                var initialSource = GetInitialDataSource(ZoneId.Value, AppId.Value, showDrafts);
				var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(ZoneId, AppId, initialSource, configurationProvider);
                moduleDataSource.ModuleId = moduleId;
                if(template != null)
                    moduleDataSource.OverrideTemplateId = template.TemplateId;
                moduleDataSource.Sexy = this;

	            var viewDataSourceUpstream = moduleDataSource;

	            // If the Template has a Data-Pipeline, use it instead of the ModuleDataSource created above
				if (template != null && template.Pipeline != null)
					viewDataSourceUpstream = null;

				var viewDataSource = DataSource.GetDataSource<ViewDataSource>(ZoneId, AppId, viewDataSourceUpstream, configurationProvider);

				// Take Publish-Properties from the View-Template
	            if (template != null)
	            {
                    viewDataSource.Publish.Enabled = template.PublishData;
                    viewDataSource.Publish.Streams = template.StreamsToPublish;

					// Append Streams of the Data-Pipeline (this doesn't require a change of the viewDataSource itself)
		            if (template.Pipeline != null)
						DataPipelineFactory.GetDataSource(AppId.Value, template.Pipeline.EntityId, configurationProvider, viewDataSource);
                }

                _viewDataSource = viewDataSource;
            }

            return _viewDataSource;
        }

        #endregion

        #region URL Handling / Toolbar
        
        /// <summary>
        /// Get the URL for editing MetaData
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="moduleId"></param>
        /// <param name="returnUrl"></param>
        /// <param name="portalSettings"></param>
        /// <param name="control"></param>
        /// <param name="attributeSetStaticName"></param>
        /// <param name="assignmentObjectTypeID"></param>
        /// <param name="keyNumber"></param>
        /// <returns></returns>
        public string GetMetaDataEditUrl(int tabId, int moduleId, string returnUrl, Control control, string attributeSetStaticName, int assignmentObjectTypeID, int keyNumber)
        {
            var assignedEntity = DataSource.GetMetaDataSource(ZoneId.Value, AppId.Value).GetAssignedEntities(assignmentObjectTypeID, keyNumber, attributeSetStaticName).FirstOrDefault();
            var entityId = assignedEntity == null ? new int?() : assignedEntity.EntityId;

            return GetEntityEditLink(entityId , moduleId, tabId, attributeSetStaticName, returnUrl,
                    assignmentObjectTypeID, keyNumber);
        }

        private string GetEntityEditLink(int? entityId, int moduleId, int tabId, string attributeSetStaticName, string returnUrl, int? assignmentObjectTypeId, int? keyNumber)
        {
            var editUrl = Globals.NavigateURL(tabId, ControlKeys.EditContentGroup, new[] { "mid", moduleId.ToString(), "AppId", AppId.ToString(),
                "AttributeSetName", attributeSetStaticName, "AssignmentObjectTypeId", assignmentObjectTypeId.ToString(), "KeyNumber", keyNumber.ToString() });
            editUrl += (editUrl.IndexOf("?") == -1 ? "?" : "&") + "popUp=true&ReturnUrl=" + HttpUtility.UrlEncode(returnUrl);

            if (!entityId.HasValue)
                editUrl += "&EditMode=New";
            else
                editUrl += "&EntityId=" + entityId.Value;

            // If Culture exists, add CultureDimension
            var languageId = GetCurrentLanguageID();
            if (languageId.HasValue)
                editUrl += "&CultureDimension=" + languageId;

            return editUrl;
        }

        /// <summary>
        /// Returns the Edit Link for given GroupID and SortOrder
        /// </summary>
        /// <param name="Element"></param>
        /// <param name="ModuleID"></param>
        /// <param name="TabID"></param>
        /// <param name="UserID"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public string GetElementEditLink(Guid ContentGroupID, int SortOrder, int ModuleID, int TabID, string ReturnUrl)
        {
            var EditUrl = Globals.NavigateURL(TabID, ControlKeys.EditContentGroup, "mid", ModuleID.ToString(), SortOrderString, SortOrder.ToString(), "ContentGroupID", ContentGroupID.ToString());
            EditUrl += (EditUrl.IndexOf("?") == -1 ? "?" : "&") + "popUp=true&ReturnUrl=" + HttpUtility.UrlEncode(ReturnUrl);

            // If Culture exists, add CultureDimension
            var LanguageID = GetCurrentLanguageID();
            if(LanguageID.HasValue)
                EditUrl += "&CultureDimension=" + LanguageID;

            return EditUrl;
        }

        public string GetElementAddWithEditLink(Guid ContentGroupID, int DestinationSortOrder, int ModuleID, int TabID, string ReturnUrl)
        {
            return GetElementEditLink(ContentGroupID, DestinationSortOrder, ModuleID, TabID, ReturnUrl) + "&EditMode=New";
        }

        public string GetElementSettingsLink(Guid ContentGroupID, int sortOrder, int ModuleID, int TabID, string ReturnUrl)
        {
            var settingsUrl = Globals.NavigateURL(TabID, ControlKeys.SettingsWrapper, "mid", ModuleID.ToString(), ContentGroupGuidString, ContentGroupID.ToString(), "SortOrder", sortOrder.ToString(), "ItemType", sortOrder == -1 ? "ListContent" : "Content");
            settingsUrl += (settingsUrl.IndexOf("?") == -1 ? "?" : "&") + "popUp=true&ReturnUrl=" + HttpUtility.UrlEncode(ReturnUrl);
            return settingsUrl;
        }

        #endregion

        #region Apps

        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <param name="includeDefaultApp"></param>
        /// <returns></returns>
        public static List<App> GetApps(int zoneId, bool includeDefaultApp, PortalSettings ownerPS)
        {
            var eavApps = ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps;
            var sexyApps = eavApps.Select(eavApp => GetApp(zoneId, eavApp.Key, ownerPS));

            if (!includeDefaultApp)
                sexyApps = sexyApps.Where(a => a.Name != "Content");

            return sexyApps.OrderBy(a => a.Name).ToList();
        }

        public static App GetApp(int zoneId, int appId, PortalSettings ownerPS)
        {
            // Get appName from cache
            var eavAppName = ((BaseCache) DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps[appId];
            App sexyApp = null;

            if(eavAppName != Constants.DefaultAppName)
            {
                EnsureAppIsConfigured(zoneId, appId);

                // Get app-describing entity
                var appMetaData = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameApps).FirstOrDefault();
                var appResources = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameAppResources).FirstOrDefault();
                var appSettings = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameAppSettings).FirstOrDefault();
                
                if (appMetaData != null)
                {
                    dynamic appMetaDataDynamic = new DynamicEntity(appMetaData, new[] { Thread.CurrentThread.CurrentCulture.Name }, null);
                    dynamic appResourcesDynamic = appResources != null ? new DynamicEntity(appResources, new[] {Thread.CurrentThread.CurrentCulture.Name}, null) : null;
                    dynamic appSettingsDynamic = appResources != null ? new DynamicEntity(appSettings, new[] {Thread.CurrentThread.CurrentCulture.Name}, null) : null;

                    sexyApp = new App(appId, zoneId, ownerPS)
                    {
                        Name = appMetaDataDynamic.DisplayName,
                        Folder = appMetaDataDynamic.Folder,
                        Configuration = appMetaDataDynamic,
                        Resources = appResourcesDynamic,
                        Settings = appSettingsDynamic,
                        Hidden = appMetaDataDynamic.Hidden is bool ? appMetaDataDynamic.Hidden : false,
                        AppGuid = eavAppName
                    };
                }
            }
            // Handle default app
            else
            {
                sexyApp = new App(appId, zoneId, ownerPS)
                {
                    AppId = appId,
                    Name = "Content",
                    Folder = "Content",
                    Configuration = null,
                    Resources = null,
                    Settings = null,
                    Hidden = true,
                    AppGuid = eavAppName
                };
            }

            return sexyApp;
        }

        /// <summary>
        /// Create app-describing entity for configuration and add Settings and Resources Content Type
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        public static void EnsureAppIsConfigured(int zoneId, int appId, string appName = null)
        {
            var appMetaData = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameApps).FirstOrDefault();
            var appResources = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameAppResources).FirstOrDefault();
            var appSettings = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameAppSettings).FirstOrDefault();

            // Get appName from cache
            var eavAppName = ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps[appId];

            if (eavAppName == Constants.DefaultAppName)
                return;

            var appContext = new SexyContent(zoneId, appId);

            if (appMetaData == null)
            {
                // Add app-describing entity
                var appAttributeSet = appContext.ContentContext.AttribSet.GetAttributeSet(AttributeSetStaticNameApps).AttributeSetID;
                var values = new OrderedDictionary
                {
                    {"DisplayName", String.IsNullOrEmpty(appName) ? eavAppName : appName },
                    {"Folder", String.IsNullOrEmpty(appName) ? eavAppName : RemoveIllegalCharsFromPath(appName) },
                    {"AllowTokenTemplates", "False"},
                    {"AllowRazorTemplates", "False"},
                    {"Version", "00.00.01"},
                    {"OriginalId", ""}
                };
                appContext.ContentContext.Entities.AddEntity(appAttributeSet, values, null, appId, AssignmentObjectTypeIDSexyContentApp);
            }

            if(appSettings == null)
            {

                AttributeSet settingsAttributeSet;
                // Add new (empty) ContentType for Settings
                if (!appContext.ContentContext.AttribSet.AttributeSetExists(AttributeSetStaticNameAppSettings, appId))
                    settingsAttributeSet = appContext.ContentContext.AttribSet.AddAttributeSet(AttributeSetStaticNameAppSettings,
                        "Stores settings for an app", AttributeSetStaticNameAppSettings, AttributeSetScopeApps);
                else
                    settingsAttributeSet = appContext.ContentContext.AttribSet.GetAttributeSet(AttributeSetStaticNameAppSettings);

                DataSource.GetCache(zoneId, appId).PurgeCache(zoneId, appId);
                appContext.ContentContext.Entities.AddEntity(settingsAttributeSet, new OrderedDictionary(), null, appId, AssignmentObjectTypeIDSexyContentApp);
            }

            if(appResources == null)
            {
                AttributeSet resourcesAttributeSet;

                // Add new (empty) ContentType for Resources
                if (!appContext.ContentContext.AttribSet.AttributeSetExists(AttributeSetStaticNameAppResources, appId))
                    resourcesAttributeSet = appContext.ContentContext.AttribSet.AddAttributeSet(
                        AttributeSetStaticNameAppResources, "Stores resources like translations for an app",
                        AttributeSetStaticNameAppResources, AttributeSetScopeApps);
                else
                    resourcesAttributeSet = appContext.ContentContext.AttribSet.GetAttributeSet(AttributeSetStaticNameAppResources);

                DataSource.GetCache(zoneId, appId).PurgeCache(zoneId, appId);
                appContext.ContentContext.Entities.AddEntity(resourcesAttributeSet, new OrderedDictionary(), null, appId, AssignmentObjectTypeIDSexyContentApp);
            }

            if (appMetaData == null || appSettings == null || appResources == null)
                DataSource.GetCache(zoneId, appId).PurgeCache(zoneId, appId);
        }

        public static App AddApp(int zoneId, string appName, PortalSettings ownerPS)
        {
            if(appName == "Content" || appName == "Default" || String.IsNullOrEmpty(appName) || !Regex.IsMatch(appName, "^[0-9A-Za-z -_]+$"))
                throw new ArgumentOutOfRangeException("appName '" + appName + "' not allowed");

            // Adding app to EAV
            var sexy = new SexyContent(zoneId, GetDefaultAppId(zoneId));
            var app = sexy.ContentContext.App.AddApp(Guid.NewGuid().ToString());
            sexy.ContentContext.SqlDb.SaveChanges();

            EnsureAppIsConfigured(zoneId, app.AppID, appName);

            return GetApp(zoneId, app.AppID, ownerPS);
        }

        public static int? GetAppSettingsAttributeSetId(int zoneId, int appId)
        {
            if (appId == GetDefaultAppId(zoneId))
                return null;

            return new SexyContent(zoneId, appId).GetAvailableContentTypes(AttributeSetScopeApps)
                .Single(p => p.StaticName == AttributeSetStaticNameAppSettings).AttributeSetId;
        }

        public static int? GetAppResourcesAttributeSetId(int zoneId, int appId)
        {
            if (appId == GetDefaultAppId(zoneId))
                return null;

            return new SexyContent(zoneId, appId).GetAvailableContentTypes(AttributeSetScopeApps)
                .Single(p => p.StaticName == AttributeSetStaticNameAppResources).AttributeSetId;
        }

        public static int? GetAppIdFromModule(ModuleInfo module)
        {
            var zoneId = GetZoneID(module.OwnerPortalID);
            
            if (module.DesktopModule.ModuleName == "2sxc")
            {
                return zoneId.HasValue ? GetDefaultAppId(zoneId.Value) : new int?();
            }

            object appIdString = null;

            if (HttpContext.Current != null)
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["AppId"]))
                    appIdString = HttpContext.Current.Request.QueryString["AppId"];
                else
                {
                    // todo: fix 2dm
                    var appName = TryToGetReliableSetting(module, AppNameString);//  module.ModuleSettings[AppNameString];

	                if (appName != null)
	                {
						// ToDo: Fix issue in EAV (cache is only ensured when a CacheItem-Property is accessed like LastRefresh)
		                var x = ((BaseCache) DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId)).LastRefresh;
						appIdString = ((BaseCache) DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId)).ZoneApps[zoneId.Value].Apps.Where(p => p.Value == (string)appName).Select(p => p.Key).FirstOrDefault();
	                }
                }
            }

            int appId;
            if (appIdString != null && int.TryParse(appIdString.ToString().Split(',')[0], out appId))
                return appId;

            return null;
        }

        public static void SetAppIdForModule(ModuleInfo module, int? appId)
        {
            var moduleController = new ModuleController();

			// Reset temporary template
			ContentGroups.DeletePreviewTemplateId(module.ModuleID);

			// ToDo: Should throw exception if a real ContentGroup exists

			var zoneId = GetZoneID(module.OwnerPortalID);
            
            if (appId == 0 || !appId.HasValue)
		        moduleController.DeleteModuleSetting(module.ModuleID, AppNameString);
            else
	        {
		        var appName = ((BaseCache) DataSource.GetCache(0, 0)).ZoneApps[zoneId.Value].Apps[appId.Value];
				moduleController.UpdateModuleSetting(module.ModuleID, AppNameString, appName);
	        }

			// Change to 1. available template if app has been set
			if (appId.HasValue)
			{
				var sexyForNewApp = new SexyContent(zoneId.Value, appId.Value, false);
				var templates = sexyForNewApp.GetAvailableTemplatesForSelector(module).ToList();
				if (templates.Any())
					sexyForNewApp.ContentGroups.SetPreviewTemplateId(module.ModuleID, templates.First().TemplateId);
			}
        }

        public void RemoveApp(int appId, int userId)
        {
            if(appId != ContentContext.AppId)
                throw new Exception("An app can only be removed inside of it's own context.");

            if(appId == GetDefaultAppId(ZoneId.Value))
                throw new Exception("The default app of a zone cannot be removed.");

            var sexyApp = GetApp(ZoneId.Value, appId, OwnerPS);

            // Delete folder
            if (!String.IsNullOrEmpty(sexyApp.Folder) &&  Directory.Exists(sexyApp.PhysicalPath))
                Directory.Delete(sexyApp.PhysicalPath, true);

            // Delete the app
            ContentContext.App.DeleteApp(appId);
        }

        public static int GetDefaultAppId(int zoneId)
        {
            return ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].DefaultAppId;
        }

        #endregion Apps

        #region Get ContentTypes

        public IEnumerable<IContentType> GetAvailableContentTypes(string scope, bool includeAttributeTypes = false)
        {
            return GetAvailableContentTypes(includeAttributeTypes).Where(p => p.Scope == scope);
            }

        public IEnumerable<IContentType> GetAvailableContentTypes(bool includeAttributeTypes = false)
            {
            var contentTypes = ((BaseCache) DataSource.GetCache(ZoneId.Value, AppId.Value)).GetContentTypes();
            return contentTypes.Select(c => c.Value).Where(c => includeAttributeTypes || !c.Name.StartsWith("@")).OrderBy(c => c.Name);
        }

        public IEnumerable<IContentType> GetAvailableContentTypesForVisibleTemplates()
        {
            var AvailableTemplates = Templates.GetVisibleTemplates();
            return GetAvailableContentTypes(AttributeSetScope).Where(p => AvailableTemplates.Any(t => t.ContentTypeStaticName == p.StaticName)).OrderBy(p => p.Name);
        }


        #endregion

        #region DNN Interface Members

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            try
            {
                return new SearchController().GetModifiedSearchDocuments(moduleInfo, beginDate);
            }
            catch (Exception e)
            {
                throw new SearchIndexException(moduleInfo, e);
            }
        }

        #endregion

        #region Zone / VDB Handling

        /// <summary>
        /// Returns the ZoneID from PortalSettings
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        public static int? GetZoneID(int portalId)
        {
            var zoneSettingKey = PortalSettingsPrefix + "ZoneID";
            var c = PortalController.GetPortalSettingsDictionary(portalId);
            var portalSettings = new PortalSettings(portalId);

            int zoneId;

            // Create new zone automatically
            if (!c.ContainsKey(zoneSettingKey))
            {
                var newZone = AddZone(portalSettings.PortalName + " (Portal " + portalId + ")");
                SetZoneID(newZone.ZoneID, portalId);
                zoneId = newZone.ZoneID;
            }
            else
            {
                zoneId = int.Parse(c[zoneSettingKey]);
            }

            return zoneId;
        }

        /// <summary>
        /// Sets the ZoneID from PortalSettings
        /// </summary>
        /// <param name="ZoneID"></param>
        /// <param name="PortalID"></param>
        public static void SetZoneID(int? ZoneID, int PortalID)
        {
            if (ZoneID.HasValue)
                PortalController.UpdatePortalSetting(PortalID, PortalSettingsPrefix + "ZoneID", ZoneID.Value.ToString());
            else
                PortalController.DeletePortalSetting(PortalID, PortalSettingsPrefix + "ZoneID");
        }

        public static List<Zone> GetZones()
        {
            return new SexyContent(Constants.DefaultZoneId, GetDefaultAppId(Constants.DefaultZoneId)).ContentContext.Zone.GetZones();
        }

        public static Zone AddZone(string zoneName)
        {
            return
                new SexyContent(Constants.DefaultZoneId, GetDefaultAppId(Constants.DefaultZoneId)).ContentContext.Zone
                    .AddZone(zoneName).Item1;
        }

        #endregion

        #region Culture Handling

        [Obsolete("Don't use this anymore, use 'GetCurrentLanguageName' (work with strings)")]
        public int? GetCurrentLanguageID(bool UseDefaultLanguageIfNotFound = false)
        {
            var LanguageID = ContentContext.Dimensions.GetLanguageId(Thread.CurrentThread.CurrentCulture.Name);
            if (!LanguageID.HasValue && UseDefaultLanguageIfNotFound)
                LanguageID = ContentContext.Dimensions.GetLanguageId(PortalSettings.Current.DefaultLanguage);
            return LanguageID;
        }

        public string GetCurrentLanguageName()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        public void SetCultureState(string CultureCode, bool Active, int PortalID)
        {
            var EAVLanguage = ContentContext.Dimensions.GetLanguages().Where(l => l.ExternalKey == CultureCode).FirstOrDefault();
            // If the language exists in EAV, set the active state, else add it
            if (EAVLanguage != null)
                ContentContext.Dimensions.UpdateDimension(EAVLanguage.DimensionID, Active);
            else
            {
                var CultureText = LocaleController.Instance.GetLocale(CultureCode).Text;
                ContentContext.Dimensions.AddLanguage(CultureText, CultureCode);
            }
        }


        /// <summary>
        /// Returns all DNN Cultures with active / inactive state
        /// </summary>
        public static List<CulturesWithActiveState> GetCulturesWithActiveState(int portalId, int zoneId)
        {
            //var DefaultLanguageID = ContentContext.GetLanguageId();
            var AvailableEAVLanguages = new SexyContent(zoneId, GetDefaultAppId(zoneId)).ContentContext.Dimensions.GetLanguages();
            var DefaultLanguageCode = new PortalSettings(portalId).DefaultLanguage;
            var DefaultLanguage = AvailableEAVLanguages.Where(p => p.ExternalKey == DefaultLanguageCode).FirstOrDefault();
            var DefaultLanguageIsActive = DefaultLanguage != null && DefaultLanguage.Active;

            return (from c in LocaleController.Instance.GetLocales(portalId)
                    select new CulturesWithActiveState
                    {
                        Code = c.Value.Code,
                        Text = c.Value.Text,
                        Active = AvailableEAVLanguages.Any(a => a.Active && a.ExternalKey == c.Value.Code && a.ZoneID == zoneId),
                        // Allow State Change only if
                        // 1. This is the default language and default language is not active or
                        // 2. This is NOT the default language and default language is active
                        AllowStateChange = (c.Value.Code == DefaultLanguageCode && !DefaultLanguageIsActive) || (DefaultLanguageIsActive && c.Value.Code != DefaultLanguageCode)
                    }).OrderByDescending(c => c.Code == DefaultLanguageCode).ThenBy(c => c.Code).ToList();

        }

        public class CulturesWithActiveState
        {
            public string Code { get; set; }
            public string Text { get; set; }
            public bool Active { get; set; }
            public bool AllowStateChange { get; set; }
        }

        public static int? GetLanguageId(int zoneId, string externalKey)
        {
            return new SexyContent(zoneId, GetDefaultAppId(zoneId)).ContentContext.Dimensions.GetLanguageId(externalKey);
        }

        #endregion

        #region Helper Methods

        public static void AddDNNVersionToBodyClass(Control Parent)
        {
            // Add DNN Version to body as CSS Class
            var CssClass = "dnn-" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.Major;
            var body = (HtmlGenericControl)Parent.Page.FindControl("ctl00$body");
            if(body.Attributes["class"] != null)
                body.Attributes["class"] += CssClass;
            else
                body.Attributes["class"] = CssClass;
        }

        public bool IsEditMode()
        {
            return Globals.IsEditMode() && (OwnerPS.PortalId == PS.PortalId);
        }

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        public string GetJsonFromStreams(IDataSource source, string[] streamsToPublish)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;

            var y = streamsToPublish.Where(k => source.Out.ContainsKey(k)).ToDictionary(k => k, s => new
            {
                List = (from c in source.Out[s].List select GetDictionaryFromEntity(c.Value, language)).ToList()
            });

            return JsonConvert.SerializeObject(y);
        }

        internal Dictionary<string, object> GetDictionaryFromEntity(IEntity entity, string language)
        {
            var dynamicEntity = new DynamicEntity(entity, new[] { language }, this);
            bool propertyNotFound;

            // Convert DynamicEntity to dictionary
            var dictionary = ((from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v =>
            {
                var value = dynamicEntity.GetEntityValue(v.Name, out propertyNotFound);
                if (v.Type == "Entity" && value is List<DynamicEntity>)
                    return ((List<DynamicEntity>) value).Select(p => new { p.EntityId, p.EntityTitle });
                return value;
            }));

            dictionary.Add("EntityId", entity.EntityId);
            dictionary.Add("Modified", entity.Modified);

	        if (entity is EntityInContentGroup && !dictionary.ContainsKey("Presentation"))
	        {
		        var entityInGroup = (EntityInContentGroup) entity;
				if(entityInGroup.Presentation != null)
					dictionary.Add("Presentation", GetDictionaryFromEntity(entityInGroup.Presentation, language));
	        }

	        if(entity is IHasEditingData)
                dictionary.Add("_2sxcEditInformation", new { sortOrder = ((IHasEditingData)entity).SortOrder });
            else
                dictionary.Add("_2sxcEditInformation", new { entityId = entity.EntityId, title = entity.Title != null ? entity.Title[language] : "(no title)" });

            return dictionary;
        }

	    private static string RemoveIllegalCharsFromPath(string path)
	    {
			var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
			return r.Replace(path, "");
	    }

        /// <summary>
        /// Resolves File and Page values
        /// For example, File:123 or Page:123
        /// </summary>
        public static string ResolveHyperlinkValues(string value, PortalSettings ownerPortalSettings)
        {
            var resultString = value;
            var regularExpression = Regex.Match(resultString, @"^(?<type>(file|page)):(?<id>[0-9]+)(?<params>(\?|\#).*)?$", RegexOptions.IgnoreCase);

            if (!regularExpression.Success)
                return value;

            var fileManager = FileManager.Instance;
            var tabController = new TabController();
            var type = regularExpression.Groups["type"].Value.ToLower();
            var id = int.Parse(regularExpression.Groups["id"].Value);
			var urlParams = regularExpression.Groups["params"].Value ?? "";

			
            switch (type)
            {
                case "file":
                    var fileInfo = fileManager.GetFile(id);
                    if (fileInfo != null)
                        resultString = (fileInfo.StorageLocation == (int)FolderController.StorageLocationTypes.InsecureFileSystem
                            ? Path.Combine(ownerPortalSettings.HomeDirectory, fileInfo.RelativePath) + urlParams
                            : fileManager.GetUrl(fileInfo));
                    break;
                case "page":
                    var portalSettings = PortalSettings.Current;

                    // Get full PortalSettings (with portal alias) if module sharing is active
                    if (PortalSettings.Current != null && PortalSettings.Current.PortalId != ownerPortalSettings.PortalId)
                    {
                        var portalAlias = ownerPortalSettings.PrimaryAlias ?? TestablePortalAliasController.Instance.GetPortalAliasesByPortalId(ownerPortalSettings.PortalId).First();
                        portalSettings = new PortalSettings(id, portalAlias);
                    }
                    var tabInfo = tabController.GetTab(id, ownerPortalSettings.PortalId, false);
                    if (tabInfo != null)
                    {
                        if (tabInfo.CultureCode != "" && tabInfo.CultureCode != PortalSettings.Current.CultureCode)
                        {
                            var cultureTabInfo = tabController.GetTabByCulture(tabInfo.TabID, tabInfo.PortalID, LocaleController.Instance.GetLocale(PortalSettings.Current.CultureCode));

                            if (cultureTabInfo != null)
                                tabInfo = cultureTabInfo;
                        }

                        // Exception in AdvancedURLProvider because ownerPortalSettings.PortalAlias is null
                        resultString = Globals.NavigateURL(tabInfo.TabID, portalSettings, "", new string[] { }) + urlParams;
                    }
                    break;
            }

            return resultString;
        }

        #endregion

        #region Upgrade

        public string UpgradeModule(string Version)
        {
			return SexyContentModuleUpgrade.UpgradeModule(Version);
        }

        #endregion

        #region Settings (because DNN doesn't do it reliably)

        public static string TryToGetReliableSetting(ModuleInfo module, string settingName)
        {
            if (module.ModuleSettings.ContainsKey(settingName))
                return module.ModuleSettings[settingName].ToString();

            // if not found, it could be a caching issue
            var settings = new ModuleController().GetModuleSettings(module.ModuleID);
            if (settings.ContainsKey(settingName))
                return settings[settingName].ToString();

            return null;
        }

        #endregion
    }
}