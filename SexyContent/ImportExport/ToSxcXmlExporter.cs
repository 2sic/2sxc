using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ImportExport
{
    public class ToSxcXmlExporter: EavXmlExporter
    {
        private readonly IFileManager _dnnFiles = DotNetNuke.Services.FileSystem.FileManager.Instance;
        internal AdamManager AdamManager;

        public ToSxcXmlExporter(int zoneId, int appId, bool appExport, string[] attrSetIds, string[] entityIds):base()
        {
            // do things first

            var app = new App(zoneId, appId, PortalSettings.Current);
            EavAppContext = new EavBridge(app).FullController;
            AdamManager = new AdamManager(PortalSettings.Current.PortalId, app);
            Constructor(app.AppGuid, appExport, attrSetIds, entityIds);

            // do following things
            // this must happen very early, to ensure that the file-lists etc. are correct for exporting when used externally
            InitExportXDocument(PortalSettings.Current.DefaultLanguage);

        }

        public override void AddFilesToExportQueue()
        {
            // Add Adam Files To Export Queue
            var adamIds = AdamManager.Export.AppFiles;
            adamIds.ForEach(AddFileAndFolderToQueue);

            // also add folders in adam - because empty folders may also have metadata assigned
            var adamFolders = AdamManager.Export.AppFolders;
            adamFolders.ForEach(ReferencedFolderIds.Add);
        }

        internal override void AddFileAndFolderToQueue(int fileNum)
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

        internal override string ResolveFolderId(int folderId)
        {
            var folderController = FolderManager.Instance;
            var folder = folderController.GetFolder(folderId);
            return folder?.FolderPath;
        }

        internal override ImpExpFileItem ResolveFile(int fileId)
        {
            var fileController = DotNetNuke.Services.FileSystem.FileManager.Instance;
            var file = fileController.GetFile(fileId);
            return new ImpExpFileItem
            {
                Id = fileId,
                RelativePath = file?.RelativePath.Replace('/', '\\'),
                Path = file?.PhysicalPath
            };
        }

    }
}