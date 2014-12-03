using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport.Extensions
{
    internal static class AttributeSetExtension
    {
        public static IEnumerable<string> GetEntitiesAttributeNames(this ToSic.Eav.AttributeSet attributeSet)
        {
            return attributeSet.AttributesInSets.Select(item => item.Attribute.StaticName).ToList();
        }

        public static IEnumerable<ToSic.Eav.Attribute> GetAttributes(this ToSic.Eav.AttributeSet attributeSet)
        {
            return attributeSet.AttributesInSets.Select(item => item.Attribute).ToList();
        }

        public static Entity GetEntity(this AttributeSet attributeSet, Guid entityGuid)
        {
            return attributeSet.Entities.FirstOrDefault(entity => entity.EntityGUID == entityGuid && entity.ChangeLogIDDeleted == null);
        }  
        
        public static bool EntityExists(this AttributeSet attributeSet, Guid entityGuid)
        {
            return attributeSet.GetEntity(entityGuid) != null;
        }

        public static Eav.Attribute GetAttribute(this AttributeSet attributeSet, string attributeName)
        {
            return attributeSet.GetAttributes().FirstOrDefault(attr => attr.StaticName == attributeName);
        }
    }
}