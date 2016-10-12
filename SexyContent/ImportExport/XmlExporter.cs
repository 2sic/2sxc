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
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.SexyContent.Adam;

namespace ToSic.SexyContent.ImportExport
{

    // todo: move all strings to XmlConstants


    public class XmlExporter
    {
        // initialize data context
        private readonly App _app;
        private readonly EavDataController _eavAppContext;
        private readonly List<int> _referencedFileIds = new List<int>();
        private readonly List<int> _referencedFolderIds = new List<int>();
        public List<IFileInfo> ReferencedFiles = new List<IFileInfo>();
        private readonly bool _isAppExport;

        // private IFolderManager DnnFolders = DotNetNuke.Services.FileSystem.FolderManager.Instance;
        private readonly IFileManager _dnnFiles = DotNetNuke.Services.FileSystem.FileManager.Instance;

        public string[] AttributeSetIDs;
        public string[] EntityIDs;
        public List<ExportImportMessage> Messages = new List<ExportImportMessage>();

        #region simple properties
        public PortalSettings Portal => PortalSettings.Current;

        #endregion

        #region Export

        public XmlExporter(int zoneId, int appId, bool appExport, string[] attrSetIds, string[] entityIds)
        {
            _isAppExport = appExport;
            //Sexy = new SxcInstance(_zoneId, _appId);
            _app = new App(zoneId, appId, PortalSettings.Current);
            _eavAppContext = _app.EavContext;
            AttributeSetIDs = attrSetIds;
            EntityIDs = entityIds;

            // this must happen very early, to ensure that the file-lists etc. are correct for exporting when used externally
            InitExportXDocument();
        }

        /// <summary>
        /// Exports given AttributeSets, Entities and Templates to an XML and returns the XML as string.
        /// </summary>
        /// <returns></returns>
        public string GenerateNiceXml()
        {
            var doc = ExportXDocument;

            // Will be used to show an export protocoll in future
            Messages = null;

            // Write XDocument to string and return it
            var xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Document,
                Indent = true
            };

