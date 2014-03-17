using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport.Extensions
{
    public static class EntityExtension
    {
        public static EavValue GetAttributeValue(this Entity entity, Attribute attribute, string language)
        {
            return entity.Values.Where(value => value.Attribute.StaticName == attribute.StaticName).FirstOrDefault(value => value.ValuesDimensions.Any(reference => reference.Dimension.ExternalKey == language));
        }

        /// <summary>
        /// Get the value of the attribute specified, in the language specified. If no value can be found in 
        /// the specified language, the value of the default language or null will be returned.
        /// </summary>
        public static EavValue GetAttributeValue(this Entity entity, Attribute attribute, string language, string languageFallback)
        {
            var attributeValue = entity.GetAttributeValue(attribute, language);
            if (attributeValue == null)
            {
                attributeValue = entity.GetAttributeValue(attribute, languageFallback);
            }
            return attributeValue;
        }
    }
}