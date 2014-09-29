using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport.Extensions
{
    internal static class AttributeExtension
    {
        /// <summary>
        /// Get the type of an attribute as Type.
        /// </summary>
        public static Type GetType(this ToSic.Eav.Attribute attribute)
        {
            return Type.GetType(attribute.AttributeType.Type);
        }
    }
}