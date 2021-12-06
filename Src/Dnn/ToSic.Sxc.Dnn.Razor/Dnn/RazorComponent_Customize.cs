//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ToSic.Sxc.Blocks;
//using ToSic.Sxc.Context;
//using ToSic.Sxc.Search;

//namespace ToSic.Sxc.Dnn
//{
//    public partial class RazorComponent
//    {
//        #region Customize Data, Search, and Purpose

//        /// <inheritdoc />
//        public virtual void CustomizeData()
//        {
//            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
//            var code = CodeManager.CodeOrNull;
//            if (code == null) return;
//            if (code is Dnn.RazorComponent codeAsRazor) codeAsRazor.CustomizeData();
//        }

//        ///// <inheritdoc />
//        //public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
//        //    DateTime beginDate)
//        //{
//        //    // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
//        //    var code = CodeManager.CodeOrNull;
//        //    if (code == null) return;
//        //    if (code is Dnn.RazorComponent codeAsRazor) codeAsRazor.CustomizeSearch(searchInfos, moduleInfo, beginDate);
//        //}

//        /// <inheritdoc />
//        public Purpose Purpose { get; internal set; }

//        #endregion


//    }
//}
