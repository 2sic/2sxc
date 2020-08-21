using System.Web.Http;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.WebApi.System
{
    [SxcWebApiExceptionHandling]
    public class InsightsController : DnnApiControllerWithFixes
    {
        #region Logging
        protected override string HistoryLogName => "Api.Debug";

        /// <summary>
        /// Enable/disable logging of access to insights
        /// Only enable this if you have trouble developing insights, otherwise it clutters our logs
        /// </summary>
        internal const bool InsightsLoggingEnabled = false;

        internal const string InsightsUrlFragment = "/sys/insights/";

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.insights";

        #endregion


        protected Web.WebApi.System.Insights Insights =>
            _insights ?? (_insights = new Web.WebApi.System.Insights(Log, ThrowIfNotSuperuser, Http.BadRequest));
        private Web.WebApi.System.Insights _insights;

        private void ThrowIfNotSuperuser()
        {
            if (!PortalSettings.UserInfo.IsSuperUser)
                throw Http.PermissionDenied("requires Superuser permissions");
        }


        #region Help and Basics

        [HttpGet]
        public string Help() => Insights.Help();

        [HttpGet]
        public bool IsAlive() => Insights.IsAlive();

        #endregion

        #region App
        [HttpGet]
        public string LoadLog(int? appId = null) => Insights.LoadLog(appId);

        [HttpGet]
        public string Cache() => Insights.Cache();

        [HttpGet]
        public string Stats(int? appId = null) => Insights.Stats(appId);
        #endregion

        #region AppActions

        [HttpGet]
        public string Purge(int? appId = null) => Insights.Purge(appId);

        #endregion

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