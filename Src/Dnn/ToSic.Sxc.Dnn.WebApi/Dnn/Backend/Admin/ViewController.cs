using System.Web;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Admin.ViewControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ViewController : DnnApiController, IViewController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    ///// <summary>
    ///// The app data controller probably needs this for every request.
    ///// </summary>
    ///// <param name="controllerContext"></param>
    //protected override void Initialize(HttpControllerContext controllerContext)
    //{
    //    base.Initialize(controllerContext);
    //    new DnnSxcWebApiInitBlockContextAndShare().InitBlockSpecs(SysHlp, controllerContext.Request, Log);
    //}

    #endregion


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
    public ImportResult Import(int zoneId, int appId)
    {
        this.PreventServerTimeout600();
        return Real.Import(new(Request, HttpContext.Current.Request), zoneId, appId);
    }
}