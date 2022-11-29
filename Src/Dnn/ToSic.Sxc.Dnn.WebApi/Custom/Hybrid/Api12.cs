using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Logging;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;

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
        IHasCodeLog
    {
        protected Api12() : base("Hyb12")
        {
            //var log = base.Log.SubLogOrNull("Hyb12.Api12"); // real log
            //_log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        protected Api12(string logSuffix) : base(logSuffix)
        {
            //var log = base.Log.SubLogOrNull($"{logSuffix}.Api12"); // real log
            //_log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        #region IHasLog

        public new ICodeLog Log => _log.Get(() => new LogAdapter(base.Log));
        private readonly GetOnce<ICodeLog> _log = new GetOnce<ICodeLog>();

        ILog IHasLog.Log => base.Log;
        public ILog Log15 => base.Log;

        #endregion

    }
}
