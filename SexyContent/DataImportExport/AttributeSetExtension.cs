using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport
{
    public static class AttributeSetExtension
    {
        public static IEnumerable<string> GetAttributeNames(this AttributeSet attributeSet)
        {
            return attributeSet.AttributesInSets.Select(item => item.Attribute.StaticName).ToList();
        }

        public static IEnumerable<Attribute> GetAttributes(this AttributeSet attributeSet)
        {
            return attributeSet.AttributesInSets.Select(item => item.Attribute).ToList();
        }
    }
}