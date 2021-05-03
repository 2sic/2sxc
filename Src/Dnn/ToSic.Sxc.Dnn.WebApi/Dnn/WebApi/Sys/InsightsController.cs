using System.Web.Http;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.Dnn.WebApi.Sys
{
    [DnnLogExceptions]
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

        protected Sxc.Web.WebApi.System.Insights GetInsights => GetService<Sxc.Web.WebApi.System.Insights>().Init(Log, ThrowIfNotSuperuser, HttpException.BadRequest);

        private void ThrowIfNotSuperuser()
        {
            if (!PortalSettings.UserInfo.IsSuperUser)
                throw HttpException.PermissionDenied("requires Superuser permissions");
        }


        #region Help and Basics

        [HttpGet]
        public string Help() => GetInsights.Help();

        [HttpGet]
        public bool IsAlive() => GetInsights.IsAlive();

        #endregion

        #region App
        [HttpGet]
        public string LoadLog(int? appId = null) => GetInsights.LoadLog(appId);

        [HttpGet]
        public string Cache() => GetInsights.Cache();

        [HttpGet]
        public string Stats(int? appId = null) => GetInsights.Stats(appId);
        #endregion

        #region AppActions

        [HttpGet]
        public string Purge(int? appId = null) => GetInsights.Purge(appId);

        #endregion

        #region Attributes

        [HttpGet]
        public string Attributes(int appId, string type = null) => GetInsights.Attributes(appId, type);

        [HttpGet]
        public string AttributeMetadata(int? appId = null, string type = null, string attribute = null) =>
            GetInsights.AttributeMetadata(appId, type, attribute);

        [HttpGet]
        public string AttributePermissions(int? appId = null, string type = null, string attribute = null) =>
            GetInsights.AttributePermissions(appId, type, attribute);

        #endregion

        #region Entities

        [HttpGet]
        public string Entities(int? appId = null, string type = null) => GetInsights.Entities(appId, type);

        [HttpGet]
        public string EntityMetadata(int? appId = null, int? entity = null) => GetInsights.EntityMetadata(appId, entity);

        [HttpGet]
        public string EntityPermissions(int? appId = null, int? entity = null) =>
            GetInsights.EntityPermissions(appId, entity);

        [HttpGet]
        public string Entity(int? appId = null, string entity = null) => GetInsights.Entity(appId, entity);
        #endregion

        #region Logs

        [HttpGet]
        public string Logs() => GetInsights.Logs();

        [HttpGet]
        public string Logs(string key) => GetInsights.Logs(key);

        [HttpGet]
        public string Logs(string key, int position) => GetInsights.Logs(key, position);

        [HttpGet]
        public string Logs(bool pause) => GetInsights.Logs(pause);

        [HttpGet]
        public string LogsFlush(string key) => GetInsights.LogsFlush(key);

        #endregion

        #region Types

        [HttpGet]
        public string Types(int? appId = null, bool detailed = false) => GetInsights.Types(appId, detailed);

        [HttpGet]
        public string GlobalTypes() => GetInsights.GlobalTypes();

        [HttpGet]
        public string GlobalTypesLog() => GetInsights.GlobalTypesLog();

        [HttpGet]
        public string TypeMetadata(int? appId = null, string type = null) => GetInsights.TypeMetadata(appId, type);

        [HttpGet]
        public string TypePermissions(int? appId = null, string type = null) => GetInsights.TypePermissions(appId, type);
        #endregion

    }
}