using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Sxc.Backend.ImportExport;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppPartsControllerReal(
    LazySvc<IContextOfSiteService> ctxSvc,
    LazySvc<ExportContent> exportContent,
    Generator<ImportContent> importContent)
    : ServiceBase("Api.APartsRl", connect: [ctxSvc, exportContent, importContent])
{
    public const string LogSuffix = "AParts";


    #region Parts Export/Import

    /// <inheritdoc cref="IAppPartsController.Get" />
    public ExportPartsOverviewDto Get(int zoneId, int appId, string scope)
        => exportContent.Value.PreExportSummary(zoneId: zoneId, appId: appId, scope: scope);


    /// <inheritdoc cref="IAppPartsController.Export" />
    public FileResult Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        => exportContent.Value.Export(zoneId: zoneId, appId: appId, contentTypeIdsString: contentTypeIdsString, entityIdsString: entityIdsString, templateIdsString: templateIdsString);


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
            return l.Return(new(false, "no file uploaded"), "no file uploaded");

        var (fileName, stream) = uploadInfo.GetStream(0);

        var result = importContent.New()
            .Import(zoneId: zoneId, appId: appId, fileName: fileName, stream: stream!, defaultLanguage: ctxSvc.Value.Context.Site.DefaultCultureCode);

        return l.ReturnAsOk(result);
    }

    #endregion
}