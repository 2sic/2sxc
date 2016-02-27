using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using Telerik.Web.Data.Extensions;
using ToSic.Eav;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent.ImportExport
{

    // todo: move all strings to XmlConstants


    public class XmlExporter
    {
        // initialize data context
        private readonly SexyContent Sexy;
        private List<int> _referencedFileIds = new List<int>();
        private List<int> _referencedFolderIds = new List<int>();
        public List<IFileInfo> ReferencedFiles = new List<IFileInfo>();
        private int _zoneId;
        private int _appId;
        private readonly bool _isAppExport;

        private IFolderManager DnnFolders = DotNetNuke.Services.FileSystem.FolderManager.Instance;
        private IFileManager DnnFiles = DotNetNuke.Services.FileSystem.FileManager.Instance;

        public string[] AttributeSetIDs;
        public string[] EntityIDs;
        public List<ExportImportMessage> Messages = new List<ExportImportMessage>();

        #region simple properties
        public PortalSettings Portal => PortalSettings.Current;

        #endregion

        #region Export

        public XmlExporter(int zoneId, int appId, bool appExport, string[] attrSetIds, string[] entityIds)
        {
            _zoneId = zoneId;
            _appId = appId;
            _isAppExport = appExport;
            Sexy = new SexyContent(_zoneId, _appId);
            AttributeSetIDs = attrSetIds;
            EntityIDs = entityIds;
        }

        /// <summary>
        /// Exports given AttributeSets, Entities and Templates to an XML and returns the XML as string.
        /// </summary>
        /// <param name="AttributeSetIDs"></param>
        /// <param name="EntityIDs"></param>
        /// <param name="TemplateIDs"></param>
        /// <param name="Messages"></param>
        /// <returns></returns>
        public string GenerateNiceXml()
        {
            var Doc = ExportXDocument;

            // Will be used to show an export protocoll in future
            Messages = null;

            // Write XDocument to string and return it
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlSettings.Indent = true;

            using (var stringWriter = new Utf8StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter, xmlSettings))
                    Doc.Save(writer);
                return stringWriter.ToString();
            }
        }

        private XDocument _exportDocument;

        public XDocument ExportXDocument 
        {
            get
            {
                // if this is the second time we're accessing this, just return the one we already created
                if (_exportDocument != null)
                    return _exportDocument;

                //_referencedFileIds = new List<int>();
                //_referencedFolderIds = new List<int>();
                //ReferencedFiles = new List<IFileInfo>();   

                // Create XML document and declaration
                var Doc = _exportDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), null);

                #region Header

                var Dimensions = Sexy.ContentContext.Dimensions.GetDimensionChildren("Culture");
                var Header = new XElement(XmlConstants.Header,
                    _isAppExport && Sexy.App.AppGuid != "Default"
                        ? new XElement(XmlConstants.App,
                            new XAttribute(XmlConstants.Guid, Sexy.App.AppGuid)
                            )
                        : null,
                    new XElement("Language", new XAttribute("Default", Portal.DefaultLanguage)),
                    new XElement("Dimensions", Dimensions.Select(d => new XElement("Dimension",
                        new XAttribute("DimensionID", d.DimensionID),
                        new XAttribute("Name", d.Name),
                        new XAttribute("SystemKey", d.SystemKey ?? String.Empty),
                        new XAttribute("ExternalKey", d.ExternalKey ?? String.Empty),
                        new XAttribute("Active", d.Active)
                        )))
                    );

                #endregion

                #region Attribute Sets

                var AttributeSets = new XElement("AttributeSets");

                // Go through each AttributeSetID
                foreach (var AttributeSetID in AttributeSetIDs)
                {
                    var ID = int.Parse(AttributeSetID);
                    var Set = Sexy.ContentContext.AttribSet.GetAttributeSet(ID);
                    var Attributes = new XElement("Attributes");

                    // Add all Attributes to AttributeSet including meta informations
                    foreach (var x in Sexy.ContentContext.Attributes.GetAttributesInSet(ID))
                    {
                        var Attribute = new XElement("Attribute",
                            new XAttribute("StaticName", x.Attribute.StaticName),
                            new XAttribute("Type", x.Attribute.Type),
                            new XAttribute("IsTitle", x.IsTitle),
                            // Add Attribute MetaData
                            from c in
                                Sexy.ContentContext.Entities.GetEntities(Constants.AssignmentObjectTypeIdFieldProperties,
                                    x.AttributeID).ToList()
                            select GetEntityXElement(c)
                            );

                        Attributes.Add(Attribute);
                    }

                    // Add AttributeSet / Content Type
                    var AttributeSet = new XElement("AttributeSet",
                        new XAttribute("StaticName", Set.StaticName),
                        new XAttribute("Name", Set.Name),
                        new XAttribute("Description", Set.Description),
                        new XAttribute("Scope", Set.Scope),
                        Attributes);

                    // Add Ghost-Info if content type inherits from another content type
                    if (Set.UsesConfigurationOfAttributeSet.HasValue)
                    {
                        var parentAttributeSet = Sexy.ContentContext.SqlDb.AttributeSets.First(a => a.AttributeSetID ==  Set.UsesConfigurationOfAttributeSet.Value && a.ChangeLogDeleted == null);
                        AttributeSet.Add(new XAttribute("UsesConfigurationOfAttributeSet", parentAttributeSet.StaticName));
                    }

                    AttributeSets.Add(AttributeSet);
                }

                #endregion

                #region Entities

                var Entities = new XElement("Entities");

                // Go through each Entity
                foreach (var EntityID in EntityIDs)
                {
                    var ID = int.Parse(EntityID);

                    // Get the entity and ContentType from ContentContext add Add it to ContentItems
                    var Entity = Sexy.ContentContext.Entities.GetEntity(ID);
                    Entities.Add(GetEntityXElement(Entity));
                }

                #endregion

                #region Adam files
                var adam = new AdamManager(Portal.PortalId, Sexy.App);
                var adamIds = adam.Export.AppFiles;
                adamIds.ForEach(AddFileAndFolderToQueue);

                // also add folders in adam - because empty folders may also have metadata assigned
                var adamFolders = adam.Export.AppFolders;
                adamFolders.ForEach(AddFolderToQueue);
                #endregion

                // Create root node "SexyContent" and add ContentTypes, ContentItems and Templates
                Doc.Add(new XElement(XmlConstants.RootNode,
                    new XAttribute("FileVersion", ImportExport.FileVersion),
                    new XAttribute("MinimumRequiredVersion", ImportExport.MinimumRequiredVersion),
                    new XAttribute("ModuleVersion", Settings.ModuleVersion),
                    new XAttribute("ExportDate", DateTime.Now),
                    Header,
                    AttributeSets,
                    Entities,
                    GetFilesXElements(),
                    GetFoldersXElements()));
                return Doc;
            }
        }

        /// <summary>
        /// Returns an Entity XElement
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private XElement GetEntityXElement(Entity e)
        {
            //var attributeSet = Sexy.ContentContext.GetAttributeSet(e.AttributeSetID);
            var entityXElement = new Eav.ImportExport.XmlExport(Sexy.ContentContext).GetEntityXElement(e.EntityID);

            foreach (var value in entityXElement.Elements("Value"))
            {
                var valueString = value.Attribute("Value").Value;
                var valueType = value.Attribute("Type").Value;
                var valueKey = value.Attribute("Key").Value;

                // Special cases for Template ContentTypes
                if (e.Set.StaticName == "2SexyContent-Template-ContentTypes" && !String.IsNullOrEmpty(valueString))
                {
                    switch (valueKey)
                    {
                        case "ContentTypeID":
                            var attributeSet = Sexy.ContentContext.AttribSet.GetAllAttributeSets().FirstOrDefault(a => a.AttributeSetID == int.Parse(valueString));
                            value.Attribute("Value").SetValue(attributeSet != null ? attributeSet.StaticName : String.Empty);
                            break;
                        case "DemoEntityID":
                            var entityID = int.Parse(valueString);
                            var demoEntity = Sexy.ContentContext.SqlDb.Entities.FirstOrDefault(en => en.EntityID == entityID);
                            value.Attribute("Value").SetValue(demoEntity != null ? demoEntity.EntityGUID.ToString() : String.Empty);
                            break;
                    }
                }

                // Collect all referenced files for adding a file list to the xml later
                if (valueType == "Hyperlink")
                {
                    var fileRegex = new Regex("^File:(?<FileId>[0-9]+)", RegexOptions.IgnoreCase);
                    var a = fileRegex.Match(valueString);
                    // try remember the file
                    if (a.Success && a.Groups["FileId"].Length > 0)
                        AddFileAndFolderToQueue(int.Parse(a.Groups["FileId"].Value));
                }
            }

	        if (e.KeyGuid.HasValue)
		        entityXElement.Add(new XAttribute("KeyGuid", e.KeyGuid));

            if (e.KeyNumber.HasValue)
                entityXElement.Add(new XAttribute("KeyNumber", e.KeyNumber));
            if (!string.IsNullOrEmpty(e.KeyString))
                entityXElement.Add(new XAttribute("KeyString", e.KeyString));

            //return new XElement("Entity",
            //    new XAttribute("AssignmentObjectType", e.AssignmentObjectType.Name),
            //    new XAttribute("AttributeSetStaticName", attributeSet.StaticName),
            //    new XAttribute("AttributeSetName", attributeSet.Name),
            //    new XAttribute("EntityGUID", e.EntityGUID),
            //    from c in Sexy.ContentContext.GetValues(e.EntityID)
            //    where c.ChangeLogDeleted == null
            //    select GetAttributeValueXElement(c.Attribute.StaticName, c, c.Attribute.Type, attributeSet));

            return entityXElement;
        }

        private void AddFileAndFolderToQueue(int fileNum)
        {
            try
            {
                _referencedFileIds.Add(fileNum);

                // also try to remember the folder
                try
                {
                    var file = DnnFiles.GetFile(fileNum);
                    AddFolderToQueue(file.FolderId);
                }
                finally
                {
                }
            }
            finally
            {
            }
        }

        private void AddFolderToQueue(int folderId)
        {
            _referencedFolderIds.Add(folderId);
        }

        #endregion

        #region Files & Pages

        private XElement GetFilesXElements()
        {
            return  new XElement("PortalFiles",
                    _referencedFileIds.Distinct().Select(GetFileXElement)
                );
        }


        private XElement GetFoldersXElements()
        {
            return  new XElement(XmlConstants.FolderGroup,
                    _referencedFolderIds.Distinct().Select(GetFolderXElement)
                );
        }

        private XElement GetFileXElement(int fileId)
        {
            var fileController = DotNetNuke.Services.FileSystem.FileManager.Instance;
            var file = fileController.GetFile(fileId);
            if (file != null)
            {
                ReferencedFiles.Add(file);

                return new XElement("File",
                        new XAttribute("Id", fileId),
                        new XAttribute("RelativePath", file.RelativePath)
                    );

            }

            return null;
        }
        private XElement GetFolderXElement(int folderId)
        {
            var folderController = DotNetNuke.Services.FileSystem.FolderManager.Instance;
            var folder = folderController.GetFolder(folderId);
            if (folder != null)
            {
                return new XElement(XmlConstants.Folder,
                        new XAttribute(XmlConstants.FolderNodeId, folderId),
                        new XAttribute(XmlConstants.FolderNodePath, folder.FolderPath) 
                    );
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Creates a new StringWriter with UTF8 Encoding
        /// </summary>
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }
    }
}