using System;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Sys;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route(WebApiConstants.DefaultRouteRoot + "/sys/[controller]/[action]")]
    [ApiController]
    public class InsightsController : IntStatelessControllerBase
    {
        private readonly Lazy<Insights> _lazyInsights;

        public InsightsController(Lazy<Insights> lazyInsights) : base()
        {
            _lazyInsights = lazyInsights;
        }


        #region Logging aspects

        protected override string HistoryLogName => "Api.Debug";

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.insights";

        #endregion

        /// <summary>
        /// Enable/disable logging of access to insights
        /// Only enable this if you have trouble developing insights, otherwise it clutters our logs
        /// </summary>
        internal const bool InsightsLoggingEnabled = false;

        //internal const string InsightsUrlFragment = "/sys/insights/";


        #region Construction and Security

        protected Insights Insights =>
            _insights ??= _lazyInsights.Value.Init(Log);
        private Insights _insights;


        #endregion

        private ContentResult Wrap(string contents) => base.Content(contents, "text/html");

        
    }
}
