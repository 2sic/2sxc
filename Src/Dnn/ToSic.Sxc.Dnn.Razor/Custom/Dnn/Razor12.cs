using Custom.Hybrid;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 12+ <br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class Razor12 : Hybrid.Razor12, IDnnDynamicCodeAdditions, IRazor12, IDnnRazor // , IDnnRazor11 /* IDnnRazor11 adds the Code property which probably doesn't make sense in future */
    {
        [PrivateApi("Hide this, no need to publish; would only confuse users")]
        protected Razor12()
        {
            // Enable CreateInstanceCshtml and RenderPage in anything that inherits these classes
            _ErrorWhenUsingCreateInstanceCshtml = null;
            _ErrorWhenUsingRenderPage = null;
        }

        /// <inheritdoc />
        public IDnnContext Dnn => (_DynCodeRoot as IDnnDynamicCode)?.Dnn;

        //#region Code Behind - a Dnn feature which probably won't exist in Oqtane

        //[PrivateApi]
        //internal RazorCodeManager CodeManager => _codeManager ?? (_codeManager = new RazorCodeManager(this));
        //private RazorCodeManager _codeManager;

        ///// <inheritdoc />
        //public dynamic Code => CodeManager.CodeOrException;

        //#endregion



    }
}
