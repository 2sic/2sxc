using System;
using System.Linq;
using System.Xml.Linq;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.ImportExport;
using ToSic.Eav.Repositories;

namespace ToSic.Sxc.Apps.ImportExport
{
    public partial class XmlImportFull: XmlImportWithFiles
    {
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly IRepositoryLoader _repositoryLoader;

        public XmlImportFull(
            Dependencies dependencies,
            Lazy<CmsManager> cmsManagerLazy,
            IRepositoryLoader repositoryLoader
            ) : base(dependencies, "Sxc.XmlImp")
        {
            _cmsManagerLazy = cmsManagerLazy;
            _repositoryLoader = repositoryLoader.Init(Log);
        }

        public new bool ImportXml(int zoneId, int appId, XDocument doc, bool leaveExistingValuesUntouched = true)
        {
            var wrapLog = Log.Call<bool>(parameters: $"{zoneId}, {appId}, ..., {leaveExistingValuesUntouched}");
            var ok = base.ImportXml(zoneId, appId, doc, leaveExistingValuesUntouched);
            if (!ok)
                return wrapLog("error", false);

            Log.Add("Now import templates - if found");

            var xmlSource = doc.Element(XmlConstants.RootNode) 
                ?? throw new Exception("error import - xmlSource should always exist");

            if (xmlSource.Elements(XmlConstants.Templates).Any())
            {
                Log.Add("found some templates");
                ImportXmlTemplates(xmlSource);
            }
            else
                Log.Add("No templates found");

            return wrapLog("ok", true);
        }

    }
}
