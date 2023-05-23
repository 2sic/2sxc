using System.Collections.Generic;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    public abstract partial class Api14<TModel, TServiceKit>
    {
        /// <inheritdoc />
        public ITypedEntity AsTyped(object target, string noParamOrder = ToSic.Eav.Parameters.Protector) => _DynCodeRoot.AsTyped(target);

        /// <inheritdoc />
        public IEnumerable<ITypedEntity> AsTypedList(object list, string noParamOrder = ToSic.Eav.Parameters.Protector) => _DynCodeRoot.AsTypedList(list);

    }
}
