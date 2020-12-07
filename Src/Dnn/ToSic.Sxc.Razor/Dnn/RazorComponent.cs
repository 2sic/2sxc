using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Search;


namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 10+ <br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class RazorComponent : Hybrid.Razor.RazorComponent, IRazorComponent
    {

        #region Link, Edit, Dnn, App, Data

        /// <inheritdoc />
        public IDnnContext Dnn => DynCode.Dnn;


        #endregion

        #region CustomizeSearch corrections

        /// <inheritdoc />
        [PrivateApi]
        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
            DateTime beginDate)
        {
            // in 2sxc 11.11 the signature changed. 
            // so the engine will call this function
            // but the override will be the other one - so I must call that
            CustomizeSearch(searchInfos, moduleInfo as IContainer, beginDate);
        }

#pragma warning disable 618
        [PrivateApi]
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
            DateTime beginDate)
        {
            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
            var code = CodeManager.CodeOrNull;
            if (code == null) return;
            if (code is RazorComponent codeAsRazor) codeAsRazor.CustomizeSearch(searchInfos, moduleInfo, beginDate);
        }
#pragma warning restore 618


        #endregion

    }
}
