using System.Web.Http;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {
        #region Attributes

        [HttpGet]
        public string Attributes(int appId, string type = null) => Insights.Attributes(appId, type);

        [HttpGet]
        public string AttributeMetadata(int? appId = null, string type = null, string attribute = null) =>
            Insights.AttributeMetadata(appId, type, attribute);

        [HttpGet]
        public string AttributePermissions(int? appId = null, string type = null, string attribute = null) =>
            Insights.AttributePermissions(appId, type, attribute);
        
        #endregion
    }
}