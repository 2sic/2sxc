using System.Web.Http;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {
        #region Types


        [HttpGet]
        public string Types(int? appId = null, bool detailed = false) => Insights.Types(appId, detailed);

        [HttpGet]
        public string GlobalTypes() => Insights.GlobalTypes();

        [HttpGet]
        public string GlobalTypesLog() => Insights.GlobalTypesLog();

        [HttpGet]
        public string TypeMetadata(int? appId = null, string type = null) => Insights.TypeMetadata(appId, type);

        [HttpGet]
        public string TypePermissions(int? appId = null, string type = null) => Insights.TypePermissions(appId, type);
        #endregion
        
    }
}