using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport
{
    public static class AttributeExtension
    {
        public static Type GetType(this Eav.Attribute attribute)
        {
            return Type.GetType(attribute.AttributeType.Type);
        }
    }
}