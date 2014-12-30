using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav;
using ToSic.SexyContent.DataImportExport.Extensions;

namespace ToSic.SexyContent.ImportExport
{
    public class XmlExport
    {
        // initialize data context
        private SexyContent Sexy;
        private List<int> _referencedFileIds;
        public List<IFileInfo> ReferencedFiles;
        private int _zoneId;
        private int _appId;
        private bool _isAppExport;

        #region Export

        public XmlExport(int zoneId, int appId, bool appExport)
        {
            _zoneId = zoneId;
            _appId = appId;
            _isAppExport = appExport;
            Sexy = new SexyContent(_zoneId, _appId);
        }

        /// <summary>
        /// Exports given AttributeSets, Entities and Templates to an XML and returns the XML as string.
        /// </summary>
        /// <param name="AttributeSetIDs"></param>
        /// <param name="EntityIDs"></param>
        /// <param name="TemplateIDs"></param>
        /// <param name="Messages"></param>
        /// <returns></returns>
        public string ExportXml(string[] AttributeSetIDs, string[] EntityIDs, string[] TemplateIDs, out List<ExportImportMessage> Messages)
        {
            _referencedFileIds = new List<int>();
            ReferencedFiles = new List<IFileInfo>();

            // Create XML document and declaration
            XDocument Doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), null);

            #region Header

