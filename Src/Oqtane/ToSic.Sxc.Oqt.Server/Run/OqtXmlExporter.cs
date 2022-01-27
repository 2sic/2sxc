using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Oqtane.Repository;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Helpers;
using ToSic.Eav.ImportExport.Environment;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Xml;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{

    public class OqtXmlExporter : XmlExporter
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Lazy<IFileRepository> _fileRepositoryLazy;
        private readonly Lazy<IFolderRepository> _folderRepositoryLazy;
        private readonly Lazy<ITenantResolver> _oqtTenantResolverLazy;
        private readonly IContextResolver _ctxResolver;

        #region Constructor / DI

        public OqtXmlExporter(
            AdamManager<int, int> adamManager,
            IContextResolver ctxResolver,
            XmlSerializer xmlSerializer,
            IWebHostEnvironment hostingEnvironment,
            Lazy<IFileRepository> fileRepositoryLazy,
            Lazy<IFolderRepository> folderRepositoryLazy,
            Lazy<ITenantResolver> oqtTenantResolverLazy,
            IAppStates appStates
            ) : base(xmlSerializer, appStates, OqtConstants.OqtLogPrefix)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileRepositoryLazy = fileRepositoryLazy;
            _folderRepositoryLazy = folderRepositoryLazy;
            _oqtTenantResolverLazy = oqtTenantResolverLazy;
            _ctxResolver = ctxResolver.Init(Log);
            AdamManager = adamManager;
        }

        internal AdamManager<int, int> AdamManager { get; }

        public override XmlExporter Init(int zoneId, int appId, AppRuntime appRuntime, bool appExport, string[] attrSetIds, string[] entityIds, ILog parentLog)
        {
            var context = _ctxResolver.App(appId);
            var contextOfSite = _ctxResolver.Site();
            var oqtSite = (OqtSite) contextOfSite.Site;
            var app = AdamManager.AppRuntime.ServiceProvider.Build<Sxc.Apps.App>().InitNoData(new AppIdentity(zoneId, appId), Log);

            AdamManager.Init(context, Constants.CompatibilityLevel10, Log);
            Constructor(zoneId, appRuntime, app.AppGuid, appExport, attrSetIds, entityIds, parentLog);

            // this must happen very early, to ensure that the file-lists etc. are correct for exporting when used externally
            InitExportXDocument(oqtSite.DefaultCultureCode, EavSystemInfo.VersionString);

            return this;
        }

        #endregion

        public override void AddFilesToExportQueue()
        {
            // Add Adam Files To Export Queue
            var adamIds = AdamManager.Export.AppFiles;
            adamIds.ForEach(AddFileAndFolderToQueue);

            // also add folders in adam - because empty folders may also have metadata assigned
            var adamFolders = AdamManager.Export.AppFolders;
            adamFolders.ForEach(ReferencedFolderIds.Add);
        }

        protected override void AddFileAndFolderToQueue(int fileNum)
        {
            try
            {
                ReferencedFileIds.Add(fileNum);

                // also try to remember the folder
                try
                {
                    var file = _fileRepositoryLazy.Value.GetFile(fileNum);
                    ReferencedFolderIds.Add(file.FolderId);
                }
                catch
                {
                    // don't do anything, because if the file doesn't exist, its FOLDER should also not land in the queue
                }
            }
            catch
            {
                // don't do anything, because if the file doesn't exist, it should also not land in the queue
            }
        }

        protected override string ResolveFolderId(int folderId)
        {
            var folderController = _folderRepositoryLazy.Value;
            var folder = folderController.GetFolder(folderId);
            return folder?.Path;
        }

        protected override TenantFileItem ResolveFile(int fileId)
        {
            var fileController = _fileRepositoryLazy.Value;
            var file = fileController.GetFile(fileId);
            var relativePath = Path.Combine(file?.Folder.Path.Backslash(), file.Name);
            var alias = _oqtTenantResolverLazy.Value.GetAlias();
            var path = ContentFileHelper.GetFilePath(_hostingEnvironment.ContentRootPath, alias, relativePath);

            return new TenantFileItem
            {
                Id = fileId,
                RelativePath = relativePath,
                Path = path
            };
        }

    }


}
