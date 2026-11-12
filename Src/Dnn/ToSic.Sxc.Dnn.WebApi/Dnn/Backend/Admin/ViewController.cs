using System.Web;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Sxc.Backend.Admin.ViewControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[DnnLogExceptions]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ViewController() : DnnSxcControllerBase(RealController.LogSuffix), IViewController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet, HttpDelete]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool Delete(int appId, int id)
        => Real.Delete(appId, id);

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage Json(int appId, int viewId)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        using (this.InitializedResponseMaker())
            return Real.Json(appId, viewId);
    }

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResultDto Import(int zoneId, int appId)
    {
        this.PreventServerTimeout600();
        return Real.Import(new(Request, HttpContext.Current.Request), zoneId, appId);
    }
}