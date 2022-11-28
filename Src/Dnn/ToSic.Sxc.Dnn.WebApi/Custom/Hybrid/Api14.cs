using Custom.Hybrid.Advanced;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;

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
        ToSic.Eav.Logging.IHasLog
    {
        protected Api14() : base("Hyb14")
        {
            var log = base.Log.SubLogOrNull("Hyb14.Api14"); // real log
            _log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        protected Api14(string logSuffix) : base(logSuffix)
        {
            var log = base.Log.SubLogOrNull($"{logSuffix}.Api14"); // real log
            _log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        #region IHasLog

        /// <inheritdoc />
        public new ToSic.Eav.Logging.ILog Log => _log ?? (_log = new LogAdapter(null)/*fallback Log*/);

        private ToSic.Eav.Logging.ILog _log;

        //ToSic.Lib.Logging.ILog ToSic.Lib.Logging.IHasLog.Log => Log.GetContents(); // explicit Log implementation (to ensure that new IHasLog.Log interface is implemented)

        #endregion

    }
}
