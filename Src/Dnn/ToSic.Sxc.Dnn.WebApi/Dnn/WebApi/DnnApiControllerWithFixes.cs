using System;
using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi
{
    [DnnLogWebApi, JsonOnlyResponse]
    [PrivateApi("This controller is never used publicly, you can rename any time you want")]
    public abstract class DnnApiControllerWithFixes<TRealController> : DnnApiController, IHasLog where TRealController : class, IHasLog
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://go.2sxc.org/proxy-controllers

        internal const string DnnSupportedModuleNames = "2sxc,2sxc-app";

        protected DnnApiControllerWithFixes(string logSuffix)
        {
            Log = new Log("Api." + logSuffix);
            // ReSharper disable once VirtualMemberCallInConstructor
            SysHlp = new DnnWebApiHelper(this, HistoryLogGroup);
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

        /// <summary>
        /// The RealController which is the full backend of this controller.
        /// Note that it's not available at construction time, because the ServiceProvider isn't ready till later.
        /// </summary>
        internal TRealController Real => _real.Get(()
            => SysHlp.GetService<TRealController>()
               ?? throw new Exception($"Can't use {nameof(Real)} for unknown reasons, got null"));
        private readonly GetOnce<TRealController> _real = new GetOnce<TRealController>();

    }
}