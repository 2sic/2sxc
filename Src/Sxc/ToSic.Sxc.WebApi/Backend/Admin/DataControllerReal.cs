using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.Bundle;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Persistence.Sys.Logging;
using ToSic.Eav.Security.Files;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Eav.WebApi.Sys.Security;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DataControllerReal(
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    IAppReaderFactory appReaders,
    AppWorkQuick<ExportBundleFile> bundleFile,
    AppWorkQuick<ExportBundleSave> contentExportLazy,
    Generator<ImportContent> importContent,
    LazySvc<IUser> userLazy,
    AppWorkQuick<WorkEntityRecycle> recycle)
    : ServiceBase("Api.DtaCtlRl",
        connect: [site, appPathSvc, appReaders, bundleFile, contentExportLazy, importContent, userLazy, recycle])
{
    public const string LogSuffix = "DataCtrl";

    public FileResult BundleExport(int appId, Guid exportConfiguration, int indentation)
        => bundleFile.New(appId).JsonBundleExport(userLazy.Value, exportConfiguration, indentation);

    public ImportResult BundleImport(HttpUploadedFile uploadInfo, int zoneId, int appId)
    {
        var l = Log.Fn<ImportResult>();

        SecurityHelpers.ThrowIfNotSiteAdmin(userLazy.Value, l);

        if (!uploadInfo.HasFiles())
            return l.Return(new(false, "no file uploaded", Message.MessageTypes.Error), "no file uploaded");

        var streams = new List<FileUploadDto>();
        for (var i = 0; i < uploadInfo.Count; i++)
        {
            var (fileName, stream) = uploadInfo.GetStream(i);
            streams.Add(new() { Name = fileName, Stream = stream! });
        }
        var result = importContent.New()
            .ImportJsonFiles(zoneId, appId, streams, site.DefaultCultureCode);

        return l.ReturnAsOk(result);
    }

    public bool BundleSave(int appId, Guid exportConfiguration, int indentation = 0)
        => contentExportLazy.New(appId).BundleSave(userLazy.Value, exportConfiguration, indentation);

    public bool BundleRestore(string fileName, int zoneId, int appId)
    {
        var l = Log.Fn<bool>();

        SecurityHelpers.ThrowIfNotSiteAdmin(userLazy.Value, l);

        var fileNameSafe = FileNames.SanitizeFileName(fileName);
        if (fileName != fileNameSafe) l.A($"File name sanitized:'{fileName}' => '{fileNameSafe}'");

        var appPaths = appPathSvc.Get(appReaders.Get(appId), site);
        var filePath = Path.Combine(appPaths.PhysicalPath, FolderConstants.DataFolderProtected, AppDataFoldersConstants.BundlesFolder, fileNameSafe);

        if (!File.Exists(filePath))
            return l.ReturnFalse($"File not found: {filePath}");

        var result = importContent.New()
            .ImportJsonFiles(zoneId, appId, [new() { Name = fileName, Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read) }], site.DefaultCultureCode);

        var message = string.Join(", ", result.Messages.Select(m => m.Text));

        return l.Return(result.Success, message);
    }

    public void Recycle(int appId, int transactionId)
    {
        var l = Log.Fn($"appId:{appId}, tx:{transactionId}");

        recycle
            .New(appId)
            .Recycle(transactionId);

        l.Done();
    }
}