using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.Xml;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Manager.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.ExportImport.Sys;
using ToSic.Sxc.Internal;
using FileManager = DotNetNuke.Services.FileSystem.FileManager;

namespace ToSic.Sxc.Dnn.ImportExport;

internal class DnnXmlExporter(
    AdamManager adamManager,
    ISxcCurrentContextService ctxService,
    XmlSerializer xmlSerializer,
    IAppsCatalog appsCat)
    : SxcXmlExporter(xmlSerializer, appsCat, ctxService, DnnConstants.LogName, connect: [adamManager])
{
    #region Constructor / DI

    private readonly IFileManager _dnnFiles = FileManager.Instance;

    protected override void PostContextInit(IContextOfApp appContext)
    {
        adamManager.Init(appCtx: appContext, compatibility: CompatibilityLevels.CompatibilityLevel10);
    }

    #endregion

    public override void AddFilesToExportQueue()
    {
        // Add Adam Files To Export Queue
        var exportList = new AdamExportListHelper<int, int>(adamManager);
        var adamIds = exportList.AppFiles;
        adamIds.ForEach(AddFileAndFolderToQueue);

        // also add folders in adam - because empty folders may also have metadata assigned
        var adamFolders = exportList.AppFolders;
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
        return new()
        {
            Id = fileId,
            RelativePath = file?.RelativePath.Replace('/', '\\'),
            Path = file?.PhysicalPath
        };
    }

}