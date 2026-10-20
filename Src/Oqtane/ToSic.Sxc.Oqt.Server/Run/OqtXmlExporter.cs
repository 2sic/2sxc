using Microsoft.AspNetCore.Hosting;
using Oqtane.Repository;
using ToSic.Eav.Context;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.XmlExport;
using ToSic.Sxc.Adam.Sys;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Server.Run;

//internal class OqtXmlExporter(
//    AdamManager adamManager,
//    IContextOfAppFactory appCtxFactory,
//    XmlSerializer xmlSerializer,
//    IWebHostEnvironment hostingEnvironment,
//    LazySvc<IFileRepository> fileRepositoryLazy,
//    LazySvc<IFolderRepository> folderRepositoryLazy,
//    LazySvc<ITenantResolver> oqtTenantResolverLazy,
//    IAppsCatalog appsCatalog,
//    LazySvc<OqtAssetsFileHelper> fileHelper)
//    : SxcXmlExporter(xmlSerializer, appsCatalog, appCtxFactory, OqtConstants.OqtLogPrefix,
//        connect:
//        [
//            hostingEnvironment, fileRepositoryLazy, folderRepositoryLazy, oqtTenantResolverLazy, fileHelper, adamManager
//        ])
//{
    //#region Constructor / DI

    //protected override void PostContextInit(IContextOfApp appContext)
    //{
    //    adamManager.Init(appContext, CompatibilityLevels.CompatibilityLevel10);
    //}


    //#endregion

    //public override void AddFilesToExportQueue()
    //{
    //    // Add Adam Files To Export Queue
    //    var exportList = new AdamExportListHelper<int, int>(adamManager);
    //    var adamIds = exportList.AppFiles;
    //    adamIds.ForEach(AddFileAndFolderToQueue);

    //    // also add folders in adam - because empty folders may also have metadata assigned
    //    var adamFolders = exportList.AppFolders;
    //    adamFolders.ForEach(ReferencedFolderIds.Add);
    //}

    //protected override void AddFileAndFolderToQueue(int fileNum)
    //{
    //    try
    //    {
    //        ReferencedFileIds.Add(fileNum);

    //        // also try to remember the folder
    //        try
    //        {
    //            var file = fileRepositoryLazy.Value.GetFile(fileNum, false);
    //            if (file != null) ReferencedFolderIds.Add(file.FolderId);
    //        }
    //        catch
    //        {
    //            // don't do anything, because if the file doesn't exist, its FOLDER should also not land in the queue
    //        }
    //    }
    //    catch
    //    {
    //        // don't do anything, because if the file doesn't exist, it should also not land in the queue
    //    }
    //}

    //protected override string ResolveFolderId(int folderId)
    //{
    //    var folderController = folderRepositoryLazy.Value;
    //    var folder = folderController.GetFolder(folderId);
    //    return folder?.Path;
    //}

    //protected override TenantFileItem ResolveFile(int fileId)
    //{
    //    var fileController = fileRepositoryLazy.Value;
    //    var file = fileController.GetFile(fileId);
    //    if (file == null) return new()
    //    {
    //        Id = fileId,
    //        RelativePath = null,
    //        Path = null
    //    };

    //    var relativePath = Path.Combine(file?.Folder.Path, file?.Name);
    //    var alias = oqtTenantResolverLazy.Value.GetAlias();
    //    var path = fileHelper.Value.GetFilePath(hostingEnvironment.ContentRootPath, alias, relativePath);

    //    return new()
    //    {
    //        Id = fileId,
    //        RelativePath = relativePath,
    //        Path = path
    //    };
    //}

//}

public class OqtXmlExporterIntegration(
    AdamManager adamManager,
    LazySvc<IFileRepository> fileRepositoryLazy,
    LazySvc<IFolderRepository> folderRepositoryLazy,
    LazySvc<OqtAssetsFileHelper> fileHelper,
    IWebHostEnvironment hostingEnvironment,
    LazySvc<ITenantResolver> oqtTenantResolverLazy
) : IXmlExporterIntegration
{
    /// <summary>
    /// Post context init the caller must be able to init Adam, which is not part of this project, so we're handling it as a callback
    /// </summary>
    /// <param name="appContext"></param>
    public void Setup(IContextOfApp appContext)
    {
        adamManager.Init(appCtx: appContext, compatibility: CompatibilityLevels.CompatibilityLevel10);
    }

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
                var file = fileRepositoryLazy.Value.GetFile(fileNum, false);
                if (file != null) ReferencedFolderIds.Add(file.FolderId);
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
        var folderController = folderRepositoryLazy.Value;
        var folder = folderController.GetFolder(folderId);
        return folder?.Path;
    }

    public TenantFileItem ResolveFile(int fileId)
    {
        var fileController = fileRepositoryLazy.Value;
        var file = fileController.GetFile(fileId);
        if (file == null) return new()
        {
            Id = fileId,
            RelativePath = null,
            Path = null
        };

        var relativePath = Path.Combine(file?.Folder.Path, file?.Name);
        var alias = oqtTenantResolverLazy.Value.GetAlias();
        var path = fileHelper.Value.GetFilePath(hostingEnvironment.ContentRootPath, alias, relativePath);

        return new()
        {
            Id = fileId,
            RelativePath = relativePath,
            Path = path
        };
    }
}