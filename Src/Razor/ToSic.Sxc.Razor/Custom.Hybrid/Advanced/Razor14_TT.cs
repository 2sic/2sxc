using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Razor14<TModel, TServiceKit>: Razor12<TModel>, IRazor14<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly GetOnce<TServiceKit> _kit = new();


        /// <inheritdoc />
        public ITypedItem AsTyped(object original, string noParamOrder = Protector, bool? required = default) => _DynCodeRoot.AsC.AsItem(original);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsTypedList(object list,
            string noParamOrder = Protector,
            bool? required = default,
            IEnumerable<ITypedItem> fallback = default)
            => _DynCodeRoot.AsC.AsItems(list, required: required, fallback: fallback);


    }
}
