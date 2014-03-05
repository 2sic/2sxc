using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport
{
    public static class EntityExtension
    {
        /// <summary>
        /// Get the value of an attribute in the dimension specified. If no value is found, null will be returned.
        /// </summary>
        public static EavValue GetAttributeValue(this Entity entity, Attribute attribute, Dimension dimension)
        {
            return entity.Values.Where(value => value.Attribute.StaticName == attribute.StaticName).FirstOrDefault(value => value.ValuesDimensions.Any(dim => dim.DimensionID == dimension.DimensionID));
        }

        /// <summary>
        /// Get the value of an attribute in the dimension specified, or in the fallback dimension. If no value is 
        /// found, null will be returned.
        /// </summary>
        public static EavValue GetAttributeValue(this Entity entity, Attribute attribute, Dimension dimension, Dimension dimesnionFallback)
        {
            var attributeValue = entity.GetAttributeValue(attribute, dimension);
            if (attributeValue == null)
            {
                attributeValue = entity.GetAttributeValue(attribute, dimesnionFallback);
            }
            return attributeValue;
        }
    }
}