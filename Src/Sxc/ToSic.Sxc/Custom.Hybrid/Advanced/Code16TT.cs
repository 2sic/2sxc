//using System;
//using System.Collections.Generic;
//using ToSic.Eav;
//using ToSic.Lib.Documentation;
//using ToSic.Sxc.Apps;
//using ToSic.Sxc.Code;
//using ToSic.Sxc.Data;
//using ToSic.Sxc.Services;

//// ReSharper disable once CheckNamespace
//namespace Custom.Hybrid.Advanced
//{
//    public abstract class Code16<TModel, TServiceKit>: Code14<TModel, TServiceKit>, IDynamicCode16 where TServiceKit : ServiceKit where TModel : class
//    {
//        public new ITypedRead Settings => _DynCodeRoot.Settings;

//        public new ITypedRead Resources => _DynCodeRoot.Resources;

//        [PrivateApi("Hide as it's nothing that should be used")]
//        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 typed. Use Data.MyContent instead.");

//        [PrivateApi("Hide as it's nothing that should be used")]
//        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use Data.MyHeader instead.");

//        [PrivateApi("Hide as it's nothing that should be used")]
//        public new object DynamicModel => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use TypedModel instead.");

//        public new IAppTyped App => (IAppTyped)base.App;

//        /// <inheritdoc />
//        public ITypedItem AsItem(object target, string noParamOrder = Parameters.Protector)
//            => _DynCodeRoot.AsC.AsTyped(target);

//        /// <inheritdoc />
//        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Parameters.Protector)
//            => _DynCodeRoot.AsC.AsTypedList(list);

//        /// <inheritdoc />
//        public ITypedRead Merge(params object[] items)
//            => _DynCodeRoot.AsC.MergeTyped(items);
//    }
//}
