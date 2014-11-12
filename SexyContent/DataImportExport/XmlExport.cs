using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ToSic.Eav;
using ToSic.SexyContent.DataImportExport.Extensions;
using ToSic.SexyContent.DataImportExport.Options;

namespace ToSic.SexyContent.DataImportExport
{
    public class XmlExport
    {
        /// <summary>
        /// Create a blank xml scheme for 2SexyContent data.
        /// </summary>
        /// <param name="zoneId">ID of 2SexyContent zone</param>
        /// <param name="applicationId">ID of 2SexyContent application</param>
        /// <param name="contentTypeId">ID of 2SexyContent type</param>
        /// <returns>A string containing the blank xml scheme</returns>
        public string CreateBlankXml(int zoneId, int applicationId, int contentTypeId)
        {
            var contentType = GetContentType(zoneId, applicationId, contentTypeId);
            if (contentType == null) 
                return null;

            var documentElement = GetDocumentEntityElement("", "");
            var documentRoot = GetDocumentRoot(contentType.Name, documentElement);
            var document = GetDocument(documentRoot);

            var attributes = contentType.GetAttributes();
            foreach (var attribute in attributes)
            {
                  documentElement.Append(attribute.StaticName, "");      
            }
       
            return document.ToString();
        }

        /// <summary>
        /// Serialize 2SexyContent data to an xml string.
        /// </summary>
        /// <param name="zoneId">ID of 2SexyContent zone</param>
        /// <param name="applicationId">ID of 2SexyContent application</param>
        /// <param name="contentTypeId">ID of 2SexyContent type</param>
        /// <param name="languageSelected">Language of the data to be serialized (null for all languages)</param>
        /// <param name="languageFallback">Language fallback of the system</param>
        /// <param name="languageScope">Languages supported of the system</param>
        /// <param name="languageReference">How value references to other languages are handled</param>
        /// <param name="resourceReference">How value references to files and pages are handled</param>
        /// <returns>A string containing the xml data</returns>
        public string CreateXml(int zoneId, int applicationId, int contentTypeId, string languageSelected, string languageFallback, IEnumerable<string> languageScope, LanguageReferenceExport languageReference, ResourceReferenceExport resourceReference)
        {
            var contentType = GetContentType(zoneId, applicationId, contentTypeId);
            if (contentType == null)
                return null;

            var languages = new List<string>();
            if (!string.IsNullOrEmpty(languageSelected))
            {
                languages.Add(languageSelected);
            }
            else if (languageScope.Any())
            {   // Export all languages
                languages.AddRange(languageScope);
            }
            else
            {
                languages.Add(string.Empty);
            }

            var documentRoot = GetDocumentRoot(contentType.Name, null);
            var document = GetDocument(documentRoot);

            var entities = contentType.Entities.Where(entity => entity.ChangeLogIDDeleted == null);
            foreach (var entity in entities)
            {
                foreach (var language in languages)
                {  
                    var documentElement = GetDocumentEntityElement(entity.EntityGUID, language);
                    documentRoot.Add(documentElement);
                    
                    var attributes = contentType.GetAttributes();
                    foreach (var attribute in attributes)
                    {
                        if (languageReference.IsResolve())
                        {
                            documentElement.AppendValueResolved(entity, attribute, language, languageFallback, resourceReference);
                        }
                        else
                        {
                            documentElement.AppendValueReferenced(entity, attribute, language, languageFallback, languageScope, languages.Count() > 1, resourceReference);
                        }
                    }
                }
            }

            return document.ToString();
        }


        private static AttributeSet GetContentType(int zoneId, int applicationId, int contentTypeId)
        {
            var contentContext = new SexyContent(zoneId, applicationId).ContentContext;
            return contentContext.GetAttributeSet(contentTypeId);
        }

        private static XDocument GetDocument(params object[] content)
        {
            return new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), content);
        }

        private static XElement GetDocumentRoot(string contentTypeName, params object[] content)
        {
            return new XElement(DocumentNodeNames.Root + contentTypeName.RemoveSpecialCharacters(), content);
        }

        private static XElement GetDocumentEntityElement(object elementGuid, object elementLanguage)
        {
            return new XElement
                (
                    DocumentNodeNames.Entity, 
                    new XElement(DocumentNodeNames.EntityGuid, elementGuid), 
                    new XElement(DocumentNodeNames.EntityLanguage, elementLanguage)
                );
        }
    }
}