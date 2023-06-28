using System.Collections.Generic;
using ToSic.Sxc.Data;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14
    {
        #region AsCms and AsTyped (new v16)

        /// <inheritdoc />
        public ITypedItem AsTyped(object original, string noParamOrder = Protector, bool? required = default) => _DynCodeRoot.AsC.AsItem(original);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsTypedList(object list,
            string noParamOrder = Protector,
            bool? required = default,
            IEnumerable<ITypedItem> fallback = default)
            => _DynCodeRoot.AsC.AsItems(list, required: required, fallback: fallback);

        #endregion

    }


}
