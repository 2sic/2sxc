namespace ToSic.Sxc.Data.Sys.Typed;

/// <summary>
/// Special helper for marking lists which are empty, but must know about the parent entity and field they belong to,
/// so that the toolbar can automatically create the correct sub-toolbar.
/// </summary>
/// <typeparam name="TTypedItem"></typeparam>
/// <param name="original"></param>
/// <param name="fieldInfo"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ListTypedItems<TTypedItem>(IEnumerable<TTypedItem> original, IEntity? fieldInfo)
    : List<TTypedItem>(original), ICanBeEntity
    where TTypedItem : class?
{
    [field: AllowNull, MaybeNull]
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public IEntity? Entity => field ??= fieldInfo ?? (this.FirstOrDefault() as ICanBeEntity)?.Entity;
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
}