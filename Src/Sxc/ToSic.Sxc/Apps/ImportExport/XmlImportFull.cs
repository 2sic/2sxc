using System;
using System.Linq;
using System.Xml.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.ImportExport;
using ToSic.Eav.Metadata;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Repositories;
using ToSic.Eav.Repository.Efc;

namespace ToSic.Sxc.Apps.ImportExport
{
    public partial class XmlImportFull: XmlImportWithFiles
    {
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly IRepositoryLoader _repositoryLoader;

        public XmlImportFull(Lazy<Import> importerLazy, 
            Lazy<CmsManager> cmsManagerLazy, 
            Lazy<DbDataController> dbDataForNewApp,
            Lazy<DbDataController> dbDataForAppImport,
            IImportExportEnvironment importExportEnvironment, 
            IRepositoryLoader repositoryLoader,
            ITargetTypes metaTargetTypes,
            SystemManager systemManager) : base(importerLazy, dbDataForNewApp, dbDataForAppImport, importExportEnvironment, metaTargetTypes, systemManager, "Sxc.XmlImp")
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
