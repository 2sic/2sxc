using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Search;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 12+ <br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class Razor12 : Hybrid.Razor12, IRazorComponent
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

        #region Code Behind - a Dnn feature which probably won't exist in Oqtane

        [PrivateApi]
        internal RazorCodeManager CodeManager => _codeManager ?? (_codeManager = new RazorCodeManager(this));
        private RazorCodeManager _codeManager;

        /// <summary>
        /// Code-Behind of this .cshtml file - located in a file with the same name but ending in .code.cshtml
        /// </summary>
        public dynamic Code => CodeManager.CodeOrException;

        #endregion


        #region CustomizeSearch corrections

        /// <inheritdoc />
        [PrivateApi("shouldn't be used any more, but was still in v12 when released. v13+ must completely remove this")]
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
            DateTime beginDate)
        {
            // in 2sxc 11.11 the signature changed. 
            // so the engine will call this function
            // but the override will be the other one - so I must call that
            // unless of course this method was overridden by the final inheriting RazorComponent
#pragma warning disable 618 // disable warning about IContainer being obsolete
            CustomizeSearch(searchInfos, moduleInfo as IContainer, beginDate);
#pragma warning restore 618
        }

        [PrivateApi("shouldn't be used any more, but was still in v12 when released. v13+ must completely remove this")]
#pragma warning disable 618 // disable warning about IContainer being obsolete
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
#pragma warning restore 618
            DateTime beginDate)
        {
            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
            var code = CodeManager.CodeOrNull;
            if (code == null) return;
            if (code is Razor12Code codeAsRazor) codeAsRazor.CustomizeSearch(searchInfos, moduleInfo, beginDate);
        }

        [PrivateApi("shouldn't be used any more, but was still in v12 when released. v13+ must completely remove this")]
        public virtual void CustomizeData()
        {
            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
            var code = CodeManager.CodeOrNull;
            if (code == null) return;
            if (code is Razor12Code codeAsRazor) codeAsRazor.CustomizeData();
        }

        /// <inheritdoc />
        [PrivateApi("shouldn't be used any more, but was still in v12 when released. v13+ must completely remove this")]
        public Purpose Purpose { get; internal set; }


        #endregion

    }
}
