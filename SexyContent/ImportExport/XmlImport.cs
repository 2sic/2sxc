using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav;
using ToSic.Eav.Import;


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
		private int? PortalId
		{
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

		public bool IsCompatible(XDocument doc)
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
			var fileManager = DotNetNuke.Services.FileSystem.FileManager.Instance;
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

		//public bool IsCompatible(string xml)
		//{
		//	// Parse XDocument from string
		//	var doc = XDocument.Parse(xml);
		//	return IsCompatible(doc);
		//}

		/// <summary>
		/// Creates an app and then imports the xml
		/// </summary>
		/// <param name="xml"></param>
		/// <returns>AppId of the new imported app</returns>
		public bool ImportApp(int zoneId, XDocument doc, out int? appId)
		{
			// Increase script timeout to prevent timeouts
			HttpContext.Current.Server.ScriptTimeout = 300;

			appId = new int?();

			// Parse XDocument from string
			//var doc = XDocument.Parse(xml);

			if (!IsCompatible(doc))
			{
				ImportLog.Add(new ExportImportMessage("The import file is not compatible with the installed version of 2sxc.", ExportImportMessage.MessageTypes.Error));
				return false;
			}

			// Get root node "SexyContent"
			var xmlSource = doc.Element("SexyContent");
			var xApp = xmlSource.Element("Header").Element("App");

			var appGuid = xApp.Attribute("Guid").Value;

			if (appGuid != "Default")
			{
				// Build Guid (take existing, or create a new)
				if (String.IsNullOrEmpty(appGuid) || appGuid == new Guid().ToString())
				{
					appGuid = Guid.NewGuid().ToString();
				}

				// Adding app to EAV
				var sexy = new SexyContent(zoneId, SexyContent.GetDefaultAppId(zoneId));
				var app = sexy.ContentContext.App.AddApp(appGuid);
				sexy.ContentContext.SqlDb.SaveChanges();

				appId = app.AppID;
			}
			else
			{
				appId = this._appId;
			}

			if (!appId.HasValue || appId <= 0)
			{
				ImportLog.Add(new ExportImportMessage("App was not created. Please try again or make sure the package you are importing is correct.", ExportImportMessage.MessageTypes.Error));
				return false;
			}

			return ImportXml(zoneId, appId.Value, doc);
		}

		/// <summary>
		/// Do the import
		/// </summary>
		public bool ImportXml(int zoneId, int appId, XDocument doc, bool leaveExistingValuesUntouched = true)
		{
			_sexy = new SexyContent(zoneId, appId, false);
			_appId = appId;
			_zoneId = zoneId;

			if (!IsCompatible(doc))
			{
				ImportLog.Add(new ExportImportMessage("The import file is not compatible with the installed version of 2sxc.", ExportImportMessage.MessageTypes.Error));
				return false;
			}

			// Get root node "SexyContent"
			var xmlSource = doc.Element("SexyContent");
			PrepareFileIdCorrectionList(xmlSource);

			#region Prepare dimensions
			_sourceDimensions = xmlSource.Element("Header").Element("Dimensions").Elements("Dimension").Select(p => new Dimension
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

			_targetDimensions = _sexy.ContentContext.Dimensions.GetDimensionChildren("Culture");
			if (_targetDimensions.Count == 0)
				_targetDimensions.Add(new Dimension
				{
					Active = true,
					ExternalKey = DefaultLanguage,
					Name = "(added by import System, default language " + DefaultLanguage + ")",
					SystemKey = "Culture"
				});
			#endregion

			var importAttributeSets = GetImportAttributeSets(xmlSource.Element("AttributeSets").Elements("AttributeSet"));
			var importEntities = GetImportEntities(xmlSource.Elements("Entities").Elements("Entity"), SexyContent.AssignmentObjectTypeIDDefault);

			var import = new Eav.Import.Import(_zoneId, _appId, UserName, leaveExistingValuesUntouched);
			import.RunImport(importAttributeSets, importEntities);
			ImportLog.AddRange(GetExportImportMessagesFromImportLog(import.ImportLog));

			if (xmlSource.Elements("Templates").Any())
				ImportXmlTemplates(xmlSource);

			return true;
		}

		/// <summary>
		/// Maps EAV import messages to 2sxc import messages
		/// </summary>
		/// <param name="importLog"></param>
		/// <returns></returns>
		public IEnumerable<ExportImportMessage> GetExportImportMessagesFromImportLog(List<ImportLogItem> importLog)
		{
			return importLog.Select(l => new ExportImportMessage(l.Message,
				l.EntryType == EventLogEntryType.Error ? ExportImportMessage.MessageTypes.Error :
				l.EntryType == EventLogEntryType.Information ? ExportImportMessage.MessageTypes.Information :
				ExportImportMessage.MessageTypes.Warning
				));
		}

		#region AttributeSets

		private List<ImportAttributeSet> GetImportAttributeSets(IEnumerable<XElement> xAttributeSets)
		{
            var importAttributeSets = new List<ImportAttributeSet>();

			// Loop through AttributeSets
			foreach (var attributeSet in xAttributeSets)
			{
				var attributes = new List<ImportAttribute>();
                var titleAttribute = new ImportAttribute();

				foreach (var xElementAttribute in attributeSet.Element("Attributes").Elements("Attribute"))
				{
                    var attribute = new ImportAttribute
					{
						StaticName = xElementAttribute.Attribute("StaticName").Value,
						Type = xElementAttribute.Attribute("Type").Value,
						AttributeMetaData = GetImportEntities(xElementAttribute.Elements("Entity"), Constants.AssignmentObjectTypeIdFieldProperties)
					};

					attributes.Add(attribute);

					// Set Title Attribute
					if (Boolean.Parse(xElementAttribute.Attribute("IsTitle").Value))
						titleAttribute = attribute;
				}

				// Add AttributeSet
                importAttributeSets.Add(new ImportAttributeSet
				{
					StaticName = attributeSet.Attribute("StaticName").Value,
					Name = attributeSet.Attribute("Name").Value,
					Description = attributeSet.Attribute("Description").Value,
					Attributes = attributes,
					Scope = attributeSet.Attributes("Scope").Any() ? attributeSet.Attribute("Scope").Value : SexyContent.AttributeSetScope,
					AlwaysShareConfiguration = AllowSystemChanges && attributeSet.Attributes("AlwaysShareConfiguration").Any() && Boolean.Parse(attributeSet.Attribute("AlwaysShareConfiguration").Value),
					TitleAttribute = titleAttribute
				});
			}

			return importAttributeSets;
		}

		#endregion

		#region Templates

		private void ImportXmlTemplates(XElement root)
		{
			var templates = root.Element("Templates");

			var cache = DataSource.GetCache(_zoneId, _appId);

			foreach (var template in templates.Elements("Template"))
			{
				var name = template.Attribute("Name").Value;
				var path = template.Attribute("Path").Value;

				var contentTypeStaticName = template.Attribute("AttributeSetStaticName").Value;

				if (!String.IsNullOrEmpty(contentTypeStaticName) && cache.GetContentType(contentTypeStaticName) == null)
				{
					ImportLog.Add(
							new ExportImportMessage(
								"Content Type for Template '" + name +
								"' could not be found. The template has not been imported.",
								ExportImportMessage.MessageTypes.Warning));
					continue;
				}

				var demoEntityGuid = template.Attribute("DemoEntityGUID").Value;
				var demoEntityId = new int?();

				if (!String.IsNullOrEmpty(demoEntityGuid))
				{
					var entityGuid = Guid.Parse(demoEntityGuid);
					if (_sexy.ContentContext.Entities.EntityExists(entityGuid))
						demoEntityId = _sexy.ContentContext.Entities.GetEntity(entityGuid).EntityID;
					else
						ImportLog.Add(
							new ExportImportMessage(
								"Demo Entity for Template '" + name + "' could not be found. (Guid: " + demoEntityGuid +
								")", ExportImportMessage.MessageTypes.Information));

				}

				var type = template.Attribute("Type").Value;
				var isHidden = Boolean.Parse(template.Attribute("IsHidden").Value);
				var location = template.Attribute("Location").Value;
				var publishData = Boolean.Parse(template.Attribute("PublishData") == null ? "False" : template.Attribute("PublishData").Value);
				var streamsToPublish = template.Attribute("StreamsToPublish") == null ? "" : template.Attribute("StreamsToPublish").Value;
				var viewNameInUrl = template.Attribute("ViewNameInUrl") == null ? null : template.Attribute("ViewNameInUrl").Value;

				var pipelineEntityGuid = template.Attribute("PipelineEntityGUID");
				var pipelineEntityId = new int?();

				if (pipelineEntityGuid != null && !string.IsNullOrEmpty(pipelineEntityGuid.Value))
				{
					var entityGuid = Guid.Parse(pipelineEntityGuid.Value);
					if (_sexy.ContentContext.Entities.EntityExists(entityGuid))
						pipelineEntityId = _sexy.ContentContext.Entities.GetEntity(entityGuid).EntityID;
					else
						ImportLog.Add(
							new ExportImportMessage(
								"Pipeline Entity for Template '" + name + "' could not be found. (Guid: " + pipelineEntityGuid.Value +
								")", ExportImportMessage.MessageTypes.Information));
				}

				var useForList = false;
				if (template.Attribute("UseForList") != null)
					useForList = Boolean.Parse(template.Attribute("UseForList").Value);

				var templateDefaults = template.Elements("Entity").Select(e =>
				{
					var xmlItemType = e.Elements("Value").FirstOrDefault(v => v.Attribute("Key").Value == "ItemType").Attribute("Value").Value;
					var xmlContentTypeStaticName = e.Elements("Value").FirstOrDefault(v => v.Attribute("Key").Value == "ContentTypeID").Attribute("Value").Value;
					var xmlDemoEntityGuidString = e.Elements("Value").FirstOrDefault(v => v.Attribute("Key").Value == "DemoEntityID").Attribute("Value").Value;
					var xmlDemoEntityId = new int?();
					if (xmlDemoEntityGuidString != "0" && xmlDemoEntityGuidString != "")
					{
						var xmlDemoEntityGuid = Guid.Parse(xmlDemoEntityGuidString);
						if (_sexy.ContentContext.Entities.EntityExists(xmlDemoEntityGuid))
							xmlDemoEntityId = _sexy.ContentContext.Entities.GetEntity(xmlDemoEntityGuid).EntityID;
					}

					return new
					{
						ItemType = xmlItemType,
						ContentTypeStaticName = xmlContentTypeStaticName == "0" || xmlContentTypeStaticName == "" ? "" : xmlContentTypeStaticName,
						DemoEntityId = xmlDemoEntityId
					};
				}).ToList();

				var presentationTypeStaticName = "";
				var presentationDemoEntityId = new int?();
				var presentationDefault = templateDefaults.FirstOrDefault(t => t.ItemType == "Presentation");
				if (presentationDefault != null)
				{
					presentationTypeStaticName = presentationDefault.ContentTypeStaticName;
					presentationDemoEntityId = presentationDefault.DemoEntityId;
				}

				var listContentTypeStaticName = "";
				var listContentDemoEntityId = new int?();
				var listContentDefault = templateDefaults.FirstOrDefault(t => t.ItemType == "ListContent");
				if (listContentDefault != null)
				{
					listContentTypeStaticName = listContentDefault.ContentTypeStaticName;
					listContentDemoEntityId = listContentDefault.DemoEntityId;
				}

				var listPresentationTypeStaticName = "";
				var listPresentationDemoEntityId = new int?();
				var listPresentationDefault = templateDefaults.FirstOrDefault(t => t.ItemType == "ListPresentation");
				if (listPresentationDefault != null)
				{
					listPresentationTypeStaticName = listPresentationDefault.ContentTypeStaticName;
					listPresentationDemoEntityId = listPresentationDefault.DemoEntityId;
				}

				_sexy.Templates.UpdateTemplate(null, name, path, contentTypeStaticName, demoEntityId, presentationTypeStaticName, presentationDemoEntityId, listContentTypeStaticName, listContentDemoEntityId, listPresentationTypeStaticName, listPresentationDemoEntityId, type, isHidden, location, useForList, publishData, streamsToPublish, pipelineEntityId, viewNameInUrl);

				ImportLog.Add(new ExportImportMessage("Template '" + name + "' successfully imported.",
													 ExportImportMessage.MessageTypes.Information));
			}
		}

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
		private List<ImportEntity> GetImportEntities(IEnumerable<XElement> entities, int assignmentObjectTypeId, int? keyNumber = null)
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
        private ImportEntity GetImportEntity(XElement xEntity, int assignmentObjectTypeId, int? keyNumber = null)
		{
			switch (xEntity.Attribute("AssignmentObjectType").Value)
			{
				// Special case: App AttributeSets must be assigned to the current app
				case "App":
					keyNumber = _appId;
					assignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDSexyContentApp;
					break;
                case "Entity":
                case "Data Pipeline": // this one is an old key, remove some time in the future; was probably almost never used...
					assignmentObjectTypeId = Constants.AssignmentObjectTypeEntity;
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

			var importEntity = Eav.ImportExport.XmlImport.GetImportEntity(xEntity, assignmentObjectTypeId,
				_targetDimensions, _sourceDimensions, _sourceDefaultDimensionId, DefaultLanguage, keyNumber, keyGuid);

			return importEntity;
		}

		#endregion
	}
}