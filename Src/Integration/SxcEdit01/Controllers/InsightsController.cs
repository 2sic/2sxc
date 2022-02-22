using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.WebApi.Sys;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route("api/sxc/sys/[controller]")]
    [ApiController]
    public class InsightsController : ControllerBase
    {
        /// <summary>
        /// Constructor which will retrieve the Insights backend for use here
        /// </summary>
        public InsightsController(Insights insights) => _insights = insights.Init(new Log("Int.Insights"));
        private readonly Insights _insights;

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
            [FromQuery] string nameId = null)
        {
            // Temporary setting to allow Insights despite minimal setup
            ToSic.Eav.Context.UserUnknown.AllowEverything = true;

            var result = _insights.Details(view, appId, key, position, type, toggle, nameId);
            return base.Content(result, "text/html");
        }
    }
}