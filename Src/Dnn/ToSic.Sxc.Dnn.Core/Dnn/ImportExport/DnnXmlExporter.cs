using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Context;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.XmlExport;
using ToSic.Sxc.Adam.Sys;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Code.Sys;
using FileManager = DotNetNuke.Services.FileSystem.FileManager;

namespace ToSic.Sxc.Dnn.ImportExport;

public class DnnXmlExporterIntegration(AdamManager adamManager) : IXmlExporterIntegration
{
    /// <summary>
    /// Post context init the caller must be able to init Adam, which is not part of this project, so we're handling it as a callback
    /// </summary>
    /// <param name="appContext"></param>
    public void Setup(IContextOfApp appContext)
    {
        adamManager.Init(appCtx: appContext, compatibility: CompatibilityLevels.CompatibilityLevel10);
    }

    private readonly IFileManager _dnnFiles = FileManager.Instance;
    private readonly IFolderManager _dnnFolders = FolderManager.Instance;

    public List<int> ReferencedFileIds { get; } = [];
    public List<int> ReferencedFolderIds { get; } = [];

    public void AddFilesToExportQueue()
    {
        // Add Adam Files To Export Queue
        var exportList = new AdamExportListHelper<int, int>(adamManager);
        var adamIds = exportList.AppFiles;
        adamIds.ForEach(AddFileAndFolderToQueue);

        // also add folders in adam - because empty folders may also have metadata assigned
        var adamFolders = exportList.AppFolders;
        adamFolders.ForEach(ReferencedFolderIds.Add);

    }

    public void AddFileAndFolderToQueue(int fileNum)
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

    public string ResolveFolderId(int folderId)
    {
        var folder = _dnnFolders.GetFolder(folderId);
        return folder?.FolderPath;
    }

    public TenantFileItem ResolveFile(int fileId)
    {
        var fileController = FileManager.Instance;
        var file = fileController.GetFile(fileId);
        return new()
        {
            Id = fileId,
            RelativePath = file?.RelativePath,
            Path = file?.PhysicalPath
        };
    }
}
