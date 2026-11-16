using System.Web;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Licenses;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Eav.WebApi.Sys.Licenses.LicenseControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Sys;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class LicenseController : DnnApiController, ILicenseController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi + ".license");

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    public LicenseFileResultDto Upload()
    {
        this.PreventServerTimeout600();
        return Real.Upload(new(Request, HttpContext.Current.Request));
    }

    /// <inheritdoc />
    [HttpGet]
    public LicenseFileResultDto Retrieve()
    {
        this.PreventServerTimeout600();
        return Real.Retrieve();
    }
}