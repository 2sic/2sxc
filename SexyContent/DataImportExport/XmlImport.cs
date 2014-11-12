using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ToSic.Eav.Import;
using ToSic.SexyContent.DataImportExport.Extensions;
using ToSic.SexyContent.DataImportExport.Options;
using AttributeSet = ToSic.Eav.AttributeSet;

namespace ToSic.SexyContent.DataImportExport
{
    public class XmlImport
    {
        private int applicationId;

        private int zoneId;

        private SexyContent contentManager;

        private AttributeSet contentType;

        /// <summary>
        /// The xml document to imported.
        /// </summary>
        public XDocument Document 
        {
            get;
            private set;
        }

        /// <summary>
        /// The elements of the xml document.
        /// </summary>
        public IEnumerable<XElement> DocumentElements
        {
            get;
            private set;
        }

        private string documentLanguageFallback;

        private IEnumerable<string> languages;

        private ResourceReferenceImport resourceReference;

        private EntityClearImport entityClear;

        /// <summary>
        /// The entities created from the document. They will be saved to the repository.
        /// </summary>
        public List<Entity> Entities
        {
            get;
            private set;
        }

        /// <summary>
        /// Errors found while importing the document to memory.
        /// </summary>
        public ImportErrorProtocol ErrorProtocol
        {
            get;
            private set;
        }

        /// <summary>
        /// True if errors has been found while importing the document to memory.
        /// </summary>
        public bool HasErrors
        {
            get { return ErrorProtocol.Count() > 0; }
        }

        private Entity GetEntity(Guid entityGuid)
        {
            return Entities.FirstOrDefault(entity => entity.EntityGuid == entityGuid);
        }

        private Entity AppendEntity(Guid entityGuid)
        {
            var entity = new Entity()
            {
                AttributeSetStaticName = contentType.StaticName,
                AssignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDDefault,
                EntityGuid = entityGuid,
                KeyNumber = null,
                Values = new Dictionary<string, List<IValueImportModel>>()
            };
            Entities.Add(entity);
            return entity;
        }

        /// <summary>
        /// Create a xml import. The data stream passed will be imported to memory, and checked 
        /// for errors. If no error could be found, the data can be persisted to the repository.
        /// </summary>
        /// <param name="zoneId">ID of 2SexyContent zone</param>
        /// <param name="applicationId">ID of 2SexyContent application</param>
        /// <param name="contentTypeId">ID of 2SexyContent type</param>
        /// <param name="dataStream">Xml data stream to import</param>
        /// <param name="languages">Languages that can be imported (2SexyContent languages enabled)</param>
        /// <param name="documentLanguageFallback">Fallback document language</param>
        /// <param name="entityClear">How to handle entities already in the repository</param>
        /// <param name="resourceReference">How value references to files and pages are handled</param>
        public XmlImport(int zoneId, int applicationId, int contentTypeId, Stream dataStream, IEnumerable<string> languages, string documentLanguageFallback, EntityClearImport entityClear, ResourceReferenceImport resourceReference)
        {
            this.Entities = new List<Entity>();
            this.ErrorProtocol = new ImportErrorProtocol();

            this.applicationId = applicationId;
            this.zoneId = zoneId;
            this.contentManager = new SexyContent(zoneId, applicationId);
            this.contentType = contentManager.ContentContext.GetAttributeSet(contentTypeId);
            this.languages = languages;
            this.documentLanguageFallback = documentLanguageFallback;
            this.entityClear = entityClear;
            this.resourceReference = resourceReference;

            ValidateAndImportToMemory(dataStream);
        }

