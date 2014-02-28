using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Search;
using ToSic.Eav;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Modules;
using System.Reflection;
using System.Web.UI.HtmlControls;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Caches;
using ToSic.SexyContent;
using FileInfo = System.IO.FileInfo;
using DotNetNuke.Common;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Business Layer
    /// Centralizes all global constants and business logic
    /// </summary>
    public class SexyContent : ISearchable, IUpgradeable
    {
        #region Constants

        public const string ModuleVersion = "05.05.01";
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
            public const string EditTemplateFile = "edittemplatefile";
            public const string AddItem = "additem";
            public const string EditTemplateDefaults = "edittemplatedefaults";
            public const string GettingStarted = "gettingstarted";
            public const string PortalConfiguration = "portalconfiguration";
            public const string EditDataSource = "editdatasource";
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
        public const string AttributeSetStaticNameTemplateMetaData = "2SexyContent-Template-Metadata";
        public const string AttributeSetStaticNameTemplateContentTypes = "2SexyContent-Template-ContentTypes";
        public const string AttributeSetStaticNameApps = "2SexyContent-App";
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
            get { return GetApp(ZoneId.Value, AppId.Value); }
        }

        /// <summary>
        /// The EAV DataSource
        /// </summary>
        public ToSic.Eav.DataSources.IDataSource SexyDataSource { get; internal set; }

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
        /// Returns the AssignmentObjectTypeID for 2SexyContent Templates
        /// </summary>        
        public static int AssignmentObjectTypeIDSexyContentTemplate
        {
            get
            {
                return DataSource.GetCache(DataSource.DefaultZoneId, DataSource.MetaDataAppId).GetAssignmentObjectTypeId("2SexyContent-Template");
            }
        }

        /// <summary>
        /// Returns the AssignmentObjectTypeID for 2SexyContent Apps
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

        // ToDo: needed for DNN search
        ///// <summary>
        ///// Constructor overload for DotNetNuke
        ///// (BusinessControllerClass needs parameterless constructor)
        ///// </summary>
        //public SexyContent()
        //    : this(new int?(), new int?(), true)
        //{
            
        //}



        /// <summary>
        /// Instanciates Content and Template-Contexts
        /// </summary>
        public SexyContent(int zoneId, int appId, bool enableCaching = true)
        {
            SetEAVConnectionString();

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
        /// <param name="Server">The HttpServerUtility for MapPath</param>
        /// <param name="PortalSettings"></param>
        /// <param name="TemplateType"></param>
        /// <param name="TemplateLocation"></param>
        /// <param name="ControlPath">The PortalModuleBase.ControlPath property</param>
        /// <returns></returns>
        public IEnumerable<string> GetTemplateFiles(HttpServerUtility Server, PortalSettings PortalSettings, string TemplateType, string TemplateLocation)
        {
            string TemplatePathRootMapPath = Server.MapPath(GetTemplatePathRoot(TemplateLocation));
            DirectoryInfo Directory = new DirectoryInfo(TemplatePathRootMapPath);

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
        /// <param name="Name"></param>
        /// <param name="Template"></param>
        /// <param name="PortalSettings"></param>
        /// <param name="Server"></param>
        /// <param name="Contents"></param>
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
            var TemplatePath = Server.MapPath(System.IO.Path.Combine(GetTemplatePathRoot(Template.Location), Template.Path));

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
            var portalPath = templateLocation == TemplateLocations.HostFileSystem ? server.MapPath(PortalHostDirectory) : PortalSettings.Current.HomeDirectoryMapPath;
            var sexyFolderPath = Path.Combine(portalPath, SexyContent.TemplateFolder);

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            if (!sexyFolder.Exists)
                sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles("web.config").Any())
                File.Copy(server.MapPath(WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, WebConfigFileName));

            // Create a Content folder (default app)
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, App.Folder));
            if (!contentFolder.Exists)
                contentFolder.Create();
        }

        /// <summary>
        /// Configures a Portal (Creates a 2sxc Folder containing a web.config File)
        /// </summary>
        /// <param name="server"></param>
        /// <param name="PortalSettings"></param>
        /// <param name="ControlPath"></param>
        public void ConfigurePortal(HttpServerUtility server)
        {
            //var templatePathRootMapPath = server.MapPath(Path.Combine(PortalSettings.Current.HomeDirectory, SexyContent.TemplateFolder));
            //var directory = new DirectoryInfo(templatePathRootMapPath);
            EnsureTemplateFolderExists(server, TemplateLocations.PortalFileSystem);
        }

        /// <summary>
        /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        /// </summary>
        /// <param name="server"></param>
        /// <param name="controlPath"></param>
        /// <param name="PortalSettings"></param>
        /// <returns></returns>
        public static bool PortalIsConfigured(HttpServerUtility server, string controlPath)
        {
            var sexyFolder = new DirectoryInfo(server.MapPath(Path.Combine(PortalSettings.Current.HomeDirectory, SexyContent.TemplateFolder)));
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, "Content"));
            var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, SexyContent.WebConfigFileName));
            return sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists;
        }

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="PortalSettings"></param>
        /// <returns></returns>
        public string GetTemplatePathRoot(string locationID)
        {
            string RootFolder = (locationID == LocationIDCurrentPortal ? PortalSettings.Current.HomeDirectory : PortalHostDirectory);
            // Hard-coded /Content/ app (default app)
            RootFolder += TemplateFolder + "/" + App.Folder;
            return RootFolder;
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

        #region User Management
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
        #endregion

        #region Preparation of 2Sexy Elements

        /// <summary>
        /// Gets the initial DataSource
        /// Will use the current portal's Zone if no zoneId is set
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public static ToSic.Eav.DataSources.IDataSource GetInitialDataSource(int zoneId, int appId)
        {
            return DataSource.GetInitialDataSource(zoneId, appId);
        }

        /// <summary>
        /// Get a list of ContentElements by ModuleId or ContentGroupID
        /// </summary>
        /// <returns></returns>
        public List<Element> GetContentElements(int ModuleID, string LanguageName, int? ContentGroupID, int PortalId)
        {
            return GetElements(ModuleID, ContentGroupID, ContentGroupItemType.Content, ContentGroupItemType.Presentation, LanguageName, PortalId);
        }

        public Element GetListElement(int ModuleID, string LanguageName, int? ContentGroupID, int PortalId)
        {
            var ListElement = GetElements(ModuleID, ContentGroupID, ContentGroupItemType.ListContent, ContentGroupItemType.ListPresentation, LanguageName, PortalId).FirstOrDefault();
            return ListElement;
        }

        private List<Element> GetElements(int ModuleID, int? ContentGroupID, ContentGroupItemType ContentItemType, ContentGroupItemType PresentationItemType, string LanguageName, int PortalId)
        {
            if (!ContentGroupID.HasValue)
                ContentGroupID = GetContentGroupIDFromModule(ModuleID);


            IEnumerable<ContentGroupItem> ItemsQuery = TemplateContext.GetContentGroupItems(ContentGroupID.Value);
            List<ContentGroupItem> Items = ItemsQuery.Where(c => c.Type == ContentItemType.ToString("F") || c.Type == PresentationItemType.ToString("F")).ToList();
            List<Element> ElementList = new List<Element>();

            if (ItemsQuery.Any(c => c.TemplateID.HasValue))
            {
                var Defaults = GetTemplateDefaults(ItemsQuery.First().TemplateID.Value);

                // Get List of Entities
                List<int> Identities = (from c in Items where c.EntityID.HasValue && c.EntityID.Value > 0 select c.EntityID.Value).ToList();
                // Add Demo Entities
                Identities.AddRange(Defaults.Where(d => d.DemoEntityID.HasValue).Select(d => d.DemoEntityID.Value));
                // Prepare Dimension List (Languages)
                var DimensionIds = new[] { LanguageName };
                // Load all Entities to list
                var InitialSource = GetInitialDataSource(ZoneId.Value, AppId.Value);
                var EntityDataSource = DataSource.GetDataSource("ToSic.Eav.DataSources.EntityIdFilter", ZoneId, AppId, InitialSource);
                ((EntityIdFilter)EntityDataSource).Configuration["EntityIds"] = String.Join(",", Identities.ToArray());
                SexyDataSource = DataSource.GetDataSource("ToSic.Eav.DataSources.PassThrough", ZoneId, AppId, EntityDataSource);
                var Entities = SexyDataSource.Out["Default"].List;// ContentContext.GetEntityModel(Identities);

                // If no Content Elements exist and type is List, add a ContentGroupItem to List (not to DB)
                if (ContentItemType == ContentGroupItemType.ListContent && Items.All(p => p.ItemType != ContentGroupItemType.ListContent))
                {
                    var ListContentDefault = Defaults.FirstOrDefault(d => d.ItemType == ContentGroupItemType.ListContent);
                    var TemplateID = ItemsQuery.First().TemplateID.Value;

                    Items.Add(new ContentGroupItem()
                    {
                        ContentGroupID = ContentGroupID.Value,
                        ContentGroupItemID = -1,
                        EntityID = ListContentDefault != null ? ListContentDefault.DemoEntityID : new int?(),
                        SortOrder = -1,
                        SysCreated = DateTime.Now,
                        SysCreatedBy = -1,
                        TemplateID = TemplateID,
                        Type = ContentGroupItemType.ListContent.ToString("F")
                    });
                }

                // Transform to list of Elements
                ElementList = (from c in Items
                               where c.ItemType == ContentItemType
                               select new Element
                               {
                                   ID = c.ContentGroupItemID,
                                   EntityId = c.EntityID,
                                   TemplateId = c.TemplateID,
                                   Content = c.EntityID.HasValue ? new DynamicEntity(Entities[c.EntityID.Value], DimensionIds) :
                                       Defaults.Where(d => d.ItemType == ContentItemType && d.DemoEntityID.HasValue)
                                       .Select(d => new DynamicEntity(Entities[d.DemoEntityID.Value], DimensionIds)).FirstOrDefault(),
                                   // Get Presentation object - Take Default if it does not exist
                                   Presentation = (from p in Items
                                                   where p.SortOrder == c.SortOrder && p.ItemType == PresentationItemType && p.EntityID.HasValue
                                                   select new DynamicEntity(Entities[p.EntityID.Value], DimensionIds)).FirstOrDefault() ??
                                                   (from d in Defaults
                                                    where d.ItemType == PresentationItemType && d.DemoEntityID.HasValue
                                                    select new DynamicEntity(Entities[d.DemoEntityID.Value], DimensionIds)).FirstOrDefault()
                                                   ,
                                   GroupID = c.ContentGroupID,
                                   SortOrder = c.SortOrder
                               }).ToList();
            }

            return ElementList;
        }

        /// <summary>
        /// Returns the ContentGroupID for a module.
        /// If it is not set, the ModuleID will set as ContentGroupID.
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public int GetContentGroupIDFromModule(int ModuleID)
        {
            ModuleController ModuleControl = new ModuleController();
            Hashtable Settings = ModuleControl.GetModuleSettings(ModuleID);

            // Set ContentGroupID if not defined in ModuleSettings yet
            if (Settings[ContentGroupIDString] == null)
                ModuleControl.UpdateModuleSetting(ModuleID, ContentGroupIDString, ModuleID.ToString());

            Settings = ModuleControl.GetModuleSettings(ModuleID);

            return int.Parse(Settings[SexyContent.ContentGroupIDString].ToString());
        }
        #endregion

        #region URL Handling / Toolbar

        /// <summary>
        /// Get Toolbar HTML for given Element
        /// </summary>
        /// <param name="Element"></param>
        /// <param name="HostingModule"></param>
        /// <param name="LocalResourcesPath"></param>
        /// <param name="ListEnabled"></param>
        /// <returns></returns>
        public string GetElementToolbar(int ContentGroupID, int SortOrder, int ContentGroupItemID, int ModuleId, string LocalResourcesPath, bool ListEnabled, Control ParentControl, string ReturnUrl)
        {
            string editLink = GetElementEditLink(ContentGroupID, SortOrder, ModuleId, PortalSettings.Current.ActiveTab.TabID, ReturnUrl);
            if (PortalSettings.Current.EnablePopUps)
                editLink = HttpUtility.UrlDecode(UrlUtils.PopUpUrl(editLink, ParentControl, PortalSettings.Current, false, false));

            string addLink = GetElementAddWithEditLink(ContentGroupID, SortOrder + 1, ModuleId, PortalSettings.Current.ActiveTab.TabID, ReturnUrl);
            if (PortalSettings.Current.EnablePopUps)
                addLink = HttpUtility.UrlDecode(UrlUtils.PopUpUrl(addLink, ParentControl, PortalSettings.Current, false, false));

            string Toolbar = "<ul class=\"sc-menu\">";
            

            Toolbar += "<li><a class=\"sc-menu-edit\" href=\"" + editLink + "\"><img src=\"" + ParentControl.ResolveClientUrl("~/DesktopModules/ToSIC_SexyContent/Images/Edit.png") + "\" /></a></li>";

            if (ListEnabled && SortOrder != -1)
            {
                Toolbar += "<li><a class=\"sc-menu-add\" href=\"javascript:void(0);\" onclick='AddContentGroupItem(this, \"" + ContentGroupItemID.ToString() + "\");'><img src=\"" + ParentControl.ResolveClientUrl("~/DesktopModules/ToSIC_SexyContent/Images/Add.png") + "\" /></a></li>";
                Toolbar += "<li><a class=\"sc-menu-addwithedit\" href=\"" + addLink + "\"><img src=\"" + ParentControl.ResolveClientUrl("~/DesktopModules/ToSIC_SexyContent/Images/AddWithEdit.png") + "\" /></a></li>";
            }

            Toolbar += "</ul>";
            return Toolbar;
        }

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
        public static string GetMetaDataEditUrl(int tabId, int moduleId, string returnUrl, Control control, string attributeSetStaticName, int assignmentObjectTypeID, int keyNumber, int zoneId, int appId)
        {
            var portalSettings = PortalSettings.Current;

            //var DefaultAppContext = new SexyContent(true, DataSource.DefaultZoneId);
            var Set = new SexyContent(zoneId, appId).ContentContext.GetAttributeSet(attributeSetStaticName);
            string NewItemUrl = UrlUtils.PopUpUrl(Globals.NavigateURL(tabId, ControlKeys.EavManagement, "mid=" + moduleId.ToString() + "&ManagementMode=NewItem&AttributeSetId=[AttributeSetId]&KeyNumber=[KeyNumber]&AssignmentObjectTypeId=[AssignmentObjectTypeId]&ReturnUrl=[ReturnUrl]&" + SexyContent.AppIDString + "=" + appId), control, portalSettings, false, true);
            string EditItemUrl = UrlUtils.PopUpUrl(Globals.NavigateURL(tabId, ControlKeys.EavManagement, "mid=" + moduleId.ToString() + "&ManagementMode=EditItem&EntityId=[EntityId]&ReturnUrl=[ReturnUrl]&" + SexyContent.AppIDString + "=" + appId), control, portalSettings, false, true);
            return ToSic.Eav.ManagementUI.Forms.GetItemFormUrl(keyNumber, Set.AttributeSetID, assignmentObjectTypeID, NewItemUrl, EditItemUrl, returnUrl);
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

        public void AttachToolbarToElements(List<Element> Elements, int ModuleID, string LocalResourcesPath, bool ListEnabled, bool IsEditable, Control ParentControl, string ReturnUrl)
        {
            // Add Toolbar if neccessary, else add empty string to dictionary
            if(!IsEditable)
                Elements.Where(p => p.Content != null).ToList().ForEach(p => ((DynamicEntity)p.Content).Toolbar = "");
            else
                Elements.Where(p => p.Content != null).ToList().ForEach(p => ((DynamicEntity)p.Content).Toolbar = GetElementToolbar(p.GroupID, p.SortOrder, p.ID, ModuleID, LocalResourcesPath, ListEnabled, ParentControl, ReturnUrl));
        }

        #endregion

        #region

        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <param name="includeDefaultApp"></param>
        /// <returns></returns>
        public static List<App> GetApps(int zoneId, bool includeDefaultApp)
        {
            var eavApps = ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps;
            var sexyApps = eavApps.Select(eavApp => GetApp(zoneId, eavApp.Key));

            if (!includeDefaultApp)
                sexyApps = sexyApps.Where(a => a.Name != "Content");

            return sexyApps.ToList();
        }

        public static App GetApp(int zoneId, int appId)
        {
            // Get appName from cache
            var eavAppName = ((BaseCache) DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps[appId];

            // Get app-describing entity
            var appMetaData = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(AssignmentObjectTypeIDSexyContentApp, appId, AttributeSetStaticNameApps).FirstOrDefault();
            App sexyApp;
            if (appMetaData != null)
            {
                dynamic appMetaDataDynamic = new DynamicEntity(appMetaData, new[] { System.Threading.Thread.CurrentThread.CurrentCulture.Name });

                sexyApp = new App()
                {
                    AppId = appId,
                    Name = appMetaDataDynamic.DisplayName,
                    Folder = appMetaDataDynamic.Folder,
                    Configuration = appMetaDataDynamic
                    // ToDo: Resources, Settings, Hidden, etc.
                };
            }
            // Handle default app
            else if (eavAppName == EavContext.DefaultAppName)
            {
                sexyApp = new App()
                {
                    AppId = appId,
                    Name = "Content",
                    Folder = "Content",
                    Configuration = null
                };
            }
            else
                throw new Exception("App must be the default app (Content) or have a description-entity.");

            return sexyApp;
        }

        public static void AddApp(int zoneId, string appName)
        {
            if(appName == "Content" || appName == "Default" || String.IsNullOrEmpty(appName) || !Regex.IsMatch(appName, "^[0-9A-Za-z -_]+$"))
                throw new ArgumentOutOfRangeException("appName '" + appName + "' not allowed");

            // Adding app to EAV
            var sexy = new SexyContent(zoneId, GetDefaultAppId(zoneId));
            var app = sexy.ContentContext.AddApp(Guid.NewGuid().ToString());
            sexy.ContentContext.SaveChanges();
            
            // Add app-describing entity
            var appContext = new SexyContent(zoneId, app.AppID);
            var appAttributeSet = appContext.ContentContext.GetAttributeSet(AttributeSetStaticNameApps).AttributeSetID;
            var values = new OrderedDictionary() {
                { "DisplayName", appName },
                { "Folder", appName }
            };
            appContext.ContentContext.AddEntity(appAttributeSet, values, null, app.AppID, AssignmentObjectTypeIDSexyContentApp);

            // Add new (empty) ContentType for Settings
            appContext.ContentContext.AddAttributeSet("App-Settings", "Stores settings for an app", "App-Settings", "2SexyContent-App");

            // Add new (empty) ContentType for Resources
            appContext.ContentContext.AddAttributeSet("App-Resources", "Stores resources like translations for an app", "App-Resources", "2SexyContent-App");

        }

        public void RemoveApp(int appId, int userId)
        {
            if(appId != this.ContentContext.AppId)
                throw new Exception("An app can only be removed inside of it's own context.");

            if(appId == GetDefaultAppId(ZoneId.Value))
                throw new Exception("The default app of a zone cannot be removed.");

            var app = ContentContext.GetApps().Single(a => a.AppID == appId);

            // Delete templates
            var attributeSets = ContentContext.AttributeSets.Where(a => a.AppID == appId).ToList();
            var templates = TemplateContext.Templates.Where(t => attributeSets.Any(a => a.AttributeSetID == t.AttributeSetID)).ToList();
            templates.ForEach(t => TemplateContext.HardDeleteTemplate(t.TemplateID, userId));

            //TemplateContext.HardDeleteTemplate();
            // ToDo: Ask 2dm how I should delete the templates...

            // Delete folder
            // ToDo: Ask 2dm if the template folder should be deleted

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

            TemplateContext.AddContentGroupItem(Item);

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

        public IEnumerable<AttributeSet> GetAvailableAttributeSets()
        {
            return from c in ContentContext.GetAllAttributeSets()
                   where !c.Name.StartsWith("@")
                         && c.Scope == SexyContent.AttributeSetScope
                         && !c.ChangeLogIDDeleted.HasValue
                   orderby c.Name
                   select c;
        }

        public IEnumerable<AttributeSet> GetAvailableAttributeSetsForVisibleTemplates(int PortalId)
        {
            var AvailableTemplates = GetVisibleTemplates(PortalId);
            return GetAvailableAttributeSets().Where(p => AvailableTemplates.Any(t => t.AttributeSetID == p.AttributeSetID)).OrderBy(p => p.Name);
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

        public SearchItemInfoCollection GetSearchItems(ModuleInfo ModInfo)
        {
            var ZoneID = GetZoneID(ModInfo.PortalID);

            // ToDo: Fix search (SexyContent Context)
            throw new NotImplementedException("ToDo: Fix search");
            // Need a new Context because PortalSettings.Current is null
            var Sexy = new SexyContent(ZoneID.Value, 0, true);

            var SearchItems = new SearchItemInfoCollection();

            var Elements = Sexy.GetContentElements(ModInfo.ModuleID, Sexy.GetCurrentLanguageName(), null, ModInfo.PortalID);
            Elements.Add(Sexy.GetListElement(ModInfo.ModuleID, Sexy.GetCurrentLanguageName(), null, ModInfo.PortalID));

            foreach (var Element in Elements)
            {
                if (Element != null && Element.EntityId.HasValue)
                {
                    var Attributes = ((DynamicEntity)Element.Content).Entity.Attributes;
                    string Content = String.Join(", ", Attributes.Select(x => x.Value[new string[] { Sexy.GetCurrentLanguageName() }]).Where(a => a != null).Select(a => StripHtmlAndHtmlDecode(a.ToString())).Where(x => !String.IsNullOrEmpty(x)));

                    var ContentGroupItem = Sexy.TemplateContext.GetContentGroupItem(Element.ID);
                    var PubDate = Sexy.ContentContext.GetValues(Element.EntityId.Value).Max(p => (DateTime?)p.ChangeLogCreated.Timestamp);

                    if (PubDate.HasValue)
                        SearchItems.Add(new SearchItemInfo(Element.Content.EntityTitle.ToString(), Content, ContentGroupItem == null ? -1 : ContentGroupItem.SysCreatedBy, PubDate.Value, ModInfo.ModuleID, Element.EntityId.ToString(), Content));
                }
            }

            return SearchItems;
        }

        #endregion

        #region Zone / VDB Handling

        /// <summary>
        /// Returns the ZoneID from PortalSettings
        /// </summary>
        /// <param name="PortalID"></param>
        /// <returns></returns>
        public static int? GetZoneID(int PortalID)
        {
            var ZoneSettingKey = SexyContent.PortalSettingsPrefix + "ZoneID";
            var c = PortalController.GetPortalSettingsDictionary(PortalID);

            if (c.ContainsKey(ZoneSettingKey))
                return int.Parse(c[ZoneSettingKey]);
            return null;
        }

        /// <summary>
        /// Sets the ZoneID from PortalSettings
        /// </summary>
        /// <param name="ZoneID"></param>
        /// <param name="PortalID"></param>
        public void SetZoneID(int? ZoneID, int PortalID)
        {
            if (ZoneID.HasValue)
                PortalController.UpdatePortalSetting(PortalID, SexyContent.PortalSettingsPrefix + "ZoneID", ZoneID.Value.ToString());
            else
                PortalController.DeletePortalSetting(PortalID, SexyContent.PortalSettingsPrefix + "ZoneID");
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

        private string StripHtmlAndHtmlDecode(string Text)
        {
            return HttpUtility.HtmlDecode(Regex.Replace(Text, "<.*?>", string.Empty));
        }

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

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="dimensionID"></param>
        /// <returns></returns>
        public string GetJsonFromElements(List<Element> elements, string language)
        {
            var x = new
            {
                Default = new
                {
                    List = (from c in elements
                            where c.EntityId.HasValue
                            select new
                            {
                                Content = GetDictionaryFromEntity(((DynamicEntity)c.Content).Entity, language)
                            }).ToList()
                }
            };

            return x.ToJson();
        }

        private Dictionary<string, object> GetDictionaryFromEntity(IEntity entity, string language)
        {
            var dictionary = ((from d in entity.Attributes select d).ToDictionary(k => k.Value.Name, v => v.Value[language]));
            dictionary.Add("EntityId", entity.EntityId);
            return dictionary;
        }

        /// <summary>
        /// Resolves File and Page values
        /// For example, File:123 or Page:123
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ResolveHyperlinkValues(string value)
        {
            var resultString = (string)value;
            var regularExpression = Regex.Match(resultString, "(?<type>(file|page)):(?<id>[0-9]+)", RegexOptions.IgnoreCase);

            if (!regularExpression.Success)
                return value;

            var fileManager = FileManager.Instance;
            var tabController = new DotNetNuke.Entities.Tabs.TabController();
            var type = regularExpression.Groups["type"].Value.ToLower();
            var id = int.Parse(regularExpression.Groups["id"].Value);

            switch (type)
            {
                case "file":
                    var fileInfo = fileManager.GetFile(id);
                    if(fileInfo != null)
                        resultString = fileManager.GetUrl(fileInfo);
                    break;
                case "page":
                    var portalId = PortalSettings.Current.PortalId;
                    var tabInfo = tabController.GetTab(id, portalId , false);
                    if (tabInfo != null)
                    {
                        //tabInfo.IsDefaultLanguage && 
                        if (tabInfo.CultureCode != "" && tabInfo.CultureCode != PortalSettings.Current.CultureCode)
                        {
                            var cultureTabInfo = tabController.GetTabByCulture(tabInfo.TabID, portalId,
                                                                    LocaleController.Instance.GetLocale(
                                                                        PortalSettings.Current.CultureCode));

                            if (cultureTabInfo != null)
                                tabInfo = cultureTabInfo;
                        }

                        resultString = tabInfo.FullUrl;
                    }
                    break;
            }

            return resultString;
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
                                File.WriteAllText(Path.Combine(oldSexyFolder.FullName, "__WARNING - old copy of files - READ ME.txt"), "This is a short information\r\n\r\n2SexyContent renamed the main folder from \"[Portal]/2Sexy\" to \"[Portal]/2sxc\" in version 5.5.\r\n\r\nTo make sure that links to images/css/js still work, the old folder was copied and this was left. Please clean up and delete the entire \"[Portal]/2Sexy/\" folder once you're done. \r\n\r\nMany thanks!\r\n2SexyContent\r\n\r\nPS: Remember that you might have ClientDependency activated, so maybe you still have bundled & minified  JS/CSS-Files in your cache pointing to the old location. When done cleaning up, we recommend increasing the version just to be sure you're not seeing an old files that don't exist any more. ");

                                // Move web.config (should be directly in 2sxc)
                                if (File.Exists(Path.Combine(newSexyContentFolder.FullName, "web.config")))
                                    File.Move(Path.Combine(newSexyContentFolder.FullName, "web.config"), Path.Combine(newSexyFolder.FullName, "web.config"));
                                
                            }
                        }
                    }
                    break;
            }
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