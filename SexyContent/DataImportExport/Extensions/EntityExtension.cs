using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport.Extensions
{
    public static class EntityExtension
    {
        /// <summary>
        /// Get the value of an attribute in the language specified.
        /// </summary>
        public static EavValue GetAttributeValue(this Entity entity, Attribute attribute, string language)
        {
            var values = entity.Values.Where(value => value.Attribute.StaticName == attribute.StaticName);
            if (string.IsNullOrEmpty(language))
            {
                return values.FirstOrDefault(value => !value.ValuesDimensions.Any());
            }
            else
            {
                return values.FirstOrDefault(value => value.ValuesDimensions.Any(reference => reference.Dimension.ExternalKey == language));
            }
        }

        /// <summary>
        /// Get the value of an attribute in the language specified.
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