using Custom.Hybrid.Advanced;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic WebAPI files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [PublicApi]
    [DnnLogExceptions]
    [UseOldNewtonsoftForHttpJson]
    public abstract class Api14: Api14<dynamic, ServiceKit14>, 
        IDynamicCode12, 
        IDynamicWebApi, 
        IHasDynamicCodeRoot,
        IHasCodeLog
    {
        protected Api14() : base("Hyb14")
        {
            //var log = base.Log.SubLogOrNull("Hyb14.Api14"); // real log
            //_log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        protected Api14(string logSuffix) : base(logSuffix)
        {
            //var log = base.Log.SubLogOrNull($"{logSuffix}.Api14"); // real log
            //_log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        #region IHasLog

        public new ICodeLog Log => _log.Get(() => new CodeLog(base.Log));
        private readonly GetOnce<ICodeLog> _log = new GetOnce<ICodeLog>();

        ILog IHasLog.Log => base.Log;

        // 2dm: Not needed ATM - reactivate if ever a child-object would really need the base log
        //public ILog Log15 => base.Log;

        #endregion

    }
}
