using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [PublicApi("This is the official base class for v12+")]
    [DnnLogExceptions]
    [UseOldNewtonsoftForHttpJson]
    public abstract partial class Api12: ApiCoreShim, 
        IDynamicCode12, 
        IDynamicWebApi, 
        IHasDynamicCodeRoot,
        ToSic.Eav.Logging.IHasLog
    {
        protected Api12() : base("Hyb12")
        {
            var log = base.Log.SubLogOrNull("Hyb12.Api12"); // real log
            _log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        protected Api12(string logSuffix) : base(logSuffix)
        {
            var log = base.Log.SubLogOrNull($"{logSuffix}.Api12"); // real log
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
