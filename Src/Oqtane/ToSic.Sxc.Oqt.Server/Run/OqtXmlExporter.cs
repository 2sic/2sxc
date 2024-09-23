using System.IO;
using Microsoft.AspNetCore.Hosting;
using Oqtane.Repository;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Eav.Helpers;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.ImportExport.Internal.Xml;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtXmlExporter(
    AdamManager<int, int> adamManager,
    ISxcContextResolver ctxResolver,
    XmlSerializer xmlSerializer,
    IWebHostEnvironment hostingEnvironment,
    LazySvc<IFileRepository> fileRepositoryLazy,
    LazySvc<IFolderRepository> folderRepositoryLazy,
    LazySvc<ITenantResolver> oqtTenantResolverLazy,
    IAppsCatalog appsCatalog,
    LazySvc<OqtAssetsFileHelper> fileHelper)
    : XmlExporter(xmlSerializer, appsCatalog, ctxResolver, OqtConstants.OqtLogPrefix,
        connect:
        [
            hostingEnvironment, fileRepositoryLazy, folderRepositoryLazy, oqtTenantResolverLazy, fileHelper, adamManager
        ])
{
    #region Constructor / DI

    internal AdamManager<int, int> AdamManager { get; } = adamManager;

    protected override void PostContextInit(IContextOfApp appContext)
    {
        AdamManager.Init(appContext, cdf: null, CompatibilityLevels.CompatibilityLevel10);
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

    protected override string ResolveFolderId(int folderId)
    {
        var folderController = folderRepositoryLazy.Value;
        var folder = folderController.GetFolder(folderId);
        return folder?.Path;
    }

    protected override TenantFileItem ResolveFile(int fileId)
    {
        var fileController = fileRepositoryLazy.Value;
        var file = fileController.GetFile(fileId);
        if (file == null) return new()
        {
            Id = fileId,
            RelativePath = null,
            Path = null
        };

        var relativePath = Path.Combine(file?.Folder.Path.Backslash(), file?.Name);
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