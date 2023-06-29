using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class RazorPro
    {
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.AsC.AsStack(items);

        /// <inheritdoc />
        public ITypedItem AsItem(object target, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItems(list);

        #region AsEntity
        /// <inheritdoc/>
        public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.AsC.AsEntity(thing);
        #endregion

    }


}
