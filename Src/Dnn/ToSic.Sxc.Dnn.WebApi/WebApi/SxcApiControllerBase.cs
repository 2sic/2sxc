using System.Web.Http.Controllers;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// This class is the base class of 2sxc API access
    /// It will auto-detect the SxcBlock context
    /// But it will NOT provide an App or anything like that
    /// </summary>
    [DnnLogExceptions]
    [PrivateApi("This was only ever used as an internal base class, so it can be modified as needed - just make sure the derived types don't break")]
    public abstract class SxcApiControllerBase: DnnApiControllerWithFixes
    {
        protected SxcApiControllerBase(string logSuffix) : base(logSuffix) { }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DynHlp.InitializeBlockContext(controllerContext.Request);
        }

        [PrivateApi]
        internal DynamicApiCodeHelpers DynHlp => _dynHlp ?? (_dynHlp = new DynamicApiCodeHelpers(this, SysHlp));
        private DynamicApiCodeHelpers _dynHlp;
        
    }
}
