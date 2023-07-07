using ToSic.Eav.Data;

namespace ToSic.Sxc.Data.Decorators
{
    public static class IEntityExtensions
    {
        public static bool IsDemoItem(this IEntity entity) => entity?.GetDecorator<EntityInBlockDecorator>()?.IsDemoItem ?? false;

        public static bool DisableInlineEdit(this IEntity entity) =>
            entity?.GetDecorator<CmsEditDecorator>()?.DisableEdit ?? IsDemoItem(entity);
    }
}