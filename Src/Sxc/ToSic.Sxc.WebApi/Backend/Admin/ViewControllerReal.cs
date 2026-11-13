using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.Persistence.Sys.Logging;
using ToSic.Sxc.Backend.Views;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ViewControllerReal(
    LazySvc<IContextOfSiteService> ctxSvc,
    AppWorkQuick<WorkViewDelete> workViewDelete,
    LazySvc<WorkViewsGetDetailed> viewsBackend,
    LazySvc<ViewsExportImport> viewExportImport)
    : ServiceBase("Api.ViewRl", connect: [ctxSvc, workViewDelete, viewsBackend, viewExportImport]),
        IViewController
{
    public const string LogSuffix = "View";

    /// <inheritdoc />
    public bool Delete(int appId, int id) => workViewDelete.New(appId).DeleteViewWithSiteSwitchCheck(id);

    /// <inheritdoc />
    public THttpResponseType Json(int appId, int viewId) => viewExportImport.Value.DownloadViewAsJson(appId, viewId);

    /// <summary>
    /// This method is not implemented for ControllerReal, because ControllerReal implements Import(HttpUploadedFile uploadInfo, int zoneId, int appId)
    /// </summary>
    /// <param name="zoneId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ImportResult Import(int zoneId, int appId) => throw new NotImplementedException();

    /// <summary>
    /// This implementation is special ControllerReal, instead of ImportResultDto Import(int zoneId, int appId) that is not implemented.
    /// </summary>
    /// <param name="uploadInfo"></param>
    /// <param name="zoneId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ImportResult Import(HttpUploadedFile uploadInfo, int zoneId, int appId)
    {
        var l = Log.Fn<ImportResult>();
            
        if (!uploadInfo.HasFiles())
            return l.Return(new(false, "no file uploaded", Message.MessageTypes.Error), "no file uploaded");

        var streams = new List<FileUploadDto>();
        for (var i = 0; i < uploadInfo.Count; i++)
        {
            var (fileName, stream) = uploadInfo.GetStream(i);
            streams.Add(new() { Name = fileName, Stream = stream! });
        }
        var result = viewExportImport.Value.ImportView(zoneId, appId, streams, ctxSvc.Value.Context.Site.DefaultCultureCode);

        return l.ReturnAsOk(result);
    }

    /// <summary>
    /// Helper method to get SiteId for ControllerReal proxy class.
    /// </summary>
    public int SiteId => ctxSvc.Value.Context.Site.Id;
}
