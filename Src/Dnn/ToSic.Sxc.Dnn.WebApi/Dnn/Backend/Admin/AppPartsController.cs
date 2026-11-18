using System.Web;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Admin.AppPartsControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;
// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
// token, which fails in the cases where the url is called using get, which should result in a download

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppPartsController: DnnApiController, IAppPartsController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ExportPartsOverviewDto Get(int zoneId, int appId, string scope)
        => Real.Get(zoneId: zoneId, appId: appId, scope: scope);


    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        => Real
            .Export(zoneId: zoneId, appId: appId, contentTypeIdsString: contentTypeIdsString,
                entityIdsString: entityIdsString, templateIdsString: templateIdsString)
            .ToHttpResponse();


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResult Import(int zoneId, int appId)
    {
        this.PreventServerTimeout600();
        return Real.Import(uploadInfo: new(Request, HttpContext.Current.Request), zoneId: zoneId, appId: appId);
    }

}