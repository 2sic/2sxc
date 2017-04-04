using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using ToSic.Eav.BLL;
using ToSic.Eav.Import;
using static System.Boolean;
using static System.String;
using Microsoft.Practices.Unity;// needed for the static Resolve<...>
using ToSic.Eav.AppEngine;

namespace ToSic.Eav.ImportExport
{
    // todo: move all strings to XmlConstants

    public class XmlImportWithFiles
	{
		public List<ExportImportMessage> ImportLog;

		private List<Dimension> _sourceDimensions;
		private string _sourceDefaultLanguage;
		private int? _sourceDefaultDimensionId;
		private List<Dimension> _targetDimensions;
        private EavDataController _eavContext;
		private int _appId;
		private int _zoneId;
		private readonly Dictionary<int, int> _fileIdCorrectionList = new Dictionary<int, int>();
	    private readonly Dictionary<int, int> _folderIdCorrectionList = new Dictionary<int, int>();

        private readonly IImportExportEnvironment _environment;

        /// <summary>
        /// The default language / culture - example: de-DE
        /// </summary>
        private string DefaultLanguage { get; }

		/// <summary>
		/// The username used for logging (history etc.)
		/// </summary>
		//private string UserName { get; }

		/// <summary>
		/// The id of the current portal
		/// </summary>
		// private int? PortalId => PortalSettings.Current != null ? PortalSettings.Current.PortalId : new int?();

        private bool AllowSystemChanges { get; }

		#region Prerequisites

		/// <summary>
		/// Create a new xmlImport instance
		/// </summary>
		/// <param name="defaultLanguage">The portals default language / culture - example: de-DE</param>
		/// <param name="userName"></param>
		/// <param name="allowSystemChanges">Specify if the import should be able to change system-wide things like shared attributesets</param>
		public XmlImportWithFiles(string defaultLanguage, /*string userName,*/ bool allowSystemChanges = false)
		{
		    _environment = Factory.Container.Resolve<IImportExportEnvironment>();// new ImportExportEnvironment(); // todo: depedency injection
			// Prepare
			ImportLog = new List<ExportImportMessage>();
			DefaultLanguage = defaultLanguage;
			AllowSystemChanges = allowSystemChanges;
		    //UserName = userName;

        }

		public bool IsCompatible(XDocument doc)
		{
		    var rns = doc.Elements(XmlConstants.RootNode);
		    var rn = doc.Element(XmlConstants.RootNode);
			// Return if no Root Node "SexyContent"
			if (!rns.Any() || rn == null)
			{
				ImportLog.Add(new ExportImportMessage("The XML file you specified does not seem to be a 2sxc Export.", ExportImportMessage.MessageTypes.Error));
				return false;
			}
			// Return if Version does not match
			if (rn.Attributes().All(a => a.Name != "MinimumRequiredVersion") || new Version(rn.Attribute("MinimumRequiredVersion").Value) > new Version(_environment.ModuleVersion))
			{
				ImportLog.Add(new ExportImportMessage("This template or app requires 2sxc " + rn.Attribute("MinimumRequiredVersion").Value + " in order to work, you have version " + _environment.ModuleVersion + " installed.", ExportImportMessage.MessageTypes.Error));
				return false;
			}

			return true;
		}

		private void PrepareFileIdCorrectionList(XElement sexyContentNode)
		{
			if (!sexyContentNode.Elements(XmlConstants.PortalFiles).Any())
				return;

			var portalFiles = sexyContentNode.Element(XmlConstants.PortalFiles)?.Elements("File");
		    if (portalFiles == null) return;
		    var filesAndPaths = portalFiles.ToDictionary(
		        p => int.Parse(p.Attribute("Id").Value),
		        v => v.Attribute("RelativePath").Value
		    );
            _environment.MapExistingFilesToImportSet(filesAndPaths, _fileIdCorrectionList);

		}


