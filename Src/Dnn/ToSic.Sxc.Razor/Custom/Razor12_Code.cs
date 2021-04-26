using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12
    {
        [PrivateApi]
        internal RazorCodeManager CodeManager => _codeManager ?? (_codeManager = new RazorCodeManager(this));
        private RazorCodeManager _codeManager;

        /// <summary>
        /// Code-Behind of this .cshtml file - located in a file with the same name but ending in .code.cshtml
        /// </summary>
        public dynamic Code => CodeManager.CodeOrException;
    }
}
