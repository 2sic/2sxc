using Custom.Hybrid.Advanced;
using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Razor16: Razor14<dynamic, ServiceKit14>, IDynamicCode16
    {
        #region New App, Settings, Resources

        /// <inheritdoc />
        public new IAppTyped App => (IAppTyped)base.App;

        /// <inheritdoc />
        public new ITypedRead Settings => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public new ITypedRead Resources => _DynCodeRoot.Resources;

        #endregion


        #region Killed Properties from base class

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 typed. Use Data.MyContent instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use Data.MyHeader instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object DynamicModel => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use TypedModel instead.");

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
