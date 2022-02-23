using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.WebApi.Admin;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route(IntegrationConstants.DefaultRouteRoot + AreaRoutes.Admin)]
    [ApiController]
    public class DialogController : IntControllerProxyBase<AdminBackend>
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public DialogController() :base("SysCnt") { }

        [HttpGet]
        public DialogContextStandalone Settings(int appId) => Real.DialogSettings(appId);
    }
}
