using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Logging;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract partial class Razor12<TModel>: Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>, IHasCodeLog
    {
        #region Constructor / DI

        [PrivateApi]
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        /// <summary>
        /// Constructor - only available for inheritance
        /// </summary>
        protected Razor12()
        {
            // TODO: @2DM reverse log priorities - new one should really exist!
            //var log = new ToSic.Lib.Logging.Log("Oqt.Rzr12");
            //Log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        public ICodeLog Log => _log.Get(() => new LogAdapter(Log15));
        private readonly GetOnce<ICodeLog> _log = new();

        ILog IHasLog.Log => Log15;
        public ILog Log15 { get; } = new Log("Oqt.Rzr12"); // Log.GetContents();

        #endregion


    }
}