        /// <summary>
        /// Deserialize a 2sxc data xml stream to the memory. The data will also be checked for 
        /// errors.
        /// </summary>
        /// <param name="dataStream">Data stream</param>
        private void ValidateAndImportToMemory(Stream dataStream)
        {
            try
            {
                if (languages == null || languages.Count() == 0)
                {
                    languages = new string[] { string.Empty };
                }

                if (contentType == null)
                {
                    ErrorProtocol.AppendError(ImportErrorCode.InvalidContentType);
                    return;
                }

                Document = XDocument.Load(dataStream);
                dataStream.Position = 0;
                if (Document == null)
                {
                    ErrorProtocol.AppendError(ImportErrorCode.InvalidDocument);
                    return;
                }

                var documentRoot = Document.Element(DocumentNodeNames.Root + contentType.Name.RemoveSpecialCharacters());
                if (documentRoot == null)
                {
                    ErrorProtocol.AppendError(ImportErrorCode.InvalidRoot);
                    return;
                }

            
                DocumentElements = documentRoot.Elements(DocumentNodeNames.Entity);
                var documentElementNumber = 0;

                var documentElementLanguagesAll = DocumentElements.GroupBy(element => element.Element(DocumentNodeNames.EntityGuid).Value)
                                                                  .Select(group => group.Select(element => element.Element(DocumentNodeNames.EntityLanguage).Value)).ToList();
                var documentElementLanguagesCount = documentElementLanguagesAll.Select(item => item.Count());
                if (documentElementLanguagesCount.Any(count => count != 1))
                {
                    // It is an all language import, so check if all languages are specified for all entities
                    foreach (var documentElementLanguages in documentElementLanguagesAll)
                    {   
                        if (languages.Except(documentElementLanguages).Any())
                        {
                            ErrorProtocol.AppendError(ImportErrorCode.MissingElementLanguage, "Langs=" + string.Join(", ", languages));
                            return;
                        }
                    }
                }
 
                var entityGuidManager = new EntityGuidManager();
                foreach (var documentElement in DocumentElements)
                {
                    documentElementNumber++;

                    var documentElementLanguage = documentElement.GetChildElementValue(DocumentNodeNames.EntityLanguage);
                    if (!languages.Any(language => language == documentElementLanguage))
                    {   // DNN does not support the language
                        ErrorProtocol.AppendError(ImportErrorCode.InvalidLanguage, "Lang=" + documentElementLanguage, documentElementNumber);
                        continue;
                    }

                    var entityGuid = entityGuidManager.GetGuid(documentElement, documentLanguageFallback);
                    var entity = GetEntity(entityGuid);
                    if (entity == null)
                    {
                        entity = AppendEntity(entityGuid);
                    }

                    var attributes = contentType.GetAttributes();
                    foreach (var attribute in attributes)
                    {   
                        var valueName = attribute.StaticName;
                        var value = documentElement.GetChildElementValue(valueName);
                        if (value == null || value.IsValueDefault())
                        {
                            continue;
                        }

                        var valueType = attribute.Type;
                        var valueReferenceLanguage = value.GetValueReferenceLanguage();
                        if (valueReferenceLanguage == null)
                        {   // It is not a value reference.. it is a normal text
                            try
                            {
                                entity.AppendAttributeValue(valueName, value, valueType, documentElementLanguage, false, resourceReference.IsResolve());
                            }
                            catch (FormatException)
                            {
                                ErrorProtocol.AppendError(ImportErrorCode.InvalidValueFormat, valueName + ":" + valueType + "=" + value, documentElementNumber);
                            }
                            continue;
                        }

                        var valueReferenceProtection = value.GetValueReferenceProtection();
                        if (valueReferenceProtection != "rw" && valueReferenceProtection != "ro")
                        {
                            ErrorProtocol.AppendError(ImportErrorCode.InvalidValueReferenceProtection, value, documentElementNumber);
                            continue;
                        }
                        var valueReadOnly = valueReferenceProtection == "ro";

                        var entityValue = entity.GetAttributeValue(valueName, valueReferenceLanguage);
                        if (entityValue != null)
                        {
                            entityValue.AppendLanguageReference(documentElementLanguage, valueReadOnly);
                            continue;
                        }

                        // We do not have the value referenced in memory, so search for the 
                        // value in the database 
                        var dbEntity = contentType.GetEntity(entityGuid);
                        if (dbEntity == null)
                        {
                            ErrorProtocol.AppendError(ImportErrorCode.InvalidValueReference, value, documentElementNumber);
                            continue;
                        }

                        var dbEntityValue = dbEntity.GetAttributeValue(attribute, valueReferenceLanguage);
                        if(dbEntityValue == null)
                        {
                            ErrorProtocol.AppendError(ImportErrorCode.InvalidValueReference, value, documentElementNumber);
                            continue;
                        }

                        entity.AppendAttributeValue(valueName, dbEntityValue.Value, valueType, valueReferenceLanguage, dbEntityValue.IsLanguageReadOnly(valueReferenceLanguage), resourceReference.IsResolve())
                              .AppendLanguageReference(documentElementLanguage, valueReadOnly);       
                    }
                }                
            }
            catch (Exception exception)
            {
                ErrorProtocol.AppendError(ImportErrorCode.Unknown, exception.ToString());
            }
        }

        /// <summary>
        /// Save the data in memory to the repository.
        /// </summary>
        /// <param name="userId">ID of the user doing the import</param>
        /// <returns>True if succeeded</returns>
        public bool PersistImportToRepository(string userId)
        {
            if (HasErrors)
            {
                return false;
            }

            if (entityClear.IsAll())
            {
                var entityDeleteGuids = GetEntityDeleteGuids();
                foreach(var entityGuid in entityDeleteGuids)
                {
                    var entityID = contentType.GetEntity(entityGuid).EntityID;
                    if (contentManager.ContentContext.CanDeleteEntity(entityID).Item1)
                    {
                        contentManager.ContentContext.DeleteEntity(entityID);
                    }
                }
            }

            var import = new ToSic.Eav.Import.Import(zoneId, applicationId, userId, true);
            import.RunImport(null, Entities, true, true);
            return true;
        }


