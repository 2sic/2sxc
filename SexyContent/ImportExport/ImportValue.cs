using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using ToSic.Eav;

namespace ToSic.SexyContent.ImportExport
{
    public class ImportValue
    {
        public XElement XmlValue;
        public List<ToSic.Eav.Import.ValueDimension> Dimensions;

        public string GetValue()
        {
            return XmlValue.Attribute("Value").Value;
        }
    }
}