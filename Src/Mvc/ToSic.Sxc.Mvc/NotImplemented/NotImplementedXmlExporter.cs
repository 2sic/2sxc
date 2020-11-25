using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.ImportExport.Environment;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Xml;

namespace ToSic.Sxc.Mvc.NotImplemented
{
    public class NotImplementedXmlExporter: XmlExporter
    {
        public NotImplementedXmlExporter(XmlSerializer xmlSerializer) : base(xmlSerializer)
        {
        }

        public override XmlExporter Init(int zoneId, int appId, AppRuntime appRuntime, bool appExport, string[] attrSetIds, string[] entityIds,
            ILog parentLog)
        {
            // do nothing
            return this;
        }

        public override void AddFilesToExportQueue()
        {
            // do nothing
        }

        protected override void AddFileAndFolderToQueue(int fileNum)
        {
            // do nothing
        }

        protected override TenantFileItem ResolveFile(int fileId)
        {
            throw new NotImplementedException();
        }

        protected override string ResolveFolderId(int folderId)
        {
            throw new NotImplementedException();
        }
    }
}
