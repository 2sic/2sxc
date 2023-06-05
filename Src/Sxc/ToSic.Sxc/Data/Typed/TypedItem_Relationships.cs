using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using static ToSic.Sxc.Code.IDynamicCodeRoot16AsExtensions;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem
    {
        /// <inheritdoc />
        public IEnumerable<ITypedItem> Parents(
            string type = null,
            string noParamOrder = Parameters.Protector,
            string field = null)
        {
            Parameters.Protect(noParamOrder, $"{nameof(field)}");
            return AsTypedList(DynEntity.Parents(type, field), _typedHelpers, 3, _Services.LogOrNull);
        }

        /// <inheritdoc />
        public IEnumerable<ITypedItem> Children(
            string field = null,
            string noParamOrder = Parameters.Protector,
            string type = null)
        {
            Parameters.Protect(noParamOrder, $"{nameof(type)}");
            return AsTypedList(DynEntity.Children(field, type), _typedHelpers, 3, _Services.LogOrNull);
        }

        /// <inheritdoc />
        public ITypedItem Child(string field) => Children(field).FirstOrDefault();
    }
}
