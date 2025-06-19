using ToSic.Eav.Data.EntityDecorators.Sys;

namespace ToSic.Sxc.Data.Internal.Decorators;

/// <summary>
/// Decorator for demo entities and similar, to disable editing so that the demo data isn't accidentally changed.
/// </summary>
public record CmsEditDecorator(bool DisableEdit)
    : IDecorator<IEntity>
{
    public static EntityWithDecorator<CmsEditDecorator> Wrap(IEntity entity, bool enableEdit) =>
        new(entity, new(!enableEdit));
}