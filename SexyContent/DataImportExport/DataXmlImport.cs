using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ToSic.Eav.Import;
using ToSic.SexyContent.DataImportExport.Extensions;
using AttributeSet = ToSic.Eav.AttributeSet;

namespace ToSic.SexyContent.DataImportExport
{
    public class DataXmlImport
    {
        private int? applicationId;

        private SexyContent contentManager;

        private AttributeSet contentType;

        private IEnumerable<XElement> documentElements;

        private ResourceReferenceImportOption resourceReferenceOption;

        private EntityClearImportOption entityClearOption;

        public List<Entity> Entities
        {
            get { return entities; }
        }
        private List<Entity> entities = new List<Entity>();

        /// <summary>
        /// Errors occured while deserializing the xml file.
        /// </summary>
        public DataImportErrorProtocol ErrorProtocol
        {
            get { return errorProtocol; }
        }
        private DataImportErrorProtocol errorProtocol = new DataImportErrorProtocol();

        public bool HasErrors
        {
            get { return errorProtocol.Count() > 0; }
        }

        private Entity GetEntity(Guid entityGuid)
        {
            return entities.FirstOrDefault(entity => entity.EntityGuid == entityGuid);
        }

        private Entity AppendEntity(Guid entityGuid)
        {
            var entity = new Entity()
            {
                AttributeSetStaticName = contentType.StaticName,
                AssignmentObjectTypeId = contentManager.DefaultAssignmentObjectTypeID,
                EntityGuid = entityGuid,
                KeyNumber = null,
                Values = new Dictionary<string, List<IValueImportModel>>()
            };
            entities.Add(entity);
            return entity;
        }



        public DataXmlImport(int? applicationId, int contentTypeId, EntityClearImportOption entityClearOption, ResourceReferenceImportOption resourceReferenceOption)
        {
            this.applicationId = applicationId;
            this.contentManager = new SexyContent(true, new int?(), applicationId);
            this.contentType = contentManager.ContentContext.GetAttributeSet(contentTypeId);
            this.entityClearOption = entityClearOption;
            this.resourceReferenceOption = resourceReferenceOption;
        }

        /// <summary>
        /// Deserialize a 2SexyContent data XML stream to the memory. By the way the data will be checked for 
        /// errors.
        /// </summary>
        public void Deserialize(IEnumerable<string> languages, string documentLanguageFallback, Stream dataStream)
        {
            if (languages == null || languages.Count() == 0)
            {
                languages = new string[] { documentLanguageFallback };
            }

            if (contentType == null)
            {
                errorProtocol.AppendError(DataImportErrorCode.InvalidContentType);
                return;
            }

            var document = XDocument.Load(dataStream);
            dataStream.Position = 0;
            if (document == null)
            {
                errorProtocol.AppendError(DataImportErrorCode.InvalidDocument);
                return;
            }

            var documentRoot = document.Element(XElementName.Root + contentType.Name.RemoveSpecialCharacters());
            if (documentRoot == null)
            {
                errorProtocol.AppendError(DataImportErrorCode.InvalidRoot);
                return;
            }
 
            var entityGuidManager = new EntityGuidManager();

            var documentElementNumber = 0;
            documentElements = documentRoot.Elements(XElementName.Entity);
            foreach (var documentElement in documentElements)
            {
                documentElementNumber++;

                var documentElementLanguage = documentElement.GetChildElementValue(XElementName.EntityLanguage);
                if (string.IsNullOrEmpty(documentElementLanguage))
                {   // Assume the fallback language
                    documentElementLanguage = documentLanguageFallback;
                }
                if (!languages.Any(language => language == documentElementLanguage))
                {   // DNN does not support the language
                    errorProtocol.AppendError(DataImportErrorCode.InvalidLanguage, "Lang=" + documentElementLanguage, documentElementNumber);
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
                            entity.AppendAttributeValue(valueName, value, valueType, documentElementLanguage, false, resourceReferenceOption.IsResolve());
                        }
                        catch (FormatException)
                        {
                            errorProtocol.AppendError(DataImportErrorCode.InvalidValueFormat, valueName + ":" + valueType + "=" + value, documentElementNumber);
                        }
                        continue;
                    }

                    var valueReadOnly = value.GetValueReadOnly();

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
                        errorProtocol.AppendError(DataImportErrorCode.InvalidValueReference, value, documentElementNumber);
                        continue;
                    }

