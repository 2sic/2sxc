using System.Data.Objects;
using DotNetNuke.Common;
using DotNetNuke.Common.Internal;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Tabs.Internal;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Search.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Caches;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.DataSources.Tokens;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Engines.TokenEngine;
using ToSic.SexyContent.Search;
using FileInfo = System.IO.FileInfo;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Business Layer
    /// Centralizes all global constants and business logic
    /// </summary>
    public class SexyContent : ModuleSearchBase, IUpgradeable
    {
        #region Constants

        public const string ModuleVersion = "06.03.02";
        public const string TemplateID = "TemplateID";
        public const string ContentGroupIDString = "ContentGroupID";
        public const string AppIDString = "AppId";
        public const string SettingsPublishDataSource = "ToSic_SexyContent_PublishDataSource";
        public const string SettingsPublishDataSourceStreams = "ToSic_SexyContent_PublishDataSource_Streams";
        public const string ContentGroupItemIDString = "ContentGroupItemID";
        public const string SortOrderString = "SortOrder";
        public const string EntityID = "EntityID";
        public const string AttributeSetIDString = "AttributeSetID";
        public const string RazorC = "C# Razor";
        public const string RazorVB = "VB Razor";
        public const string TokenReplace = "Token";

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
            public const string AddItem = "additem";
            public const string EditTemplateDefaults = "edittemplatedefaults";
            public const string GettingStarted = "gettingstarted";
            public const string PortalConfiguration = "portalconfiguration";
            public const string EditDataSource = "editdatasource";
            public const string AppExport = "appexport";
            public const string AppImport = "appimport";
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
        public const string SexyContentGroupName = "2SexyContent Designers";
        public const string AttributeSetScope = "2SexyContent";
        public const string AttributeSetScopeApps = "2SexyContent-App";
        public const string AttributeSetStaticNameTemplateMetaData = "2SexyContent-Template-Metadata";
        public const string AttributeSetStaticNameTemplateContentTypes = "2SexyContent-Template-ContentTypes";
        public const string AttributeSetStaticNameApps = "2SexyContent-App";
        public const string AttributeSetStaticNameAppResources = "App-Resources";
        public const string AttributeSetStaticNameAppSettings = "App-Settings";
        public const string TemporaryDirectory = "~/DesktopModules/ToSIC_SexyContent/Temporary";

        #endregion

        #region Properties

        /// <summary>
        /// The Content Data Context
        /// </summary>
        public EavContext ContentContext;

        private int? ZoneId { get; set; }

        public int? AppId { get; private set; }

        /// <summary>
        /// The Template Data Context
        /// </summary>
        public SexyContentContext TemplateContext { get; internal set; }

        public App App {
            get { return GetApp(ZoneId.Value, AppId.Value, OwnerPS); }
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
                return  DataSource.GetCache(DataSource.DefaultZoneId, DataSource.MetaDataAppId).GetAssignmentObjectTypeId("Default");
            }
        }

        /// <summary>
        /// Returns the AssignmentObjectTypeID for 2sxc Templates
        /// </summary>        
        public static int AssignmentObjectTypeIDSexyContentTemplate
        {
            get
            {
                return DataSource.GetCache(DataSource.DefaultZoneId, DataSource.MetaDataAppId).GetAssignmentObjectTypeId("2SexyContent-Template");
            }
        }

        /// <summary>
        /// Returns the AssignmentObjectTypeID for 2sxc Apps
        /// </summary>        
        public static int AssignmentObjectTypeIDSexyContentApp
        {
            get
            {
                return DataSource.GetCache(DataSource.DefaultZoneId, DataSource.MetaDataAppId).GetAssignmentObjectTypeId("App");
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
            SexyContent.SetEAVConnectionString();
        }

        /// <summary>
        /// Instanciates Content and Template-Contexts
        /// </summary>
        public SexyContent(int zoneId, int appId, bool enableCaching = true, int? ownerPortalId = null)
        {
            OwnerPS = ownerPortalId.HasValue ? new PortalSettings(ownerPortalId.Value) : PortalSettings.Current;
            this.PS = PortalSettings.Current;

            if (zoneId == 0)
                if (OwnerPS == null || !GetZoneID(OwnerPS.PortalId).HasValue)
                    zoneId = DataSource.DefaultZoneId;
                else
                    zoneId = GetZoneID(OwnerPS.PortalId).Value;

            if (appId == 0)
                appId = GetDefaultAppId(zoneId);

            // Only disable caching of templates and contentgroupitems
            // if AppSetting "ToSIC_SexyContent_EnableCaching" is disabled
            if(enableCaching)
            {
                var cachingSetting = System.Configuration.ConfigurationManager.AppSettings["ToSIC_SexyContent_EnableCaching"];
                if (!String.IsNullOrEmpty(cachingSetting) && cachingSetting.ToLower() == "false")
                    enableCaching = false;
            }

            // Get Entity Framework ConnectionString
            var entityBuilder = new System.Data.EntityClient.EntityConnectionStringBuilder();
            entityBuilder.ProviderConnectionString = Config.GetConnectionString();
            entityBuilder.Metadata = @"res://ToSic.SexyContent/SexyContent.SexyContentContext.csdl|
								res://ToSic.SexyContent/SexyContent.SexyContentContext.ssdl|
								res://ToSic.SexyContent/SexyContent.SexyContentContext.msl";
            entityBuilder.Provider = "System.Data.SqlClient";

            // Create TemplateContext
            TemplateContext = new SexyContentContext(entityBuilder.ToString(), enableCaching);

            // Set Properties on ContentContext
            ContentContext = EavContext.Instance(zoneId, appId);
            ContentContext.UserName = (HttpContext.Current == null || HttpContext.Current.User == null) ? "Internal" : HttpContext.Current.User.Identity.Name;

            this.ZoneId = zoneId;
            this.AppId = appId;

        }

        /// <summary>
        /// Set EAV's connection string to DNN's
        /// </summary>
        public static void SetEAVConnectionString()
        {
            ToSic.Eav.Configuration.SetConnectionString("SiteSqlServer");
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
            string FileFilter = "*.html";
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

            FileInfo[] Files = Directory.GetFiles(FileFilter, SearchOption.AllDirectories);
            return (from d in Files where d.Name != SexyContent.WebConfigFileName select (d.FullName).Replace(TemplatePathRootMapPath + "\\", "").Replace('\\','/'));
        }

        /// <summary>
        /// Creates a template file if it does not already exists, and uses a default text to insert.
        /// </summary>
        public void CreateTemplateFileIfNotExists(string Name, Template Template, HttpServerUtility Server, string Contents = "")
        {
            if (Template.Type == RazorC)
            {
                if (!Name.StartsWith("_"))
                    Name = "_" + Name;
                if (Path.GetExtension(Name) != ".cshtml")
                    Name += ".cshtml";
            }
            else if (Template.Type == RazorVB)
            {
                if (!Name.StartsWith("_"))
                    Name = "_" + Name;
                if (Path.GetExtension(Name) != ".vbhtml")
                    Name += ".vbhtml";
            }
            else if (Template.Type == TokenReplace)
            {
                if (Path.GetExtension(Name) != ".html")
                    Name += ".html";
            }

            Template.Path = System.Text.RegularExpressions.Regex.Replace(Name, @"[?:\/*""<>|]", "");
            var TemplatePath = Server.MapPath(System.IO.Path.Combine(GetTemplatePathRoot(Template.Location, App), Template.Path));

            if (!File.Exists(TemplatePath))
            {
                StreamWriter Stream = new StreamWriter(File.Create(TemplatePath));
                Stream.Write(Contents);
                Stream.Flush();
                Stream.Close();
            }
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
            var sexyFolderPath = Path.Combine(portalPath, SexyContent.TemplateFolder);

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
            return Path.Combine(ownerPS.HomeDirectory, SexyContent.TemplateFolder);
        }

        /// <summary>
        /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        /// </summary>
        public bool PortalIsConfigured(HttpServerUtility server, string controlPath)
        {
            var sexyFolder = new DirectoryInfo(server.MapPath(Path.Combine(OwnerPS.HomeDirectory, SexyContent.TemplateFolder)));
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, "Content"));
            var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, SexyContent.WebConfigFileName));
            return sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists;
        }

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public static string GetTemplatePathRoot(string locationID, App app)
        {
            string rootFolder = (locationID == LocationIDCurrentPortal ? app.OwnerPS.HomeDirectory : PortalHostDirectory);
            rootFolder += TemplateFolder + "/" + app.Folder;
            return rootFolder;
        }

        #endregion

        #region Template Configuration

        public void CreateOrUpdateTemplateDefault(int TemplateID, string ItemType, int? ContentTypeID, int? DemoEntityID)
        {
            //var DefaultAppContext = new SexyContent(true, DataSource.DefaultZoneId);
            var AttributeSetID = ContentContext.GetAttributeSet(AttributeSetStaticNameTemplateContentTypes).AttributeSetID;
            var Entities = ContentContext.GetEntities(AssignmentObjectTypeIDSexyContentTemplate, TemplateID, null, null);

            var Values = new OrderedDictionary()
                                {
                                    {"ItemType", ItemType},
                                    {"ContentTypeID", ContentTypeID.HasValue ? ContentTypeID.Value.ToString() : "0" },
                                    {"DemoEntityID", DemoEntityID.HasValue ? DemoEntityID.Value.ToString() : "0" }
                                };

            var ExistingEntity =
                Entities.FirstOrDefault(
                    p => p.Values.Any(v => v.Attribute.StaticName == "ItemType" && v.Value == ItemType));

            if (ExistingEntity != null)
                ContentContext.UpdateEntity(ExistingEntity.EntityID, Values);
            else
                ContentContext.AddEntity(AttributeSetID, Values, null, TemplateID, AssignmentObjectTypeIDSexyContentTemplate, 0);
        }

        public TemplateDefault GetTemplateDefault(int templateId, ContentGroupItemType ItemType)
        {
            return GetTemplateDefaults(templateId).FirstOrDefault(t => t.ItemType == ItemType);
        }

        public List<TemplateDefault> GetTemplateDefaults(int TemplateID)
        {
            var Result = new List<TemplateDefault>();
            var Entities = DataSource.GetMetaDataSource(ZoneId.Value, AppId.Value).GetAssignedEntities(AssignmentObjectTypeIDSexyContentTemplate, TemplateID, AttributeSetStaticNameTemplateContentTypes);

            // Add TemplateDefault configured directly in Template
            var Template = TemplateContext.GetTemplate(TemplateID);
            if(Template == null)
                return new List<TemplateDefault>();

            Result.Add(new TemplateDefault() { ContentTypeID = Template.AttributeSetID, DemoEntityID = Template.DemoEntityID, ItemType = ContentGroupItemType.Content });

            Result.AddRange(Entities.Select(e => new TemplateDefault()
                {
                    ItemType = (ContentGroupItemType) Enum.Parse(typeof (ContentGroupItemType), (string) e.Attributes["ItemType"][0]),
                    ContentTypeID = e.Attributes.ContainsKey("ContentTypeID") && e.Attributes["ContentTypeID"][0] != null && (decimal)e.Attributes["ContentTypeID"][0] != 0 ? Convert.ToInt32((decimal)e.Attributes["ContentTypeID"][0]) : new int?(),
                    DemoEntityID = e.Attributes.ContainsKey("DemoEntityID") && e.Attributes["DemoEntityID"][0] != null && (decimal) e.Attributes["DemoEntityID"][0] != 0 ? Convert.ToInt32((decimal) e.Attributes["DemoEntityID"][0]) : new int?()
                }));

            return Result;
        }

        public List<Template> GetCompatibleTemplates(int PortalID, int ContentGroupID)
        {
            var ContentGroupItems = TemplateContext.GetContentGroupItems(ContentGroupID).ToList();
            List<Template> CompatibleTemplates;

            // Prepare some variables
            var List = ContentGroupItems.Count(p => p.ItemType == ContentGroupItemType.Content) > 1;
            var CurrentTemplate = TemplateContext.GetTemplate(ContentGroupItems.First().TemplateID.Value);
            var CurrentDefaults = GetTemplateDefaults(CurrentTemplate.TemplateID);

            CompatibleTemplates = GetTemplates(PortalID).Where(t => t.UseForList || !List).ToList();
            CompatibleTemplates = CompatibleTemplates.Where(c =>
                AreTemplateDefaultsCompatible(ContentGroupItems, CurrentDefaults, GetTemplateDefaults(c.TemplateID))).ToList();

            return CompatibleTemplates;
        }

        private bool AreTemplateDefaultsCompatible(List<ContentGroupItem> ContentGroupItems, IEnumerable<TemplateDefault> Current, IEnumerable<TemplateDefault> New)
        {
            return Current.All(c => IsTemplateDefaultCompatible(ContentGroupItems, c, New.FirstOrDefault(d => d.ItemType == c.ItemType)));
        }

        private bool IsTemplateDefaultCompatible(List<ContentGroupItem> ContentGroupItems, TemplateDefault Current, TemplateDefault New)
        {
            if (Current == null || New == null)
                return false;

            if (!ContentGroupItems.Any(c => c.ItemType == Current.ItemType && c.EntityID.HasValue))
                return true;

            return (Current.ItemType == New.ItemType && Current.ContentTypeID == New.ContentTypeID);
        }

        /// <summary>
        /// Returns all templates from the specified DotNetNuke portal and the current app
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetTemplates(int portalId)
        {
            return TemplateContext.GetAllTemplates().Where(a => a.PortalID == portalId && a.AppID == AppId);
        }

        /// <summary>
        /// Returns all visible templates with the specified PortalID
        /// </summary>
        /// <param name="PortalID"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetVisibleTemplates(int PortalID)
        {
            return GetTemplates(PortalID).Where(t => !t.IsHidden);
        }

        /// <summary>
        /// Returns all visible templates that belongs to the specified portal and use the given AttributeSet.
        /// </summary>
        /// <param name="PortalID">The id of the portal to get the templates from</param>
        /// <param name="AttributeSetID">The id of the AttributeSet</param>
        /// <returns></returns>
        public IEnumerable<Template> GetVisibleTemplates(int PortalID, int AttributeSetID)
        {
            return GetVisibleTemplates(PortalID).Where(t => t.AttributeSetID == AttributeSetID);
        }

        /// <summary>
        /// Returns all visible templates that belongs to the specified portal and use the given AttributeSet, and can be used for lists.
        /// </summary>
        /// <param name="PortalID"></param>
        /// <param name="AttributeSetID"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetVisibleListTemplates(int PortalID, int AttributeSetID)
        {
            return GetVisibleTemplates(PortalID, AttributeSetID).Where(t => t.UseForList);
        }

        /// <summary>
        /// Returns all visible templates that belongs to the specified portal and use the given AttributeSet, and can be used for lists.
        /// </summary>
        /// <param name="PortalID"></param>
        /// <param name="AttributeSetID"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetVisibleListTemplates(int PortalID)
        {
            return GetVisibleTemplates(PortalID).Where(t => t.UseForList);
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
            var roleControl = new DotNetNuke.Security.Roles.RoleController();
            RoleInfo Role = roleControl.GetRoleByName(portalId, SexyContentGroupName);
            if (Role != null)
            {
                System.Collections.ArrayList t = roleControl.GetUsersByRoleName(portalId, SexyContentGroupName);
                if (t.Count > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if a user is in the SexyContent Designers group
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsInSexyContentDesignersGroup(DotNetNuke.Entities.Users.UserInfo user)
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

        #region Preparation of 2Sexy Elements

        /// <summary>
        /// Gets the initial DataSource
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static ToSic.Eav.DataSources.IDataSource GetInitialDataSource(int zoneId, int appId, bool showDrafts = false)
        {
            return DataSource.GetInitialDataSource(zoneId, appId, showDrafts);
        }

        private ConfigurationProvider _configurationProvider;
        private ConfigurationProvider ConfigurationProvider
        {
            get
            {
                if (_configurationProvider == null)
                {
                    _configurationProvider = new ConfigurationProvider();
                    _configurationProvider.Sources.Add("querystring", new QueryStringPropertyAccess());
                    _configurationProvider.Sources.Add("app", new AppPropertyAccess(App));
                    _configurationProvider.Sources.Add("appsettings", new DynamicEntityPropertyAccess(App.Settings));
                    _configurationProvider.Sources.Add("appresources", new DynamicEntityPropertyAccess(App.Resources));
                }
                return _configurationProvider;
            }
        }

        /// <summary>
        /// The EAV DataSource
        /// </summary>
        private ToSic.Eav.DataSources.IDataSource ViewDataSource { get; set; }
        public ToSic.Eav.DataSources.IDataSource GetViewDataSource(int moduleId, bool showDrafts, bool includeEditingData)
        {
            if (ViewDataSource == null)
            {
                var initialSource = GetInitialDataSource(ZoneId.Value, AppId.Value, showDrafts);
                var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(ZoneId, AppId, initialSource, ConfigurationProvider);
                moduleDataSource.ModuleId = moduleId;
                moduleDataSource.IncludeEditingData = true;
                moduleDataSource.Sexy = this;

                var viewDataSource = DataSource.GetDataSource<ViewDataSource>(ZoneId, AppId, moduleDataSource, ConfigurationProvider);
                var moduleSettings = new ModuleController().GetModuleSettings(moduleId);
                viewDataSource.Publish.Enabled = moduleSettings.ContainsKey(SettingsPublishDataSource) && Boolean.Parse(moduleSettings[SettingsPublishDataSource].ToString());
                viewDataSource.Publish.Streams = moduleSettings.ContainsKey(SettingsPublishDataSourceStreams) ? moduleSettings[SettingsPublishDataSourceStreams].ToString() : "Content,ListContent";
                ViewDataSource = viewDataSource;
            }

            return ViewDataSource;
        }


        /// <summary>
        /// Get a list of ContentElements by ModuleId or ContentGroupID
        /// </summary>
        /// <returns></returns>
        public List<Element> GetContentElements(int ModuleID, string LanguageName, int? ContentGroupID, int PortalId, bool showDrafts)
        {
            // ToDo: Refactor
            //return GetElements(ModuleID, ContentGroupID, ContentGroupItemType.Content, ContentGroupItemType.Presentation, LanguageName, PortalId, showDrafts);

            var dataSource = (ModuleDataSource)((IDataTarget)GetViewDataSource(ModuleID, showDrafts, false)).In["Default"].Source;
            return dataSource.ContentElements;
        }

        public Element GetListElement(int ModuleID, string LanguageName, int? ContentGroupID, int PortalId, bool showDrafts)
        {
            // ToDo: Refactor (does not need all parameters)
            //var ListElement = GetElements(ModuleID, ContentGroupID, ContentGroupItemType.ListContent, ContentGroupItemType.ListPresentation, LanguageName, PortalId, showDrafts).FirstOrDefault();
            //return ListElement;

            var dataSource = (ModuleDataSource)((IDataTarget) GetViewDataSource(ModuleID, showDrafts, false)).In["Default"].Source;
            return dataSource.ListElement;
        }

        /// <summary>
        /// Returns the ContentGroupID for a module.
        /// If it is not set, the ModuleID will set as ContentGroupID.
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public int GetContentGroupIdFromModule(int ModuleID)
        {
            var moduleControl = new ModuleController();
            var settings = moduleControl.GetModuleSettings(ModuleID);

            // Set ContentGroupID if not defined in ModuleSettings yet
            if (settings[ContentGroupIDString] == null)
                moduleControl.UpdateModuleSetting(ModuleID, ContentGroupIDString, ModuleID.ToString());

            settings = moduleControl.GetModuleSettings(ModuleID);
            return int.Parse(settings[SexyContent.ContentGroupIDString].ToString());
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
            string editUrl = Globals.NavigateURL(tabId, ControlKeys.EditContentGroup, new string[] { "mid", moduleId.ToString(), "AppId", AppId.ToString(),
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
        public string GetElementEditLink(int ContentGroupID, int SortOrder, int ModuleID, int TabID, string ReturnUrl)
        {
            string EditUrl = Globals.NavigateURL(TabID, ControlKeys.EditContentGroup, "mid", ModuleID.ToString(), SortOrderString, SortOrder.ToString(), ContentGroupIDString, ContentGroupID.ToString());
            EditUrl += (EditUrl.IndexOf("?") == -1 ? "?" : "&") + "popUp=true&ReturnUrl=" + HttpUtility.UrlEncode(ReturnUrl);

            // If Culture exists, add CultureDimension
            var LanguageID = GetCurrentLanguageID();
            if(LanguageID.HasValue)
                EditUrl += "&CultureDimension=" + LanguageID;

            return EditUrl;
        }

        public string GetElementAddWithEditLink(int ContentGroupID, int DestinationSortOrder, int ModuleID, int TabID, string ReturnUrl)
        {
            return GetElementEditLink(ContentGroupID, DestinationSortOrder, ModuleID, TabID, ReturnUrl) + "&EditMode=New";
        }

        public string GetElementSettingsLink(int ContentGroupItemID, int ModuleID, int TabID, string ReturnUrl)
        {
            string SettingsUrl = DotNetNuke.Common.Globals.NavigateURL(TabID, ControlKeys.SettingsWrapper, "mid", ModuleID.ToString(), ContentGroupItemIDString, ContentGroupItemID.ToString());
            SettingsUrl += (SettingsUrl.IndexOf("?") == -1 ? "?" : "&") + "popUp=true&ReturnUrl=" + HttpUtility.UrlEncode(ReturnUrl);
            return SettingsUrl;
        }

        //public void AttachToolbarToElements(List<Element> Elements)
        //{
            
        //}

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

            if(eavAppName != EavContext.DefaultAppName)
            {
                EnsureAppIsConfigured(zoneId, appId);

                // Get app-describing entity
                var appMetaData = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameApps).FirstOrDefault();
                var appResources = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameAppResources).FirstOrDefault();
                var appSettings = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameAppSettings).FirstOrDefault();
                
                if (appMetaData != null)
                {
                    dynamic appMetaDataDynamic = new DynamicEntity(appMetaData, new[] { System.Threading.Thread.CurrentThread.CurrentCulture.Name }, null);
                    dynamic appResourcesDynamic = appResources != null ? new DynamicEntity(appResources, new[] {System.Threading.Thread.CurrentThread.CurrentCulture.Name}, null) : null;
                    dynamic appSettingsDynamic = appResources != null ? new DynamicEntity(appSettings, new[] {System.Threading.Thread.CurrentThread.CurrentCulture.Name}, null) : null;

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

            if (eavAppName == EavContext.DefaultAppName)
                return;

            var appContext = new SexyContent(zoneId, appId);

            if (appMetaData == null)
            {
                // Add app-describing entity
                var appAttributeSet = appContext.ContentContext.GetAttributeSet(AttributeSetStaticNameApps).AttributeSetID;
                var values = new OrderedDictionary()
                {
                    {"DisplayName", String.IsNullOrEmpty(appName) ? eavAppName : appName },
                    {"Folder", String.IsNullOrEmpty(appName) ? eavAppName : appName },
                    {"AllowTokenTemplates", "False"},
                    {"AllowRazorTemplates", "False"},
                    {"Version", "00.00.01"},
                    {"OriginalId", ""}
                };
                appContext.ContentContext.AddEntity(appAttributeSet, values, null, appId, AssignmentObjectTypeIDSexyContentApp);
            }

            if(appSettings == null)
            { 
                // Add new (empty) ContentType for Settings
                var settingsAttributeSet = appContext.ContentContext.AddAttributeSet(AttributeSetStaticNameAppSettings, "Stores settings for an app", AttributeSetStaticNameAppSettings, AttributeSetScopeApps);
                DataSource.GetCache(zoneId, appId).PurgeCache(zoneId, appId);
                appContext.ContentContext.AddEntity(settingsAttributeSet, new OrderedDictionary() { }, null, appId, AssignmentObjectTypeIDSexyContentApp);
            }

            if(appResources == null)
            {
                // Add new (empty) ContentType for Resources
                var resourcesAttributeSet = appContext.ContentContext.AddAttributeSet(AttributeSetStaticNameAppResources, "Stores resources like translations for an app", AttributeSetStaticNameAppResources, AttributeSetScopeApps);
                DataSource.GetCache(zoneId, appId).PurgeCache(zoneId, appId);
                appContext.ContentContext.AddEntity(resourcesAttributeSet, new OrderedDictionary() { }, null, appId, AssignmentObjectTypeIDSexyContentApp);
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
            var app = sexy.ContentContext.AddApp(Guid.NewGuid().ToString());
            sexy.ContentContext.SaveChanges();

            EnsureAppIsConfigured(zoneId, app.AppID, appName);

            return GetApp(zoneId, app.AppID, ownerPS);
        }

        public static int? GetAppSettingsAttributeSetId(int zoneId, int appId)
        {
            if (appId == GetDefaultAppId(zoneId))
                return null;

            return new SexyContent(zoneId, appId).GetAvailableAttributeSets(AttributeSetScopeApps)
                .Single(p => p.StaticName == AttributeSetStaticNameAppSettings).AttributeSetID;
        }

        public static int? GetAppResourcesAttributeSetId(int zoneId, int appId)
        {
            if (appId == GetDefaultAppId(zoneId))
                return null;

            return new SexyContent(zoneId, appId).GetAvailableAttributeSets(AttributeSetScopeApps)
                .Single(p => p.StaticName == AttributeSetStaticNameAppResources).AttributeSetID;
        }

        public static int? GetAppIdFromModule(ModuleInfo module)
        {
            var zoneId = GetZoneID(module.OwnerPortalID);
            
            if (module.DesktopModule.ModuleName == "2sxc")
            {
                if (zoneId.HasValue)
                    return SexyContent.GetDefaultAppId(zoneId.Value);
                else
                    return new int?();
            }

            object appIdString = null;

            if (HttpContext.Current != null)
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["AppId"]))
                    appIdString = HttpContext.Current.Request.QueryString["AppId"];
                else
                {
                    // Get AppId from ModuleSettings
                    appIdString = new ModuleController().GetModuleSettings(module.ModuleID)[SexyContent.AppIDString];
                }
            }

            int appId;
            if (appIdString != null && int.TryParse(appIdString.ToString(), out appId))
                return appId;

            return null;
        }

        public void RemoveApp(int appId, int userId)
        {
            if(appId != this.ContentContext.AppId)
                throw new Exception("An app can only be removed inside of it's own context.");

            if(appId == GetDefaultAppId(ZoneId.Value))
                throw new Exception("The default app of a zone cannot be removed.");

            var sexyApp = GetApp(ZoneId.Value, appId, OwnerPS);
            var eavApp = ContentContext.GetApps().Single(a => a.AppID == appId);

            // Delete templates
            var templates = TemplateContext.Templates.Where(t => t.AppID == appId).ToList();
            templates.ForEach(t => TemplateContext.HardDeleteTemplate(t.TemplateID, userId));
            TemplateContext.SaveChanges();

            // Delete folder
            if (!String.IsNullOrEmpty(sexyApp.Folder) &&  Directory.Exists(sexyApp.PhysicalPath))
                Directory.Delete(sexyApp.PhysicalPath, true);

            // Delete the app
            ContentContext.DeleteApp(appId);
        }

        public static int GetDefaultAppId(int zoneId)
        {
            return ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].DefaultAppId;
        }

        #endregion Apps

        #region ContentGroupItem Management

        /// <summary>
        /// Adds a ContentGroupItem to a specified ContentGroup and returns the created ContentGroupItem.
        /// </summary>
        /// <param name="ContentGroupID"></param>
        /// <param name="UserID"></param>
        /// <param name="TemplateID"></param>
        /// <param name="EntityID"></param>
        /// <param name="DestinationSortOrder"></param>
        /// <param name="AutoSave">Prevents from saving the item if false</param>
        /// <returns></returns>
        public ContentGroupItem AddContentGroupItem(int ContentGroupID, int UserID, int? TemplateID, int? EntityID, int? DestinationSortOrder, bool AutoSave, ContentGroupItemType ItemType, bool PreventSorting)
        {
            var Item = new ContentGroupItem()
            {
                ContentGroupID = ContentGroupID,
                SysCreatedBy = UserID,
                SysModifiedBy = UserID,
                SysCreated = DateTime.Now,
                SysModified = DateTime.Now,
                SortOrder = DestinationSortOrder.HasValue ? DestinationSortOrder.Value : 0,
                TemplateID = TemplateID,
                EntityID = EntityID,
                Type = ItemType.ToString()
            };

            Item = TemplateContext.AddContentGroupItem(Item);

            if (AutoSave)
                TemplateContext.SaveChanges();

            if (!PreventSorting)
            {
                var GroupItems = TemplateContext.GetContentGroupItems(ContentGroupID);

                if (GroupItems.Any(p => p != Item))
                {
                    Item.SortOrder = GroupItems.Where(p => p != Item).Max(p => p.SortOrder) + 1;
                }

                if (DestinationSortOrder.HasValue)
                    TemplateContext.ReorderContentGroupItem(Item, DestinationSortOrder.Value, AutoSave);
            }

            if (AutoSave)
                TemplateContext.SaveChanges();

            return Item;
        }

        public void UpdateTemplateForGroup(int ContentGroupID, int? TemplateID, int UserID)
        {
            List<ContentGroupItem> Items = TemplateContext.GetContentGroupItems(ContentGroupID, ContentGroupItemType.Content).ToList();

            if (!Items.Any())
                Items.Add(AddContentGroupItem(ContentGroupID, UserID, null, null, null, true, ContentGroupItemType.Content, false));

            TemplateID = TemplateID == 0 ? null : TemplateID;
            Items.ForEach(p => p.TemplateID = TemplateID);

            TemplateContext.SaveChanges();
        }

        public IEnumerable<AttributeSet> GetAvailableAttributeSets(string scope)
        {
            return GetAvailableAttributeSets().Where(p => p.Scope == scope);
        }

        public IEnumerable<AttributeSet> GetAvailableAttributeSets()
        {
            return ContentContext.GetAllAttributeSets().Where(c => !c.Name.StartsWith("@") && !c.ChangeLogIDDeleted.HasValue).OrderBy(c => c.Name);
        }

        public IEnumerable<AttributeSet> GetAvailableAttributeSetsForVisibleTemplates(int PortalId)
        {
            var AvailableTemplates = GetVisibleTemplates(PortalId);
            return GetAvailableAttributeSets(SexyContent.AttributeSetScope).Where(p => AvailableTemplates.Any(t => t.AttributeSetID == p.AttributeSetID)).OrderBy(p => p.Name);
        }

        /// <summary>
        /// Returns if any ContentItem with the TemplateID and ItemType specified is in use.
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <param name="ItemType"></param>
        /// <returns></returns>
        public bool IsTemplateDefaultInUse(int TemplateID, ContentGroupItemType ItemType)
        {
            return TemplateContext.GetContentGroupItems().Any(c => c.TemplateID == TemplateID && c.ItemType == ItemType && c.EntityID.HasValue);
        }

        #endregion

        #region DNN Interface Members

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            return new SearchController().GetModifiedSearchDocuments(moduleInfo, beginDate);
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
            var zoneSettingKey = SexyContent.PortalSettingsPrefix + "ZoneID";
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
                PortalController.UpdatePortalSetting(PortalID, SexyContent.PortalSettingsPrefix + "ZoneID", ZoneID.Value.ToString());
            else
                PortalController.DeletePortalSetting(PortalID, SexyContent.PortalSettingsPrefix + "ZoneID");
        }

        public static List<Zone> GetZones()
        {
            return new SexyContent(DataSource.DefaultZoneId, GetDefaultAppId(DataSource.DefaultZoneId)).ContentContext.GetZones();
        }

        public static Zone AddZone(string zoneName)
        {
            return
                new SexyContent(DataSource.DefaultZoneId, GetDefaultAppId(DataSource.DefaultZoneId)).ContentContext
                    .AddZone(zoneName).Item1;
        }

        #endregion

        #region Culture Handling

        [Obsolete("Don't use this anymore, use 'GetCurrentLanguageName' (work with strings)")]
        public int? GetCurrentLanguageID(bool UseDefaultLanguageIfNotFound = false)
        {
            var LanguageID = ContentContext.GetLanguageId(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            if (!LanguageID.HasValue && UseDefaultLanguageIfNotFound)
                LanguageID = ContentContext.GetLanguageId(PortalSettings.Current.DefaultLanguage);
            return LanguageID;
        }

        public string GetCurrentLanguageName()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        public void SetCultureState(string CultureCode, bool Active, int PortalID)
        {
            var EAVLanguage = ContentContext.GetLanguages().Where(l => l.ExternalKey == CultureCode).FirstOrDefault();
            // If the language exists in EAV, set the active state, else add it
            if (EAVLanguage != null)
                ContentContext.UpdateDimension(EAVLanguage.DimensionID, Active);
            else
            {
                var CultureText = LocaleController.Instance.GetLocale(CultureCode).Text;
                ContentContext.AddLanguage(CultureText, CultureCode);
            }
        }


        /// <summary>
        /// Returns all DNN Cultures with active / inactive state
        /// </summary>
        public static List<CulturesWithActiveState> GetCulturesWithActiveState(int portalId, int zoneId)
        {
            //var DefaultLanguageID = ContentContext.GetLanguageId();
            var AvailableEAVLanguages = new SexyContent(zoneId, SexyContent.GetDefaultAppId(zoneId)).ContentContext.GetLanguages();
            var DefaultLanguageCode = new PortalSettings(portalId).DefaultLanguage;
            var DefaultLanguage = AvailableEAVLanguages.Where(p => p.ExternalKey == DefaultLanguageCode).FirstOrDefault();
            var DefaultLanguageIsActive = DefaultLanguage != null && DefaultLanguage.Active;

            return (from c in LocaleController.Instance.GetLocales(portalId)
                    select new CulturesWithActiveState()
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
            return new SexyContent(zoneId, GetDefaultAppId(zoneId)).ContentContext.GetLanguageId(externalKey);
        }

        #endregion

        #region Helper Methods

        public static void AddDNNVersionToBodyClass(Control Parent)
        {
            // Add DNN Version to body as CSS Class
            string CssClass = "dnn-" + System.Reflection.Assembly.GetAssembly(typeof(DotNetNuke.Common.Globals)).GetName().Version.Major;
            HtmlGenericControl body = (HtmlGenericControl)Parent.Page.FindControl("ctl00$body");
            if(body.Attributes["class"] != null)
                body.Attributes["class"] += CssClass;
            else
                body.Attributes["class"] = CssClass;
        }

        public bool IsEditMode()
        {
            return DotNetNuke.Common.Globals.IsEditMode() && (OwnerPS.PortalId == PS.PortalId);
        }

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="dimensionID"></param>
        /// <returns></returns>
        public string GetJsonFromStreams(ToSic.Eav.DataSources.IDataSource source, string[] streamsToPublish)
        {
            var language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            var y = source.Out.Where(s => streamsToPublish.Contains(s.Key)).ToDictionary(k => k.Key, s => new
            {
                List = (from c in s.Value.List select GetDictionaryFromEntity(c.Value, language)).ToList()
            });

            return y.ToJson();
        }

        internal Dictionary<string, object> GetDictionaryFromEntity(IEntity entity, string language)
        {
            var dynamicEntity = new DynamicEntity(entity, new[] { language }, this);
            bool propertyNotFound;
            var dictionary = ((from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v => dynamicEntity.GetEntityValue(v.Name, out propertyNotFound)));
            dictionary.Add("EntityId", entity.EntityId);

            if(entity is IHasEditingData)
                dictionary.Add("_2sxcEditInformation", new { sortOrder = ((IHasEditingData)entity).SortOrder });
            else
                dictionary.Add("_2sxcEditInformation", new { entityId = entity.EntityId, title = entity.Title != null ? entity.Title[language] : "(no title)" });

            return dictionary;
        }

        /// <summary>
        /// Resolves File and Page values
        /// For example, File:123 or Page:123
        /// </summary>
        public static string ResolveHyperlinkValues(string value, PortalSettings ownerPortalSettings)
        {
            var resultString = (string)value;
            var regularExpression = Regex.Match(resultString, @"^(?<type>(file|page)):(?<id>[0-9]+)(?<params>(\?|\#).*)?$", RegexOptions.IgnoreCase);

            if (!regularExpression.Success)
                return value;

            var fileManager = FileManager.Instance;
            var tabController = new DotNetNuke.Entities.Tabs.TabController();
            var type = regularExpression.Groups["type"].Value.ToLower();
            var id = int.Parse(regularExpression.Groups["id"].Value);
			var urlParams = regularExpression.Groups["params"].Value ?? "";

            switch (type)
            {
                case "file":
                    var fileInfo = fileManager.GetFile(id);
                    if(fileInfo != null)
                        resultString = fileManager.GetUrl(fileInfo) + urlParams;
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

        public bool CanDeleteEntity(int entityId)
        {
            var templates = GetTemplates(OwnerPS.PortalId);
            var templateDefaults = templates.ToList().Select(t => new {Template = t, Defaults = GetTemplateDefaults(t.TemplateID)});
            var contentGroupItems = TemplateContext.GetContentGroupItems();

            // Check all templates
            if (templates.Any(t => t.DemoEntityID == entityId))
                return false;

            // Check template defaults (Presentation, ListContent, ListPresentation)
            if(templateDefaults.Any(d => d.Defaults.Any(de => de.DemoEntityID == entityId)))
                return false;

            // Check ContentGroupItems
            if (contentGroupItems.Any(c => c.EntityID == entityId))
                return false;

            return true;
        }

        #endregion

        #region Upgrade

        public string UpgradeModule(string Version)
        {
            switch (Version)
            {
                case "05.05.00":
                    // While upgrading to 05.04.02, make sure the template folders get renamed to "2sxc"
                    var portalController = new PortalController();
                    var portals = portalController.GetPortals();
                    var pathsToCopy = portals.Cast<PortalInfo>().Select(p => p.HomeDirectoryMapPath).ToList();
                    pathsToCopy.Add(HttpContext.Current.Server.MapPath("~/Portals/_default/"));
                    foreach (var path in pathsToCopy)
                    {
                        var portalFolder = new DirectoryInfo(path);
                        if (portalFolder.Exists)
                        {
                            var oldSexyFolder = new DirectoryInfo(Path.Combine(path, "2sexy"));
                            var newSexyFolder = new DirectoryInfo(Path.Combine(path, "2sxc"));
                            var newSexyContentFolder = new DirectoryInfo(Path.Combine(newSexyFolder.FullName, "Content"));
                            if (oldSexyFolder.Exists && !newSexyFolder.Exists)
                            {
                                // Move 2sexy directory to 2scx/Content
                                DirectoryCopy(oldSexyFolder.FullName, newSexyContentFolder.FullName, true);

                                // Leave info message in the content folder
                                File.WriteAllText(Path.Combine(oldSexyFolder.FullName, "__WARNING - old copy of files - READ ME.txt"), "This is a short information\r\n\r\n2sxc renamed the main folder from \"[Portal]/2Sexy\" to \"[Portal]/2sxc\" in version 5.5.\r\n\r\nTo make sure that links to images/css/js still work, the old folder was copied and this was left. Please clean up and delete the entire \"[Portal]/2Sexy/\" folder once you're done. \r\n\r\nMany thanks!\r\n2sxc\r\n\r\nPS: Remember that you might have ClientDependency activated, so maybe you still have bundled & minified  JS/CSS-Files in your cache pointing to the old location. When done cleaning up, we recommend increasing the version just to be sure you're not seeing an old files that don't exist any more. ");

                                // Move web.config (should be directly in 2sxc)
                                if (File.Exists(Path.Combine(newSexyContentFolder.FullName, "web.config")))
                                    File.Move(Path.Combine(newSexyContentFolder.FullName, "web.config"), Path.Combine(newSexyFolder.FullName, "web.config"));
                                
                            }
                        }
                    }
                    break;
            }

            // Increase ClientDependency version upon each upgrade (System and all Portals)
            // prevents browsers caching old JS and CSS files for editing, which could cause several errors
            ClientResourceManager.UpdateVersion();

            return Version;
        }

        /// <summary>
        /// Method to copy a directory, source: http://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        
        #endregion

    }
}