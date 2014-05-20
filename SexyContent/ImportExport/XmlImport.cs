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

        #region Prerequisites
        public XmlImport()
        {
            // Prepare
            ImportLog = new List<ExportImportMessage>();
        }

        private bool IsCompatible(XDocument doc)
        {
            // Return if no Root Node "SexyContent"
            if (!doc.Elements("SexyContent").Any())
            {
                ImportLog.Add(new ExportImportMessage("The XML file you specified does not seem to be a 2SexyContent Export.", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            // Return if Version does not match
            if (!doc.Element("SexyContent").Attributes().Any(a => a.Name == "MinimumRequiredVersion") || new Version(doc.Element("SexyContent").Attribute("MinimumRequiredVersion").Value) > new Version(SexyContent.ModuleVersion))
            {
                ImportLog.Add(new ExportImportMessage("This template or app requires 2SexyContent " + doc.Element("SexyContent").Attribute("MinimumRequiredVersion").Value + " in order to work, you have version " + SexyContent.ModuleVersion + " installed.", ExportImportMessage.MessageTypes.Error));
                return false;
            }

            return true;
        }

        private void PrepareFileIdCorrectionList(XElement sexyContentNode)
        {
            _fileIdCorrectionList = new Dictionary<int, int>();

            if (!sexyContentNode.Elements("PortalFiles").Any())
                return;

            var portalId = PortalSettings.Current.PortalId;
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
                ImportLog.Add(new ExportImportMessage("The import file is not compatible with the installed version of 2SexyContent.", ExportImportMessage.MessageTypes.Error));
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
                ImportLog.Add(new ExportImportMessage("The import file is not compatible with the installed version of 2SexyContent.", ExportImportMessage.MessageTypes.Error));
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
                        ExternalKey = PortalSettings.Current.DefaultLanguage,
                        Name = "(added by import System, default language " + PortalSettings.Current.DefaultLanguage + ")",
                        SystemKey = "Culture"
                    });
            #endregion

            var importAttributeSets = GetImportAttributeSets(xmlSource.Element("AttributeSets").Elements("AttributeSet"));
            var importEntities = GetImportEntities(xmlSource.Elements("Entities").Elements("Entity"), SexyContent.AssignmentObjectTypeIDDefault);
            
            var import = new ToSic.Eav.Import.Import(_zoneId, _appId, PortalSettings.Current.UserInfo.DisplayName);
            import.RunImport(importAttributeSets, importEntities, true, true);
            ImportLog.AddRange(GetExportImportMessagesFromImportLog(import.ImportLog));
            
            if (xmlSource.Elements("Templates").Any())
            {
                if(_sexy.TemplateContext.Connection.State != ConnectionState.Open)
                    _sexy.TemplateContext.Connection.Open();
                var transaction = _sexy.TemplateContext.Connection.BeginTransaction();
                List<Entity> templateDescribingEntities;
                ImportXmlTemplates(xmlSource, out templateDescribingEntities);

                var import2 = new ToSic.Eav.Import.Import(_zoneId, _appId, PortalSettings.Current.UserInfo.DisplayName);
                import2.RunImport(new List<AttributeSet>(), templateDescribingEntities, true, true);
                ImportLog.AddRange(GetExportImportMessagesFromImportLog(import2.ImportLog));

                transaction.Commit();
            }

            return true;
        }

        /// <summary>
        /// Maps EAV import messages to 2SexyContent import messages
        /// </summary>
        /// <param name="importLog"></param>
        /// <returns></returns>
        public IEnumerable<ExportImportMessage> GetExportImportMessagesFromImportLog(List<LogItem> importLog)
        {
            return importLog.Select(l => new ExportImportMessage(l.Message, 
                l.EntryType == EventLogEntryType.Error ? ExportImportMessage.MessageTypes.Error :
                l.EntryType == EventLogEntryType.Information ? ExportImportMessage.MessageTypes.Information :
                l.EntryType == EventLogEntryType.Warning? ExportImportMessage.MessageTypes.Warning :
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
                        TitleAttribute = titleAttribute
                    });
            }

            return importAttributeSets;
        }

        #endregion

        #region Templates

        private void ImportXmlTemplates(XElement Root, out List<Entity> entities)
        {
            var templates = Root.Element("Templates");
            entities = new List<Entity>();

            foreach (var template in templates.Elements("Template"))
            {
                string attributeSetStaticName = template.Attribute("AttributeSetStaticName").Value;
                ToSic.Eav.AttributeSet Set = _sexy.ContentContext.GetAttributeSet(attributeSetStaticName);

                Template t = _sexy.TemplateContext.GetNewTemplate(_sexy.AppId.Value);
                t.Name = template.Attribute("Name").Value;
                t.Path = template.Attribute("Path").Value;

                if (Set == null)
                {
                    ImportLog.Add(
                        new ExportImportMessage("Content Type for Template '" + t.Name + "' could not be found. The template has not been imported.",
                                                ExportImportMessage.MessageTypes.Warning));
                    continue;
                }
                else
                    t.AttributeSetID = _sexy.ContentContext.GetAttributeSet(attributeSetStaticName).AttributeSetID;

                string DemoEntityGuid = template.Attribute("DemoEntityGUID").Value;
                if (!String.IsNullOrEmpty(DemoEntityGuid))
                {
                    var EntityGuid = Guid.Parse(DemoEntityGuid);
                    if (_sexy.ContentContext.EntityExists(EntityGuid))
                        t.DemoEntityID = _sexy.ContentContext.GetEntity(EntityGuid).EntityID;
                    else
                        ImportLog.Add(
                            new ExportImportMessage(
                                "Demo Entity for Template '" + t.Name + "' could not be found. (Guid: " + DemoEntityGuid +
                                ")", ExportImportMessage.MessageTypes.Information));

                }

                t.Script = template.Attribute("Script").Value;
                t.IsFile = Boolean.Parse(template.Attribute("IsFile").Value);
                t.Type = template.Attribute("Type").Value;
                t.IsHidden = Boolean.Parse(template.Attribute("IsHidden").Value);
                t.Location = template.Attribute("Location").Value;

                if (template.Attribute("UseForList") != null)
                    t.UseForList = Boolean.Parse(template.Attribute("UseForList").Value);

                // Stop if the same template already exists
                if (_sexy.GetTemplates(PortalSettings.Current.PortalId)
                         .Any(p => p.AttributeSetID == t.AttributeSetID
                                   && p.Path == t.Path
                                   && p.UseForList == t.UseForList && p.SysDeleted == null))
                    continue;

                t.PortalID = PortalSettings.Current.PortalId;
                _sexy.TemplateContext.AddTemplate(t);

                foreach (XElement xEntity in template.Elements("Entity"))
                    entities.Add(GetImportEntity(xEntity, SexyContent.AssignmentObjectTypeIDSexyContentTemplate, t.TemplateID));

                ImportLog.Add(new ExportImportMessage("Template '" + t.Name + "' successfully imported.",
                                                     ExportImportMessage.MessageTypes.Information));
            }

            _sexy.TemplateContext.SaveChanges();
        }

        #endregion

        #region Entities

        /// <summary>
        /// Returns a collection of EAV import entities
        /// </summary>
        /// <param name="entities"></param>
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
        /// <returns></returns>
        private Entity GetImportEntity(XElement xEntity, int assignmentObjectTypeId, int? keyNumber = null)
        {
            // Special case: App AttributeSets must be assigned to the current app
            if (xEntity.Attribute("AssignmentObjectType").Value == "App")
            {
                keyNumber = _appId;
                assignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDSexyContentApp;
            }

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
                _targetDimensions, _sourceDimensions, _sourceDefaultDimensionId, PortalSettings.Current.DefaultLanguage, keyNumber);

            return importEntity;

            //var attributeSetStaticName = xEntity.Attribute("AttributeSetStaticName").Value;



            //var targetEntity = new Entity()
            //    {
            //        AssignmentObjectTypeId = assignmentObjectTypeId,
            //        AttributeSetStaticName = xEntity.Attribute("AttributeSetStaticName").Value,
            //        EntityGuid = Guid.Parse(xEntity.Attribute("EntityGUID").Value),
            //        KeyNumber = keyNumber
            //    };

            //var targetValues = new Dictionary<string, List<IValueImportModel>>();

            //// Group values by StaticName
            //var valuesGroupedByStaticName = xEntity.Elements("Value")
            //    .GroupBy(v => v.Attribute("Key").Value, e => e, (key, e) => new {StaticName = key, Values = e.ToList()});

            //// Process each attribute (values grouped by StaticName)
            //foreach (var sourceAttribute in valuesGroupedByStaticName)
            //{
            //    var sourceValues = sourceAttribute.Values;
            //    var tempTargetValues = new List<ImportValue>();

            //    // Process each target's language
            //    foreach (var targetDimension in _targetDimensions.OrderByDescending(p => p.ExternalKey == PortalSettings.Current.DefaultLanguage).ThenBy(p => p.ExternalKey))
            //    {
            //        // This list will contain all source dimensions
            //        List<Dimension> sourceLanguages = new List<Dimension>();

            //        // Add exact match source language, if exists
            //        var exactMatchSourceDimension = _sourceDimensions.FirstOrDefault(p => p.ExternalKey == targetDimension.ExternalKey);
            //        if (exactMatchSourceDimension != null)
            //            sourceLanguages.Add(exactMatchSourceDimension);

            //        // Add un-exact match language
            //        var unExactMatchSourceDimensions = _sourceDimensions.Where(p => p.ExternalKey != targetDimension.ExternalKey && p.ExternalKey.StartsWith(targetDimension.ExternalKey.Substring(0, 3)))
            //            .OrderByDescending(p => p.ExternalKey == PortalSettings.Current.DefaultLanguage)
            //            .ThenByDescending(p => p.ExternalKey.Substring(0, 2) == p.ExternalKey.Substring(3, 2))
            //            .ThenBy(p => p.ExternalKey);
            //        sourceLanguages.AddRange(unExactMatchSourceDimensions);

            //        // Add primary language, if current target is primary
            //        if (targetDimension.ExternalKey == PortalSettings.Current.DefaultLanguage)
            //        {
            //            var sourcePrimaryLanguage = _sourceDimensions.FirstOrDefault(p => p.DimensionID == int.Parse(_sourceDefaultDimensionId));
            //            if (sourcePrimaryLanguage != null && !sourceLanguages.Contains(sourcePrimaryLanguage))
            //                sourceLanguages.Add(sourcePrimaryLanguage);
            //        }

            //        XElement sourceValue = null;
            //        bool readOnly = false;

            //        foreach (var sourceLanguage in sourceLanguages)
            //        {
            //            sourceValue = sourceValues.FirstOrDefault(p => p.Elements("Dimension").Any(d => d.Attribute("DimensionID").Value == sourceLanguage.DimensionID.ToString()));

            //            if (sourceValue == null)
            //                continue;

            //            readOnly = Boolean.Parse(sourceValue.Elements("Dimension").FirstOrDefault(p => p.Attribute("DimensionID").Value == sourceLanguage.DimensionID.ToString()).Attribute("ReadOnly").Value);

            //            // Override ReadOnly for primary target language
            //            if (targetDimension.ExternalKey == PortalSettings.Current.DefaultLanguage)
            //                readOnly = false;

            //            break;
            //        }

            //        // Take first value if there is only one value wihtout a dimension (default / fallback value), but only in primary language
            //        if (sourceValue == null && sourceValues.Count == 1 && !sourceValues.Elements("Dimension").Any() && targetDimension.ExternalKey == PortalSettings.Current.DefaultLanguage)
            //            sourceValue = sourceValues.First();

            //        // Process found value
            //        if (sourceValue != null)
            //        {
            // Special cases for template-describing values
            //if (attributeSetStaticName == SexyContent.AttributeSetStaticNameTemplateContentTypes)
            //{
            //    var sourceValueString = sourceValue.Attribute("Value").Value;
            //    if (!String.IsNullOrEmpty(sourceValueString))
            //    {
            //        switch (sourceAttribute.StaticName)
            //        {
            //            case "ContentTypeID":
            //                var attributeSet = _sexy.ContentContext.AttributeSetExists(sourceValueString, _sexy.ContentContext.AppId) ? _sexy.ContentContext.GetAttributeSet(sourceValueString) : null;
            //                sourceValue.Attribute("Value").SetValue(attributeSet != null ? attributeSet.AttributeSetID.ToString() : "0");
            //                break;
            //            case "DemoEntityID":
            //                var entityGuid = new Guid(sourceValue.Attribute("Value").Value);
            //                var demoEntity = _sexy.ContentContext.EntityExists(entityGuid) ? _sexy.ContentContext.GetEntity(entityGuid) : null;
            //                sourceValue.Attribute("Value").SetValue(demoEntity != null ? demoEntity.EntityID.ToString() : "0");
            //                break;
            //        }
            //    }
            //}

            //// Correct FileId in Hyperlink fields (takes XML data that lists files)
            //if (sourceValue.Attribute("Type").Value == "Hyperlink")
            //{
            //    var sourceValueString = sourceValue.Attribute("Value").Value;
            //    var fileRegex = new Regex("^File:(?<FileId>[0-9]+)", RegexOptions.IgnoreCase);
            //    var a = fileRegex.Match(sourceValueString);
            //    if (a.Success && a.Groups["FileId"].Length > 0)
            //    {
            //        var originalId = int.Parse(a.Groups["FileId"].Value);

            //        if (_fileIdCorrectionList.ContainsKey(originalId))
            //        {
            //            var newValue = fileRegex.Replace(sourceValueString, "File:" + _fileIdCorrectionList[originalId].ToString());
            //            sourceValue.Attribute("Value").SetValue(newValue);
            //        }

            //    }
            //}

            //var dimensionsToAdd = new List<ToSic.Eav.Import.ValueDimension>();
            //if (_targetDimensions.Single(p => p.ExternalKey == targetDimension.ExternalKey).DimensionID >= 1)
            //    dimensionsToAdd.Add(new ToSic.Eav.Import.ValueDimension() { DimensionExternalKey = targetDimension.ExternalKey, ReadOnly = readOnly });

            //// If value has already been added to the list, add just dimension with original ReadOnly state
            //var existingImportValue = tempTargetValues.FirstOrDefault(p => p.XmlValue == sourceValue);
            //if (existingImportValue != null)
            //    existingImportValue.Dimensions.AddRange(dimensionsToAdd);
            //else
            //{
            //    tempTargetValues.Add(new ImportValue()
            //    {
            //        Dimensions = dimensionsToAdd,
            //        XmlValue = sourceValue
            //    });
            //}

            //        }

            //    }

            //    var currentAttributesImportValues = tempTargetValues.Select(tempImportValue => GetImportValue(tempImportValue.XmlValue, tempImportValue.Dimensions, targetEntity)).ToList();
            //    targetValues.Add(sourceAttribute.StaticName, currentAttributesImportValues);
            //}

            //targetEntity.Values = targetValues;

            //return targetEntity;


        }

        //private ToSic.Eav.Import.IValueImportModel GetImportValue(XElement xValue, List<ToSic.Eav.Import.ValueDimension> valueDimensions, ToSic.Eav.Import.Entity referencingEntity)
        //{
        //    var stringValue = xValue.Attribute("Value").Value;
        //    var type = xValue.Attribute("Type").Value;

        //    IValueImportModel value;

        //    switch (type)
        //    {
        //        case "String":
        //        case "Hyperlink":
        //            value = new ValueImportModel<string>(referencingEntity) { Value = stringValue };
        //            break;
        //        case "Number":
        //            decimal typedDecimal;
        //            var isDecimal = Decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out typedDecimal);
        //            decimal? typedDecimalNullable = null;
        //            if(isDecimal)
        //                typedDecimalNullable = typedDecimal;
        //            value = new ValueImportModel<decimal?>(referencingEntity)
        //                {
        //                    Value = typedDecimalNullable
        //                };
        //            break;
        //        case "Entity":
        //            throw new NotImplementedException();
        //        case "DateTime":
        //            DateTime typedDateTime;
        //            value = new ValueImportModel<DateTime?>(referencingEntity)
        //                {
        //                    Value = DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out typedDateTime) ? typedDateTime : new DateTime?()
        //                };
        //            break;
        //        case "Boolean":
        //            bool typedBoolean;
        //            value = new ValueImportModel<bool?>(referencingEntity)
        //                {
        //                    Value = Boolean.TryParse(stringValue, out typedBoolean) ? typedBoolean : new bool?()
        //                };
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(type, stringValue, "Unknown type argument found in import XML.");
        //    }

        //    value.ValueDimensions = valueDimensions;

        //    return value;
        //}

        #endregion
    }
}