                    var dbEntityValue = dbEntity.GetAttributeValue(attribute, valueReferenceLanguage);
                    if(dbEntityValue == null)
                    {
                        errorProtocol.AppendError(DataImportErrorCode.InvalidValueReference, value, documentElementNumber);
                        continue;
                    }

                    entity.AppendAttributeValue(valueName, dbEntityValue.Value, valueType, valueReferenceLanguage, dbEntityValue.IsLanguageReadOnly(valueReferenceLanguage), resourceReferenceOption.IsResolve())
                          .AppendLanguageReference(documentElementLanguage, valueReadOnly);       
                }
            }
        }

        /// <summary>
        /// Save the data in memory to the EAV data base.
        /// </summary>
        public bool Pesrist(int? zoneId, string userId)
        {
            if (HasErrors)
            {
                return false;
            }

            if (entityClearOption.IsAll())
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
            import.RunImport(null, entities, true, true);
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
            var newGuids = entities.Select(entity => entity.EntityGuid.Value).ToList();
            return newGuids;
        }

        public int GetDocumentElementCount()
        {
            return documentElements.Count();
        }

        public int GetLanguageCount()
        {
            return documentElements.Select(element => element.Element(XElementName.EntityLanguage).Value).Distinct().Count();
        }

        public int GetEntitiesCreateCount()
        {
            var existingGuids = GetExistingEntityGuids();
            var createdGuids = GetCreatedEntityGuids();
            return createdGuids.Except(existingGuids).Count();
        }

        public int GetEntitiesUpdateCount()
        {
            var existingGuids = GetExistingEntityGuids();
            var createdGuids = GetCreatedEntityGuids();
            return createdGuids.Where(guid => existingGuids.Contains(guid)).Count();
        }

        private List<Guid> GetEntityDeleteGuids()
        {
            var existingGuids = GetExistingEntityGuids();
            var createdGuids = GetCreatedEntityGuids();
            return existingGuids.Except(createdGuids).ToList();
        }

        public int GetEntitiesDeleteCount()
        {
            if (entityClearOption.IsNone())
            {
                return 0;
            }
            return GetEntityDeleteGuids().Count();
        }

        public int GetAttributeCount()
        {
            return contentType.GetAttributeNames().Count();
        }

        public IEnumerable<string> GetAttributeNames()
        {
            return contentType.GetAttributeNames();
        }

        public string GetAttributeNames(string nameSeparator)
        {
            return string.Join(nameSeparator, GetAttributeNames());
        }

        public int GetAttributeIgnoreCount()
        {
            return GetAttributeIgnoredNames().Count();
        }

        public IEnumerable<string> GetAttributeIgnoredNames()
        {
            var existingAttributes = contentType.GetAttributeNames();
            var creatdAttributes = documentElements.SelectMany(element => element.Elements())
                                                   .GroupBy(attribute => attribute.Name.LocalName)
                                                   .Select(group => group.Key)
                                                   .Where(name => name != XElementName.EntityGuid && name != XElementName.EntityLanguage);
            return creatdAttributes.Except(existingAttributes);
        }

        public string GetAttributeIgnoredNames(string nameSeparator)
        {
            return string.Join(nameSeparator, GetAttributeIgnoredNames());
        }
        #endregion Deserialize statistics methods


        public string GetEntitiesDebugString()
        {
            var result = string.Empty;
            foreach (var entity in entities)
            {
                result += "<br><br><b>Entity: " + entity.EntityGuid + "</b>";
                foreach (var value in entity.Values)
                {
                    result += "<br><b>Attribute: " + value.Key + "</b>";
                    foreach (var content in value.Value)
                    {
                        var contentKnown = content as ValueImportModel<string>;
                        if (contentKnown != null)
                        {
                            result += "<br>Value: " + contentKnown.Value;
                        }
                        else
                        {
                            result += "<br>Value: --";
                        }
                        foreach (var dimension in content.ValueDimensions)
                        {
                            result += "<br>Lang: " + dimension.DimensionExternalKey + ",ro=" + dimension.ReadOnly;
                        }
                    }
                }
                result += "<br>";
            }
            return result;
        }
    }
}