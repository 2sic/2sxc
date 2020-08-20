using System.Web.Http;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {
        #region Entities

        [HttpGet]
        public string Entities(int? appId = null, string type = null) => Insights.Entities(appId, type);

        [HttpGet]
        public string EntityMetadata(int? appId = null, int? entity = null) => Insights.EntityMetadata(appId, entity);

        [HttpGet]
        public string EntityPermissions(int? appId = null, int? entity = null) =>
            Insights.EntityPermissions(appId, entity);

        [HttpGet]
        public string Entity(int? appId = null, string entity = null) => Insights.Entity(appId, entity);
        #endregion

    }
}