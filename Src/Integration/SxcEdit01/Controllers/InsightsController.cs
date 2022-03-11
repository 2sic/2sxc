using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Sys;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route(IntegrationConstants.DefaultRouteRoot + Areas.Sys + "/[controller]")]
    [ApiController]
    public class InsightsController : IntControllerBase<InsightsControllerReal>
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public InsightsController(): base(InsightsControllerReal.LogSuffix) {}

        /// <summary>
        /// The main call on this controller, will return all kinds of views with information
        /// </summary>
        [HttpGet("{view}")]
        public ContentResult Details(
            [FromRoute] string view,
            [FromQuery] int? appId = null,
            [FromQuery] string key = null,
            [FromQuery] int? position = null,
            [FromQuery] string type = null,
            [FromQuery] bool? toggle = null,
            [FromQuery] string nameId = null
        ) => Content(Real.Details(view, appId, key, position, type, toggle, nameId), "text/html");
    }
}