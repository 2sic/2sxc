using ToSic.Eav.Persistence.Logging;
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
    LazySvc<IContextOfSite> context,
    LazySvc<ContentExportApi> contentExportLazy,
    Generator<ImportContent> importContent,
    LazySvc<IUser> userLazy)
    : ServiceBase("Api.DtaCtlRl",
        connect: [context, contentExportLazy, importContent, userLazy])/*, IAdminDataController*/
{
    public const string LogSuffix = "DataCtrl";

    public THttpResponseType BundleExport(int appId, Guid exportConfiguration, int indentation)
        => contentExportLazy.Value.Init(appId).JsonBundleExport(userLazy.Value, exportConfiguration, indentation);

    public ImportResultDto BundleImport(HttpUploadedFile uploadInfo, int zoneId, int appId)
    {
        var l = Log.Fn<ImportResultDto>();

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
    {
        // TODO: @STV implement BundleSave
        return false;
        // throw new NotImplementedException();
    }

    public bool BundleRestore(int zoneId, int appId)
    {
        // TODO: @STV implement BundleRestore
        return false;
        // throw new NotImplementedException();
    }
}