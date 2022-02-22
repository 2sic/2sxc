using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    /// <summary>
    /// This is the base class for controllers which just serve as a proxy to a Real backend.
    /// </summary>
    /// <typeparam name="TRealController"></typeparam>
    public abstract class IntControllerProxyBase<TRealController>: IntControllerBase where TRealController : class, IHasLog<TRealController>
    {
        protected IntControllerProxyBase(string logName) : base(logName) { }

        /// <summary>
        /// The RealController which is the full backend of this controller.
        /// Note that it's not available at construction time, because the ServiceProvider isn't ready till later.
        /// </summary>
        public TRealController Real => _real
            ??= ServiceProvider?.Build<TRealController>().Init(Log) ??
                throw new Exception($"Can't use {nameof(Real)} before OnActionExecuting");
        private TRealController _real;

    }
}
