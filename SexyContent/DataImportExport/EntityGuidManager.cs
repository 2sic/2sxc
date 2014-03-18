using System;
using System.Xml.Linq;
using ToSic.SexyContent.DataImportExport.Extensions;

namespace ToSic.SexyContent.DataImportExport
{
    public class EntityGuidManager
    {
        private Guid entityGuidLast = Guid.Empty;

        /// <summary>
        /// Get the entity GUID for a document element of the XML file (maybe the last GUID or the next 
        /// one... depends on some rules).
        /// </summary>
        public Guid GetGuid(XElement element, string languageFallback)
        {
            Guid entityGuid;

            var elementGuid = element.GetChildElementValue(XElementName.EntityGuid);
            if (string.IsNullOrEmpty(elementGuid))
            {
                var elementLanguage = element.GetChildElementValue(XElementName.EntityLanguage);
                if (elementLanguage == languageFallback)
                {   // If the element does not have a GUID and the element has data for the default 
                    // language, create a new GUID
                    entityGuid = Guid.NewGuid();
                }
                else
                {
                    entityGuid = entityGuidLast;
                }
            }
            else
            {
                entityGuid = Guid.Parse(elementGuid);
            }

            entityGuidLast = entityGuid;
            return entityGuid;
        }
    }
}