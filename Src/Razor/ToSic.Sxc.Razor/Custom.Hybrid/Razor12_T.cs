using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract partial class Razor12<TModel>: Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>, ToSic.Eav.Logging.IHasLog
    {
        #region Constructor / DI

        [PrivateApi]
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        /// <summary>
        /// Constructor - only available for inheritance
        /// </summary>
        protected Razor12()
        {
            var log = new ToSic.Lib.Logging.Log("Oqt.Rzr12");
            Log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        public ToSic.Eav.Logging.ILog Log { get; }

        ILog IHasLog.Log => Log.GetContents();
        #endregion


    }
}
