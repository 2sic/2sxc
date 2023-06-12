using System.Collections.Generic;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Data
{
    public class ListTypedItems: List<ITypedItem>, ICanBeEntity
    {
        public ListTypedItems(IList<ITypedItem> original, IEntity fieldInfo) : base(original)
        {
            Entity = fieldInfo;
        }

        public IEntity Entity { get; }
    }
}
