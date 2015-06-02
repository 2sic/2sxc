using System;
using System.Xml.Linq;
using ToSic.Eav.ImportExport.Refactoring.Extensions;

namespace ToSic.Eav.ImportExport.Refactoring
{
    internal class EntityGuidManager
    {
        private Guid entityGuidLast = Guid.Empty;

        /// <summary>
        /// Get the entity GUID for a document element of the XML file (maybe the last GUID or the next 
        /// one... depends on some rules).
        /// </summary>
        public Guid GetGuid(XElement element, string languageFallback)
        {
            Guid entityGuid;

            var elementGuid = element.GetChildElementValue(DocumentNodeNames.EntityGuid);
            if (string.IsNullOrEmpty(elementGuid))
            {
                var elementLanguage = element.GetChildElementValue(DocumentNodeNames.EntityLanguage);
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