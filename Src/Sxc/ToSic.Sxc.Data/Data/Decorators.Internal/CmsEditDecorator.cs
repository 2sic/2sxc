namespace ToSic.Sxc.Data.Internal.Decorators;

/// <summary>
/// Decorator for demo entities and similar, to disable editing so that the demo data isn't accidentally changed.
/// </summary>
public class CmsEditDecorator : IDecorator<IEntity>
{
    public bool DisableEdit { get; }

    private CmsEditDecorator(bool enableEdit)
    {
        DisableEdit = !enableEdit;
    }

    public static EntityDecorator12<CmsEditDecorator> Wrap(IEntity entity, bool enableEdit) =>
        new(entity, new(enableEdit));
}