using System.Net;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Install;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Sys.InstallControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Sys;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ShowApiWhenReleased(ShowApiMode.Never)]
public class InstallController : DnnApiController, IInstallController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi + ".install");

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public bool Resume()
        => Real.Resume();


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken] // Now with RVT as it's post now. Previously not, because this was a GET and could not include the RVT
    public HttpResponseMessage RemotePackage(string packageUrl, string newName = null)
    {
        this.PreventServerTimeout600();
        var result = Real.RemotePackage(packageUrl, SysHlp.GetService<DnnModuleFactory>().Get(ActiveModule), newName);
        return result == null
            ? Request.CreateResponse(HttpStatusCode.OK)
            : Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result);
    }
}
