using System.Collections.Generic;
using static ToSic.Sxc.Code.IDynamicCodeRoot16AsExtensions;

namespace ToSic.Sxc.Data
{
    public partial class TypedEntity
    {
        public IEnumerable<ITypedEntity> Parents(string type = null, string field = null) 
            => AsTypedList(DynEntity.Parents(type, field), _Services, _Services.LogOrNull);

        public IEnumerable<ITypedEntity> Children(string field = null, string type = null)
            => AsTypedList(DynEntity.Children(field, type), _Services, _Services.LogOrNull);
    }
}
