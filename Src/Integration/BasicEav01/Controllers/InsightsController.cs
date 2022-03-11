using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Sys;

namespace IntegrationSamples.BasicEav01.Controllers
{
    [Route("api/sxc/" + Areas.Sys + "/[controller]")]
    [ApiController]
    public class InsightsController : ControllerBase
    {
        /// <summary>
        /// Constructor which will retrieve the Insights backend for use here
        /// </summary>
        public InsightsController(InsightsControllerReal insights) => _insights = insights;
        private readonly InsightsControllerReal _insights;

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
        ) => Content(_insights.Details(view, appId, key, position, type, toggle, nameId), "text/html");
    }
}
