using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + "/sys/[controller]/")]
    [Route(WebApiConstants.ApiRootPathOrLang + "/sys/[controller]/")]
    [Route(WebApiConstants.ApiRootPathNdLang + "/sys/[controller]/")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/sys/[controller]/")]

    [ApiController]
    public class InsightsController : OqtControllerBase
    {
        public InsightsController(InsightsControllerReal insights) => _insights = insights;
        private readonly InsightsControllerReal _insights;

        #region Logging aspects

        protected override string HistoryLogName => "Api.Debug";

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.insights";

        /// <summary>
        /// Enable/disable logging of access to insights
        /// Only enable this if you have trouble developing insights, otherwise it clutters our logs
        /// </summary>
        internal const bool InsightsLoggingEnabled = false;

        #endregion

        private ContentResult Wrap(string contents) => base.Content(contents, "text/html");

        [HttpGet("{view}")]
        public ContentResult Details([FromRoute] string view, 
            [FromQuery] int? appId = null, [FromQuery] string key = null, [FromQuery] int? position = null,
            [FromQuery] string type = null, [FromQuery] bool? toggle = null, string nameId = null)
            => Wrap(_insights.Init(Log).Details(view, appId, key, position, type, toggle, nameId));
        
    }
}
