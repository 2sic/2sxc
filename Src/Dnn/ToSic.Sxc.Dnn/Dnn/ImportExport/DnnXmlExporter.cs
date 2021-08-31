using System;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Context;
using ToSic.Eav.ImportExport.Environment;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Xml;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Dnn.ImportExport
{
    public class DnnXmlExporter: XmlExporter
    {
        #region Constructor / DI

        public DnnXmlExporter(IServiceProvider serviceProvider, ISite site, AdamManager<int, int> adamManager, IContextResolver ctxResolver, XmlSerializer xmlSerializer, IAppStates appStates)
            : base(xmlSerializer, appStates, DnnConstants.LogName)
        {
            _serviceProvider = serviceProvider;
            _site = site;
            _ctxResolver = ctxResolver.Init(Log);
            AdamManager = adamManager;
        }
        private readonly IServiceProvider _serviceProvider;
        private readonly ISite _site;
        private readonly IContextResolver _ctxResolver;


        private readonly IFileManager _dnnFiles = FileManager.Instance;
        internal AdamManager<int, int> AdamManager { get; }

        public override XmlExporter Init(int zoneId, int appId, AppRuntime appRuntime, bool appExport, string[] attrSetIds, string[] entityIds, ILog parentLog)
        {
            var context = _ctxResolver.App(appId);
            //var tenant = new DnnSite();
            var app = _serviceProvider.Build<App>().InitNoData(new AppIdentity(zoneId, appId), Log);
            AdamManager.Init(context, 10, Log);
            Constructor(zoneId, appRuntime, app.AppGuid, appExport, attrSetIds, entityIds, parentLog);

            // this must happen very early, to ensure that the file-lists etc. are correct for exporting when used externally
            InitExportXDocument(_site.DefaultCultureCode/* tenant.DefaultCultureCode*/, Settings.ModuleVersion);

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
                    var file = _dnnFiles.GetFile(fileNum);
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
            var folderController = FolderManager.Instance;
            var folder = folderController.GetFolder(folderId);
            return folder?.FolderPath;
        }

        protected override TenantFileItem ResolveFile(int fileId)
        {
            var fileController = FileManager.Instance;
            var file = fileController.GetFile(fileId);
            return new TenantFileItem
            {
                Id = fileId,
                RelativePath = file?.RelativePath.Replace('/', '\\'),
                Path = file?.PhysicalPath
            };
        }

    }
}