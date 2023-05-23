using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using static ToSic.Sxc.Code.IDynamicCodeRoot16AsExtensions;

namespace ToSic.Sxc.Data
{
    public partial class TypedEntity
    {
        public IEnumerable<ITypedEntity> Parents(
            string type = null,
            string noParamOrder = Parameters.Protector,
            string field = null)
        {
            Parameters.Protect(noParamOrder, $"{nameof(field)}");
            return AsTypedList(DynEntity.Parents(type, field), _Services, 3, _Services.LogOrNull);
        }

        public IEnumerable<ITypedEntity> Children(
            string field = null,
            string noParamOrder = Parameters.Protector,
            string type = null)
        {
            Parameters.Protect(noParamOrder, $"{nameof(type)}");
            return AsTypedList(DynEntity.Children(field, type), _Services, 3, _Services.LogOrNull);
        }

        public ITypedEntity Child(string field) => Children(field).FirstOrDefault();
    }
}
