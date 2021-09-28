using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 10+ <br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class RazorComponent : Custom.Dnn.Razor12
    {
        //[PrivateApi("Hide this, no need to publish; would only confuse users")]
        //protected RazorComponent()
        //{
        //    // Enable CreateInstanceCshtml and RenderPage in anything that inherits these classes
        //    _ErrorWhenUsingCreateInstanceCshtml = null;
        //    _ErrorWhenUsingRenderPage = null;
        //}

//        /// <inheritdoc />
//        public IDnnContext Dnn => (_DynCodeRoot as IDnnDynamicCode)?.Dnn;

//        #region CustomizeSearch corrections

//        /// <inheritdoc />
//        [PrivateApi]
//        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
//            DateTime beginDate)
//        {
//            // in 2sxc 11.11 the signature changed. 
//            // so the engine will call this function
//            // but the override will be the other one - so I must call that
//            // unless of course this method was overridden by the final inheriting RazorComponent
//#pragma warning disable 618 // disable warning about IContainer being obsolete
//            CustomizeSearch(searchInfos, moduleInfo as IContainer, beginDate);
//#pragma warning restore 618
//        }

//        [PrivateApi]
//#pragma warning disable 618 // disable warning about IContainer being obsolete
//        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
//#pragma warning restore 618
//            DateTime beginDate)
//        {
//            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
//            var code = CodeManager.CodeOrNull;
//            if (code == null) return;
//            if (code is RazorComponent codeAsRazor) codeAsRazor.CustomizeSearch(searchInfos, moduleInfo, beginDate);
//        }


//        #endregion

    }
}
