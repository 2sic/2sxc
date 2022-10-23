using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract partial class Razor12<TModel>: Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        #region Constructor / DI

        [PrivateApi]
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        /// <summary>
        /// Constructor - only available for inheritance
        /// </summary>
        protected Razor12()
        {
            Log = new Log("Mvc.SxcRzr");
        }

        public ILog Log { get; }
        #endregion

    }
}
