using System;
using System.Web;
using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;

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
            // ReSharper disable VirtualMemberCallInConstructor
            WebApiLogging = new DnnWebApiLogging(Log, GetService<ILogStore>(), HistoryLogGroup);
            // ReSharper restore VirtualMemberCallInConstructor
        }

        internal DnnWebApiLogging WebApiLogging;

        protected override void Initialize(HttpControllerContext controllerContext) => Log.Do(() =>
        {
            // Add the logger to the request, in case it's needed in error-reporting
            WebApiLogging.OnInitialize(controllerContext);
            base.Initialize(controllerContext);
        });

        protected override void Dispose(bool disposing)
        {
            WebApiLogging.OnDispose();
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
        ///  Extend Time so Web Server doesn't time out
        /// </summary>
        protected void PreventServerTimeout300() => HttpContext.Current.Server.ScriptTimeout = 300;

        /// <inheritdoc />
        public virtual TService GetService<TService>() => _serviceProvider.Get(DnnStaticDi.GetPageScopedServiceProvider).Build<TService>(Log);
        // Must cache it, to be really sure we use the same ServiceProvider in the same request
        private readonly GetOnce<IServiceProvider> _serviceProvider = new GetOnce<IServiceProvider>();

        /// <summary>
        /// The RealController which is the full backend of this controller.
        /// Note that it's not available at construction time, because the ServiceProvider isn't ready till later.
        /// </summary>
        protected virtual TRealController Real
            => _real.Get(() => GetService<TRealController>()
                               ?? throw new Exception($"Can't use {nameof(Real)} for unknown reasons"));
        private readonly GetOnce<TRealController> _real = new GetOnce<TRealController>();

    }
}