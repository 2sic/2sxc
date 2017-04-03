using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.ImportExport
{
    public class XmlExporter: EavXmlExporter
    {
        public XmlExporter(int zoneId, int appId, bool appExport, string[] attrSetIds, string[] entityIds):base()
        {
            // do things first

            Constructor(zoneId, appId, appExport, attrSetIds, entityIds);

            // do following things
        }
    }
}