            using (var stringWriter = new Utf8StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter, xmlSettings))
                    doc.Save(writer);
                return stringWriter.ToString();
            }
        }

        private XDocument _exportDocument;

        public XDocument ExportXDocument => _exportDocument;

        private void InitExportXDocument()
        {

            //_referencedFileIds = new List<int>();
            //_referencedFolderIds = new List<int>();
            //ReferencedFiles = new List<IFileInfo>();   

            // Create XML document and declaration
            var doc = _exportDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), null);

            #region Header

            var dimensions = _eavAppContext.Dimensions.GetDimensionChildren("Culture");
            var header = new XElement(XmlConstants.Header,
                _isAppExport && _app.AppGuid != "Default"
                    ? new XElement(XmlConstants.App,
                        new XAttribute(XmlConstants.Guid, _app.AppGuid)
                        )
                    : null,
                new XElement("Language", new XAttribute("Default", Portal.DefaultLanguage)),
                new XElement("Dimensions", dimensions.Select(d => new XElement("Dimension",
                    new XAttribute("DimensionID", d.DimensionID),
                    new XAttribute("Name", d.Name),
                    new XAttribute("SystemKey", d.SystemKey ?? String.Empty),
                    new XAttribute("ExternalKey", d.ExternalKey ?? String.Empty),
                    new XAttribute("Active", d.Active)
                    )))
                );

            #endregion

            #region Attribute Sets

            var attributeSets = new XElement("AttributeSets");

            // Go through each AttributeSetID
            foreach (var attributeSetId in AttributeSetIDs)
            {
                var id = int.Parse(attributeSetId);
                var set = _eavAppContext.AttribSet.GetAttributeSet(id);
                var attributes = new XElement("Attributes");

                // Add all Attributes to AttributeSet including meta informations
                foreach (var x in _eavAppContext.Attributes.GetAttributesInSet(id))
                {
                    var attribute = new XElement("Attribute",
                        new XAttribute(Const2.Static, x.Attribute.StaticName),
                        new XAttribute(Const2.Type, x.Attribute.Type),
                        new XAttribute(Const2.IsTitle, x.IsTitle),
                        // Add Attribute MetaData
                        from c in
                            _eavAppContext.Entities.GetEntities(Constants.AssignmentObjectTypeIdFieldProperties,
                                x.AttributeID).ToList()
                        select GetEntityXElement(c)
                        );

                    attributes.Add(attribute);
                }

                // Add AttributeSet / Content Type
                var attributeSet = new XElement("AttributeSet",
                    new XAttribute(Const2.Static, set.StaticName),
                    new XAttribute(Const2.Name, set.Name),
                    new XAttribute(Const2.Description, set.Description),
                    new XAttribute(Const2.Scope, set.Scope),
                    new XAttribute(Const2.AlwaysShareConfig, set.AlwaysShareConfiguration),
                    attributes);

                // Add Ghost-Info if content type inherits from another content type
                if (set.UsesConfigurationOfAttributeSet.HasValue)
                {
                    var parentAttributeSet =
                        _eavAppContext.SqlDb.AttributeSets.First(
                            a =>
                                a.AttributeSetID == set.UsesConfigurationOfAttributeSet.Value &&
                                a.ChangeLogDeleted == null);
                    attributeSet.Add(new XAttribute("UsesConfigurationOfAttributeSet", parentAttributeSet.StaticName));
                }

                attributeSets.Add(attributeSet);
            }

            #endregion

            #region Entities

            var entities = new XElement("Entities");

            // Go through each Entity
            foreach (var entityId in EntityIDs)
            {
                var id = int.Parse(entityId);

                // Get the entity and ContentType from ContentContext add Add it to ContentItems
                var entity = _eavAppContext.Entities.GetEntity(id);
                entities.Add(GetEntityXElement(entity));
            }

            #endregion

            // init ADAM files (add to queue)
            AddAdamFilesToExportQueue();

            // Create root node "SexyContent" and add ContentTypes, ContentItems and Templates
            doc.Add(new XElement(XmlConstants.RootNode,
                new XAttribute("FileVersion", ImportExport.FileVersion),
                new XAttribute("MinimumRequiredVersion", ImportExport.MinimumRequiredVersion),
                new XAttribute("ModuleVersion", Settings.ModuleVersion),
                new XAttribute("ExportDate", DateTime.Now),
                header,
                attributeSets,
                entities,
                GetFilesXElements(),
                GetFoldersXElements()));
        }

        private void AddAdamFilesToExportQueue()
        {
            var adam = new AdamManager(Portal.PortalId, _app);
            var adamIds = adam.Export.AppFiles;
            adamIds.ForEach(AddFileAndFolderToQueue);

            // also add folders in adam - because empty folders may also have metadata assigned
            var adamFolders = adam.Export.AppFolders;
            adamFolders.ForEach(AddFolderToQueue);
        }

        /// <summary>
        /// Returns an Entity XElement
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private XElement GetEntityXElement(Entity e)
        {
            //Note that this often throws errors in a dev environment, where the data may be mangled manually in the DB
            XElement entityXElement;
            try
            {
                entityXElement = new Eav.ImportExport.XmlExport(_eavAppContext).GetEntityXElement(e.EntityID);
            }
            catch (Exception ex)
            {
                throw new Exception("failed on entity id '" + e.EntityID + "' of set-type '" + e.AttributeSetID + "'", ex);
            }

            foreach (var value in entityXElement.Elements("Value"))
            {
                var valueString = value.Attribute("Value").Value;
                var valueType = value.Attribute("Type").Value;
                var valueKey = value.Attribute("Key").Value;

                // Special cases for Template ContentTypes
                if (e.Set.StaticName == "2SexyContent-Template-ContentTypes" && !string.IsNullOrEmpty(valueString))
                {
                    switch (valueKey)
                    {
                        case "ContentTypeID":
                            var attributeSet = _eavAppContext.AttribSet.GetAllAttributeSets().FirstOrDefault(a => a.AttributeSetID == int.Parse(valueString));
                            value.Attribute("Value").SetValue(attributeSet != null ? attributeSet.StaticName : string.Empty);
                            break;
                        case "DemoEntityID":
                            var entityId = int.Parse(valueString);
                            var demoEntity = _eavAppContext.SqlDb.Entities.FirstOrDefault(en => en.EntityID == entityId);
                            value.Attribute("Value").SetValue(demoEntity?.EntityGUID.ToString() ?? string.Empty);
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
                    var file = _dnnFiles.GetFile(fileNum);
                    AddFolderToQueue(file.FolderId);
                }
                catch
                {
                    // don't do anything, because if the file doesn't exist, its FOLDER should also not land in the queue
                }
            }
            catch
            {
                // don't do anything, because if the file doesn't exist, it should also not land in the queue
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
            var folderController = FolderManager.Instance;
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
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}