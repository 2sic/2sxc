using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi
{
    [DnnLogWebApi, JsonOnlyResponse]
    [PrivateApi("This controller is never used publicly, you can rename any time you want")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class DnnApiControllerWithFixes : DnnApiController, IHasLog
    {
        internal const string DnnSupportedModuleNames = "2sxc,2sxc-app";

        protected DnnApiControllerWithFixes(string logSuffix, string insightsGroup = default, string firstMessage = default)
        {
            Log = new Log("Api." + logSuffix);
            // ReSharper disable once VirtualMemberCallInConstructor
            SysHlp = new DnnWebApiHelper(this, insightsGroup ?? HistoryLogGroup, firstMessage);
        }

        /// <summary>
        /// Special helper to move all Razor logic into a separate class.
        /// For architecture of Composition over Inheritance.
        /// </summary>
        [PrivateApi]
        internal DnnWebApiHelper SysHlp { get; }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            var l = Log.Fn();
            // Add the logger to the request, in case it's needed in error-reporting
            SysHlp.WebApiLogging.OnInitialize(controllerContext);
            base.Initialize(controllerContext);
            l.Done();
        }

        protected override void Dispose(bool disposing)
        {
            SysHlp.OnDispose();
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public ILog Log { get; }

        /// <summary>
        /// The group name for log entries in insights.
        /// Helps group various calls by use case. 
        /// </summary>
        protected virtual string HistoryLogGroup => EavWebApiConstants.HistoryNameWebApi;

    }
}