using System.Web.Http;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {
        #region App
        [HttpGet]
        public string LoadLog(int? appId = null) => Insights.LoadLog(appId);

        [HttpGet]
        public string Cache() => Insights.Cache();

        [HttpGet]
        public string Stats(int? appId = null) => Insights.Stats(appId);
        #endregion

    }
}