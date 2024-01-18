namespace ToSic.Sxc.Data.Internal.Decorators;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal static class IEntityExtensions
{
    public static bool IsDemoItemSafe(this IEntity entity) => entity?.GetDecorator<EntityInBlockDecorator>()?.IsDemoItem ?? false;

    public static bool DisableInlineEditSafe(this IEntity entity)
    {
        if (entity == null) return true;
        return entity.GetDecorator<CmsEditDecorator>()?.DisableEdit ?? IsDemoItemSafe(entity);
    }
}