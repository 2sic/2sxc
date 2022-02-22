using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.WebApi.Admin;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route(IntegrationConstants.DefaultRouteRoot + AreaRoutes.Admin)]
    [ApiController]
    public class DialogController : IntControllerProxyBase<AdminBackend>
    {
        public DialogController() :base("SysCnt") { }

        [HttpGet]
        public DialogContextStandalone Settings(int appId) => Real.DialogSettings(appId);
    }
}
