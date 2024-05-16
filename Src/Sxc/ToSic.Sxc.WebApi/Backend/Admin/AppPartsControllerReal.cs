using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Admin;
using ToSic.Sxc.Backend.ImportExport;
using ServiceBase = ToSic.Lib.Services.ServiceBase;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppPartsControllerReal(
    LazySvc<IContextOfSite> context,
    LazySvc<ExportContent> exportContent,
    Generator<ImportContent> importContent)
    : ServiceBase("Api.APartsRl", connect: [context, exportContent, importContent]), IAppPartsController
{
    public const string LogSuffix = "AParts";


    #region Parts Export/Import

    /// <inheritdoc />
    public ExportPartsOverviewDto Get(int zoneId, int appId, string scope) => exportContent.Value.PreExportSummary(zoneId: zoneId, appId: appId, scope: scope);


    /// <inheritdoc />
    public THttpResponseType Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        => exportContent.Value.Export(zoneId: zoneId, appId: appId, contentTypeIdsString: contentTypeIdsString, entityIdsString: entityIdsString, templateIdsString: templateIdsString);


    /// <summary>
    /// This method is not implemented for ControllerReal, because ControllerReal implements Import(HttpUploadedFile uploadInfo, int zoneId, int appId)
    /// </summary>
    /// <param name="zoneId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ImportResultDto Import(int zoneId, int appId)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// This implementation is special ControllerReal, instead of ImportResultDto Import(int zoneId, int appId) that is not implemented.
    /// </summary>
    /// <param name="uploadInfo"></param>
    /// <param name="zoneId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ImportResultDto Import(HttpUploadedFile uploadInfo, int zoneId, int appId)
    {
        var l = Log.Fn<ImportResultDto>();

        if (!uploadInfo.HasFiles()) 
            return l.Return(new(false, "no file uploaded"), "no file uploaded");

        var (fileName, stream) = uploadInfo.GetStream(0);

        var result = importContent.New()
            .Import(zoneId: zoneId, appId: appId, fileName: fileName, stream: stream, defaultLanguage: context.Value.Site.DefaultCultureCode);

        return l.ReturnAsOk(result);
    }

    #endregion
}