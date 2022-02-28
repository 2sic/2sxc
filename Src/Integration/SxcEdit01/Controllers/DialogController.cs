using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.WebApi.Admin;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route(IntegrationConstants.DefaultRouteRoot + AreaRoutes.Admin)]
    [ApiController]
    public class DialogController : IntControllerBase<DialogControllerReal>, IDialogController
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public DialogController() :base(DialogControllerReal.LogSuffix) { }

        [HttpGet]
        public DialogContextStandaloneDto Settings(int appId) => Real.Settings(appId);
    }
}
