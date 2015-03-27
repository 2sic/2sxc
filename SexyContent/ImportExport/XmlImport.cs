using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav;
using ToSic.Eav.Import;
using AttributeSet = ToSic.Eav.Import.AttributeSet;
using Entity = ToSic.Eav.Import.Entity;

namespace ToSic.SexyContent.ImportExport
{
    public class XmlImport
    {
        public List<ExportImportMessage> ImportLog;
        
        private List<Dimension> _sourceDimensions;
        private string _sourceDefaultLanguage;
        private int? _sourceDefaultDimensionId;
        private List<Dimension> _targetDimensions;
        private SexyContent _sexy;
        private int _appId;
        private int _zoneId;
        private Dictionary<int, int> _fileIdCorrectionList;
		
		/// <summary>
		/// The default language / culture - example: de-DE
		/// </summary>
		private string DefaultLanguage { get; set; }

		/// <summary>
		/// The username used for logging (history etc.)
		/// </summary>
		private string UserName { get; set; }

		/// <summary>
		/// The id of the current portal
		/// </summary>
		private int? PortalId {
			get
			{
				if (PortalSettings.Current != null)
					return PortalSettings.Current.PortalId;
				return new int?();
			}
		}

		private bool AllowSystemChanges { get; set; }

        #region Prerequisites

	    /// <summary>
	    /// Create a new xmlImport instance
	    /// </summary>
	    /// <param name="defaultLanguage">The portals default language / culture - example: de-DE</param>
	    /// <param name="userName"></param>
	    /// <param name="allowSystemChanges">Specify if the import should be able to change system-wide things like shared attributesets</param>
	    public XmlImport(string defaultLanguage, string userName, bool allowSystemChanges = false)
        {
            // Prepare
            ImportLog = new List<ExportImportMessage>();
	        DefaultLanguage = defaultLanguage;
			AllowSystemChanges = allowSystemChanges;
        }

