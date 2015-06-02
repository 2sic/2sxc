using System;
using Attribute = ToSic.Eav.Attribute;

namespace ToSic.Eav.ImportExport.Refactoring.Extensions
{
    internal static class AttributeExtension
    {
        /// <summary>
        /// Get the type of an attribute as Type.
        /// </summary>
        public static Type GetType(this Attribute attribute)
        {
            return Type.GetType(attribute.AttributeType.Type);
        }
    }
}