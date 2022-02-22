using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.WebApi.Admin;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route(IntegrationConstants.DefaultRouteRoot + "/admin" + IntegrationConstants.DefaultRouteControllerAction)]
    [ApiController]
    public class DialogController : IntControllerProxyBase<AdminBackend>
    {
        public DialogController() :base("SysCnt") { }

        [HttpGet]
        public DialogContextStandalone Settings(int appId) => Real.DialogSettings(appId);
    }
}
