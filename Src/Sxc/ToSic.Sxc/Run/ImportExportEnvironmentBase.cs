using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
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

        public class Dependencies
        {
            internal readonly ISite Site;
            internal readonly App NewApp;
            internal readonly TemplateHelpers TemplateHelpers;

            public Dependencies(ISite site, App newApp, TemplateHelpers templateHelpers)
            {
                Site = site;
                NewApp = newApp;
                TemplateHelpers = templateHelpers;
            }
        }

        private readonly Dependencies _dependencies;

        /// <summary>
        /// DI Constructor
        /// </summary>
        // todo: replace IEnvironment with IHttp ?
        protected ImportExportEnvironmentBase(Dependencies dependencies, string logName) : base(logName)
        {
            _dependencies = dependencies;
            Site = dependencies.Site;
        }

        protected readonly ISite Site;

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

        public string DefaultLanguage => Site.DefaultLanguage;

        public string TemplatesRoot(int zoneId, int appId)
        {
            var app = _dependencies.NewApp.InitNoData(new AppIdentity(zoneId, appId), Log);

            // Copy all files in 2sexy folder to (portal file system) 2sexy folder
            var templateRoot = _dependencies.TemplateHelpers.Init(app, Log)
                .AppPathRoot(false, PathTypes.PhysFull);
            return templateRoot;
        }

        public string TargetPath(string folder)
        {
            var appPath = Path.Combine(Site.AppsRootPhysicalFull, folder);
            return appPath;
        }

        public abstract void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap);

        public abstract void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<Message> importLog);
        
        public SaveOptions SaveOptions(int zoneId) 
            => new SaveOptions(DefaultLanguage, new ZoneRuntime().Init(zoneId, Log).Languages(true));
    }
}
