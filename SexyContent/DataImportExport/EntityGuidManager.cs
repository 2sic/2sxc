using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ToSic.SexyContent.DataImportExport
{
    public class EntityGuidManager
    {
        private Guid entityGuidLast = Guid.Empty;


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