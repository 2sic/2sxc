using System.Web.Http.Controllers;
using ToSic.SexyContent.WebApi;
using ToSic.Sxc.Dnn.WebApi;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    [SxcWebApiExceptionHandling]
    public partial class InsightsController : DnnApiControllerWithFixes
    {
        /// <summary>
        /// Enable/disable logging of access to insights
        /// Only enable this if you have trouble developing insights, otherwise it clutters our logs
        /// </summary>
        internal const bool InsightsLoggingEnabled = false;

        internal const string InsightsUrlFragment = "/sys/insights/";


        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.Debug");
            Log.New("InsightsController");
        }

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string LogHistoryName { get; } = "web-api.insights";


    }
}