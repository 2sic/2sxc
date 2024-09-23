namespace ToSic.Sxc.Data.Internal.Typed;

/// <summary>
/// Special helper for marking lists which are empty, but must know about the parent entity and field they belong to,
/// so that the toolbar can automatically create the correct sub-toolbar.
/// </summary>
/// <typeparam name="TTypedItem"></typeparam>
/// <param name="original"></param>
/// <param name="fieldInfo"></param>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ListTypedItems<TTypedItem>(IEnumerable<TTypedItem> original, IEntity fieldInfo)
    : List<TTypedItem>(original), ICanBeEntity
    where TTypedItem : class
{
    public IEntity Entity { get; } = fieldInfo ?? (original?.FirstOrDefault() as ICanBeEntity)?.Entity;
}