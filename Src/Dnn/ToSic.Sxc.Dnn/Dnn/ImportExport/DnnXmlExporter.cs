using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.ImportExport.Environment;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Xml;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Dnn.Run;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Dnn.ImportExport
{
    public class DnnXmlExporter: XmlExporter
    {
        #region Constructor / DI

        public DnnXmlExporter(AdamAppContext<int, int> adamAppContext, XmlSerializer xmlSerializer): base(xmlSerializer, DnnConstants.LogName)
        {
            AdamAppContext = adamAppContext;
        }

        private readonly IFileManager _dnnFiles = FileManager.Instance;
        internal AdamAppContext<int, int> AdamAppContext { get; }

        public override XmlExporter Init(int zoneId, int appId, AppRuntime appRuntime, bool appExport, string[] attrSetIds, string[] entityIds, ILog parentLog)
        {
            var tenant = new DnnSite(PortalSettings.Current);
            var app = AdamAppContext.AppRuntime.ServiceProvider.Build<App>().InitNoData(new AppIdentity(zoneId, appId), Log);
            AdamAppContext.Init(tenant, app, null, 10, Log);
            Constructor(zoneId, appRuntime, app.AppGuid, appExport, attrSetIds, entityIds, parentLog);

            // this must happen very early, to ensure that the file-lists etc. are correct for exporting when used externally
            InitExportXDocument(tenant.DefaultLanguage, Settings.ModuleVersion);

            return this;
        }

        #endregion

        public override void AddFilesToExportQueue()
        {
            // Add Adam Files To Export Queue
            var adamIds = AdamAppContext.Export.AppFiles;
            adamIds.ForEach(AddFileAndFolderToQueue);

            // also add folders in adam - because empty folders may also have metadata assigned
            var adamFolders = AdamAppContext.Export.AppFolders;
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