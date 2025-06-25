using ToSic.Eav.Data.EntityDecorators.Sys;
using ToSic.Eav.Data.Sys.Entities;

namespace ToSic.Sxc.Data.Sys.Decorators;

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