using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Engines;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Run
{
    public abstract class ImportExportEnvironmentBase: HasLog, IImportExportEnvironment
    {
        #region constructor / DI
        /// <summary>
        /// DI Constructor
        /// </summary>
        // todo: replace IEnvironment with IHttp ?
        protected ImportExportEnvironmentBase(ISite site, string logName) : base(logName)
        {
            Tenant = site;
        }

        protected readonly ISite Tenant;

        public IImportExportEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }
        #endregion

        public abstract List<Message> TransferFilesToSite(string sourceFolder, string destinationFolder);

        public abstract Version TenantVersion { get; }

        public string ModuleVersion => Settings.ModuleVersion;

        public string FallbackContentTypeScope => Settings.AttributeSetScope;

        public string DefaultLanguage => Tenant.DefaultLanguage;

        public string TemplatesRoot(int zoneId, int appId)
        {
            var app = Factory.Resolve<App>().InitNoData(new AppIdentity(zoneId, appId), Log);

            // Copy all files in 2sexy folder to (portal file system) 2sexy folder
            var templateRoot = Factory.Resolve<TemplateHelpers>().Init(app, Log)
                .AppPathRoot(Settings.TemplateLocations.PortalFileSystem, PathTypes.PhysFull);
            return templateRoot;
        }

        public string TargetPath(string folder)
        {
            var appPath = Path.Combine(Tenant.AppsRootPhysicalFull, folder);
            return appPath;
        }

        public abstract void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap);

        public abstract void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<Message> importLog);
        
        public SaveOptions SaveOptions(int zoneId) 
            => new SaveOptions(DefaultLanguage, new ZoneRuntime(zoneId, Log).Languages(true));
    }
}
