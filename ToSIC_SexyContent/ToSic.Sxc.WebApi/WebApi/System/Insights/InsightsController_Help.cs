using System.Web.Http;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {

        [HttpGet]
        public string Help() => Insights.Help();
        
    }
}