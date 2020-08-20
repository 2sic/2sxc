using System.Web.Http;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {
        #region Logs
        
        [HttpGet]
        public string Logs() => Insights.Logs();

        [HttpGet]
        public string Logs(string key) => Insights.Logs(key);

        [HttpGet]
        public string Logs(string key, int position) => Insights.Logs(key, position);

        [HttpGet]
        public string Logs(bool pause) => Insights.Logs(pause);

        [HttpGet]
        public string LogsFlush(string key) => Insights.LogsFlush(key);

        #endregion
    }
}