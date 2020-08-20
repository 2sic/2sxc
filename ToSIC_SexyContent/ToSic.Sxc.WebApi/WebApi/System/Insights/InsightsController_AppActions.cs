using System.Web.Http;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {
        #region AppActions

        [HttpGet]
        public string Purge(int? appId = null) => Insights.Purge(appId);
        
        #endregion
    }
}