        private bool IsCompatible(XDocument doc)
        {
            // Return if no Root Node "SexyContent"
            if (!doc.Elements("SexyContent").Any())
            {
                ImportLog.Add(new ExportImportMessage("The XML file you specified does not seem to be a 2sxc Export.", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            // Return if Version does not match
            if (!doc.Element("SexyContent").Attributes().Any(a => a.Name == "MinimumRequiredVersion") || new Version(doc.Element("SexyContent").Attribute("MinimumRequiredVersion").Value) > new Version(SexyContent.ModuleVersion))
            {
                ImportLog.Add(new ExportImportMessage("This template or app requires 2sxc " + doc.Element("SexyContent").Attribute("MinimumRequiredVersion").Value + " in order to work, you have version " + SexyContent.ModuleVersion + " installed.", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            return true;
        }

        private void PrepareFileIdCorrectionList(XElement sexyContentNode)
        {
            _fileIdCorrectionList = new Dictionary<int, int>();

            if (!sexyContentNode.Elements("PortalFiles").Any())
                return;

            var portalId = PortalId.Value;
            var fileManager = FileManager.Instance;
            var folderManager = FolderManager.Instance;

            var portalFiles = sexyContentNode.Element("PortalFiles").Elements("File");
            foreach (var portalFile in portalFiles)
            {
                var fileId = int.Parse(portalFile.Attribute("Id").Value);
                var relativePath = portalFile.Attribute("RelativePath").Value;
                var fileName = Path.GetFileName(relativePath);
                var directory = Path.GetDirectoryName(relativePath).Replace('\\', '/');

                if (!folderManager.FolderExists(portalId, directory))
                    continue;

                var folderInfo = folderManager.GetFolder(portalId, directory);

                if (!fileManager.FileExists(folderInfo, fileName))
                    continue;

                var fileInfo = fileManager.GetFile(folderInfo, fileName);
                _fileIdCorrectionList.Add(fileId, fileInfo.FileId);
            }

        }
        #endregion

        public bool IsCompatible(int zoneId, string xml)
        {
            // Parse XDocument from string
            XDocument doc = XDocument.Parse(xml);
            return IsCompatible(doc);
        }

        /// <summary>
        /// Creates an app and then imports the xml
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>AppId of the new imported app</returns>
        public bool ImportApp(int zoneId, string xml, out int? appId)
        {
            // Increase script timeout to prevent timeouts
            HttpContext.Current.Server.ScriptTimeout = 300;

            appId = new int?();

            // Parse XDocument from string
            XDocument doc = XDocument.Parse(xml);

            if (!IsCompatible(doc))
            {
                ImportLog.Add(new ExportImportMessage("The import file is not compatible with the installed version of 2sxc.", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            // Get root node "SexyContent"
            XElement xmlSource = doc.Element("SexyContent");
            var xApp = xmlSource.Element("Header").Element("App");

            // Build Guid (take existing, or create a new)
            var appGuid = xApp.Attribute("Guid").Value;
            if (String.IsNullOrEmpty(appGuid) || appGuid == new Guid().ToString())
            {
                appGuid = Guid.NewGuid().ToString();
            }

            // Adding app to EAV
            var sexy = new SexyContent(zoneId, SexyContent.GetDefaultAppId(zoneId));
            var app = sexy.ContentContext.AddApp(appGuid);
            sexy.ContentContext.SaveChanges();

            appId = app.AppID;

            if (!appId.HasValue || appId <= 0)
            {
                ImportLog.Add(new ExportImportMessage("App was not created. Please try again or make sure the package you are importing is correct.", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            return ImportXml(zoneId, app.AppID, xml);
        }

	    /// <summary>
	    /// Do the import
	    /// </summary>
	    /// <param name="xml">The previously exported XML</param>
	    /// <param name="userName">The username of the current user (will be logged in history)</param>
	    /// <returns></returns>
	    public bool ImportXml(int zoneId, int appId, string xml)
        {
            _sexy = new SexyContent(zoneId, appId, false);
            _appId = appId;
            _zoneId = zoneId;

            // Parse XDocument from string
            XDocument doc = XDocument.Parse(xml);

            if (!IsCompatible(doc))
            {
                ImportLog.Add(new ExportImportMessage("The import file is not compatible with the installed version of 2sxc.", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            // Get root node "SexyContent"
            XElement xmlSource = doc.Element("SexyContent");
            PrepareFileIdCorrectionList(xmlSource);

            #region Prepare dimensions
            _sourceDimensions = xmlSource.Element("Header").Element("Dimensions").Elements("Dimension").Select(p => new Dimension()
                {
                    DimensionID = int.Parse(p.Attribute("DimensionID").Value),
                    Name = p.Attribute("Name").Value,
                    SystemKey = p.Attribute("SystemKey").Value,
                    ExternalKey = p.Attribute("ExternalKey").Value,
                    Active = Boolean.Parse(p.Attribute("Active").Value)
                }).ToList();

            _sourceDefaultLanguage = xmlSource.Element("Header").Element("Language").Attribute("Default").Value;
            _sourceDefaultDimensionId = _sourceDimensions.Any() ? 
                _sourceDimensions.FirstOrDefault(p => p.ExternalKey == _sourceDefaultLanguage).DimensionID
                : new int?();

            _targetDimensions = _sexy.ContentContext.GetDimensionChildren("Culture");
            if(_targetDimensions.Count == 0)
                _targetDimensions.Add(new Dimension()
                    {
                        Active = true,
                        ExternalKey = DefaultLanguage,
                        Name = "(added by import System, default language " + DefaultLanguage + ")",
                        SystemKey = "Culture"
                    });
            #endregion

            var importAttributeSets = GetImportAttributeSets(xmlSource.Element("AttributeSets").Elements("AttributeSet"));
            var importEntities = GetImportEntities(xmlSource.Elements("Entities").Elements("Entity"), SexyContent.AssignmentObjectTypeIDDefault);
            
            var import = new ToSic.Eav.Import.Import(_zoneId, _appId, UserName);
            import.RunImport(importAttributeSets, importEntities, true, true);
            ImportLog.AddRange(GetExportImportMessagesFromImportLog(import.ImportLog));
            
			//if (xmlSource.Elements("Templates").Any())
			//{
			//	if(_sexy.Templates.Connection.State != ConnectionState.Open)
			//		_sexy.Templates.Connection.Open();
			//	var transaction = _sexy.Templates.Connection.BeginTransaction();
			//	List<Entity> templateDescribingEntities;
			//	ImportXmlTemplates(xmlSource, out templateDescribingEntities);

			//	var import2 = new ToSic.Eav.Import.Import(_zoneId, _appId, userName);
			//	import2.RunImport(new List<AttributeSet>(), templateDescribingEntities, true, true);
			//	ImportLog.AddRange(GetExportImportMessagesFromImportLog(import2.ImportLog));

			//	transaction.Commit();
			//}

            return true;
        }

        /// <summary>
        /// Maps EAV import messages to 2sxc import messages
        /// </summary>
        /// <param name="importLog"></param>
        /// <returns></returns>
        public IEnumerable<ExportImportMessage> GetExportImportMessagesFromImportLog(List<LogItem> importLog)
        {
            return importLog.Select(l => new ExportImportMessage(l.Message, 
                l.EntryType == EventLogEntryType.Error ? ExportImportMessage.MessageTypes.Error :
                l.EntryType == EventLogEntryType.Information ? ExportImportMessage.MessageTypes.Information :
                ExportImportMessage.MessageTypes.Warning
                ));
        }

        #region AttributeSets

        private List<ToSic.Eav.Import.AttributeSet> GetImportAttributeSets(IEnumerable<XElement> xAttributeSets)
        {
            var importAttributeSets = new List<ToSic.Eav.Import.AttributeSet>();

            // Loop through AttributeSets
            foreach (var attributeSet in xAttributeSets)
            {
                var attributes = new List<ToSic.Eav.Import.Attribute>();
                var titleAttribute = new ToSic.Eav.Import.Attribute();
                
                foreach (XElement xElementAttribute in attributeSet.Element("Attributes").Elements("Attribute"))
                {
                    var attribute = new ToSic.Eav.Import.Attribute()
                        {
                            StaticName = xElementAttribute.Attribute("StaticName").Value,
                            Type = xElementAttribute.Attribute("Type").Value,
                            AttributeMetaData = GetImportEntities(xElementAttribute.Elements("Entity"), DataSource.AssignmentObjectTypeIdFieldProperties)
                        };

                    attributes.Add(attribute);

                    // Set Title Attribute
                    if (Boolean.Parse(xElementAttribute.Attribute("IsTitle").Value))
                        titleAttribute = attribute;
                }

                // Add AttributeSet
                importAttributeSets.Add(new AttributeSet()
                    {
                        StaticName = attributeSet.Attribute("StaticName").Value,
                        Name = attributeSet.Attribute("Name").Value,
                        Description = attributeSet.Attribute("Description").Value,
                        Attributes = attributes,
                        Scope = attributeSet.Attributes("Scope").Any() ? attributeSet.Attribute("Scope").Value : SexyContent.AttributeSetScope,
						AlwaysShareConfiguration = AllowSystemChanges && attributeSet.Attributes("AlwaysShareConfiguration").Any() && Boolean.Parse(attributeSet.Attribute("Scope").Value),
                        TitleAttribute = titleAttribute
                    });
            }

            return importAttributeSets;
        }

        #endregion

        #region Templates

		// ToDo: Remove this if not needed anymore
		//private void ImportXmlTemplates(XElement Root, out List<Entity> entities)
		//{
		//	var templates = Root.Element("Templates");
		//	entities = new List<Entity>();

		//	foreach (var template in templates.Elements("Template"))
		//	{
		//		Template t = _sexy.Templates.GetNewTemplate(_sexy.AppId.Value);
		//		t.Name = template.Attribute("Name").Value;
		//		t.Path = template.Attribute("Path").Value;

		//		string attributeSetStaticName = template.Attribute("AttributeSetStaticName").Value;


		//		if (!String.IsNullOrEmpty(attributeSetStaticName))
		//		{
		//			ToSic.Eav.AttributeSet Set = _sexy.ContentContext.GetAttributeSet(attributeSetStaticName);

		//			if (Set == null)
		//			{
		//				ImportLog.Add(
		//					new ExportImportMessage(
		//						"Content Type for Template '" + t.Name +
		//						"' could not be found. The template has not been imported.",
		//						ExportImportMessage.MessageTypes.Warning));
		//				continue;
		//			}
		//			else
		//				t.AttributeSetID = _sexy.ContentContext.GetAttributeSet(attributeSetStaticName).AttributeSetID;
		//		}
		//		else
		//		{
		//			t.AttributeSetID = new int?();
		//		}

		//		string DemoEntityGuid = template.Attribute("DemoEntityGUID").Value;
		//		if (!String.IsNullOrEmpty(DemoEntityGuid))
		//		{
		//			var EntityGuid = Guid.Parse(DemoEntityGuid);
		//			if (_sexy.ContentContext.EntityExists(EntityGuid))
		//				t.DemoEntityID = _sexy.ContentContext.GetEntity(EntityGuid).EntityID;
		//			else
		//				ImportLog.Add(
		//					new ExportImportMessage(
		//						"Demo Entity for Template '" + t.Name + "' could not be found. (Guid: " + DemoEntityGuid +
		//						")", ExportImportMessage.MessageTypes.Information));

		//		}

		//		t.Script = template.Attribute("Script").Value;
		//		t.IsFile = Boolean.Parse(template.Attribute("IsFile").Value);
		//		t.Type = template.Attribute("Type").Value;
		//		t.IsHidden = Boolean.Parse(template.Attribute("IsHidden").Value);
		//		t.Location = template.Attribute("Location").Value;
		//		t.PublishData = Boolean.Parse(template.Attribute("PublishData") == null ? "False" : template.Attribute("PublishData").Value);
		//		t.StreamsToPublish = template.Attribute("StreamsToPublish") == null ? "" : template.Attribute("StreamsToPublish").Value;
		//		t.ViewNameInUrl = template.Attribute("ViewNameInUrl") == null ? null : template.Attribute("ViewNameInUrl").Value;

		//		var pipelineEntityGuid = template.Attribute("PipelineEntityGUID");
		//		if (pipelineEntityGuid != null && !string.IsNullOrEmpty(pipelineEntityGuid.Value))
		//		{
		//			var entityGuid = Guid.Parse(pipelineEntityGuid.Value);
		//			if (_sexy.ContentContext.EntityExists(entityGuid))
		//				t.PipelineEntityID = _sexy.ContentContext.GetEntity(entityGuid).EntityID;
		//			else
		//				ImportLog.Add(
		//					new ExportImportMessage(
		//						"Pipeline Entity for Template '" + t.Name + "' could not be found. (Guid: " + pipelineEntityGuid.Value +
		//						")", ExportImportMessage.MessageTypes.Information));
		//		}

		//		if (template.Attribute("UseForList") != null)
		//			t.UseForList = Boolean.Parse(template.Attribute("UseForList").Value);

		//		// Stop if the same template already exists
		//		if (_sexy.GetTemplates(PortalId.Value)
		//				 .Any(p => p.AttributeSetID == t.AttributeSetID
		//						   && p.Path == t.Path
		//						   && p.UseForList == t.UseForList && p.SysDeleted == null))
		//			continue;

		//		t.PortalID = PortalId.Value;
		//		_sexy.Templates.AddTemplate(t);

		//		foreach (XElement xEntity in template.Elements("Entity"))
		//			entities.Add(GetImportEntity(xEntity, SexyContent.AssignmentObjectTypeIDSexyContentTemplate, t.TemplateID));

		//		ImportLog.Add(new ExportImportMessage("Template '" + t.Name + "' successfully imported.",
		//											 ExportImportMessage.MessageTypes.Information));
		//	}

		//	_sexy.Templates.SaveChanges();
		//}

        #endregion

        #region Entities

	    /// <summary>
	    /// Returns a collection of EAV import entities
	    /// </summary>
	    /// <param name="entities"></param>
	    /// <param name="defaultLanguage">example: de-DE</param>
	    /// <param name="assignmentObjectTypeId"></param>
	    /// <param name="keyNumber"></param>
	    /// <returns></returns>
	    private List<ToSic.Eav.Import.Entity> GetImportEntities(IEnumerable<XElement> entities, int assignmentObjectTypeId, int? keyNumber = null)
        {
            return entities.Select(e => GetImportEntity(e, assignmentObjectTypeId, keyNumber)).ToList();
        }


        /// <summary>
		/// Returns an EAV import entity
        /// </summary>
        /// <param name="xEntity">The xml-Element of the entity to import</param>
        /// <param name="assignmentObjectTypeId">assignmentObjectTypeId</param>
        /// <param name="defaultLanguage">The default language / culture - exmple: de-DE</param>
        /// <param name="keyNumber">The entity will be assigned to this keyNumber (optional)</param>
        /// <returns></returns>
		private Entity GetImportEntity(XElement xEntity, int assignmentObjectTypeId, int? keyNumber = null)
        {
			switch (xEntity.Attribute("AssignmentObjectType").Value)
	        {
				// Special case: App AttributeSets must be assigned to the current app
				case "App":
					keyNumber = _appId;
					assignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDSexyContentApp;
					break;
				case "Data Pipeline":
			        assignmentObjectTypeId = DataSource.AssignmentObjectTypeIdDataPipeline;
					break;
	        }

			Guid? keyGuid = null;
			if (xEntity.Attribute("KeyGuid") != null)
		        keyGuid = Guid.Parse(xEntity.Attribute("KeyGuid").Value);

            // Special case #2: Corrent values of Template-Describing entities, and resolve files
            
            foreach (var sourceValue in xEntity.Elements("Value"))
            {
                var sourceValueString = sourceValue.Attribute("Value").Value;
                var sourceKey = sourceValue.Attribute("Key").Value;

                
                if (!String.IsNullOrEmpty(sourceValueString))
                {
                    // Correct ContentTypeID and DemoEntityID
                    if (xEntity.Attribute("AttributeSetStaticName").Value == SexyContent.AttributeSetStaticNameTemplateContentTypes)
                    {
                        switch (sourceKey)
                        {
                            case "ContentTypeID":
                                var attributeSet = _sexy.ContentContext.AttributeSetExists(sourceValueString, _sexy.ContentContext.AppId) ? _sexy.ContentContext.GetAttributeSet(sourceValueString) : null;
                                sourceValue.Attribute("Value").SetValue(attributeSet != null ? attributeSet.AttributeSetID.ToString() : "0");
                                break;
                            case "DemoEntityID":
                                var entityGuid = new Guid(sourceValue.Attribute("Value").Value);
                                var demoEntity = _sexy.ContentContext.EntityExists(entityGuid) ? _sexy.ContentContext.GetEntity(entityGuid) : null;
                                sourceValue.Attribute("Value").SetValue(demoEntity != null ? demoEntity.EntityID.ToString() : "0");
                                break;
                        }
                    }

                    // Correct FileId in Hyperlink fields (takes XML data that lists files)
                    if (sourceValue.Attribute("Type").Value == "Hyperlink")
                    {
                        var fileRegex = new Regex("^File:(?<FileId>[0-9]+)", RegexOptions.IgnoreCase);
                        var a = fileRegex.Match(sourceValueString);
                        if (a.Success && a.Groups["FileId"].Length > 0)
                        {
                            var originalId = int.Parse(a.Groups["FileId"].Value);

                            if (_fileIdCorrectionList.ContainsKey(originalId))
                            {
                                var newValue = fileRegex.Replace(sourceValueString, "File:" + _fileIdCorrectionList[originalId]);
                                sourceValue.Attribute("Value").SetValue(newValue);
                            }

                        }
                    }
                }

            }

            var importEntity = ToSic.Eav.ImportExport.XmlImport.GetImportEntity(xEntity, assignmentObjectTypeId,
				_targetDimensions, _sourceDimensions, _sourceDefaultDimensionId, DefaultLanguage, keyNumber);

            return importEntity;
        }

        #endregion
    }
}