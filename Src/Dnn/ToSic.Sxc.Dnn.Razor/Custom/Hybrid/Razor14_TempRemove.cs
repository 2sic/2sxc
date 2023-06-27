using System.Collections.Generic;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14
    {
        #region AsCms and AsTyped (new v16)

        /// <inheritdoc />
        public ITypedItem AsTyped(object target, string noParamOrder = ToSic.Eav.Parameters.Protector) => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsTypedList(object list, string noParamOrder = ToSic.Eav.Parameters.Protector) => _DynCodeRoot.AsC.AsItems(list);

        #endregion

    }


}
