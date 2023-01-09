using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Dnn
{
    public abstract partial class RazorComponent
    {
        #region Code Behind - a Dnn feature which probably won't exist in Oqtane

        [PrivateApi]
        internal RazorCodeManager CodeManager => _codeManager ?? (_codeManager = new RazorCodeManager(this).Init((Log as CodeLog)?.GetContents()));
        private RazorCodeManager _codeManager;

        /// <inheritdoc />
        public dynamic Code => CodeManager.CodeOrException;

        #endregion

    }
}