        #region Deserialize statistics methods
        private List<Guid> GetExistingEntityGuids()
        {
            var existingGuids = contentType.Entities.Where(entity => entity.ChangeLogIDDeleted == null).Select(entity => entity.EntityGUID).ToList();
            return existingGuids;
        }

        private List<Guid> GetCreatedEntityGuids()
        {
            var newGuids = Entities.Select(entity => entity.EntityGuid.Value).ToList();
            return newGuids;
        }

        /// <summary>
        /// Get the languages found in the xml document.
        /// </summary>
        public IEnumerable<string> LanguagesInDocument
        {
            get
            {
                return DocumentElements.Select(element => element.Element(DocumentNodeNames.EntityLanguage).Value).Distinct();
            }
        }

        /// <summary>
        /// Get the attribute names in the xml document.
        /// </summary>
        public IEnumerable<string> AttributeNamesInDocument
        {
            get 
            {
                return DocumentElements.SelectMany(element => element.Elements())
                                       .GroupBy(attribute => attribute.Name.LocalName)
                                       .Select(group => group.Key)
                                       .Where(name => name != DocumentNodeNames.EntityGuid && name != DocumentNodeNames.EntityLanguage)
                                       .ToList();
            }
        }

        /// <summary>
        /// The amount of enities created in the repository on data import.
        /// </summary>
        public int AmountOfEntitiesCreated
        {
            get
            {
                var existingGuids = GetExistingEntityGuids();
                var createdGuids = GetCreatedEntityGuids();
                return createdGuids.Except(existingGuids).Count();
            }          
        }

        /// <summary>
        /// The amount of enities updated in the repository on data import.
        /// </summary>
        public int AmountOfEntitiesUpdated
        {
           get 
           {
               var existingGuids = GetExistingEntityGuids();
               var createdGuids = GetCreatedEntityGuids();
               return createdGuids.Where(guid => existingGuids.Contains(guid)).Count();
           }
        }

        private List<Guid> GetEntityDeleteGuids()
        {
            var existingGuids = GetExistingEntityGuids();
            var createdGuids = GetCreatedEntityGuids();
            return existingGuids.Except(createdGuids).ToList();
        }
        
        /// <summary>
        /// The amount of enities deleted in the repository on data import.
        /// </summary>
        public int AmountOfEntitiesDeleted
        {
            get 
            {
                if (entityClear.IsNone())
                {
                    return 0;
                }
                return GetEntityDeleteGuids().Count();
            }
        }

        /// <summary>
        /// Get the attribute names in the content type.
        /// </summary>
        public IEnumerable<string> AttributeNamesInContentType
        {
            get { return contentType.GetEntitiesAttributeNames(); }
        }

        /// <summary>
        /// Get the attributes not imported (ignored) from the document to the repository.
        /// </summary>
        public IEnumerable<string> AttributeNamesNotImported
        {
            get 
            {
                var existingAttributes = contentType.GetEntitiesAttributeNames();
                var creatdAttributes = AttributeNamesInDocument;
                return existingAttributes.Except(creatdAttributes);
            }            
        }

        #endregion Deserialize statistics methods

        /// <summary>
        /// Get a debug report about the import as html string.
        /// </summary>
        /// <returns>Report as HTML string</returns>
        public string GetDebugReport()
        {
            var result = "<p>Details:</p>";
            result += "<ul>";
            foreach (var entity in Entities)
            {
                result += "<li><div>Entity: " + entity.EntityGuid + "</div><ul>";
                foreach (var value in entity.Values)
                {
                    result += "<li><div>Attribute: " + value.Key + "</div>";
                    foreach (var content in value.Value)
                    {
                        if (content is ValueImportModel<string>)
                        {
                            result += string.Format("<div>Value: {0}</div>", ((ValueImportModel<string>)content).Value);
                        }
                        else if (content is ValueImportModel<bool?>)
                        {
                            result += string.Format("<div>Value: {0}</div>", ((ValueImportModel<bool?>)content).Value);
                        }
                        else if (content is ValueImportModel<decimal?>)
                        {
                            result += string.Format("<div>Value: {0}</div>", ((ValueImportModel<decimal?>)content).Value);
                        }
                        else if (content is ValueImportModel<DateTime?>)
                        {
                            result += string.Format("<div>Value: {0}</div>", ((ValueImportModel<DateTime?>)content).Value);
                        }
                        else
                        {
                            result += "<div>Value: --</div>";
                        }
                        foreach (var dimension in content.ValueDimensions)
                        {
                            result += string.Format("<div>Language: {0},{1}</div>", dimension.DimensionExternalKey, dimension.ReadOnly);
                        }
                    }
                    result += "</li>";
                }
                result += "</ul></li>";
            }
            result += "</ul>";
            return result;
        }
    }
}