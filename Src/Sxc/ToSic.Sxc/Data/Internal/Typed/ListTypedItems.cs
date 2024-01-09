using System.Collections.Generic;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Data.Internal.Typed;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ListTypedItems(IList<ITypedItem> original, IEntity fieldInfo) : List<ITypedItem>(original), ICanBeEntity
{
    public IEntity Entity { get; } = fieldInfo;
}