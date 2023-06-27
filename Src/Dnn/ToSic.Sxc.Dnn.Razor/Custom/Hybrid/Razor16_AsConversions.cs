using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor16
    {
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.AsC.MergeTyped(items);

        /// <inheritdoc />
        public ITypedItem AsItem(object target, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItems(list);

        /// <inheritdoc />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot.AsC.Folder(item, fieldName);

    }


}
