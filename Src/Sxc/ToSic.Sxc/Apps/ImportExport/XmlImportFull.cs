using System;
using System.Linq;
using System.Xml.Linq;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.ImportExport;
using ToSic.Lib.Logging;
using ToSic.Eav.Repositories;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Apps.ImportExport
{
    public partial class XmlImportFull: XmlImportWithFiles
    {
        private readonly LazyInit<CmsManager> _cmsManagerLazy;
        private readonly IRepositoryLoader _repositoryLoader;

        public XmlImportFull(
            Dependencies dependencies,
            LazyInit<CmsManager> cmsManagerLazy,
            IRepositoryLoader repositoryLoader
            ) : base(dependencies, "Sxc.XmlImp")
        {
            ConnectServices(
                _cmsManagerLazy = cmsManagerLazy,
                _repositoryLoader = repositoryLoader
            );
        }

        public new bool ImportXml(int zoneId, int appId, XDocument doc, bool leaveExistingValuesUntouched = true)
        {
            var wrapLog = Log.Fn<bool>(parameters: $"{zoneId}, {appId}, ..., {leaveExistingValuesUntouched}");
            var ok = base.ImportXml(zoneId, appId, doc, leaveExistingValuesUntouched);
            if (!ok)
                return wrapLog.ReturnFalse("error");

            Log.A("Now import templates - if found");

            var xmlSource = doc.Element(XmlConstants.RootNode) 
                ?? throw new Exception("error import - xmlSource should always exist");

            if (xmlSource.Elements(XmlConstants.Templates).Any())
            {
                Log.A("found some templates");
                ImportXmlTemplates(xmlSource);
            }
            else
                Log.A("No templates found");

            return wrapLog.ReturnTrue("ok");
        }

    }
}
