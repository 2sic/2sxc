using System.Web.Http;

namespace ToSic.Sxc.WebApi.System
{
    [SxcWebApiExceptionHandling]
    public partial class InsightsController
    {
        [HttpGet]
        public bool IsAlive() => Insights.IsAlive();
        

    }
}