            var Dimensions = Sexy.ContentContext.GetDimensionChildren("Culture");
            XElement Header = new XElement("Header",
                _isAppExport ? new XElement("App",
                    new XAttribute("Guid", Sexy.App.AppGuid)
                    //new XAttribute("Name", Sexy.App.Name),
                    //new XAttribute("Version", Sexy.App.Configuration.Version),
                    //new XAttribute("Folder", Sexy.App.Folder)
                ) : null,
                new XElement("Language", new XAttribute("Default", PortalSettings.Current.DefaultLanguage)),
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

            XElement AttributeSets = new XElement("AttributeSets");

            // Go through each AttributeSetID
            foreach (string AttributeSetID in AttributeSetIDs)
            {
                int ID = int.Parse(AttributeSetID);
                AttributeSet Set = Sexy.ContentContext.GetAttributeSet(ID);
                XElement Attributes = new XElement("Attributes");

                // Add all Attributes to AttributeSet including meta informations
                foreach (ToSic.Eav.AttributeInSet x in Sexy.ContentContext.GetAttributesInSet(ID))
                {
                    XElement Attribute = new XElement("Attribute",
                        new XAttribute("StaticName", x.Attribute.StaticName),
                        new XAttribute("Type", x.Attribute.Type),
                        new XAttribute("IsTitle", x.IsTitle),
                        // Add Attribute MetaData
                        from c in Sexy.ContentContext.GetEntities(DataSource.AssignmentObjectTypeIdFieldProperties, x.AttributeID, null, null)
                        select GetEntityXElement(c)
                    );

                    Attributes.Add(Attribute);
                }

                // Add AttributeSet / Content Type
                XElement AttributeSet = new XElement("AttributeSet",
                    new XAttribute("StaticName", Set.StaticName),
                    new XAttribute("Name", Set.Name),
                    new XAttribute("Description", Set.Description),
                    new XAttribute("Scope", Set.Scope),
                    Attributes);

                AttributeSets.Add(AttributeSet);
            }

            #endregion

            #region Entities

            XElement Entities = new XElement("Entities");

            // Go through each Entity
            foreach (string EntityID in EntityIDs)
            {
                int ID = int.Parse(EntityID);

                // Get the entity and ContentType from ContentContext add Add it to ContentItems
                var Entity = Sexy.ContentContext.GetEntity(ID);
                Entities.Add(GetEntityXElement(Entity));
            }

            #endregion

            #region Templates

            XElement Templates = new XElement("Templates");

            // Go through each Template
            foreach (string TemplateID in TemplateIDs)
            {
                int ID = int.Parse(TemplateID);
                Template t = Sexy.TemplateContext.GetTemplate(ID);
                Entity DemoEntity = t.DemoEntityID.HasValue ? Sexy.ContentContext.GetEntity(t.DemoEntityID.Value) : null;
				var pipelineEntity = t.PipelineEntityID.HasValue ? Sexy.ContentContext.GetEntity(t.PipelineEntityID.Value) : null;

                XElement Template = new XElement("Template",
                    new XAttribute("Name", t.Name),
                    new XAttribute("Path", t.Path),
                    new XAttribute("Location", t.Location),
                    new XAttribute("IsFile", t.IsFile.ToString()),
                    new XAttribute("Script", t.Script),
                    new XAttribute("Type", t.Type),
                    new XAttribute("AttributeSetStaticName", t.AttributeSetID.HasValue ? Sexy.ContentContext.GetAttributeSet(t.AttributeSetID.Value).StaticName : ""),
                    new XAttribute("IsHidden", t.IsHidden.ToString()),
                    new XAttribute("UseForList", t.UseForList.ToString()),
                    new XAttribute("DemoEntityGUID", DemoEntity != null ? DemoEntity.EntityGUID.ToString() : ""),
                    new XAttribute("PublishData", t.PublishData),
                    new XAttribute("StreamsToPublish", t.StreamsToPublish),
					new XAttribute("ViewNameInUrl", t.ViewNameInUrl),
					new XAttribute("PipelineEntityGUID", pipelineEntity != null ? pipelineEntity.EntityGUID.ToString() : ""),
                    (from c in Sexy.ContentContext.GetEntities(SexyContent.AssignmentObjectTypeIDSexyContentTemplate, t.TemplateID, null, null)
                     select GetEntityXElement(c))
                );

                Templates.Add(Template);
            }

            #endregion

            // Create root node "SexyContent" and add ContentTypes, ContentItems and Templates
            Doc.Add(new XElement("SexyContent",
                new XAttribute("FileVersion", ImportExport.FileVersion),
                new XAttribute("MinimumRequiredVersion", ImportExport.MinimumRequiredVersion),
                new XAttribute("ModuleVersion", SexyContent.ModuleVersion),
                new XAttribute("ExportDate", DateTime.Now),
                Header,
                AttributeSets,
                Entities,
                Templates,
                GetFilesXElements()));

            // Will be used to show an export protocoll in future
            Messages = null;

            // Write XDocument to string and return it
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Encoding = System.Text.Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlSettings.Indent = true;

            using (var stringWriter = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, xmlSettings))
                {
                    Doc.Save(writer);
                }
                return stringWriter.ToString();
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
            var entityXElement = new ToSic.Eav.ImportExport.XmlExport(Sexy.ContentContext).GetEntityXElement(e.EntityID);

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
                            var attributeSet = Sexy.ContentContext.GetAllAttributeSets().FirstOrDefault(a => a.AttributeSetID == int.Parse(valueString));
                            value.Attribute("Value").SetValue(attributeSet != null ? attributeSet.StaticName : String.Empty);
                            break;
                        case "DemoEntityID":
                            var entityID = int.Parse(valueString);
                            var demoEntity = Sexy.ContentContext.Entities.FirstOrDefault(en => en.EntityID == entityID);
                            value.Attribute("Value").SetValue(demoEntity != null ? demoEntity.EntityGUID.ToString() : String.Empty);
                            break;
                    }
                }

                // Collect all referenced files for adding a file list to the xml later
                if (valueType == "Hyperlink")
                {
                    var fileRegex = new Regex("^File:(?<FileId>[0-9]+)", RegexOptions.IgnoreCase);
                    var a = fileRegex.Match(valueString);
                    if (a.Success && a.Groups["FileId"].Length > 0)
                        _referencedFileIds.Add(int.Parse(a.Groups["FileId"].Value));
                }
            }

	        if (e.KeyGuid.HasValue)
		        entityXElement.Add(new XAttribute("KeyGuid", e.KeyGuid));

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

        ///// <summary>
        ///// Gets an Entity Value-Key XElement
        ///// </summary>
        ///// <param name="Key"></param>
        ///// <param name="Type"></param>
        ///// <param name="Value"></param>
        ///// <returns></returns>
        //private XElement GetAttributeValueXElement(string Key, EavValue Value, string Type, AttributeSet set)
        //{
        //    var value = Value.Value == null ? String.Empty : Value.Value.ToString(CultureInfo.InvariantCulture);

        //    var x = new XElement("Value",
        //        new XAttribute("Key", Key),
        //        new XAttribute("Value", value),
        //        !String.IsNullOrEmpty(Type) ? new XAttribute("Type", Type) : null,
        //        Value.ValuesDimensions.Select(p => new XElement("Dimension",
        //                new XAttribute("DimensionID", p.DimensionID),
        //                new XAttribute("ReadOnly", p.ReadOnly)
        //            ))
        //        );

        //    // Special cases for Template ContentTypes
        //    if (set.StaticName == "2SexyContent-Template-ContentTypes" && !String.IsNullOrEmpty(value))
        //    {
        //        switch (Key)
        //        {
        //            case "ContentTypeID":
        //                var attributeSet = Sexy.ContentContext.GetAllAttributeSets().FirstOrDefault(a => a.AttributeSetID == int.Parse(x.Attribute("Value").Value));
        //                x.Attribute("Value").SetValue(attributeSet != null ? attributeSet.StaticName : String.Empty);
        //                break;
        //            case "DemoEntityID":
        //                var entityID = int.Parse(x.Attribute("Value").Value);
        //                var demoEntity = Sexy.ContentContext.Entities.FirstOrDefault(e => e.EntityID == entityID);
        //                x.Attribute("Value").SetValue(demoEntity != null ? demoEntity.EntityGUID.ToString() : String.Empty);
        //                break;
        //        }
        //    }

        //    // Collect all referenced files for adding a file list to the xml later
        //    if (Type == "Hyperlink")
        //    {
        //        var fileRegex = new Regex("^File:(?<FileId>[0-9]+)", RegexOptions.IgnoreCase);
        //        var a = fileRegex.Match(value);
        //        if(a.Success && a.Groups["FileId"].Length > 0)
        //            _referencedFileIds.Add(int.Parse(a.Groups["FileId"].Value));
        //    }

        //    return x;
        //}

        #endregion

        #region Files & Pages

        private XElement GetFilesXElements()
        {
            return  new XElement("PortalFiles",
                    _referencedFileIds.Distinct().Select(GetFileXElement)
                );
        }

        private XElement GetFileXElement(int fileId)
        {
            var fileController = FileManager.Instance;
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

        #endregion

        /// <summary>
        /// Creates a new StringWriter with UTF8 Encoding
        /// </summary>
        public class Utf8StringWriter : StringWriter
        {
            public override System.Text.Encoding Encoding { get { return System.Text.Encoding.UTF8; } }
        }
    }
}