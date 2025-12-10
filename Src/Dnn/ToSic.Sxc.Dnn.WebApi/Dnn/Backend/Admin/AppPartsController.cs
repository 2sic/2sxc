using System.Web;
using ToSic.Eav.WebApi.Sys.Admin;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Sxc.Backend.Admin.AppPartsControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;
// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
// token, which fails in the cases where the url is called using get, which should result in a download

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppPartsController() : DnnSxcControllerRoot(RealController.LogSuffix), IAppPartsController
{
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
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);

        return Real.Export(zoneId: zoneId, appId: appId, contentTypeIdsString: contentTypeIdsString,
            entityIdsString: entityIdsString, templateIdsString: templateIdsString);
    }


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResultDto Import(int zoneId, int appId)
    {
        SysHlp.PreventServerTimeout600();
        return Real.Import(uploadInfo: new(Request, HttpContext.Current.Request), zoneId: zoneId, appId: appId);
    }

}