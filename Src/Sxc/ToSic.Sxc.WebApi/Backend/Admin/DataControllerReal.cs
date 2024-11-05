using System.IO;
using ToSic.Eav;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Internal.Loaders;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Files;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Sxc.Backend.ImportExport;
using ServiceBase = ToSic.Lib.Services.ServiceBase;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DataControllerReal(
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    AppWorkContextService appWorkCtxSvc,
    LazySvc<IContextOfSite> context,
    LazySvc<ContentExportApi> contentExportLazy,
    Generator<ImportContent> importContent,
    LazySvc<IUser> userLazy)
    : ServiceBase("Api.DtaCtlRl",
        connect: [site, appPathSvc, appWorkCtxSvc, context, contentExportLazy, importContent, userLazy])/*, IAdminDataController*/
{
    public const string LogSuffix = "DataCtrl";

    public THttpResponseType BundleExport(int appId, Guid exportConfiguration, int indentation)
        => contentExportLazy.Value.Init(appId).JsonBundleExport(userLazy.Value, exportConfiguration, indentation);

    public ImportResultDto BundleImport(HttpUploadedFile uploadInfo, int zoneId, int appId)
    {
        var l = Log.Fn<ImportResultDto>();

        SecurityHelpers.ThrowIfNotSiteAdmin(userLazy.Value, l);

        if (!uploadInfo.HasFiles())
            return l.Return(new(false, "no file uploaded", Message.MessageTypes.Error), "no file uploaded");

        var streams = new List<FileUploadDto>();
        for (var i = 0; i < uploadInfo.Count; i++)
        {
            var (fileName, stream) = uploadInfo.GetStream(i);
            streams.Add(new() { Name = fileName, Stream = stream });
        }
        var result = importContent.New()
            .ImportJsonFiles(zoneId, appId, streams, context.Value.Site.DefaultCultureCode);

        return l.ReturnAsOk(result);
    }

    public bool BundleSave(int appId, Guid exportConfiguration, int indentation = 0)
        => contentExportLazy.Value.Init(appId).BundleSave(userLazy.Value, exportConfiguration, indentation);

    public bool BundleRestore(string fileName, int zoneId, int appId)
    {
        var l = Log.Fn<bool>();

        SecurityHelpers.ThrowIfNotSiteAdmin(userLazy.Value, l);

        var fileNameSafe = FileNames.SanitizeFileName(fileName);
        if (fileName != fileNameSafe) l.A($"File name sanitized:'{fileName}' => '{fileNameSafe}'");

        var appPaths = appPathSvc.Get(appWorkCtxSvc.Context(appId).AppReader, site);
        var filePath = Path.Combine(appPaths.PhysicalPath, Constants.AppDataProtectedFolder, FsDataConstants.BundlesFolder, fileNameSafe);

        if (!File.Exists(filePath))
            return l.ReturnFalse($"File not found: {filePath}");

        var result = importContent.New()
            .ImportJsonFiles(zoneId, appId, [new() { Name = fileName, Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read) }], context.Value.Site.DefaultCultureCode);

        var message = string.Join(", ", result.Messages.Select(m => m.Text));

        return l.Return(result.Success, message);
    }
}