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
        private readonly LazySvc<CmsManager> _cmsManagerLazy;
        private readonly IRepositoryLoader _repositoryLoader;

        public XmlImportFull(
            Dependencies services,
            LazySvc<CmsManager> cmsManagerLazy,
            IRepositoryLoader repositoryLoader
            ) : base(services, "Sxc.XmlImp")
        {
            ConnectServices(
                _cmsManagerLazy = cmsManagerLazy,
                _repositoryLoader = repositoryLoader
            );
        }

        // ReSharper disable once UnusedMember.Global
        // The system says it's never used, but it's provided through DI as the base class
        public new bool ImportXml(int zoneId, int appId, XDocument doc, bool leaveExistingValuesUntouched = true
        ) => Log.Func($"{zoneId}, {appId}, ..., {leaveExistingValuesUntouched}", l =>
        {
            var ok = base.ImportXml(zoneId, appId, doc, leaveExistingValuesUntouched);
            if (!ok)
                return (false, "error");

            l.A("Now import templates - if found");

            var xmlSource = doc.Element(XmlConstants.RootNode)
                            ?? throw new Exception("error import - xmlSource should always exist");

            if (xmlSource.Elements(XmlConstants.Templates).Any())
            {
                l.A("found some templates");
                ImportXmlTemplates(xmlSource);
            }
            else
                l.A("No templates found");

            return (true, "ok");
        });

    }
}
