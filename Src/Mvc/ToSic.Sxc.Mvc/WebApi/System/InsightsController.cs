using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Web.WebApi.System;

namespace ToSic.Sxc.Mvc.WebApi.System
{
    [Route(WebApiConstants.WebApiDefaultRoute)]
    [ApiController]
    public class InsightsController : SxcStatelessControllerBase
    {
        private readonly Lazy<Insights> _lazyInsights;

        #region Logging aspects

         protected override string HistoryLogName => "Api.Debug";

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.insights";
       
        #endregion

        /// <summary>
        /// Enable/disable logging of access to insights
        /// Only enable this if you have trouble developing insights, otherwise it clutters our logs
        /// </summary>
        internal const bool InsightsLoggingEnabled = false;

        //internal const string InsightsUrlFragment = "/sys/insights/";


        #region Construction and Security

        public InsightsController(Lazy<Insights> lazyInsights)
        {
            _lazyInsights = lazyInsights;
        }

        protected Insights Insights => _insights ??= _lazyInsights.Value.Init(Log, ThrowIfNotSuperuser, msg => new Exception(msg));
        private Insights _insights;


        private void ThrowIfNotSuperuser()
        {
            // todo: security relevant before final release
            //if (!PortalSettings.UserInfo.IsSuperUser)
            //    throw Http.PermissionDenied("requires Superuser permissions");
        }

        #endregion

        private ContentResult Wrap(string contents) => base.Content(contents, "text/html");


        #region Help and Basics

        [HttpGet]
        public ContentResult Help() => Wrap(Insights.Help());

        [HttpGet]
        public ContentResult IsAlive() => Wrap(Insights.IsAlive().ToString());

        #endregion

        #region App
        [HttpGet]
        public ContentResult LoadLog(int? appId = null) => Wrap(Insights.LoadLog(appId));

        [HttpGet]
        public ContentResult Cache() => Wrap(Insights.Cache());

        [HttpGet]
        public ContentResult Stats(int? appId = null) => Wrap(Insights.Stats(appId));
        #endregion

        #region Logs

        /// <summary>
        /// Logs is different from the WebForms implementation, because route detection isn't as rigid in .net core
        /// </summary>
        /// <param name="key"></param>
        /// <param name="position"></param>
        /// <param name="pause"></param>
        /// <returns></returns>
        [HttpGet]
        public ContentResult Logs([FromQuery]string key, [FromQuery] int? position, [FromQuery] bool? pause)
        {
            // pause command, in this case it's the only parameter
            if (pause != null) return Wrap(Insights.Logs(pause.Value));

            if (key == null) return Wrap(Insights.Logs());
            if (position == null) return Wrap(Insights.Logs(key));
            return Wrap(Insights.Logs(key, position.Value));
        }

        [HttpGet]
        public ContentResult LogsFlush(string key) => Wrap(Insights.LogsFlush(key));
        #endregion

        #region AppActions

        [HttpGet]
        public ContentResult Purge(int? appId = null) => Wrap(Insights.Purge(appId));

        #endregion

        #region Attributes

        [HttpGet]
        public ContentResult Attributes(int appId, string type = null) => Wrap(Insights.Attributes(appId, type));

        [HttpGet]
        public ContentResult AttributeMetadata(int? appId = null, string type = null, string attribute = null) =>
            Wrap(Insights.AttributeMetadata(appId, type, attribute));

        [HttpGet]
        public ContentResult AttributePermissions(int? appId = null, string type = null, string attribute = null) =>
            Wrap(Insights.AttributePermissions(appId, type, attribute));

        #endregion


        #region Entities

        [HttpGet]
        public ContentResult Entities(int? appId = null, string type = null) => Wrap(Insights.Entities(appId, type));

        [HttpGet]
        public ContentResult EntityMetadata(int? appId = null, int? entity = null) => Wrap(Insights.EntityMetadata(appId, entity));

        [HttpGet]
        public ContentResult EntityPermissions(int? appId = null, int? entity = null) =>
            Wrap(Insights.EntityPermissions(appId, entity));

        [HttpGet]
        public ContentResult Entity(int? appId = null, string entity = null) => Wrap(Insights.Entity(appId, entity));
        #endregion


        #region Types

        [HttpGet]
        public ContentResult Types(int? appId = null, bool detailed = false) => Wrap(Insights.Types(appId, detailed));

        [HttpGet]
        public ContentResult GlobalTypes() => Wrap(Insights.GlobalTypes());

        [HttpGet]
        public ContentResult GlobalTypesLog() => Wrap(Insights.GlobalTypesLog());

        [HttpGet]
        public ContentResult TypeMetadata(int? appId = null, string type = null) => Wrap(Insights.TypeMetadata(appId, type));

        [HttpGet]
        public ContentResult TypePermissions(int? appId = null, string type = null) => Wrap(Insights.TypePermissions(appId, type));
        #endregion

    }
}
