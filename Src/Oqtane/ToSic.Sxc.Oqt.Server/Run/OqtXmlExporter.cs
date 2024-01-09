using System.IO;
using Microsoft.AspNetCore.Hosting;
using Oqtane.Repository;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Eav.Helpers;
using ToSic.Eav.ImportExport.Environment;
using ToSic.Eav.Persistence.Xml;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtXmlExporter : XmlExporter
{
    #region Constructor / DI

    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly LazySvc<IFileRepository> _fileRepositoryLazy;
    private readonly LazySvc<IFolderRepository> _folderRepositoryLazy;
    private readonly LazySvc<ITenantResolver> _oqtTenantResolverLazy;
    private readonly LazySvc<OqtAssetsFileHelper> _fileHelper;

    public OqtXmlExporter(
        AdamManager<int, int> adamManager,
        ISxcContextResolver ctxResolver,
        XmlSerializer xmlSerializer,
        IWebHostEnvironment hostingEnvironment,
        LazySvc<IFileRepository> fileRepositoryLazy,
        LazySvc<IFolderRepository> folderRepositoryLazy,
        LazySvc<ITenantResolver> oqtTenantResolverLazy,
        IAppStates appStates,
        LazySvc<OqtAssetsFileHelper> fileHelper
    ) : base(xmlSerializer, appStates, ctxResolver, OqtConstants.OqtLogPrefix)
    {
        ConnectServices(
            _hostingEnvironment = hostingEnvironment,
            _fileRepositoryLazy = fileRepositoryLazy,
            _folderRepositoryLazy = folderRepositoryLazy,
            _oqtTenantResolverLazy = oqtTenantResolverLazy,
            _fileHelper = fileHelper,
            AdamManager = adamManager
        );
    }

    internal AdamManager<int, int> AdamManager { get; }

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
                var file = _fileRepositoryLazy.Value.GetFile(fileNum, false);
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
        var folderController = _folderRepositoryLazy.Value;
        var folder = folderController.GetFolder(folderId);
        return folder?.Path;
    }

    protected override TenantFileItem ResolveFile(int fileId)
    {
        var fileController = _fileRepositoryLazy.Value;
        var file = fileController.GetFile(fileId);
        if (file == null) return new()
        {
            Id = fileId,
            RelativePath = null,
            Path = null
        };

        var relativePath = Path.Combine(file?.Folder.Path.Backslash(), file?.Name);
        var alias = _oqtTenantResolverLazy.Value.GetAlias();
        var path = _fileHelper.Value.GetFilePath(_hostingEnvironment.ContentRootPath, alias, relativePath);

        return new()
        {
            Id = fileId,
            RelativePath = relativePath,
            Path = path
        };
    }

}