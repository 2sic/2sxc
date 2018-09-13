using System.Web.Http.Controllers;
using ToSic.SexyContent.WebApi;
using ToSic.SexyContent.WebApi.Dnn;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    [SxcWebApiExceptionHandling]
    public partial class InsightsController : DnnApiControllerWithFixes
    {
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
        protected override string LogHistorySetName { get; set; } = "web-api.insights";


    }
}