using System;
using System.Collections.Generic;
using Custom.Hybrid.Advanced;
using ToSic.Eav;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic Code files.
    /// 
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [WorkInProgressApi("WIP 16.02")]
    public abstract class Code16 :  Code14<dynamic, ServiceKit14>, IDynamicCode16
    {
        #region Killed Properties from base class

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 typed. Use Data.MyContent instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use Data.MyHeader instead.");

        #endregion

        #region New App, Settings, Resources

        /// <inheritdoc />
        public new IAppTyped App => (IAppTyped)base.App;

        /// <inheritdoc />
        public new ITypedStack Settings => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public new ITypedStack Resources => _DynCodeRoot.Resources;

        #endregion


        #region AsItem(s) / Merge

        /// <inheritdoc />
        public ITypedRead Merge(params object[] items)
            => _DynCodeRoot.AsC.MergeTyped(items);

        /// <inheritdoc />
        public ITypedItem AsItem(object target, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItems(list);

        #endregion
    }
}
