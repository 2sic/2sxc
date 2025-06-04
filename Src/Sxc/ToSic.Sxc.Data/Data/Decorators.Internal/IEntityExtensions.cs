using ToSic.Eav.Data.EntityDecorators.Sys;

namespace ToSic.Sxc.Data.Internal.Decorators;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class IEntityExtensions
{
    public static bool IsDemoItemSafe(this IEntity? entity)
        => entity?.GetDecorator<EntityInBlockDecorator>()?.IsDemoItem
           ?? false;

    public static bool DisableInlineEditSafe(this IEntity? entity)
        => entity == null
           || (entity.GetDecorator<CmsEditDecorator>()?.DisableEdit ?? IsDemoItemSafe(entity));
}