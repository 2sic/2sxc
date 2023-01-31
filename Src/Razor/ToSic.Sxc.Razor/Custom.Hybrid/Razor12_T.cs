using ToSic.Eav.Plumbing;
using ToSic.Lib;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
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
        protected Razor12() { }

        /// <inheritdoc />
        public ICodeLog Log => _codeLog.Get(() => new CodeLog(Log15));
        private readonly GetOnce<ICodeLog> _codeLog = new();

        [PrivateApi] ILog IHasLog.Log => Log15;
        [PrivateApi] public ILog Log15 { get; } = new Log("Oqt.Rzr12");

        #endregion


    }
}