	    private void PrepareFolderIdCorrectionListAndCreateMissingFolders(XElement sexyContentNode)
        {
            if (!sexyContentNode.Elements(XmlConstants.FolderGroup).Any()) 
                return;

            var portalFiles = sexyContentNode.Element(XmlConstants.FolderGroup)?.Elements(XmlConstants.Folder);
            if (portalFiles == null) return;

            var foldersAndPath = portalFiles.ToDictionary(
                p => int.Parse(p.Attribute(XmlConstants.FolderNodeId).Value),
                v => v.Attribute(XmlConstants.FolderNodePath).Value
            );
            _environment.CreateFoldersAndMapToImportIds(foldersAndPath, _folderIdCorrectionList, ImportLog);
        }
	    #endregion

        /// <summary>
        /// Creates an app and then imports the xml
        /// </summary>
        /// <returns>AppId of the new imported app</returns>
        public bool ImportApp(int zoneId, XDocument doc, out int? appId)
		{
			// Increase script timeout to prevent timeouts
			HttpContext.Current.Server.ScriptTimeout = 300;

			appId = new int?();

			if (!IsCompatible(doc))
			{
				ImportLog.Add(new ExportImportMessage("The import file is not compatible with the installed version of 2sxc.", ExportImportMessage.MessageTypes.Error));
				return false;
			}

			// Get root node "SexyContent"
			var xmlSource = doc.Element(XmlConstants.RootNode);
			var xApp = xmlSource?.Element(XmlConstants.Header)?.Element(XmlConstants.App);

			var appGuid = xApp?.Attribute(XmlConstants.Guid).Value;

            if (appGuid == null)
            {
                ImportLog.Add(new ExportImportMessage("Something is wrong in the xml structure, can't get an app-guid", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            if (appGuid != XmlConstants.AppContentGuid)
			{
				// Build Guid (take existing, or create a new)
				if (IsNullOrEmpty(appGuid) || appGuid == new Guid().ToString())
					appGuid = Guid.NewGuid().ToString();

				// Adding app to EAV
                var eavDc = EavDataController.Instance(zoneId, null);
			    var app = eavDc.App.AddApp(null, appGuid);
				eavDc.SqlDb.SaveChanges();

				appId = app.AppID;
			}
			else
			{
				appId = _appId;
			}

			if (appId <= 0)
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
			//_sexy = new SxcInstance(zoneId, appId); // 2016-03-26 2dm this used to have a third parameter false = don't enable caching, which hasn't been respected for a while; removed it
            // App = new App(zoneId, appId, PortalSettings.Current); // 2016-04-07 2dm refactored this out of this, as using App had side-effects
		    _eavContext = EavDataController.Instance(zoneId, appId);
            
			_appId = appId;
			_zoneId = zoneId;

			if (!IsCompatible(doc))
			{
				ImportLog.Add(new ExportImportMessage("The import file is not compatible with the installed version of 2sxc.", ExportImportMessage.MessageTypes.Error));
				return false;
			}

			// Get root node "SexyContent"
			var xmlSource = doc.Element(XmlConstants.RootNode);
            if (xmlSource == null)
            {
                ImportLog.Add(new ExportImportMessage("Xml doesn't have expected root node: " + XmlConstants.RootNode, ExportImportMessage.MessageTypes.Error));
                return false;
            }
            PrepareFolderIdCorrectionListAndCreateMissingFolders(xmlSource);
			PrepareFileIdCorrectionList(xmlSource);

			#region Prepare dimensions
			_sourceDimensions = xmlSource.Element(XmlConstants.Header)?.Element(XmlConstants.Dimensions)?.Elements(XmlConstants.Dim).Select(p => new Dimension
			{
				DimensionID = int.Parse(p.Attribute(XmlConstants.DimId).Value),
				Name = p.Attribute("Name").Value,
				SystemKey = p.Attribute("SystemKey").Value,
				ExternalKey = p.Attribute("ExternalKey").Value,
				Active = Parse(p.Attribute("Active").Value)
			}).ToList();

			_sourceDefaultLanguage = xmlSource.Element(XmlConstants.Header)?.Element(XmlConstants.Language)?.Attribute(XmlConstants.LangDefault).Value;
		    if (_sourceDimensions == null || _sourceDefaultLanguage == null)
		    {
                ImportLog.Add(new ExportImportMessage("Cant find source dimensions or source-default language.", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            _sourceDefaultDimensionId = _sourceDimensions.Any() ?
				_sourceDimensions.FirstOrDefault(p => p.ExternalKey == _sourceDefaultLanguage)?.DimensionID
				: new int?();

			_targetDimensions = _eavContext.Dimensions.GetDimensionChildren("Culture");
			if (_targetDimensions.Count == 0)
				_targetDimensions.Add(new Dimension
				{
					Active = true,
					ExternalKey = DefaultLanguage,
					Name = "(added by import System, default language " + DefaultLanguage + ")",
					SystemKey = "Culture"
				});
			#endregion

		    var atsNodes = xmlSource.Element(XmlConstants.AttributeSets)?.Elements(XmlConstants.AttributeSet);
		    var entNodes = xmlSource.Elements(XmlConstants.Entities).Elements(XmlConstants.Entity);

            var importAttributeSets = GetImportAttributeSets(atsNodes);
			var importEntities = GetImportEntities(entNodes, Configuration.AssignmentObjectTypeIdDefault);

			var import = new Import.Import(_zoneId, _appId, /*UserName,*/ leaveExistingValuesUntouched);
			import.RunImport(importAttributeSets, importEntities);
			ImportLog.AddRange(GetExportImportMessagesFromImportLog(import.ImportLog));

			if (xmlSource.Elements(XmlConstants.Templates).Any())
				ImportXmlTemplates(xmlSource);

			return true;
		}

        /// <summary>
        /// Maps EAV import messages to 2sxc import messages
        /// </summary>
        /// <param name="importLog"></param>
        /// <returns></returns>
        private IEnumerable<ExportImportMessage> GetExportImportMessagesFromImportLog(List<ImportLogItem> importLog)
            => importLog.Select(l => new ExportImportMessage(l.Message,
                l.EntryType == EventLogEntryType.Error
                    ? ExportImportMessage.MessageTypes.Error
                    : l.EntryType == EventLogEntryType.Information
                        ? ExportImportMessage.MessageTypes.Information
                        : ExportImportMessage.MessageTypes.Warning
                ));
		

		#region AttributeSets

		private List<ImportAttributeSet> GetImportAttributeSets(IEnumerable<XElement> xAttributeSets)
		{
            var importAttributeSets = new List<ImportAttributeSet>();

			// Loop through AttributeSets
			foreach (var attributeSet in xAttributeSets)
			{
				var attributes = new List<ImportAttribute>();
                var titleAttribute = new ImportAttribute();
			    var attsetElem = attributeSet.Element(XmlConstants.Attributes);
                if (attsetElem != null)
                    foreach (var xElementAttribute in attsetElem.Elements(XmlConstants.Attribute))
                    {
                        var attribute = new ImportAttribute
                        {
                            StaticName = xElementAttribute.Attribute("StaticName").Value,
                            Type = xElementAttribute.Attribute("Type").Value,
                            AttributeMetaData = GetImportEntities(xElementAttribute.Elements(XmlConstants.Entity), Constants.MetadataForField)//.AssignmentObjectTypeIdFieldProperties)
                        };

                        attributes.Add(attribute);

                        // Set Title Attribute
                        if (Parse(xElementAttribute.Attribute("IsTitle").Value))
                            titleAttribute = attribute;
                    }

				// Add AttributeSet
                importAttributeSets.Add(new ImportAttributeSet
				{
					StaticName = attributeSet.Attribute(Const2.Static).Value,
					Name = attributeSet.Attribute(Const2.Name).Value,
					Description = attributeSet.Attribute(Const2.Description).Value,
					Attributes = attributes,
					Scope = attributeSet.Attributes(Const2.Scope).Any() ? attributeSet.Attribute(Const2.Scope).Value : _environment.FallbackContentTypeScope,
					AlwaysShareConfiguration = AllowSystemChanges && attributeSet.Attributes(Const2.AlwaysShareConfig).Any() && Parse(attributeSet.Attribute(Const2.AlwaysShareConfig).Value),
                    UsesConfigurationOfAttributeSet = attributeSet.Attributes("UsesConfigurationOfAttributeSet").Any() ? attributeSet.Attribute("UsesConfigurationOfAttributeSet").Value : "",
                    TitleAttribute = titleAttribute,
                    SortAttributes = attributeSet.Attributes(Const2.SortAttributes).Any() && bool.Parse(attributeSet.Attribute(Const2.SortAttributes).Value)
				});
			}

			return importAttributeSets;
		}

		#endregion

		#region Templates

        private void ImportXmlTemplates(XElement root)
        {
            var templates = root.Element(XmlConstants.Templates);
            if (templates == null) return;

            var cache = DataSource.GetCache(_zoneId, _appId);

            foreach (var template in templates.Elements(XmlConstants.Template))
            {
                var name = "";
                try
                {
                    name = template.Attribute("Name").Value;
                    var path = template.Attribute("Path").Value;

                    var contentTypeStaticName = template.Attribute(XmlConstants.AttSetStatic).Value;

                    if (!IsNullOrEmpty(contentTypeStaticName) && cache.GetContentType(contentTypeStaticName) == null)
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

                    if (!IsNullOrEmpty(demoEntityGuid))
                    {
                        var entityGuid = Guid.Parse(demoEntityGuid);
                        if ( /*App.*/_eavContext.Entities.EntityExists(entityGuid))
                            demoEntityId = /*App.*/ _eavContext.Entities.GetEntity(entityGuid).EntityID;
                        else
                            ImportLog.Add(
                                new ExportImportMessage(
                                    "Demo Entity for Template '" + name + "' could not be found. (Guid: " +
                                    demoEntityGuid +
                                    ")", ExportImportMessage.MessageTypes.Information));

                    }

                    var type = template.Attribute("Type").Value;
                    var isHidden = Parse(template.Attribute("IsHidden").Value);
                    var location = template.Attribute("Location").Value;
                    var publishData =
                        Parse(template.Attribute("PublishData") == null
                            ? "False"
                            : template.Attribute("PublishData").Value);
                    var streamsToPublish = template.Attribute("StreamsToPublish") == null
                        ? ""
                        : template.Attribute("StreamsToPublish").Value;
                    var viewNameInUrl = template.Attribute("ViewNameInUrl") == null
                        ? null
                        : template.Attribute("ViewNameInUrl").Value;

                    var pipelineEntityGuid = template.Attribute("PipelineEntityGUID");
                    var pipelineEntityId = new int?();

                    if (!IsNullOrEmpty(pipelineEntityGuid?.Value))
                    {
                        var entityGuid = Guid.Parse(pipelineEntityGuid.Value);
                        if (_eavContext.Entities.EntityExists(entityGuid))
                            pipelineEntityId = _eavContext.Entities.GetEntity(entityGuid).EntityID;
                        else
                            ImportLog.Add(
                                new ExportImportMessage(
                                    "Pipeline Entity for Template '" + name + "' could not be found. (Guid: " +
                                    pipelineEntityGuid.Value +
                                    ")", ExportImportMessage.MessageTypes.Information));
                    }

                    var useForList = false;
                    if (template.Attribute("UseForList") != null)
                        useForList = Parse(template.Attribute("UseForList").Value);

                    var lstTemplateDefaults = template.Elements(XmlConstants.Entity).Select(e =>
                    {
                        var xmlItemType =
                            e.Elements("Value")
                                .FirstOrDefault(v => v.Attribute("Key").Value == "ItemType")?
                                .Attribute("Value")
                                .Value;
                        var xmlContentTypeStaticName =
                            e.Elements("Value")
                                .FirstOrDefault(v => v.Attribute("Key").Value == "ContentTypeID")?
                                .Attribute("Value")
                                .Value;
                        var xmlDemoEntityGuidString =
                            e.Elements("Value")
                                .FirstOrDefault(v => v.Attribute("Key").Value == "DemoEntityID")?
                                .Attribute("Value")
                                .Value;
                        if (xmlItemType == null || xmlContentTypeStaticName == null || xmlDemoEntityGuidString == null)
                        {
                            ImportLog.Add(new ExportImportMessage(
                                $"trouble with template '{name}' - either type, static or guid are null", ExportImportMessage.MessageTypes.Error));
                            return null;
                        }
                        var xmlDemoEntityId = new int?();
                        if (xmlDemoEntityGuidString != "0" && xmlDemoEntityGuidString != "")
                        {
                            var xmlDemoEntityGuid = Guid.Parse(xmlDemoEntityGuidString);
                            if (_eavContext.Entities.EntityExists(xmlDemoEntityGuid))
                                xmlDemoEntityId = _eavContext.Entities.GetEntity(xmlDemoEntityGuid).EntityID;
                        }

                        return new TemplateDefault
                        {
                            ItemType = xmlItemType,
                            ContentTypeStaticName =
                                xmlContentTypeStaticName == "0" || xmlContentTypeStaticName == ""
                                    ? ""
                                    : xmlContentTypeStaticName,
                            DemoEntityId = xmlDemoEntityId
                        };
                    }).ToList();

                    // Array lstTemplateDefaults has null objects 

                    //Remove null objects if any
                    var templateDefaults = new List<TemplateDefault>();
                    foreach (var lstItem in lstTemplateDefaults)
                    {
                        if (lstItem != null)
                        {
                            templateDefaults.Add(lstItem);
                        }
                    }

                    var presentationTypeStaticName = "";
                    var presentationDemoEntityId = new int?();
                    //if list templateDefaults would have null objects, we would have an exception
                    var presentationDefault = templateDefaults.FirstOrDefault(t => t.ItemType == Constants.PresentationKey);
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

                    //var tm = new TemplateManager(_eavContext.ZoneId, _eavContext.AppId);
                    //tm.UpdateTemplate(
                    new AppManager(_eavContext.ZoneId, _eavContext.AppId).Templates.CreateOrUpdate(
                        null, name, path, contentTypeStaticName, demoEntityId, presentationTypeStaticName,
                        presentationDemoEntityId, listContentTypeStaticName, listContentDemoEntityId,
                        listPresentationTypeStaticName, listPresentationDemoEntityId, type, isHidden, location,
                        useForList, publishData, streamsToPublish, pipelineEntityId, viewNameInUrl);

                    ImportLog.Add(new ExportImportMessage("Template '" + name + "' successfully imported.",
                        ExportImportMessage.MessageTypes.Information));
                }

                catch (Exception e)
                {
                    ImportLog.Add(new ExportImportMessage("Import for template '" + name + "' failed.",
                        ExportImportMessage.MessageTypes.Information));
                }

            }
        }

        #endregion

		#region Entities

        /// <summary>
        /// Returns a collection of EAV import entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="assignmentObjectTypeId"></param>
        /// <returns></returns>
        private List<ImportEntity> GetImportEntities(IEnumerable<XElement> entities, int assignmentObjectTypeId)
            => entities.Select(e => GetImportEntity(e, assignmentObjectTypeId)).ToList();
		


        /// <summary>
        /// Returns an EAV import entity
        /// </summary>
        /// <param name="entityNode">The xml-Element of the entity to import</param>
        /// <param name="assignmentObjectTypeId">assignmentObjectTypeId</param>
        /// <returns></returns>
        private ImportEntity GetImportEntity(XElement entityNode, int assignmentObjectTypeId)
		{
            #region retrieve optional metadata keys in the import - must happen before we apply corrections like AppId
            Guid? keyGuid = null;
            if (entityNode.Attribute(XmlConstants.KeyGuid) != null)
                keyGuid = Guid.Parse(entityNode.Attribute(XmlConstants.KeyGuid).Value);
            int? keyNumber = null;
            if (entityNode.Attribute(XmlConstants.KeyNumber) != null)
                keyNumber = int.Parse(entityNode.Attribute(XmlConstants.KeyNumber).Value);

            var keyString = entityNode.Attribute(XmlConstants.KeyString)?.Value;
            #endregion

            #region check if the xml has an own assignment object type (then we wouldn't use the default)
            switch (entityNode.Attribute(XmlConstants.KeyTargetType).Value)
			{
				// Special case: App AttributeSets must be assigned to the current app
				case XmlConstants.App:
					keyNumber = _appId;
					assignmentObjectTypeId = State.GetAssignmentTypeId(Constants.AppAssignmentName);// ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp;
					break;
                case XmlConstants.Entity:
                case "Data Pipeline": // this one is an old key, remove some time in the future; was probably almost never used...
					assignmentObjectTypeId = Constants.MetadataForEntity;
					break;
                case XmlConstants.ContentType:
			        assignmentObjectTypeId = Constants.MetadataForContentType;
                    break;
                case XmlConstants.CmsObject:
			        assignmentObjectTypeId = Constants.MetadataForContentType;

                    if(keyString == null)
                        throw new Exception("found cms object, but couldn't find metadata-key of type string, will abort");
			        var newKey = GetMappedLink(keyString);
			        if (newKey != null)
			            keyString = newKey;
			        break;
			}
            #endregion


            // Special case #2: Corrent values of Template-Describing entities, and resolve files

            foreach (var sourceValue in entityNode.Elements("Value"))
			{
				var sourceValueString = sourceValue.Attribute("Value").Value;
				// var sourceKey = sourceValue.Attribute("Key").Value;


				// Correct FileId in Hyperlink fields (takes XML data that lists files)
			    if (!IsNullOrEmpty(sourceValueString) && sourceValue.Attribute("Type").Value == "Hyperlink")
			    {
			        string newValue = GetMappedLink(sourceValueString);
			        if (newValue != null)
			            sourceValue.Attribute("Value").SetValue(newValue);
			    }
			}

			var importEntity = Eav.ImportExport.XmlImport.GetImportEntity(entityNode, assignmentObjectTypeId,
				_targetDimensions, _sourceDimensions, _sourceDefaultDimensionId, DefaultLanguage, keyNumber, keyGuid, keyString);

			return importEntity;
		}

        /// <summary>
        /// Try to map a link like "file:275" from the import to the target system
        /// Will return null if nothing appropriate found, so the caller can choose to not do anything
        /// </summary>
        /// <param name="sourceValueString"></param>
        /// <returns></returns>
	    private string GetMappedLink(string sourceValueString)
	    {
            // file
	        var fileRegex = new Regex("^File:(?<Id>[0-9]+)", RegexOptions.IgnoreCase);
	        var a = fileRegex.Match(sourceValueString);

	        if (a.Success && a.Groups["Id"].Length > 0)
	        {
	            var originalId = int.Parse(a.Groups["Id"].Value);

	            if (_fileIdCorrectionList.ContainsKey(originalId))
	                return fileRegex.Replace(sourceValueString, "file:" + _fileIdCorrectionList[originalId]);
	        }

            // folder
	        var folderRegEx = new Regex("^folder:(?<Id>[0-9]+)", RegexOptions.IgnoreCase);
	        var f = folderRegEx.Match(sourceValueString);

	        if (f.Success && f.Groups["Id"].Length > 0)
	        {
	            var originalId = int.Parse(f.Groups["Id"].Value);

	            if (_folderIdCorrectionList.ContainsKey(originalId))
	                return folderRegEx.Replace(sourceValueString, "folder:" + _folderIdCorrectionList[originalId]);
	        }

	        return null;
	    }

	    #endregion
	}

    public class TemplateDefault
    {

        public string ItemType;
        public string ContentTypeStaticName;
        public int? DemoEntityId;

    }
}