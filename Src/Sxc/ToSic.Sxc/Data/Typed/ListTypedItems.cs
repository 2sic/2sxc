using System.Collections.Generic;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Data
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ListTypedItems: List<ITypedItem>, ICanBeEntity
    {
        public ListTypedItems(IList<ITypedItem> original, IEntity fieldInfo) : base(original)
        {
            Entity = fieldInfo;
        }

        public IEntity Entity { get; }
    }
}
