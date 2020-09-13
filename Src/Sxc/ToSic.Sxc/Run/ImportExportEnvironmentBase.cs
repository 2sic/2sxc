using System;
using System.Collections.Generic;
using System.IO;
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
        protected ImportExportEnvironmentBase(IEnvironment environment, ITenant tenant, string logName) : base(logName)
        {
            Environment = environment;
            Tenant = tenant;
        }

        protected readonly IEnvironment Environment;
        protected readonly ITenant Tenant;

        public IImportExportEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }
        #endregion

        public List<Message> Messages { get; } = new List<Message>();

        public abstract void TransferFilesToTenant(string sourceFolder, string destinationFolder);
        public abstract Version TenantVersion { get; }
        public string ModuleVersion => Settings.ModuleVersion;

        public string FallbackContentTypeScope => Settings.AttributeSetScope;

        public string DefaultLanguage => Tenant.DefaultLanguage;
        public string TemplatesRoot(int zoneId, int appId)
        {
            var app = Eav.Factory.Resolve<App>().InitNoData(new AppIdentity(zoneId, appId), Log);

            // Copy all files in 2sexy folder to (portal file system) 2sexy folder
            var templateRoot = Environment.MapPath(TemplateHelpers.GetTemplatePathRoot(Settings.TemplateLocations.PortalFileSystem, app));
            return templateRoot;
        }
        public string TargetPath(string folder)
        {
            var appPath = Path.Combine(Tenant.AppsRoot, folder);
            return Environment.MapPath(appPath);
        }
        public abstract void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap);
        public abstract void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<Message> importLog);
        
        public SaveOptions SaveOptions(int zoneId) 
            => new SaveOptions(DefaultLanguage, new ZoneRuntime(zoneId, Log).Languages(true));
    }
}
